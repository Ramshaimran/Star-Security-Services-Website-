using Microsoft.AspNetCore.Mvc;
using Star_Security_Services.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Star_Security_Services.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly mycontext _context;


        public EmployeeController(mycontext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }
            //// Check if the user is authenticated
            //if (!User.Identity.IsAuthenticated)
            //{
            //    // If the user is not authenticated, redirect to the login page
            //    return RedirectToAction("Login", "Admin"); // Modify "Account" to your actual controller name
            //}

            // If the user is authenticated, show the index view
            return View();
        }


        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }
            // Get the logged-in employee's code from the session
            string empCode = HttpContext.Session.GetString("employee_code");

            if (string.IsNullOrEmpty(empCode))
                return RedirectToAction("Login", "Admin");

          

            // Fetch employee details
            var employee = _context.tbl_a_employee.FirstOrDefault(e => e.Code == empCode);

            if (employee == null)
                return NotFound("Employee not found.");

            // Fetch the corresponding QR code details from the tbl_qrcodes table
            var qr = _context.tbl_qrcode.FirstOrDefault(q => q.EmployeeCode == empCode);

            // Prepare the view model with employee and QR code info
            var viewModel = new EmployeeQrCodeViewModel
            {
                Employee = employee,
                QRCodePath = qr?.QRImagePath // If QR exists, use it, else null
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            string empCode = HttpContext.Session.GetString("employee_code");

            if (string.IsNullOrEmpty(empCode))
                return RedirectToAction("Login", "Admin");
            

            var employee = _context.tbl_a_employee.FirstOrDefault(e => e.Code == empCode);
            if (employee == null)
                return NotFound();

            return View("UpdateProfile", employee);
        }

        // POST: Employee/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(Employee updatedEmployee)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }
            string empCode = HttpContext.Session.GetString("employee_code");
           
            if (string.IsNullOrEmpty(empCode))
                return RedirectToAction("Login", "Admin");

            var employee = _context.tbl_a_employee.FirstOrDefault(e => e.Id == updatedEmployee.Id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return View("UpdateProfile", updatedEmployee);
            }

            // Update properties
            employee.Name = updatedEmployee.Name;
            employee.email = updatedEmployee.email;

            // Only update password if it’s entered
            if (!string.IsNullOrWhiteSpace(updatedEmployee.password))
            {
                employee.password = updatedEmployee.password;
            }

            employee.ContactNumber = updatedEmployee.ContactNumber;
            employee.Address = updatedEmployee.Address;
            employee.Department = updatedEmployee.Department;
            employee.Role = updatedEmployee.Role;
            employee.Client = updatedEmployee.Client;
            employee.Achievements = updatedEmployee.Achievements;

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
        // GET: Employee/Colleagues
        public IActionResult Colleagues()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }


            var employees = _context.tbl_a_employee.ToList();
            return View(employees); // Pass the list directly to the view
        }



        public IActionResult Trainings()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }

            var trainings = _context.tbl_training.ToList();
            return View("trainings", trainings);  // Specify lowercase view name
        }

        [HttpPost]
        public IActionResult ApplyForTraining(int trainingId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }

            string empCode = HttpContext.Session.GetString("employee_code");

            if (string.IsNullOrEmpty(empCode))
                return RedirectToAction("Trainings", "Employee");

            var employee = _context.tbl_a_employee.FirstOrDefault(e => e.Code == empCode);
            var training = _context.tbl_training.FirstOrDefault(t => t.TrainingID == trainingId);

            if (employee == null || training == null)
                return NotFound();

            // ✅ Save to file
            try
            {
                string logPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "training_applications.txt");

                // Ensure the App_Data folder exists
                var folder = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string logLine = $"{trainingId}|{employee.Name}|{employee.email}|{employee.Code}|{DateTime.Now}\n";

                System.IO.File.AppendAllText(logPath, logLine);  // 👈 Write the log

                // Send confirmation email (optional)
                var toEmail = employee.email;
                var subject = "Training Application Confirmation";
                var body = $@"
Dear {employee.Name},

You have successfully applied for the training: {training.Title}.

Details:
Department: {training.Department}
Mode: {training.Mode}
Trainer: {training.TrainerName}
Start Date: {training.StartDate:dd MMM yyyy}
End Date: {training.EndDate:dd MMM yyyy}

Thank you,
Star Security Team
";

                using (var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new System.Net.NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx"); // Update
                    client.EnableSsl = true;

                    var mail = new System.Net.Mail.MailMessage("starservices921@gmail.com", toEmail, subject, body);
                    client.Send(mail);
                }

                TempData["Success"] = "You have successfully applied for the training. A confirmation email has been sent.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to apply or send email. Please contact support.";
            }

            return RedirectToAction("Trainings");
        }

        public async Task<IActionResult> Vacancies()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }
            var vacancies = await _context.tbl_vacan.ToListAsync();
            return View(vacancies);
        }

        //[HttpPost]
        //public IActionResult ApplyForVacancy(int vacancyId)
        //{
        //    var employeeCode = HttpContext.Session.GetString("employee_code");

        //    if (string.IsNullOrEmpty(employeeCode))
        //    {
        //        TempData["Error"] = "You must be logged in to apply.";
        //        return RedirectToAction("Login", "Admin");
        //    }

        //    var employee = _context.tbl_a_employee.FirstOrDefault(e => e.Code == employeeCode);
        //    var vacancy = _context.tbl_vacan.FirstOrDefault(v => v.VacancyID == vacancyId);

        //    if (employee == null || vacancy == null)
        //    {
        //        TempData["Error"] = "Something went wrong. Please try again.";
        //        return RedirectToAction("Vacancies");
        //    }

        //    // Check if already applied
        //    var existingApp = _context.tbl_vacancyapplication
        //        .FirstOrDefault(a => a.VacancyId == vacancyId && a.EmployeeCode == employeeCode);

        //    if (existingApp != null)
        //    {
        //        TempData["Error"] = "You've already applied for this vacancy.";
        //        return RedirectToAction("Vacancies");
        //    }

        //    var application = new VacancyApplication
        //    {
        //        VacancyId = vacancyId,
        //        EmployeeCode = employee.Code,
        //        EmployeeName = employee.Name,
        //        EmployeeEmail = employee.email,
        //        AppliedOn = DateTime.Now,
        //        JobTitle = vacancy.JobTitle,
        //        Department = vacancy.Department,
        //        Location = vacancy.Location
        //    };

        //    _context.tbl_vacancyapplication.Add(application);
        //    _context.SaveChanges();

        //    // ✅ Send email
        //    try
        //    {
        //        using (var client = new SmtpClient("smtp.gmail.com", 587))
        //        {
        //            client.UseDefaultCredentials = false;
        //            client.Credentials = new NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx");
        //            client.EnableSsl = true;

        //            var mail = new MailMessage();
        //            mail.From = new MailAddress("starservices921@gmail.com", "Star Security Services");
        //            mail.To.Add(employee.email);
        //            mail.Subject = "Application Received - " + vacancy.JobTitle;
        //            mail.Body = $"Dear {employee.Name},\n\n" +
        //                        $"Thank you for applying for the position of {vacancy.JobTitle} in the {vacancy.Department} department.\n" +
        //                        $"We will review your application and contact you if you're shortlisted.\n\n" +
        //                        $"Regards,\nStar Security Services";

        //            client.Send(mail);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // You can log this error for debugging
        //        Console.WriteLine("Email send failed: " + ex.Message);
        //    }

        //    TempData["Success"] = "Application submitted successfully!";
        //    return RedirectToAction("Vacancies");
        //}
        public async Task<IActionResult> ClientAssignments()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("employee_code")))
            {
                return RedirectToAction("Login", "Admin");
            }
            // Get the logged-in user's email from the session
            var userEmail = HttpContext.Session.GetString("user_email");

            // Check if the user is logged in
            if (string.IsNullOrEmpty(userEmail))
            {
                // If no user is logged in, redirect to login or show an error message
                TempData["ErrorMessage"] = "You are not logged in.";
                return RedirectToAction("Login", "Admin");
            }

            // Fetch the assignments for the logged-in employee using the email
            var assignments = await _context.tbl_assign
                .Where(a => a.EmployeeEmail == userEmail)
                .ToListAsync();

            // Pass the assignments to the view
            return View(assignments);
        }


    }
}
