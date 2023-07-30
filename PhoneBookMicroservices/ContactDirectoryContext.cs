using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PhoneBookMicroservices.Models;

namespace PhoneBookMicroservices
{
    public interface IContactDirectoryContext
    {
        DbSet<Contact> Contacts { get; set; }
        DbSet<ContactDetail> ContactDetails { get; set; }
        DbSet<Report> Reports { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry<T> Update<T>(T entity) where T : class;
        EntityEntry<T> Entry<T>(T entity) where T : class;
        bool ContactDetailExists(Guid id); // Yeni eklenen metot
    }



    public class ContactDirectoryContext : DbContext, IContactDirectoryContext
    {
        public ContactDirectoryContext(DbContextOptions<ContactDirectoryContext> options)
        : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public virtual DbSet<Report> Reports { get; set; } // Add 'virtual' here

        public void Update<T>(T entity) where T : class
        {
            base.Update(entity);
        }

        public override int SaveChanges() => base.SaveChanges();
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => base.SaveChangesAsync(cancellationToken);
        public bool ContactDetailExists(Guid id)
        {
            return ContactDetails.Any(e => e.Id == id);
        }
    }
}
