using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class Admin
    {
        [Key]
        public int admin_id { get; set; }
        public string admin_name { get; set; }
        public string admin_email { get; set; }
        public string admin_password { get; set; }
        public string admin_image { get; set; }
       
    }
}
