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


//        [Fact]
//public async Task GetReports_ReturnsAllReports()
//{
//    // Arrange
//    var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
//    var mockDbSet = new Mock<DbSet<Report>>();
//    var testReports = new List<Report>
//    {
//        new Report { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Preparing },
//        new Report { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed }
//    }.AsQueryable();

//    mockDbSet.As<IQueryable<Report>>().Setup(m => m.Provider).Returns(testReports.Provider);
//    mockDbSet.As<IQueryable<Report>>().Setup(m => m.Expression).Returns(testReports.Expression);
//    mockDbSet.As<IQueryable<Report>>().Setup(m => m.ElementType).Returns(testReports.ElementType);
//    mockDbSet.As<IQueryable<Report>>().Setup(m => m.GetEnumerator()).Returns(testReports.GetEnumerator());

//    mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object); // Mock the Reports property

//    var mockMessageQueueService = new Mock<IMessageQueueService>();
//    var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

//    // Act
//    var result = await controller.GetReports();

//    // Assert
//    var actionResult = Assert.IsType<ActionResult<IEnumerable<Report>>>(result);
//    var returnedReports = Assert.IsType<List<Report>>(actionResult.Value);
//    Assert.Equal(2, returnedReports.Count);
//}


    }
}
