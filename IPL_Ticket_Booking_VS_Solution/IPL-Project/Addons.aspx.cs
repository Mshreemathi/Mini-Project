using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class AddonsPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack)
        {
            gvAddons.DataSource = Db.GetTable("SELECT * FROM AddonMaster");
            gvAddons.DataBind();
            RestoreSelection();
        }
    }

    void RestoreSelection()
    {
        List<int> sel = Session["SelectedAddons"] as List<int>;
        if (sel == null) return;
        foreach (GridViewRow row in gvAddons.Rows)
        {
            CheckBox chk = (CheckBox)row.FindControl("chkSelect");
            HiddenField hf = (HiddenField)row.FindControl("hfAddonID");
            if (chk != null && hf != null && sel.Contains(Convert.ToInt32(hf.Value)))
            {
                chk.Checked = true;
            }
        }
    }

    protected void btnProceed_Click(object sender, EventArgs e)
    {
        List<int> sel = new List<int>();
        foreach (GridViewRow row in gvAddons.Rows)
        {
            CheckBox chk = (CheckBox)row.FindControl("chkSelect");
            HiddenField hf = (HiddenField)row.FindControl("hfAddonID");
            if (chk != null && chk.Checked && hf != null) sel.Add(Convert.ToInt32(hf.Value));
        }
        Session["SelectedAddons"] = sel;
        Response.Redirect("Payment.aspx");
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Booking.aspx");
    }
}
