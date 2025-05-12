using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class EmployeeDash
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
      
    }
}
