using System.ComponentModel.DataAnnotations;
namespace Star_Security_Services.Models
{
    public class Assign
    {
        [Key]
        public int A_id { get; set; }

        // Booking Info
        public string Username { get; set; }
        public string Email { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceType { get; set; }
        public string ServiceArea { get; set; }
        public string Personnel { get; set; }
        public string ServiceDuration { get; set; }
        public int BookingId { get; set; }

        // Assigned Employee Info
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeDepartment { get; set; }
    }
}
