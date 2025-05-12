using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

      

        [Required]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CompanyName { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateSent { get; set; } = DateTime.Now;
    }

}
