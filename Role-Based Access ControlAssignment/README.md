# Project Tracker (ASP.NET Web Forms, VS2022, SQL Server 2019)

## Features
- JWT-based login (HS256, custom implementation)
- RBAC roles: Admin, Project Manager, Developer, Viewer
- Simple pages for login, user management (Admin), projects, tasks
- SQL Server 2019 schema + seed admin (username: `admin`, password: `Admin@123`)

## Setup (Visual Studio 2022)
1. Create a new **ASP.NET Web Forms (.NET Framework 4.8)** project named `ProjectTrackerWebForms`.
2. Replace the project files with the contents of this zip (or open this folder as the project).
3. Update `Web.config`:
   - Set `JWT:Secret` to a long random string.
   - Update the `DefaultConnection` connection string to your SQL Server (e.g. `Data Source=.;Initial Catalog=ProjectTracker;Integrated Security=True`).
4. Run the SQL script `db/01_schema.sql` in **SQL Server 2019** (SSMS). It creates DB/tables + admin user.
   - If you need a `.bak`: after running the script, in SSMS: Right-click the `ProjectTracker` DB → **Tasks** → **Back Up...** → Device → add path → **OK** → you'll get a `.bak`.
5. Press **F5**. Login with `admin` / `Admin@123`.

## Notes
- For HTTPS in production, set cookie `Secure=true` and host under TLS.
- This is a minimal scaffold intended for extension (validation, paging, error handling, etc.).
- To add users/roles, use **Admin → Users** page.
