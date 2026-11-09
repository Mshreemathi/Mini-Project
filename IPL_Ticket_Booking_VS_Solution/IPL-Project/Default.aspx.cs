using System;
public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack) { gvMatches.DataSource = Db.GetTable("SELECT * FROM MatchDetails ORDER BY MatchDate"); gvMatches.DataBind(); }
    }
}
