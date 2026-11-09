using System;
using System.Web.UI.WebControls;

public partial class MatchPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack) BindGrid();
    }

    void BindGrid()
    {
        gvM.DataSource = Db.GetTable("SELECT * FROM MatchDetails ORDER BY MatchDate");
        gvM.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Session["Role"] == null || Session["Role"].ToString() != "Admin") return;
        Db.Execute("INSERT INTO MatchDetails (MatchNumber, Team1, Team2, MatchDate, MatchTime, Stadium, City) VALUES (?, ?, ?, ?, ?, ?, ?)",
            txtNum.Text, txtT1.Text, txtT2.Text, txtDate.Text, txtTime.Text, txtStadium.Text, txtCity.Text);
        lblMsg.CssClass = "notice success";
        lblMsg.Text = "New Match Fixture Added!";
        BindGrid();
    }

    protected void gvM_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteMatch" && Session["Role"] != null && Session["Role"].ToString() == "Admin")
        {
            int mId = Convert.ToInt32(e.CommandArgument);
            Db.Execute("DELETE FROM MatchDetails WHERE MatchID=?", mId);
            lblMsg.CssClass = "notice success";
            lblMsg.Text = "Match deleted.";
            BindGrid();
        }
    }
}
