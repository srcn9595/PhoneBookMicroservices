using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneBookMicroservices.Models
{
    public class ContactDetail
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public int InfoTypeValue { get; set; } 
        public string InfoContent { get; set; }

        [NotMapped] 
        public InfoType InfoType
        {
            get { return (InfoType)InfoTypeValue; }
            set { InfoTypeValue = (int)value; }
        }
    }
    public enum InfoType
    {
        PhoneNumber = 1,
        Email = 2,
        Location = 3
    }
}
