using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Star_Security_Services.Models
{
    public class Final
    {
        [Key]
        public int F_id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string email { get; set; }

  
      
        [ForeignKey("Service")]
        public int ServiceID { get; set; }

  

    [Required(ErrorMessage = "Service name is required.")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Service code is required.")]
        public string ServiceCode { get; set; }

        [Required(ErrorMessage = "Service type is required.")]
        public string ServiceType { get; set; }

        [Required(ErrorMessage = "Service area is required.")]
        public string ServiceArea { get; set; }

        [Required(ErrorMessage = "Personnel is required.")]
        public string Personnel { get; set; }

        [Required(ErrorMessage = "Service duration is required.")]
        public string ServiceDuration { get; set; }


        [NotMapped]
        public bool IsEmployeeAssigned { get; set; }
    }
}
