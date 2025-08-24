using System;
using System.Web;
using System.Web.UI;
public partial class Site : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var user = JwtHelper.GetUserFromContext(HttpContext.Current);
        lblUser.Text = user != null ? (" | " + user.Username + " (" + string.Join(",", user.Roles) + ")") : " | Guest";
        lnkAdmin.Visible = user != null && user.IsInRole("Admin");
        lnkProjects.Visible = user != null;
        btnLogout.Visible = user != null;
    }
    protected void btnLogout_Click(object sender, EventArgs e)
    {
        JwtHelper.ClearAuthCookie(HttpContext.Current);
        Response.Redirect("~/Account/Login.aspx");
    }
}
