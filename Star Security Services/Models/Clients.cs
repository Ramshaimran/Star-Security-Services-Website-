using System.ComponentModel.DataAnnotations;

namespace Star_Security_Services.Models
{
    public class Clients
    {
        [Key]
        public int ClientID { get; set; }           // Unique ID for each client

        public string CompanyName { get; set; }     // Name of the client company

        public string ContactPerson { get; set; }   // Person to contact from client side

        public string Email { get; set; }    
        // Email ID of the client

        public string PhoneNumber { get; set; }     // Contact number

        public string Address { get; set; }         // Office or billing address

        public string Industry { get; set; }        // Industry type like banking, logistics, etc.

        public string ServicesAvailed { get; set; } // Which services they are using

        public string ClientStatus { get; set; }    // Active / Inactive status
    }
}
