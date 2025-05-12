using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class Services
    {
        [Key]
        public int ServiceID { get; set; }

        public string ServiceCode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ServiceType { get; set; }

        public string ServiceArea { get; set; }

        public bool IsActive { get; set; }
    }
}
