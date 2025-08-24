using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Xml.Linq;


public partial class Projects_Projects : System.Web.UI.Page
{
    static string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    string TxtId = "0";
    protected void Page_Load(object sender, EventArgs e)
    {
        var user = JwtHelper.RequireAuthenticated(Context);
        pnlCreate.Visible = user.IsInRole("Project Manager") || user.IsInRole("Admin");
        if (!IsPostBack)
        {
            Bind();
        }
    }
    void Bind()
    {
        var user = JwtHelper.GetUserFromContext(Context);
        gvProjects.DataSource = Db.GetProjectsVisibleTo(user);
        gvProjects.DataBind();
    }
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        if (lblId.Text == "")
        {
            var user = JwtHelper.GetUserFromContext(Context);
            if (!(user.IsInRole("Project Manager") || user.IsInRole("Admin"))) { lblMsg.Text = "Not allowed."; return; }
            Db.CreateProject(txtName.Text.Trim(), txtDesc.Text.Trim(), user.UserId);
        }
        else
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"UPDATE Projects 
                                  SET Name = @n,
                                      Description = @d
                                  WHERE ProjectId = @id", con))
            {
                cmd.Parameters.AddWithValue("@n", txtName.Text);
                cmd.Parameters.AddWithValue("@d", txtDesc.Text ?? txtDesc.Text);
                cmd.Parameters.AddWithValue("@id", lblId.Text); // yeh ID pass karna hoga jiska record update karna hai

                con.Open();
               cmd.ExecuteNonQuery();  // yeh rows updated count return karega
            }
        }
        Bind();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        using (var con = new SqlConnection(CS))
        using (var cmd = new SqlCommand("delete  from Projects where ProjectId =@u;", con))
        {
            cmd.Parameters.AddWithValue("@u", lblId.Text);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        Bind();
    }

    protected void gvProjects_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblId.Text = gvProjects.DataKeys[gvProjects.SelectedRow.RowIndex].Values[0].ToString();
        string sql = " select * from Projects where ProjectId = '" + lblId.Text + "'";
        DataSet dss = new DataSet();
        using (SqlConnection con = new SqlConnection(CS))
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
            {
                da.Fill(dss);
            }
        }
        // अब DataRow loop
        foreach (DataRow DrSel1 in dss.Tables[0].Rows)
        {
            // Example: MID निकालने के लिए
            txtName.Text = DrSel1["Name"].ToString();
            txtDesc.Text = DrSel1["Description"].ToString();
            // यहां अपना logic लगाओ
        }
    }
    }
