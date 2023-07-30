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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

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
        [Fact]
        public async Task GetReports_ReturnsAllReports()
        {
            // Arrange
            var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
            var mockDbSet = new Mock<DbSet<Report>>();
            var data = new List<Report>
    {
        new Report { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Preparing },
        new Report { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed },
    }.AsQueryable();

            mockDbSet.As<IAsyncEnumerable<Report>>().Setup(m => m.GetAsyncEnumerator(new CancellationToken()))
                .Returns(new TestAsyncEnumerator<Report>(data.GetEnumerator()));

            mockDbSet.As<IQueryable<Report>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Report>(data.Provider));

            mockDbSet.As<IQueryable<Report>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<Report>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<Report>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.GetReports();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Report>>>(result);
            var reports = Assert.IsType<List<Report>>(actionResult.Value);
            Assert.Equal(2, reports.Count);
        }


        [Fact]
        public async Task GetReport_ReturnsReport_WhenReportExists()
        {
            // Arrange
            var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
            var mockDbSet = new Mock<DbSet<Report>>();
            var reportId = Guid.NewGuid();
            var report = new Report { Id = reportId, RequestedAt = DateTime.UtcNow, Status = ReportStatus.Preparing };

            mockDbSet.Setup(m => m.FindAsync(reportId)).Returns(new ValueTask<Report>(report));
            mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.GetReport(reportId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Report>>(result);
            var returnedReport = Assert.IsType<Report>(actionResult.Value);
            Assert.Equal(reportId, returnedReport.Id);
        }
        [Fact]
        public async Task GetReport_ReturnsNotFound_WhenReportDoesNotExist()
        {
            // Arrange
            var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
            var mockDbSet = new Mock<DbSet<Report>>();
            var reportId = Guid.NewGuid();

            mockDbSet.Setup(m => m.FindAsync(reportId)).Returns(new ValueTask<Report>((Report)null));
            mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.GetReport(reportId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Report>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }






        [Fact]
        public async Task UpdateReport_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
            var mockDbSet = new Mock<DbSet<Report>>();
            var reportId = Guid.NewGuid();
            var report = new Report { Id = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Preparing };

            mockDbSet.Setup(m => m.FindAsync(reportId)).Returns(new ValueTask<Report>(report));
            mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.UpdateReport(reportId, report);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateReport_ReturnsNotFound_WhenReportDoesNotExist()
        {
            // Arrange
            var mockContext = new Mock<ContactDirectoryContext>(new DbContextOptions<ContactDirectoryContext>());
            var mockDbSet = new Mock<DbSet<Report>>();
            var reportId = Guid.NewGuid();

            mockDbSet.Setup(m => m.FindAsync(reportId)).Returns(new ValueTask<Report>((Report)null));
            mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new ReportsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.UpdateReport(reportId, new Report());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
