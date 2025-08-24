using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
public class Db
{
    static string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    static string Salt = System.Configuration.ConfigurationManager.AppSettings["PasswordSalt"];
    static string Hash(string password) {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(Salt + password);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
    public class Role
    {
        public int RoleId { get; set; }    
        public string Name { get; set; }    
    }

    public class User
    {
        public int UserId { get; set; }      
        public string Username { get; set; }   
        public string FullName { get; set; }
        public string Email { get; set; }      
        public bool IsActive { get; set; }     
        public string[] Roles { get; set; }    
    }

    public class Project
    {
        public int ProjectId { get; set; }   
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class TaskItem
    {
        public int TaskId { get; set; }       
        public string Title { get; set; }     
        public string Description { get; set; }  
        public string Status { get; set; }    
        public string Assignee { get; set; }  
    }

    public static JwtUser AuthenticateUser(string username, string password)
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"SELECT u.UserId,u.Username,u.FullName,u.Email,u.IsActive FROM Users u WHERE u.Username=@u AND u.PasswordHash=@p AND u.IsActive=1", con))
        {
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", Hash(password));
            con.Open();
            using (var r = cmd.ExecuteReader())
            {
                if (!r.Read()) return null;
                int uid = r.GetInt32(0);
                var roles = GetRolesForUser(uid).Select(x=>x.Name).ToArray();
                return new JwtUser { UserId = uid, Username = username, Roles = roles };
            }
        }
    }

    public static IEnumerable<Role> GetRoles()
    {
        var list = new List<Role>();
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand("SELECT RoleId,Name FROM Roles", con))
        {
            con.Open();
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new Role { RoleId = r.GetInt32(0), Name = r.GetString(1) });
        }
        return list;
    }
    public static IEnumerable<Role> GetRolesForUser(int userId)
    {
        var list = new List<Role>();
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"SELECT r.RoleId,r.Name FROM UserRoles ur JOIN Roles r ON ur.RoleId=r.RoleId WHERE ur.UserId=@id", con))
        {
            cmd.Parameters.AddWithValue("@id", userId);
            con.Open();
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new Role { RoleId = r.GetInt32(0), Name = r.GetString(1) });
        }
        return list;
    }

    public static IEnumerable<User> GetUsers()
    {
        var list = new List<User>();
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand("SELECT UserId,Username,FullName,Email,IsActive FROM Users", con))
        {
            con.Open();
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    list.Add(new User { UserId=r.GetInt32(0), Username=r.GetString(1), FullName=r.IsDBNull(2)?"":r.GetString(2), Email=r.IsDBNull(3)?"":r.GetString(3), IsActive=r.GetBoolean(4), Roles=GetRolesForUser(r.GetInt32(0)).Select(x=>x.Name).ToArray() });
        }
        return list;
    }

    public static int CreateUser(string username, string password, string fullname, string email, bool active)
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"INSERT INTO Users(Username,PasswordHash,FullName,Email,IsActive,CreatedAt) 
VALUES(@u,@p,@f,@e,@a,GETDATE()); SELECT SCOPE_IDENTITY();", con))
        {
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", Hash(password));
            cmd.Parameters.AddWithValue("@f", (object)fullname ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@a", active);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }

    public static void AssignRole(int userId, string roleName)
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"INSERT INTO UserRoles(UserId,RoleId) SELECT @u, RoleId FROM Roles WHERE Name=@r AND NOT EXISTS(SELECT 1 FROM UserRoles WHERE UserId=@u AND RoleId=(SELECT RoleId FROM Roles WHERE Name=@r))", con))
        {
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@r", roleName);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
    public static void DeleteUser(int userId)
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand("DELETE FROM UserRoles WHERE UserId=@u; DELETE FROM Users WHERE UserId=@u;", con))
        {
            cmd.Parameters.AddWithValue("@u", userId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static IEnumerable<Project> GetProjectsVisibleTo(JwtUser user)
    {
        var list = new List<Project>();
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"
SELECT DISTINCT p.ProjectId as ProjectId,p.Name as Name ,p.Description as Description
FROM Projects p
LEFT JOIN ProjectMembers m ON m.ProjectId=p.ProjectId
WHERE (@isAdmin=1)
   OR (@isPm=1 AND p.OwnerUserId=@uid)
   OR (@isDev=1 AND m.UserId=@uid)
   OR (@isViewer=1)", con))
        {
            con.Open();
            cmd.Parameters.AddWithValue("@uid", user.UserId);
            cmd.Parameters.AddWithValue("@isAdmin", user.IsInRole("Admin") ? 1 : 0);
            cmd.Parameters.AddWithValue("@isPm", user.IsInRole("Project Manager") ? 1 : 0);
            cmd.Parameters.AddWithValue("@isDev", user.IsInRole("Developer") ? 1 : 0);
            cmd.Parameters.AddWithValue("@isViewer", user.IsInRole("Viewer") ? 1 : 0);
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new Project { ProjectId=r.GetInt32(0), Name=r.GetString(1), Description=r.IsDBNull(2)?"" : r.GetString(2) });
        }
        return list;
    }

    public static int CreateProject(string name, string desc, int ownerUserId)
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"INSERT INTO Projects(Name,Description,OwnerUserId,CreatedAt) VALUES(@n,@d,@o,GETDATE()); SELECT SCOPE_IDENTITY();", con))
        {
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@d", (object)desc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@o", ownerUserId);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }

    public static IEnumerable<TaskItem> GetTasks(int projectId)
    {
        var list = new List<TaskItem>();
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"SELECT t.TaskId,t.Title,t.Description,t.Status,u.Username AS Assignee FROM Tasks t LEFT JOIN Users u ON t.AssignedTo=u.UserId WHERE t.ProjectId=@p", con))
        {
            cmd.Parameters.AddWithValue("@p", projectId);
            con.Open();
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new TaskItem { TaskId=r.GetInt32(0), Title=r.GetString(1), Description=r.IsDBNull(2)?"" : r.GetString(2), Status=r.GetString(3), Assignee=r.IsDBNull(4)?"" : r.GetString(4) });
        }
        return list;
    }

    public static void CreateTask(int projectId, string title, string desc, string status, string assigneeUsername, int createdBy)
    {
        int? assigneeId = null;
        if (!string.IsNullOrWhiteSpace(assigneeUsername))
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT UserId FROM Users WHERE Username=@u", con))
            {
                cmd.Parameters.AddWithValue("@u", assigneeUsername);
                con.Open();
                var x = cmd.ExecuteScalar();
                if (x != null) assigneeId = Convert.ToInt32(x);
            }
        }
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand(@"INSERT INTO Tasks(ProjectId,Title,Description,Status,AssignedTo,CreatedAt,UpdatedAt,CreatedBy) VALUES(@p,@t,@d,@s,@a,GETDATE(),GETDATE(),@c)", con))
        {
            cmd.Parameters.AddWithValue("@p", projectId);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@d", (object)desc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@a", (object)assigneeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@c", createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
