using PhoneBookMicroservices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Models
{
    public class ContactDetailTests
    {
        [Fact]
        public void ContactDetail_InfoType_ShouldMapToEnum()
        {
            // Arrange
            var contactDetail = new ContactDetail
            {
                Id = Guid.NewGuid(),
                ContactId = Guid.NewGuid(),
                InfoTypeValue = 2, // InfoType enumunda Email'e denk geliyor.
                InfoContent = "john.doe@example.com"
            };

            // Act
            var infoType = contactDetail.InfoType;

            // Assert
            Assert.Equal(InfoType.Email, infoType);
        }

        [Fact]
        public void ContactDetail_InfoType_ShouldMapFromEnum()
        {
            // Arrange
            var contactDetail = new ContactDetail
            {
                Id = Guid.NewGuid(),
                ContactId = Guid.NewGuid(),
                InfoType = InfoType.PhoneNumber, // InfoType'ı PhoneNumber olarak ayarladık (değeri 1)
                InfoContent = "+1234567890"
            };

            // Act
            var infoTypeValue = contactDetail.InfoTypeValue;

            // Assert
            Assert.Equal(1, infoTypeValue);
        }
    }
}
