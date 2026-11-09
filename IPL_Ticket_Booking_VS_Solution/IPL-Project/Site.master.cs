using System;
public partial class SiteMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e) { }
    protected string GetUserName() { return Session["FullName"] != null ? Session["FullName"].ToString() : "User"; }
    protected string GetUserRole() { return Session["Role"] != null ? Session["Role"].ToString() : "User"; }
    protected string GetUserInitial() { string n = GetUserName(); return !string.IsNullOrEmpty(n) ? n.Substring(0, 1).ToUpper() : "U"; }
    protected void btnLogout_Click(object sender, EventArgs e) { Session.Clear(); Session.Abandon(); Response.Redirect("Login.aspx"); }
}
