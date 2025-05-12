# Star Security Services Web Application

An ASP.NET Core MVC project built to manage and display company information, business services, regional networks, clients, employee records, job vacancies, and admin-controlled content updates.

##  Functional Requirements

This web application includes the following modules:

### 1. About Us
- **History**: Organization foundation and journey.
- **Chairman’s Profile**: Details about the Chairman.
- **Board of Directors**: Information about the Board members.

### 2. Our Business
- **Manned Guarding**: Fire squad, dog squad, bodyguards for industries, banks, residentials, etc.
- **Cash Services**: Cash transfers, ATM replenishment, multi-point collection, vaulting.
- **Recruitment & Training**: Recruits and trains personnel for all divisions.
- **Electronic Security Systems**: CCTV, fire alarms, intrusion alarms, perimeter protection, etc.

### 3. Our Network
- Regional coverage across **North**, **South**, **East**, and **West** zones.
- Each region includes contact information, office location, and a contact person.

### 4. Careers
- Dynamic listing of job vacancies.
- Auto-removal of filled positions.

### 5. Clients
- Client name
- Services availed
- Assigned staff

### 6. Employee Login
- Restricted access based on user roles.
- Staff can view limited content.
- Admins can modify core data (services, vacancies, clients, staff info).

### 7. Testimonials

### 8. Sitemap

### 9. Contact Us

### 10. Employee Details (Displayed)
- Name, Address, Contact, Qualification
- Code, Department, Role, Grade, Client, Achievements

### 11. Admin Privileges
Admins can:
- Add/Edit/Delete:
  - Employee records
  - Vacancies
  - Services
  - Clients

##  Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (local DB or remote)
- Bootstrap & jQuery
- Identity Management (role-based)

##  Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Visual Studio Code](https://code.visualstudio.com/)
- SQL Server (or LocalDB)

### Run Locally
1. Clone the repo
   git clone https://github.com/<your-username>/star-security-services.git
   cd star-security-services
2. Restore packages and build:
   dotnet restore
   dotnet build
3. Run the project:
   dotnet run
