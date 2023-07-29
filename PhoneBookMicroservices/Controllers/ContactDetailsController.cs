using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookMicroservices.Models;

namespace PhoneBookMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactDetailsController : ControllerBase
    {
        private readonly ContactDirectoryContext _context;

        public ContactDetailsController(ContactDirectoryContext context)
        {
            _context = context;
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

            return CreatedAtAction(nameof(GetContactDetails), new { contactId = contactDetail.ContactId }, contactDetail);
        }

        // PUT: api/contactdetails/{contactId}/{id}
        [HttpPut("{contactId}/{id}")]
        public async Task<IActionResult> UpdateContactDetail(Guid contactId, Guid id, ContactDetail contactDetail)
        {
            if (id != contactDetail.Id || contactId != contactDetail.ContactId)
            {
                return BadRequest();
            }

            _context.Entry(contactDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactDetailExists(id))
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
            var contactDetail = await _context.ContactDetails
                .Where(cd => cd.ContactId == contactId && cd.Id == id)
                .FirstOrDefaultAsync();

            if (contactDetail == null)
            {
                return NotFound();
            }

            _context.ContactDetails.Remove(contactDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactDetailExists(Guid id)
        {
            return _context.ContactDetails.Any(e => e.Id == id);
        }
    }
}
