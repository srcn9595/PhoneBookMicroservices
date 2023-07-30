namespace PhoneBookMicroservices.Models
{
    public class Contact
    {
        private string _name; // Kullanmak üzere private bir alan oluşturduk.

        public Guid Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be empty or null.");
                }
                _name = value;
            }
        }

        public string Surname { get; set; }
        public string Company { get; set; }
    }
}
