using System;
using System.Data;
using System.Collections.Generic;

[Serializable]
public class CartItem
{
    public int MatchID { get; set; }
    public string MatchName { get; set; }
    public int StandID { get; set; }
    public string StandName { get; set; }
    public int TicketTypeID { get; set; }
    public string CategoryName { get; set; }
    public int SeatID { get; set; }
    public string SeatNumber { get; set; }
    public decimal Price { get; set; }
}

public partial class BookingPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack)
        {
            BindDDLs();
            if (Request.QueryString["MatchID"] != null) ddlMatch.SelectedValue = Request.QueryString["MatchID"];
            ReloadSeats(null, null);
            BindCart();
        }
    }

    void BindDDLs()
    {
        ddlMatch.DataSource = Db.GetTable("SELECT MatchID, (Team1 + ' vs ' + Team2) AS MName FROM MatchDetails"); ddlMatch.DataTextField = "MName"; ddlMatch.DataValueField = "MatchID"; ddlMatch.DataBind();
        ddlStand.DataSource = Db.GetTable("SELECT StandID, (StandName + ' - Rs. ' + CAST(CAST(BasePrice AS INT) AS VARCHAR)) AS SName FROM StandDetails"); ddlStand.DataTextField = "SName"; ddlStand.DataValueField = "StandID"; ddlStand.DataBind();
        ddlCategory.DataSource = Db.GetTable("SELECT TicketTypeID, TicketName FROM TicketType"); ddlCategory.DataTextField = "TicketName"; ddlCategory.DataValueField = "TicketTypeID"; ddlCategory.DataBind();
    }

    protected void ReloadSeats(object sender, EventArgs e)
    {
        ddlSeat.Items.Clear();
        int mId = Db.ToInt(ddlMatch.SelectedValue);
        int sId = Db.ToInt(ddlStand.SelectedValue);
        int cId = Db.ToInt(ddlCategory.SelectedValue);
        DataTable dt = Db.GetTable("SELECT SeatID, SeatNumber FROM SeatDetails WHERE MatchID=? AND StandID=? AND TicketTypeID=? AND Status='Available'", mId, sId, cId);
        ddlSeat.DataSource = dt; ddlSeat.DataTextField = "SeatNumber"; ddlSeat.DataValueField = "SeatID"; ddlSeat.DataBind();
        ddlSeat.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Available Seat --", ""));
        CalcPrice(null, null);
    }

    protected void CalcPrice(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlSeat.SelectedValue))
        {
            object p = Db.Scalar("SELECT FinalPrice FROM SeatDetails WHERE SeatID=?", Db.ToInt(ddlSeat.SelectedValue));
            if (p != null) lblPrice.Text = "Rs. " + Convert.ToDecimal(p).ToString("N2");
        }
        else { lblPrice.Text = "Rs. 0.00"; }
    }

    protected void btnAddCart_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlSeat.SelectedValue)) { lblMsg.CssClass = "notice error"; lblMsg.Text = "Please select an available seat."; return; }

        List<CartItem> cart = Session["BookingCart"] as List<CartItem> ?? new List<CartItem>();
        int seatId = Db.ToInt(ddlSeat.SelectedValue);

        if (cart.Exists(c => c.SeatID == seatId))
        {
            lblMsg.CssClass = "notice error"; lblMsg.Text = "This seat is already in your cart."; return;
        }

        cart.Add(new CartItem
        {
            MatchID = Db.ToInt(ddlMatch.SelectedValue),
            MatchName = ddlMatch.SelectedItem.Text,
            StandID = Db.ToInt(ddlStand.SelectedValue),
            StandName = ddlStand.SelectedItem.Text,
            TicketTypeID = Db.ToInt(ddlCategory.SelectedValue),
            CategoryName = ddlCategory.SelectedItem.Text,
            SeatID = seatId,
            SeatNumber = ddlSeat.SelectedItem.Text,
            Price = Convert.ToDecimal(lblPrice.Text.Replace("Rs. ", ""))
        });

        Session["BookingCart"] = cart;
        lblMsg.CssClass = "notice success"; lblMsg.Text = "Seat added to multi-match cart successfully!";
        BindCart();
        ReloadSeats(null, null);
    }

    void BindCart()
    {
        List<CartItem> cart = Session["BookingCart"] as List<CartItem> ?? new List<CartItem>();
        gvCart.DataSource = cart;
        gvCart.DataBind();
    }

    protected void gvCart_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RemoveSeat")
        {
            int idx = Convert.ToInt32(e.CommandArgument);
            List<CartItem> cart = Session["BookingCart"] as List<CartItem>;
            if (cart != null && idx < cart.Count)
            {
                cart.RemoveAt(idx);
                Session["BookingCart"] = cart;
                lblMsg.CssClass = "notice success"; lblMsg.Text = "Seat removed from cart.";
                BindCart();
                ReloadSeats(null, null);
            }
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("MatchExplorer.aspx");
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        List<CartItem> cart = Session["BookingCart"] as List<CartItem>;
        if (cart == null || cart.Count == 0) { lblMsg.CssClass = "notice error"; lblMsg.Text = "Your cart is empty. Please add at least one seat."; return; }
        Response.Redirect("Addons.aspx");
    }
}
