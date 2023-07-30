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
    public class ContactDetailsController : ControllerBase
    {
        private readonly IContactDirectoryContext _context;
        private readonly IMessageQueueService _messageQueueService;

        public ContactDetailsController(IContactDirectoryContext context, IMessageQueueService messageQueueService)
        {
            _context = context;
            _messageQueueService = messageQueueService;
        }


        // GET: api/contactdetails/{contactId}
        [HttpGet("{contactId}")]
        public async Task<ActionResult<IEnumerable<ContactDetail>>> GetContactDetails(Guid contactId)
        {
            var contactDetails = await _context.ContactDetails
                .Where(cd => cd.ContactId == contactId)
                .ToListAsync();

            if (!contactDetails.Any())
            {
                return NotFound();
            }

            return contactDetails;
        }

        // POST: api/contactdetails/{contactId}
        [HttpPost("{contactId}")]
        public async Task<ActionResult<ContactDetail>> CreateContactDetail(Guid contactId, ContactDetail contactDetail)
        {
            if (contactId != contactDetail.ContactId)
            {
                return BadRequest();
            }

            _context.ContactDetails.Add(contactDetail);
            await _context.SaveChangesAsync();

            // Send a message to RabbitMQ
            _messageQueueService.SendMessageToQueue("contactDetailsQueue", $"Contact detail created: {contactDetail.Id}");

            return CreatedAtAction(nameof(GetContactDetails), new { contactId = contactDetail.ContactId }, contactDetail);
        }

        // PUT: api/contactdetails/{contactId}/{id}
        [HttpPut("{contactId}/{id}")]
        public async Task<IActionResult> UpdateContactDetail(Guid contactId, Guid id, ContactDetail contactDetail)
        {
            if (contactDetail == null || id != contactDetail.Id || contactId != contactDetail.ContactId)
            {
                return BadRequest();
            }

            var existingContactDetail = await _context.ContactDetails.FindAsync(id);
            if (existingContactDetail == null)
            {
                return NotFound();
            }

            existingContactDetail.InfoContent = contactDetail.InfoContent;
            existingContactDetail.InfoTypeValue = contactDetail.InfoTypeValue;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ContactDetailExists(id))
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



        // DELETE: api/contactdetails/{contactId}/{id}
        [HttpDelete("{contactId}/{id}")]
        public async Task<IActionResult> DeleteContactDetail(Guid contactId, Guid id)
        {
            var contactDetail = await _context.ContactDetails.FindAsync(id);

            if (contactDetail == null)
            {
                return NotFound();
            }

            if (contactDetail.ContactId != contactId)
            {
                return BadRequest("ContactId and id do not match.");
            }

            try
            {
                _context.ContactDetails.Remove(contactDetail);
                await _context.SaveChangesAsync();

                // Send a message to RabbitMQ
                _messageQueueService.SendMessageToQueue("contactDetailsQueue", $"Contact detail deleted: {contactDetail.Id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the delete operation
                return StatusCode(500, "An error occurred while deleting the contact detail.");
            }
        }

    }
}
