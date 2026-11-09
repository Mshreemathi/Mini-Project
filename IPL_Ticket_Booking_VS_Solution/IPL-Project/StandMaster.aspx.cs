using System;
using System.Web.UI.WebControls;

public partial class StandMasterPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack) BindGrid();
    }

    void BindGrid()
    {
        gvS.DataSource = Db.GetTable("SELECT * FROM StandDetails");
        gvS.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Session["Role"] == null || Session["Role"].ToString() != "Admin") return;
        Db.Execute("INSERT INTO StandDetails (StandName, Tier, BasePrice, TotalCapacity) VALUES (?, ?, ?, ?)",
            txtName.Text, txtTier.Text, Convert.ToDecimal(txtPrice.Text), Convert.ToInt32(txtCap.Text));
        lblMsg.CssClass = "notice success";
        lblMsg.Text = "Stand added successfully.";
        BindGrid();
    }

    protected void gvS_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteStand" && Session["Role"] != null && Session["Role"].ToString() == "Admin")
        {
            int sId = Convert.ToInt32(e.CommandArgument);
            Db.Execute("DELETE FROM StandDetails WHERE StandID=?", sId);
            lblMsg.CssClass = "notice success";
            lblMsg.Text = "Stand deleted.";
            BindGrid();
        }
    }
}
