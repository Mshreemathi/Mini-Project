using System;
using System.Data;
public partial class CustomerProfilePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack) LoadProf();
    }
    void LoadProf()
    {
        int uId = Convert.ToInt32(Session["UserID"]);
        DataTable dt = Db.GetTable("SELECT * FROM Customer WHERE UserID=?", uId);
        if (dt.Rows.Count > 0)
        {
            DataRow r = dt.Rows[0];
            hfCID.Value = r["CustomerID"].ToString();
            txtName.Text = r["CustomerName"].ToString();
            txtEmail.Text = r["Email"].ToString();
            txtMobile.Text = r["Mobile"].ToString();
            txtIdNumber.Text = r["IdNumber"].ToString();
            txtTeam.Text = r["FavoriteTeam"].ToString();
            txtCity.Text = r["City"].ToString();
            txtState.Text = r["State"].ToString();
            txtAddress.Text = r["Address"].ToString();
            ddlIdType.SelectedValue = r["IdType"].ToString();
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int uId = Convert.ToInt32(Session["UserID"]);
        Db.Execute("UPDATE Customer SET CustomerName=?, Email=?, Mobile=?, Address=?, City=?, State=?, IdType=?, IdNumber=?, FavoriteTeam=? WHERE UserID=?",
            txtName.Text, txtEmail.Text, txtMobile.Text, txtAddress.Text, txtCity.Text, txtState.Text, ddlIdType.SelectedValue, txtIdNumber.Text, txtTeam.Text, uId);
        Session["CustomerID"] = Db.Scalar("SELECT CustomerID FROM Customer WHERE UserID=?", uId);
        lblMsg.CssClass = "notice success";
        lblMsg.Text = "Profile updated successfully!";
    }
    protected void btnProceed_Click(object sender, EventArgs e)
    {
        btnSave_Click(sender, e);
        Response.Redirect("MatchExplorer.aspx");
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}
