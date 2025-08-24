using System;
public partial class Projects_Tasks : System.Web.UI.Page
{
    int pid;
    protected void Page_Load(object sender, EventArgs e)
    {
        var user = JwtHelper.RequireAuthenticated(Context);
        var p = 0;
        pid = int.TryParse(Request["pid"], out p) ? p : 0;
        hfPid.Value = pid.ToString();
        if (!IsPostBack) Bind();
        pnlEditor.Visible = user.IsInRole("Developer") || user.IsInRole("Project Manager") || user.IsInRole("Admin");
    }
    void Bind()
    {
        gvTasks.DataSource = Db.GetTasks(pid);
        gvTasks.DataBind();
    }
    protected void btnAddTask_Click(object sender, EventArgs e)
    {
        var user = JwtHelper.GetUserFromContext(Context);
        if (!(user.IsInRole("Developer") || user.IsInRole("Project Manager") || user.IsInRole("Admin"))) { lblMsg.Text = "Not allowed."; return; }
        Db.CreateTask(pid, txtTitle.Text.Trim(), txtTaskDesc.Text.Trim(), ddlStatus.SelectedValue, txtAssignTo.Text.Trim(), user.UserId);
        Bind();
    }
}
