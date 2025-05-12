using Microsoft.AspNetCore.Mvc;
using Star_Security_Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.IO;
using QRCoder;
using SkiaSharp;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
using Star_Security_Services.Migrations;
using iText.Commons.Actions.Contexts;
using Microsoft.Extensions.Hosting;


namespace Star_Security_Services.Controllers
{
    public class AdminController : Controller
    {
        private readonly mycontext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(mycontext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }


            ViewBag.EmployeeCount = await _context.tbl_a_employee.CountAsync();
            ViewBag.ClientCount = await _context.tbl_clients.CountAsync();
            ViewBag.VacancyCount = await _context.tbl_vacan
                .Where(v => v.VacancyStatus.ToLower() == "open")
                .CountAsync();
            ViewBag.ServiceCount = await _context.tbl_service.CountAsync();

            return View();
        }




        public IActionResult AddEmployee()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]


        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {

                _context.tbl_a_employee.Add(employee);
                await _context.SaveChangesAsync();


                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {

                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(employee.Code, QRCodeGenerator.ECCLevel.Q);


                    using (var qrCode = new QRCoder.QRCode(qrCodeData))
                    {

                        int pixelSize = 20;
                        int width = qrCodeData.ModuleMatrix.Count * pixelSize;
                        int height = qrCodeData.ModuleMatrix.Count * pixelSize;

                        using (SKBitmap bitmap = new SKBitmap(width, height))
                        {
                            using (SKCanvas canvas = new SKCanvas(bitmap))
                            {

                                canvas.Clear(SKColors.White);


                                SKPaint paint = new SKPaint
                                {
                                    Color = SKColors.Black,
                                    IsAntialias = true,
                                    Style = SKPaintStyle.Fill
                                };

                                for (int i = 0; i < qrCodeData.ModuleMatrix.Count; i++)
                                {
                                    for (int j = 0; j < qrCodeData.ModuleMatrix[i].Count; j++)
                                    {
                                        if (qrCodeData.ModuleMatrix[i][j])
                                        {

                                            float x = j * pixelSize;
                                            float y = i * pixelSize;
                                            canvas.DrawRect(x, y, pixelSize, pixelSize, paint);
                                        }
                                    }
                                }
                            }

                            var fileName = $"{employee.Code}.png";
                            var qrCodesFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "qr_codes");

                            if (!Directory.Exists(qrCodesFolderPath))
                            {
                                Directory.CreateDirectory(qrCodesFolderPath);
                            }

                            var filePath = Path.Combine(qrCodesFolderPath, fileName);

                            using (var imageStream = System.IO.File.OpenWrite(filePath))
                            {
                                bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
                            }


                            var qrCodeEntry = new qrcode
                            {
                                EmployeeCode = employee.Code,
                                QRImagePath = $"/qr_codes/{fileName}"
                            };

                            _context.tbl_qrcode.Add(qrCodeEntry);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                TempData["SuccessMessage"] = "Employee added successfully with QR code!";
                return RedirectToAction("ManageEmployees");
            }

            TempData["ErrorMessage"] = "There was an error adding the employee.";
            return View(employee);
        }

        public async Task<IActionResult> ManageEmployees()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            var employeesWithQRCode = await _context.tbl_a_employee
                .Select(employee => new EmployeeQrCodeViewModel
                {
                    Employee = employee,
                    QRCodePath = _context.tbl_qrcode
                        .Where(q => q.EmployeeCode == employee.Code)
                        .Select(q => q.QRImagePath)
                        .FirstOrDefault()
                })
                .ToListAsync();

            if (!employeesWithQRCode.Any())
            {
                ViewData["Message"] = "No employees found.";
            }

            return View(employeesWithQRCode);
        }


        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
           
            var employee = await _context.tbl_a_employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }
           

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ManageEmployees));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.tbl_a_employee.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    ViewData["ConcurrencyError"] = "Another user has updated or deleted this employee.";
                }
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            
            var employee = await _context.tbl_a_employee.FindAsync(id);
            if (employee != null)
            {

                _context.tbl_a_employee.Remove(employee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Employee not found.";
            }


            return RedirectToAction("ManageEmployees");
        }



        [HttpGet]
        public IActionResult PostVacancy()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostVacancy(vacan vacancy)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                _context.tbl_vacan.Add(vacancy);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vacancy posted successfully!";
                return RedirectToAction("ManageVacancies");
            }

            return View(vacancy);
        }


        [HttpGet]
        public async Task<IActionResult> ManageVacancies()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            var vacancies = await _context.tbl_vacan.ToListAsync();
            if (!vacancies.Any())
            {
                ViewData["Message"] = "No vacancies found.";
            }

            return View(vacancies);
        }


        [HttpGet]
        public async Task<IActionResult> EditVacancy(int id)
        {
           
            var vacancy = await _context.tbl_vacan.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVacancy(int id, vacan vacancy)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            if (id != vacancy.VacancyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var existingVacancy = await _context.tbl_vacan.FindAsync(id);
                    if (existingVacancy == null)
                    {
                        return NotFound();
                    }


                    existingVacancy.JobTitle = vacancy.JobTitle;
                    existingVacancy.Department = vacancy.Department;
                    existingVacancy.JobDescription = vacancy.JobDescription;
                    existingVacancy.Location = vacancy.Location;
                    existingVacancy.VacancyStatus = vacancy.VacancyStatus;
                    existingVacancy.PostedDate = vacancy.PostedDate;
                    existingVacancy.ClosingDate = vacancy.ClosingDate;
                    existingVacancy.ExperienceRequired = vacancy.ExperienceRequired;
                    existingVacancy.EducationalQualification = vacancy.EducationalQualification;
                    existingVacancy.SalaryRange = vacancy.SalaryRange;
                    existingVacancy.VacancyLink = vacancy.VacancyLink;
                    existingVacancy.PostedBy = vacancy.PostedBy;
                    existingVacancy.NoOfOpenings = vacancy.NoOfOpenings;
                    existingVacancy.SkillsRequired = vacancy.SkillsRequired;


                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vacancy updated successfully!";
                    return RedirectToAction("ManageVacancies");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ViewData["Error"] = $"Error while updating the vacancy. Please try again later. {ex.Message}";
                    return View(vacancy);
                }
                catch (Exception ex)
                {

                    ViewData["Error"] = $"Unexpected error: {ex.Message}";
                    return View(vacancy);
                }
            }


            return View(vacancy);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVacancy(int id)
        {
            
            var vacancy = await _context.tbl_vacan.FindAsync(id);
            if (vacancy != null)
            {
                _context.tbl_vacan.Remove(vacancy);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vacancy deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Vacancy not found.";
            }

            return RedirectToAction("ManageVacancies");
        }


        [HttpGet]
        public IActionResult AddService()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddService(Services service)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                service.ServiceCode = $"SRV-{DateTime.Now.Ticks}";

                _context.tbl_service.Add(service);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Service added successfully!";
                return RedirectToAction("ManageServices");
            }

            TempData["ErrorMessage"] = "Please fill out all required fields.";
            return View(service);
        }


        [HttpGet]
        public async Task<IActionResult> ManageServices()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            var services = await _context.tbl_service.ToListAsync();
            return View(services);
        }


        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
           
            var service = await _context.tbl_service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, Services updatedService)
        {
            if (id != updatedService.ServiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var service = await _context.tbl_service.FindAsync(id);
                    if (service == null)
                        return NotFound();

                    service.Title = updatedService.Title;
                    service.Description = updatedService.Description;
                    service.ServiceCode = updatedService.ServiceCode;
                    service.ServiceType = updatedService.ServiceType;
                    service.ServiceArea = updatedService.ServiceArea;

                    service.IsActive = updatedService.IsActive;


                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Service updated successfully!";
                    return RedirectToAction(nameof(ManageServices));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Something went wrong while updating.";
                }
            }

            return View(updatedService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            
            var service = await _context.tbl_service.FindAsync(id);
            if (service != null)
            {
                _context.tbl_service.Remove(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Service deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Service not found.";
            }

            return RedirectToAction(nameof(ManageServices));
        }
        public IActionResult AddClient()
        {
          
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClient(User client)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }


            if (ModelState.IsValid)
            {
                _context.tbl_user.Add(client);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Client added successfully!";
                return RedirectToAction("ManageClients");
            }

            return View(client);
        }


        [HttpGet]
        public async Task<IActionResult> ManageClients()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            var clients = await _context.tbl_user.ToListAsync();
            return View(clients);
        }
        [HttpGet]
        public async Task<IActionResult> EditClient(int id)
        {
            var user = await _context.tbl_user.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClient(int id, User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.tbl_user.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }


                    existingUser.username = updatedUser.username;
                    existingUser.Email = updatedUser.Email;
                    existingUser.CompanyName = updatedUser.CompanyName;
                    existingUser.PhoneNumber = updatedUser.PhoneNumber;
                    existingUser.Address = updatedUser.Address;



                    if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                    {
                        existingUser.Password = updatedUser.Password;
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client updated successfully!";
                    return RedirectToAction(nameof(ManageClients));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.tbl_user.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(updatedUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var user = await _context.tbl_user.FindAsync(id);
            if (user != null)
            {
                _context.tbl_user.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Client deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Client not found.";
            }

            return RedirectToAction(nameof(ManageClients));
        }









        //public IActionResult ScanOrUploadQRCode()
        //{
        //    return View();
        //}
     
        //public IActionResult ProcessScannedQRCode([FromBody] QRCodeRequest qrCodeData)
        //{
        //    if (string.IsNullOrEmpty(qrCodeData.QRCodeData))
        //    {
        //        return Json(new { success = false, message = "Invalid QR Code data." });
        //    }

        //    // Search for the employee by QR Code
        //    var employee = _context.tbl_a_employee
        //        .Where(e => e.Code.Trim().Equals(qrCodeData.QRCodeData.Trim(), StringComparison.OrdinalIgnoreCase))
        //        .FirstOrDefault();

        //    if (employee == null)
        //    {
        //        return Json(new { success = false, message = "No employee found for this QR code." });
        //    }

        //    // Return employee details
        //    return Json(new
        //    {
        //        success = true,
        //        employee = new
        //        {
        //            Code = employee.Code,
        //            Name = employee.Name,
        //            Department = employee.Department,
        //            Role = employee.Role,
        //            ContactNumber = employee.ContactNumber
        //        }
        //    });
        //}
        
    

    public class QRCodeRequest
    {
        public string QRCodeData { get; set; }
}











        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(login model)
        {
            if (ModelState.IsValid)
            {
                // 🔒 Try Admin login
                var admin = _context.tbl_admin
                    .FirstOrDefault(a => a.admin_email == model.Email && a.admin_password == model.Password);

                if (admin != null)
                {
                    // Clear all session keys before setting new ones
                    HttpContext.Session.Clear();

                    HttpContext.Session.SetString("admin_email", admin.admin_email);
                    return RedirectToAction("Index", "Admin");
                }

                // 👤 Try Employee login
                var employee = _context.tbl_a_employee
                    .FirstOrDefault(e => e.email == model.Email && e.password == model.Password);

                if (employee != null)
                {
                    HttpContext.Session.Clear();

                    HttpContext.Session.SetString("employee_code", employee.Code);
                    HttpContext.Session.SetString("employee_name", employee.Name);
                    HttpContext.Session.SetString("user_email", employee.email); // Shared for employees & users
                    return RedirectToAction("Index", "Employee");
                }

                // 👨‍💼 Try User login
                var user = _context.tbl_user
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.Clear();

                    HttpContext.Session.SetString("user_email", user.Email);
                    HttpContext.Session.SetString("username", user.username ?? "");
                    HttpContext.Session.SetString("profile_image", user.ProfileImage ?? "default-image.png");
                    return RedirectToAction("Home", "User");
                }

                // ❌ If no match found
                TempData["ErrorMessage"] = "Invalid email or password.";
            }

            // If model is invalid or login failed
            return View(model);
        }




        private void EnsureEmployeeLoggedIn()
        {
            // Check if employee is logged in, otherwise redirect to login page
            if (HttpContext.Session.GetString("employee_code") == null)
            {
                // Throwing a redirect to be handled by the caller
                Response.Redirect("/Admin/Login"); // Redirect to login page
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Admin");
        }
        public IActionResult AddTraining()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTraining([Bind("Title,Description,Department,Mode,StartDate,EndDate,TrainerName")] Training training)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageTrainings", "Admin");
            }
            return View(training);
        }
        public async Task<IActionResult> ManageTrainings()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            var trainings = await _context.tbl_training.ToListAsync();
            return View(trainings);
        }


        [HttpGet]
        public IActionResult EditTraining(int id)
        {
            var training = _context.tbl_training.Find(id);
            if (training == null)
            {
                return NotFound();
            }
            return View(training);
        }


        [HttpPost]
        public IActionResult EditTraining(Training model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = _context.tbl_training.Find(model.TrainingID);
            if (existing == null)
            {
                return NotFound();
            }


            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Department = model.Department;
            existing.Mode = model.Mode;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.TrainerName = model.TrainerName;

            _context.SaveChanges();

            return RedirectToAction("ManageTrainings");
        }

        [HttpPost]
        public IActionResult DeleteTraining(int id)
        {
            var training = _context.tbl_training.Find(id);
            if (training == null)
            {
                return NotFound();
            }

            _context.tbl_training.Remove(training);
            _context.SaveChanges();

            return RedirectToAction("ManageTrainings");
        }



        private string GetTrainingNameById(int trainingId)
        {

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "trainings.txt");

            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int id) && id == trainingId)
                    {
                        return parts[1];
                    }
                }
            }

            return "Unknown Training";
        }








        public IActionResult ViewApplicants(int trainingId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("admin_email")))
            {
                return RedirectToAction("Login", "Admin");
            }

            string logPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "training_applications.txt");

            List<dynamic> applicants = new List<dynamic>();
            string trainingTitle = "";


            var training = _context.tbl_training.FirstOrDefault(t => t.TrainingID == trainingId);
            if (training != null)
            {
                trainingTitle = training.Title;
            }

            if (System.IO.File.Exists(logPath))
            {
                var lines = System.IO.File.ReadAllLines(logPath);

                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 5 && int.Parse(parts[0]) == trainingId)
                    {
                        applicants.Add(new
                        {
                            Name = parts[1],
                            Email = parts[2],
                            Code = parts[3],
                            AppliedOn = parts[4]
                        });
                    }
                }
            }

            ViewBag.TrainingId = trainingId;
            ViewBag.TrainingName = trainingTitle;

            return View(applicants);
        }

        private bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx");
                    client.EnableSsl = true;

                    var mail = new MailMessage("starservices921@gmail.com", toEmail, subject, body);
                    mail.IsBodyHtml = false;
                    client.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false;
            }
        }







        [HttpPost]
        public IActionResult SendRegistrationConfirmation(string email, string name)
        {
            string subject = "Training Registration Confirmed";
            string body = $"Dear {name},\n\nYour registration for the training has been confirmed.\n\nRegards,\nStar Security Team";

            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx");
                    client.EnableSsl = true;

                    var mail = new MailMessage("starservices921@gmail.com", email, subject, body);
                    client.Send(mail);
                }

                TempData["SuccessMessage"] = "Confirmation email sent successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to send confirmation email.";
            }

            return RedirectToAction("ManageTrainings");
        }





        public async Task<IActionResult> ViewApplicantsForVacancy(int vacancyId)
        {
            

            Console.WriteLine($"Fetching applicants for VacancyId: {vacancyId}");


            var applicants = await _context.tbl_vacancyapplication
                                           .Where(a => a.VacancyId == vacancyId)
                                           .ToListAsync();


            Console.WriteLine($"Number of applicants found: {applicants.Count}");


            return View(applicants);
        }

        [HttpPost]
        public async Task<IActionResult> HandleApplicationForVacancy(int applicationId, string actionType)
        {
            var application = await _context.tbl_vacancyapplication
                                            .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Application not found.";
                return RedirectToAction("ViewApplicantsForVacancy", new { vacancyId = 0 });
            }

            string subject = actionType == "Confirm"
                ? "Job Application Confirmed"
                : "Job Application Rejected";

            string message = actionType == "Confirm"
                ? $"Dear {application.UserName},\n\nYour application for the vacancy \"{application.JobTitle}\" has been confirmed.\n\nRegards,\nStar Security Team"
                : $"Dear {application.UserName},\n\nYour application for the vacancy \"{application.JobTitle}\" has been rejected.\n\nRegards,\nStar Security Team";

            bool emailSent = SendEmail(application.UserEmail, subject, message);

            if (emailSent)
                TempData["SuccessMessage"] = $"Email {actionType.ToLower()}ed to {application.UserEmail} successfully.";
            else
                TempData["ErrorMessage"] = "Failed to send the email.";

            return RedirectToAction("ViewApplicantsForVacancy", new { vacancyId = application.VacancyId });
        }

        public IActionResult ManageBookings()
        {
            var bookings = _context.tbl_final.ToList();

            foreach (var booking in bookings)
            {

                booking.IsEmployeeAssigned = _context.tbl_assign.Any(a => a.BookingId == booking.F_id);
            }

            return View(bookings);
        }



        [HttpGet]
        public IActionResult BookEmployee(string serviceType, int bookingId)
        {

            var employees = _context.tbl_a_employee
                .Where(e => e.Department == serviceType)
                .ToList();

            ViewBag.ServiceType = serviceType;
            ViewBag.BookingId = bookingId;

            return View(employees);
        }
        [HttpPost]
        public IActionResult AssignEmployees(List<int> selectedEmployees, int bookingId)
        {
            if (selectedEmployees == null || !selectedEmployees.Any())
            {
                TempData["ErrorMessage"] = "No employees selected for assignment.";
                return RedirectToAction("BookEmployee", new { serviceType = "YourServiceType", bookingId = bookingId });
            }

            var booking = _context.tbl_final.FirstOrDefault(b => b.F_id == bookingId);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Invalid booking ID.";
                return RedirectToAction("ManageBookings");
            }

            var assignedEmployees = new List<Assign>();

            foreach (var empId in selectedEmployees)
            {
                var employee = _context.tbl_a_employee.FirstOrDefault(e => e.Id == empId);
                if (employee != null)
                {
                    var assignment = new Assign
                    {

                        Username = booking.username,
                        Email = booking.email,
                        ServiceName = booking.ServiceName,
                        ServiceCode = booking.ServiceCode,
                        ServiceType = booking.ServiceType,
                        ServiceArea = booking.ServiceArea,
                        Personnel = booking.Personnel,
                        ServiceDuration = booking.ServiceDuration,
                        BookingId = bookingId,

                        EmployeeName = employee.Name,
                        EmployeeEmail = employee.email,
                        EmployeeDepartment = employee.Department
                    };

                    _context.tbl_assign.Add(assignment);
                    assignedEmployees.Add(assignment);
                }
            }

            _context.SaveChanges();

            string userEmailBody = $@"
<h2>Booking Confirmation</h2>
<p><strong>Username:</strong> {booking.username}</p>
<p><strong>Email:</strong> {booking.email}</p>
<p><strong>Service:</strong> {booking.ServiceName} ({booking.ServiceCode})</p>
<p><strong>Type:</strong> {booking.ServiceType}</p>
<p><strong>Area:</strong> {booking.ServiceArea}</p>
<p><strong>Personnel:</strong> {booking.Personnel}</p>
<p><strong>Duration:</strong> {booking.ServiceDuration}</p>
<hr>
<h3>Assigned Employees:</h3>
<ul>";


            foreach (var emp in assignedEmployees)
            {
                userEmailBody += $"<li><strong>{emp.EmployeeName}</strong> ({emp.EmployeeEmail}) - {emp.EmployeeDepartment}</li>";
            }

            userEmailBody += "</ul>";


            SendEmailToUser(booking.email, "Your Service Booking & Assigned Employees", userEmailBody);


            foreach (var emp in assignedEmployees)
            {
                string employeeEmailBody = $@"
<h2>Employee Assignment Notification</h2>
<p>Hi {emp.EmployeeName},</p>
<p>You have been assigned to the following service booking:</p>
<p><strong>Service:</strong> {booking.ServiceName} ({booking.ServiceCode})</p>
<p><strong>Booking Username:</strong> {booking.username}</p>
<p><strong>Booking Email:</strong> {booking.email}</p>
<p><strong>Service Type:</strong> {booking.ServiceType}</p>
<p><strong>Area:</strong> {booking.ServiceArea}</p>
<p><strong>Personnel:</strong> {booking.Personnel}</p>
<p><strong>Duration:</strong> {booking.ServiceDuration}</p>
<hr>
<p>Please reach out if you have any questions regarding the assignment.</p>
<p>Regards,<br>Star Security Team</p>";


                SendEmailToEmployee(emp.EmployeeEmail, "Assigned to Service Booking", employeeEmailBody);
            }

            TempData["SuccessMessage"] = "Employees successfully assigned and user and employees notified via email.";
            return RedirectToAction("ManageBookings");
        }


        private void SendEmailToUser(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx");
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage("starservices921@gmail.com", toEmail, subject, htmlBody);
                    mailMessage.IsBodyHtml = true;

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send email to user. Error: {ex.Message}";
            }
        }

        private void SendEmailToEmployee(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx");
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage("starservices921@gmail.com", toEmail, subject, htmlBody);
                    mailMessage.IsBodyHtml = true;

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send email to employee. Error: {ex.Message}";
            }
        }


        public IActionResult ManageAssignments()
        {
          
            var assignments = _context.tbl_assign.ToList();
            return View(assignments);
        }

        public IActionResult AssignmentDetails(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "Admin"); // Modify "Account" to your actual controller name
            }
            var assign = _context.tbl_assign.FirstOrDefault(a => a.A_id == id);
            if (assign == null) return NotFound();
            return View(assign);
        }

        [HttpGet]
        public async Task<IActionResult> EditAssignment(int id)
        {
            var assignment = await _context.tbl_assign.FindAsync(id);
            if (assignment == null)
                return NotFound();

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(Assign updated)
        {
            if (!ModelState.IsValid)
                return View(updated);

            _context.Update(updated);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Assignment updated successfully.";
            return RedirectToAction("ManageAssignments");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAssignment(int id)
        {
            var assign = _context.tbl_assign.FirstOrDefault(a => a.A_id == id);
            if (assign == null) return NotFound();

            _context.tbl_assign.Remove(assign);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Assignment deleted successfully.";
            return RedirectToAction("ManageAssignments");
        }

        public IActionResult ManageTestimonials()
        {
            var testimonials = _context.tbl_testimonal.ToList();
            var contacts = _context.tbl_contact.ToList();

            ViewBag.Contacts = contacts;
            return View(testimonials);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var testimonial = _context.tbl_testimonal.FirstOrDefault(t => t.Id == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            _context.tbl_testimonal.Remove(testimonial);
            _context.SaveChanges();

            TempData["Success"] = "Testimonial deleted successfully.";
            return RedirectToAction("ManageTestimonials");
        }


        [HttpGet]
        public IActionResult DeleteContact(int id)
        {
            var contact = _context.tbl_contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.tbl_contact.Remove(contact);
            _context.SaveChanges();

            TempData["Success"] = "Contact message deleted successfully.";
            return RedirectToAction("ManageTestimonials");
        }

        public IActionResult UpdateProfile()
        {
            string adminEmail = HttpContext.Session.GetString("admin_email");
            var admin = _context.tbl_admin.FirstOrDefault(a => a.admin_email == adminEmail);

            if (admin == null)
                return RedirectToAction("Login", "Account"); // Or handle unauthorized access

            var model = new Admin
            {
                admin_email = admin.admin_email,
                admin_name = admin.admin_name,
                admin_password = admin.admin_password
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(Admin model)
        {
            if (ModelState.IsValid)
            {
                string currentEmail = HttpContext.Session.GetString("admin_email");
                var admin = _context.tbl_admin.FirstOrDefault(a => a.admin_email == currentEmail);

                if (admin != null)
                {
                    admin.admin_email = model.admin_email;
                    admin.admin_name = model.admin_name;
                    admin.admin_password = model.admin_password;

                    _context.SaveChanges();

                    // Update session if email was changed
                    HttpContext.Session.SetString("admin_email", model.admin_email);

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Admin not found.";
            }

            return View(model);
        }

    }
}
