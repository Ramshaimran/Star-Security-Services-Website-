using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class Testimonials
    {
        [Key]
        public int Id { get; set; }

     
        

       public string Username { get; set; }
        public string Image { get; set; }

        [Required]
        [StringLength(1000)]
        public string Feedback { get; set; }
    }
}
