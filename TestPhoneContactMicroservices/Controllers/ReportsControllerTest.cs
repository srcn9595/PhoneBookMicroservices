using Microsoft.AspNetCore.Mvc;
using Moq;
using PhoneBookMicroservices.Controllers;
using PhoneBookMicroservices.Services;
using PhoneBookMicroservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhoneBookMicroservices.Models;

namespace TestPhoneContactMicroservices.Controllers
{
    public class ReportsControllerTest
    {
        [Fact]
        public void RequestReport_CreatesNewReport()
        {
            // Arrange
            var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
            var mockDbSet = new Mock<DbSet<Report>>();
            mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object); // Mock the Reports property
            mockContext.Setup(c => c.SaveChanges()).Returns(1); // Mock the SaveChanges method

            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = controller.RequestReport();

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var reportId = Assert.IsType<Guid>(actionResult.Value);
            mockDbSet.Verify(dbSet => dbSet.Add(It.IsAny<Report>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }





    }
}
