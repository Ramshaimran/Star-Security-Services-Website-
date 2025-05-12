namespace Star_Security_Services.Models
{
    public class qrcode
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }  // This links to Employee.Code
        public string QRImagePath { get; set; }       // QR Code image stored as byte array
    }

}
