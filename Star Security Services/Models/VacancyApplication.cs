using System;
using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class VacancyApplication
    {
        [Key]
        public int Id { get; set; }

        public int VacancyId { get; set; }

        // Replacing EmployeeCode with UserId (using Id from User model)
        [Required]
        public int UserId { get; set; }

        // Renaming EmployeeName and EmployeeEmail to UserName and UserEmail to reflect User model
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public DateTime AppliedOn { get; set; }

        public string JobTitle { get; set; }

        public string Department { get; set; }

        public string Location { get; set; }
    }
}
