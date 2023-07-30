using PhoneBookMicroservices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Models
{
    public class ReportTests
    {
        [Fact]
        public void Report_RequestedAt_ShouldBeSetCorrectly()
        {
            // Arrange
            var requestedAt = DateTime.Now;

            // Act
            var report = new Report { RequestedAt = requestedAt };

            // Assert
            Assert.Equal(requestedAt, report.RequestedAt);
        }

        [Fact]
        public void Report_Status_ShouldBeSetCorrectly()
        {
            // Arrange
            var status = ReportStatus.Completed;

            // Act
            var report = new Report { Status = status };

            // Assert
            Assert.Equal(status, report.Status);
        }

        [Fact]
        public void Report_Location_ShouldBeSetCorrectly()
        {
            // Arrange
            var location = "New York, USA";

            // Act
            var report = new Report { Location = location };

            // Assert
            Assert.Equal(location, report.Location);
        }

        [Fact]
        public void Report_NumberOfContacts_ShouldBeSetCorrectly()
        {
            // Arrange
            var numberOfContacts = 100;

            // Act
            var report = new Report { NumberOfContacts = numberOfContacts };

            // Assert
            Assert.Equal(numberOfContacts, report.NumberOfContacts);
        }

        [Fact]
        public void Report_NumberOfPhoneNumbers_ShouldBeSetCorrectly()
        {
            // Arrange
            var numberOfPhoneNumbers = 250;

            // Act
            var report = new Report { NumberOfPhoneNumbers = numberOfPhoneNumbers };

            // Assert
            Assert.Equal(numberOfPhoneNumbers, report.NumberOfPhoneNumbers);
        }
    }
}
