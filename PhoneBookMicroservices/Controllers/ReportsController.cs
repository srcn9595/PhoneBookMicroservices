using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneBookMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ContactDirectoryContext _context;
        private readonly IMessageQueueService _messageQueueService;

        public ReportsController(ContactDirectoryContext context, IMessageQueueService messageQueueService)
        {
            _context = context;
            _messageQueueService = messageQueueService;
        }

        // GET: api/reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            return await _context.Reports.ToListAsync();
        }

        // GET: api/reports/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReport(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            return report;
        }

        // POST: api/reports
        [HttpPost]
        public async Task<ActionResult<Report>> CreateReport(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }

        // PUT: api/reports/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, Report report)
        {
            var existingReport = await _context.Reports.FindAsync(id);
            if (existingReport == null)
            {
                return NotFound(); // Return NotFoundResult if the report does not exist
            }

            if (id != report.Id)
            {
                return BadRequest();
            }

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/reports/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/reports/request
        [HttpPost("request")]
        public ActionResult<Guid> RequestReport()
        {
            var reportId = Guid.NewGuid();

            // Create a new report with status "Processing"
            var report = new Report
            {
                Id = reportId,
                RequestedAt = DateTime.UtcNow,
                Status = ReportStatus.Preparing
            };
            _context.Reports.Add(report);
            _context.SaveChanges();

            // Send a message to the queue with the report id
            _messageQueueService.SendMessageToQueue("reportRequests", reportId.ToString());

            // Return the report id
            return reportId;
        }

        private bool ReportExists(Guid id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
