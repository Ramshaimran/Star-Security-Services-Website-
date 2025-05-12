using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For Session
using Star_Security_Services.Models; // Your Models
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Star_Security_Services.Migrations;
using System.Net.Mail;
using System.Net;

namespace Star_Security_Services.Controllers
{
    public class UserController : Controller
    {
        private readonly mycontext _context;

        public UserController(mycontext context)
        {
            _context = context;
        }

        // Home view
        public IActionResult Home()
        {
            string userEmail = Request.Query["user_email"];
            string userName = Request.Query["username"];

            ViewData["UserEmail"] = userEmail;
            ViewData["Username"] = userName;

            // Fetch the latest 4 testimonials by sorting them by ID or creation date in descending order
            var testimonials = _context.tbl_testimonal
                                       .OrderByDescending(t => t.Id)  // Assuming 'Id' is an auto-increment primary key
                                       .Take(4)  // Take only the top 4
                                       .ToList();

            ViewData["Testimonials"] = testimonials;

            return View();
        }




        // Services view
        public IActionResult Services()
        {
            var services = _context.tbl_service
                                   .Where(s => s.IsActive)
                                   .ToList();

            return View(services);
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(User model, IFormFile ProfileImage)
        {
            Console.WriteLine("Signup POST hit");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Model Error: " + error.ErrorMessage);
                }
                return View(model);
            }

            // Debugging: Check if ProfileImage is received
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                Console.WriteLine("Profile Image uploaded: " + ProfileImage.FileName);
            }
            else
            {
                Console.WriteLine("No Profile Image uploaded.");
            }

            // Handle the profile image only if it's uploaded
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/users");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Path.GetFileName(ProfileImage.FileName);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(stream);
                }

                model.ProfileImage = uniqueFileName;
            }
            else
            {
                Console.WriteLine("No Profile Image uploaded.");
                model.ProfileImage = "default-image.png"; // Default if no file uploaded
            }

            _context.tbl_user.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login", "Admin");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.tbl_user.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    var client = _context.tbl_clients.FirstOrDefault(c => c.Email == user.Email);
                    if (client != null)
                    {
                        // ✅ Save user info in session
                        HttpContext.Session.SetString("user_email", user.Email);
                        HttpContext.Session.SetString("username", user.username ?? client.ContactPerson);
                        HttpContext.Session.SetString("client_email", client.Email);
                        HttpContext.Session.SetString("client_name", client.ContactPerson);

                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        TempData["Error"] = "You are not registered as a client. Contact admin.";
                    }
                }
                else
                {
                    TempData["Error"] = "Invalid email or password.";
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Home");
        }
        // POST: BookService
        // GET: BookService
        [HttpGet]
        public IActionResult BookServices(int id)
        {
            string userEmail = HttpContext.Session.GetString("user_email");
            string userName = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }

            var service = _context.tbl_service.FirstOrDefault(s => s.ServiceID == id);
            if (service == null)
            {
                return NotFound();
            }

            var finalBooking = new Final
            {
                ServiceID = service.ServiceID,          // Set the ServiceID here
                ServiceName = service.Title,            // Set the ServiceName here
                ServiceCode = service.ServiceCode,      // Set the ServiceCode here
                ServiceType = service.ServiceType,      // Set the ServiceType here
                ServiceArea = service.ServiceArea,      // Set the ServiceArea here
                email = userEmail,
                username = userName
            };

            return View(finalBooking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookServices(Final finalServiceBooking)
        {
            Console.WriteLine("Post method hit");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState Invalid");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(finalServiceBooking);
            }

            Console.WriteLine("Saving booking...");
            _context.tbl_final.Add(finalServiceBooking);
            _context.SaveChanges();
            Console.WriteLine("Booking saved");

            return RedirectToAction("home");
        }

        [HttpGet]
        public IActionResult Testimonal()
        {
            var allTestimonials = _context.tbl_testimonal.ToList(); // Fetch all feedbacks
            return View(allTestimonials); // Pass list to the view
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Testimonal(Testimonials testimonial)
        {
            // Check if the user is logged in
            var userName = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(userName))
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "Admin"); // Change to your login route
            }

            // If the user is logged in, proceed with the form submission
            if (ModelState.IsValid)
            {
                // Set the username and image from the session
                testimonial.Username = userName;
                testimonial.Image = HttpContext.Session.GetString("profile_image") ?? "default-image.png";

                // Add the testimonial to the database
                _context.tbl_testimonal.Add(testimonial);
                _context.SaveChanges();

                // Set a success message
                TempData["Success"] = "Thank you for your feedback!";

                // Redirect back to the testimonial page
                return RedirectToAction("testimonal");
            }

            // If validation fails, return the view with the current list and form
            var allTestimonials = _context.tbl_testimonal.ToList();
            return View(allTestimonials);
        }


        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CareersWithUs()
        {
            var userEmail = HttpContext.Session.GetString("user_email");

            // Retrieve open vacancies
            var vacancies = await _context.tbl_vacan
                .Where(v => v.VacancyStatus == "Open")
                .ToListAsync();

            // Check if the user is logged in and get the list of applied vacancies
            List<int> appliedVacancies = new List<int>();
            if (!string.IsNullOrEmpty(userEmail))
            {
                appliedVacancies = _context.tbl_vacancyapplication
                    .Where(a => a.UserEmail == userEmail)
                    .Select(a => a.VacancyId)
                    .ToList();
            }

            // Filter out the vacancies the user has already applied for
            var vacanciesToShow = vacancies.Where(v => !appliedVacancies.Contains(v.VacancyID)).ToList();

            // Pass the filtered vacancies to the view
            ViewData["AppliedVacancies"] = appliedVacancies;

            return View(vacanciesToShow);
        }




        [HttpPost]
        public IActionResult ApplyForVacancy(int vacancyId)
        {
            var userEmail = HttpContext.Session.GetString("user_email");

            if (string.IsNullOrEmpty(userEmail))
            {
                // Store the target URL (CareersWithUs) in TempData
                TempData["RedirectUrl"] = Url.Action("CareersWithUs", "User");

                // Redirect to login page
                return RedirectToAction("Login", "Admin");
            }

            var user = _context.tbl_user.FirstOrDefault(u => u.Email == userEmail);
            var vacancy = _context.tbl_vacan.FirstOrDefault(v => v.VacancyID == vacancyId);

            if (user == null || vacancy == null)
            {
                TempData["Error"] = "Something went wrong. Please try again.";
                return RedirectToAction("CareersWithUs");
            }

            // Check if already applied
            var existingApp = _context.tbl_vacancyapplication
                .FirstOrDefault(a => a.VacancyId == vacancyId && a.UserEmail == userEmail);

            if (existingApp != null)
            {
                TempData["Error"] = "You've already applied for this vacancy.";
                return RedirectToAction("CareersWithUs");
            }

            var application = new VacancyApplication
            {
                VacancyId = vacancyId,
                UserId = user.Id,
                UserName = user.username,
                UserEmail = user.Email,
                AppliedOn = DateTime.Now,
                JobTitle = vacancy.JobTitle,
                Department = vacancy.Department,
                Location = vacancy.Location
            };

            _context.tbl_vacancyapplication.Add(application);
            _context.SaveChanges();

            // Email confirmation (Optional)
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("starservices921@gmail.com", "uapb rled cgtu onzx");
                    client.EnableSsl = true;

                    var mail = new MailMessage();
                    mail.From = new MailAddress("starservices921@gmail.com", "Star Security Services");
                    mail.To.Add(user.Email);
                    mail.Subject = "Application Received - " + vacancy.JobTitle;
                    mail.Body = $"Dear {user.username},\n\n" +
                                $"Thank you for applying for the position of {vacancy.JobTitle} in the {vacancy.Department} department.\n" +
                                $"We will review your application and contact you if you're shortlisted.\n\n" +
                                $"Regards,\nStar Security Services";

                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email send failed: " + ex.Message);
            }

            TempData["Success"] = "Application submitted successfully!";
            return RedirectToAction("CareersWithUs");
        }

        public IActionResult OurBusiness()
        {
            return View();
        }

        public IActionResult OurNetwork()
        {
            return View();
        }
        public IActionResult Clients()
        {
            // Fetch data from Assign table
            var clients = _context.tbl_assign.ToList();
            return View(clients);
        }

        // GET: /User/Contact
        public IActionResult Contact()
        {
            var userEmail = HttpContext.Session.GetString("user_email");

            // If user is not logged in, redirect to Admin login
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Admin"); // Adjust if your login controller is different
            }

            var user = _context.tbl_user.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                var contactModel = new Contact
                {
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CompanyName = user.CompanyName
                };

                return View(contactModel);
            }

            // Redirect to login if user record is not found
            return RedirectToAction("Login", "Admin");
        }


        // POST: /User/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(Contact model)
        {
            var userEmail = HttpContext.Session.GetString("user_email");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Admin");
            }

            var user = _context.tbl_user.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            // Refill readonly fields
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.CompanyName = user.CompanyName;

            if (ModelState.IsValid)
            {
                _context.tbl_contact.Add(model);
                _context.SaveChanges();

                ViewBag.Message = "Your message was sent successfully!";
                return RedirectToAction("Contacts");
            }

            return View(model);
        }


        public IActionResult Contacts()
        {
            return View();
        }






        }
}
