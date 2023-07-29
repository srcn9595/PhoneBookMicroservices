using Microsoft.EntityFrameworkCore;
using PhoneBookMicroservices.Models;

namespace PhoneBookMicroservices
{
    public class ContactDirectoryContext : DbContext
    {
        public ContactDirectoryContext(DbContextOptions<ContactDirectoryContext> options)
        : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}
