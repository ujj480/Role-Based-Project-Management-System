using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

public partial class Projects_AssignDev : System.Web.UI.Page
{
    string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadProjects();
        }
    }

    void LoadProjects()
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand("SELECT ProjectId, Name FROM Projects", con))
        {
            con.Open();
            ddlProjects.DataSource = cmd.ExecuteReader();
            ddlProjects.DataTextField = "Name";
            ddlProjects.DataValueField = "ProjectId";
            ddlProjects.DataBind();
        }
        ddlProjects.Items.Insert(0, new ListItem("-- Select Project --", "0"));
    }

    void LoadUsers()
    {
        if (ddlProjects.SelectedValue == "0") return;

        string q = @"
SELECT u.UserId, u.FullName, u.Email,
       CASE WHEN pm.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsAssigned
FROM Users u
LEFT JOIN ProjectMembers pm 
     ON pm.UserId = u.UserId AND pm.ProjectId ='" + ddlProjects.SelectedValue + @"'
WHERE EXISTS (
    SELECT 1 
    FROM UserRoles ur 
    JOIN Roles r ON ur.RoleId = r.RoleId 
    WHERE ur.UserId = u.UserId AND r.Name='Developer'
)";

        using (SqlConnection con = new SqlConnection(CS))
        using (SqlCommand cmd = new SqlCommand(q, con))
        {
            con.Open();
            gvUsers.DataSource = cmd.ExecuteReader();
            gvUsers.DataBind();
        }
    
    }

    protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadUsers();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        int pid = int.Parse(ddlProjects.SelectedValue);
        using (var con = new SqlConnection(CS))
        {
            con.Open();
            foreach (GridViewRow row in gvUsers.Rows)
            {
                int uid = int.Parse((row.Cells[0]).Text);
                CheckBox chk = (CheckBox)row.FindControl("chkAssign");

                if (chk.Checked)
                {
                    using (var cmd = new SqlCommand("IF NOT EXISTS (SELECT 1 FROM ProjectMembers WHERE ProjectId=@pid AND UserId=@uid) INSERT INTO ProjectMembers(ProjectId,UserId,RoleInProject) VALUES(@pid,@uid,'Developer')", con))
                    {
                        cmd.Parameters.AddWithValue("@pid", pid);
                        cmd.Parameters.AddWithValue("@uid", uid);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand("DELETE FROM ProjectMembers WHERE ProjectId=@pid AND UserId=@uid", con))
                    {
                        cmd.Parameters.AddWithValue("@pid", pid);
                        cmd.Parameters.AddWithValue("@uid", uid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        LoadUsers();
    }
}