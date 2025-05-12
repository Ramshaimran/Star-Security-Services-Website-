using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Star_Security_Services.Models
{
    public class Training
    {
        [Key]
        public int TrainingID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Department { get; set; }

        public string Mode { get; set; } // Online / In-Person

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TrainerName { get; set; }

        
    }

}
