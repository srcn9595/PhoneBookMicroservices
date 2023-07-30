using Microsoft.Extensions.DependencyInjection;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices.Services;

namespace PhoneBookMicroservices
{
    public class ReportWorker : IHostedService
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ReportWorker(IMessageQueueService messageQueueService, IServiceScopeFactory serviceScopeFactory)
        {
            _messageQueueService = messageQueueService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(Start);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void Start()
        {
            while (true)
            {
                var reportIdStr = _messageQueueService.ReceiveMessageFromQueue("reportRequests");
                if (reportIdStr != null)
                {
                    var reportId = Guid.Parse(reportIdStr);

                    // Process the report (this is just a placeholder, replace with your actual report processing logic)
                    ProcessReport(reportId);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ContactDirectoryContext>();

                        // Update the report status to "Completed"
                        var report = context.Reports.Find(reportId);
                        report.Status = ReportStatus.Completed;
                        context.SaveChanges();
                    }
                }

                // Wait a bit before checking the queue again
                Thread.Sleep(1000);
            }
        }

       private void ProcessReport(Guid reportId)
        {
             using (var scope = _serviceScopeFactory.CreateScope())
            {
                 var context = scope.ServiceProvider.GetRequiredService<ContactDirectoryContext>();

                  // Get the report
                 var report = context.Reports.Find(reportId);

               
                  var location = report.Location;

 
                  var contactCount = context.ContactDetails.Count(cd => cd.InfoType == InfoType.Location && cd.InfoContent == location);


                  var phoneNumberCount = context.ContactDetails.Count(cd => cd.InfoType == InfoType.PhoneNumber && cd.InfoContent == location);

                 // Update the report with the calculated data
                 report.ContactCount = contactCount;
                 report.PhoneNumberCount = phoneNumberCount;

                 context.SaveChanges();
             }

                Thread.Sleep(5000);
        }

    }
}
