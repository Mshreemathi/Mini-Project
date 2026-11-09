using System;
public partial class MatchExplorerPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack) { gvFixtures.DataSource = Db.GetTable("SELECT * FROM MatchDetails ORDER BY MatchDate"); gvFixtures.DataBind(); }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerProfile.aspx");
    }
}
