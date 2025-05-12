using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class login
    {
        [Key]
        public int Logid { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
