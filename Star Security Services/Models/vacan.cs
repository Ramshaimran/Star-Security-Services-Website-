using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class vacan
    {
        [Key]
        public int VacancyID { get; set; } // Unique identifier for the vacancy

        public string JobTitle { get; set; } // Job title (e.g., Security Guard, Supervisor)

        public string Department { get; set; } // Department name (e.g., Manned Guarding, Cash Services, etc.)

        public string JobDescription { get; set; } // Detailed description of the job

        public string Location { get; set; } // Location of the vacancy (e.g., Mumbai, North Region)

        public string VacancyStatus { get; set; } // Status of the vacancy (e.g., "Open", "Closed")

        public string PostedDate { get; set; } // Date when the vacancy was posted

        public string ClosingDate { get; set; } // Last date to apply for the vacancy

        public int ExperienceRequired { get; set; } // Number of years of experience required

        public string EducationalQualification { get; set; } // Educational qualification required (e.g., High School, Bachelor's Degree)

        public string SalaryRange { get; set; } // Salary range for the role (e.g., ₹20,000 - ₹30,000)

        public string VacancyLink { get; set; } // Link to apply for the vacancy

        public string PostedBy { get; set; } // Person or system that posted the vacancy (e.g., HR Admin)

        public int NoOfOpenings { get; set; } // Number of available positions for this vacancy

        public string SkillsRequired { get; set; } // Skills required for the job (e.g., First Aid, Surveillance Skills)
    }
}
