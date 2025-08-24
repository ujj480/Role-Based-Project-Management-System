using System;
using System.Linq;
using System.Web.UI.WebControls;

public partial class Admin_Users : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        JwtHelper.RequireRoles(Context, "Admin");
        if (!IsPostBack)
        {
            ddlRole.DataSource = Db.GetRoles().Select(r => r.Name);
            ddlRole.DataBind();
            BindUsers();
        }
    }

    void BindUsers()
    {
        gvUsers.DataSource = Db.GetUsers();
        gvUsers.DataBind();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            var id = Db.CreateUser(txtU.Text.Trim(), txtP.Text, txtF.Text.Trim(), txtE.Text.Trim(), true);
            Db.AssignRole(id, ddlRole.SelectedValue);
            lblMsg.Text = "User created.";
            BindUsers();
        }
        catch (Exception ex) { lblMsg.Text = ex.Message; }
    }

    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Del")
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            var key = (int)gvUsers.DataKeys[rowIndex].Value;
            Db.DeleteUser(key);
            BindUsers();
        }
    }
}
