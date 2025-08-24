using System;
using System.Data.SqlClient;
using System.Web;

public partial class Account_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        var username = txtUsername.Text.Trim();
        var password = txtPassword.Text;
        try
        {
            var user = Db.AuthenticateUser(username, password);
            if (user != null)
            {
                // issue JWT
                var token = JwtHelper.GenerateToken(user);
                JwtHelper.SetAuthCookie(HttpContext.Current, token);
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                lblMsg.Text = "Invalid username or password.";
            }
        }
        catch (Exception ex)
        {
            lblMsg.Text = ex.Message;
        }
    }
}
