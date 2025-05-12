using Microsoft.EntityFrameworkCore;

namespace Star_Security_Services.Models
{
    public class mycontext:DbContext
    {
        public mycontext(DbContextOptions<mycontext> options) : base(options) { }
        public DbSet<Admin> tbl_admin { get; set; }

        public DbSet<Employee> tbl_a_employee { get; set; }

        public DbSet<vacan> tbl_vacan { get; set; }

        public DbSet<Services> tbl_service { get; set; }

        public DbSet<Clients> tbl_clients { get; set; }
        public DbSet<qrcode> tbl_qrcode { get; set; }

        public DbSet<EmployeeDash> tbl_employeeDash { get; set; }

        public DbSet<login> tbl_login { get; set; }

        public DbSet<Training> tbl_training { get; set; }

        public DbSet<VacancyApplication> tbl_vacancyapplication { get; set; }

        public DbSet<User> tbl_user { get; set; }

        public DbSet<Final> tbl_final { get; set; }

        public DbSet<Assign> tbl_assign { get; set; }

        public DbSet<Testimonials> tbl_testimonal { get; set; }

        public DbSet<Contact> tbl_contact { get; set; }






    }
}
