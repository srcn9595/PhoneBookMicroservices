using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PhoneBookMicroservices.Controllers;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices;
using System.Collections.Generic;

public class ContactsControllerTests
{
    [Fact]
    public async Task GetContacts_ReturnsCorrectNumberOfContacts()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var expectedContacts = new List<Contact>
        {
            new Contact { Id = Guid.NewGuid(), Name = "Test1", Surname = "Test1", Company = "Test1" },
            new Contact { Id = Guid.NewGuid(), Name = "Test2", Surname = "Test2", Company = "Test2" }
        };

        var mockSet = new Mock<DbSet<Contact>>();
        mockSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(expectedContacts.AsQueryable().Provider);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(expectedContacts.AsQueryable().Expression);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(expectedContacts.AsQueryable().ElementType);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(expectedContacts.AsQueryable().GetEnumerator());

        // Add this line
        mockSet.As<IAsyncEnumerable<Contact>>().Setup(m => m.GetAsyncEnumerator(new CancellationToken()))
         .Returns(new TestAsyncEnumerator<Contact>(expectedContacts.GetEnumerator()));





        mockContext.Setup(ctx => ctx.Contacts).Returns(mockSet.Object);

        var controller = new ContactsController(mockContext.Object);

        // Act
        var result = await controller.GetContacts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Contact>>>(result);
        var contacts = Assert.IsType<List<Contact>>(actionResult.Value);
        Assert.Equal(expectedContacts.Count, contacts.Count);
    }
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }
    }

    public class TestAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;

        public TestAsyncEnumerable(IEnumerable<T> inner)
        {
            _inner = inner;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new TestAsyncEnumerator<T>(_inner.GetEnumerator());
        }
    }

}


