namespace PhoneBookMicroservices.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public DateTime RequestedAt { get; set; }
        public ReportStatus Status { get; set; }
        public string Location { get; set; }
        public int NumberOfContacts { get; set; }
        public int NumberOfPhoneNumbers { get; set; }
        public int ContactCount { get; set; } 
        public int PhoneNumberCount { get; set; } 
    }
    public enum ReportStatus
    {
        Preparing,
        Completed
    }
}
