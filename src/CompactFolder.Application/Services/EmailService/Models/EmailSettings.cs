namespace CompactFolder.Application.Services.EmailService.Models
{
    public class EmailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; } = false;
        public string DeliveryMethod { get; set; }
        public string PickupDirectoryLocation { get; set; }
        public string FromEmail { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public EmailSettings() { }
    }
}
