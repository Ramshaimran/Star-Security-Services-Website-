using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }




        public string Name { get; set; }
       
        public string Code { get; set; }

        public string email { get; set; }
        public string password { get; set; }
        public string Address { get; set; }

       
        public string ContactNumber { get; set; }

     

        public string Department { get; set; }

        public string Role { get; set; }


        public string Client { get; set; }

        public string Achievements { get; set; }

        //public string QRCodeImagePath { get; set; }
    }
}
