namespace Star_Security_Services.Models
{
    public class EmployeeQrCodeViewModel
    {

        public Employee Employee { get; set; }  // This will store the Employee object
        public string QRCodePath { get; set; }  // This will store the QR code image path
    }
}
