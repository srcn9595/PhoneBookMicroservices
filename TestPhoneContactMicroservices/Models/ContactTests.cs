using PhoneBookMicroservices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Models
{
    public class ContactTests
    {
        [Fact]
        public void Contact_WithValidData_ShouldBeCreated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "John";
            var surname = "Doe";
            var company = "ABC Company";

            // Act
            var contact = new Contact { Id = id, Name = name, Surname = surname, Company = company };

            // Assert
            Assert.Equal(id, contact.Id);
            Assert.Equal(name, contact.Name);
            Assert.Equal(surname, contact.Surname);
            Assert.Equal(company, contact.Company);
        }

        [Fact]
        public void Contact_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "";
            var surname = "Doe";
            var company = "ABC Company";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Contact { Id = id, Name = name, Surname = surname, Company = company });
        }
    }
}
