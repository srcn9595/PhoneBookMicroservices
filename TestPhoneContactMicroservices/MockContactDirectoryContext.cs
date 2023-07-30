using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices
{
    public class MockContactDirectoryContext : IContactDirectoryContext
    {
        private readonly MockDbSet<Contact> _mockContacts;
        private readonly MockDbSet<ContactDetail> _mockContactDetails;
        private readonly MockDbSet<Report> _mockReports;

        public MockContactDirectoryContext()
        {
            _mockContacts = new MockDbSet<Contact>();
            _mockContactDetails = new MockDbSet<ContactDetail>();
            _mockReports = new MockDbSet<Report>();
        }

        public DbSet<Contact> Contacts
        {
            get => _mockContacts;
            set => throw new NotImplementedException();
        }

        public DbSet<ContactDetail> ContactDetails
        {
            get => _mockContactDetails;
            set => throw new NotImplementedException();
        }

        public DbSet<Report> Reports
        {
            get => _mockReports;
            set => throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public EntityEntry<T> Update<T>(T entity) where T : class
        {
            var mockEntry = new Mock<EntityEntry<T>>();
            mockEntry.Setup(e => e.State).Returns(EntityState.Modified);
            return mockEntry.Object;
        }

        public bool ContactDetailExists(Guid id)
        {
            return _mockContactDetails.Any(e => e.Id == id);
        }

        public EntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
