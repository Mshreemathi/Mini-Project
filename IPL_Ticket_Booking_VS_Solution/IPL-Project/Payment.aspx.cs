using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

public partial class PaymentPage : System.Web.UI.Page
{
    decimal seatSubtotal = 0, addonTotal = 0, totalPayable = 0;
    List<CartItem> cart;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        cart = Session["BookingCart"] as List<CartItem>;
        if (cart == null || cart.Count == 0) { Response.Redirect("Booking.aspx"); return; }
        CalcTotals();
    }

    void CalcTotals()
    {
        foreach (var item in cart) seatSubtotal += item.Price;
        
        List<int> selAddons = Session["SelectedAddons"] as List<int>;
        if (selAddons != null && selAddons.Count > 0)
        {
            foreach (int aId in selAddons)
            {
                object p = Db.Scalar("SELECT Price FROM AddonMaster WHERE AddonID=?", aId);
                if (p != null) addonTotal += Convert.ToDecimal(p);
            }
        }

        decimal sub = seatSubtotal + addonTotal;
        decimal tax = sub * 0.18m;
        totalPayable = sub + tax;

        lblSeatCount.Text = cart.Count.ToString();
        lblSeatAmt.Text = "Rs. " + seatSubtotal.ToString("N2");
        lblAddonAmt.Text = "Rs. " + addonTotal.ToString("N2");
        lblTax.Text = "Rs. " + tax.ToString("N2");
        lblTotal.Text = "Rs. " + totalPayable.ToString("N2");
    }

    protected void btnPay_Click(object sender, EventArgs e)
    {
        try
        {
            int custId = Convert.ToInt32(Session["CustomerID"]);

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    List<string> refs = new List<string>();

                    foreach (var item in cart)
                    {
                        using (SqlCommand cmdCheck = new SqlCommand("SELECT Status FROM SeatDetails WITH (UPDLOCK, HOLDLOCK) WHERE SeatID=@s AND Status='Available'", con, tx))
                        {
                            cmdCheck.Parameters.AddWithValue("@s", item.SeatID);
                            if (cmdCheck.ExecuteScalar() == null) throw new Exception("Seat " + item.SeatNumber + " is no longer available.");
                        }

                        using (SqlCommand cmdUpd = new SqlCommand("UPDATE SeatDetails SET Status='Booked' WHERE SeatID=@s", con, tx))
                        {
                            cmdUpd.Parameters.AddWithValue("@s", item.SeatID);
                            cmdUpd.ExecuteNonQuery();
                        }

                        string bref = "IPL-2026-" + new Random().Next(1000, 9999);
                        refs.Add(bref);
                        int bId = 0;

                        using (SqlCommand cmdB = new SqlCommand("INSERT INTO Booking (BookingRef, CustomerID, MatchID, StandID, TicketTypeID, SeatID, TotalAmount, BookingStatus) VALUES (@r, @c, @m, @st, @t, @se, @amt, 'Confirmed'); SELECT SCOPE_IDENTITY();", con, tx))
                        {
                            cmdB.Parameters.AddWithValue("@r", bref);
                            cmdB.Parameters.AddWithValue("@c", custId);
                            cmdB.Parameters.AddWithValue("@m", item.MatchID);
                            cmdB.Parameters.AddWithValue("@st", item.StandID);
                            cmdB.Parameters.AddWithValue("@t", item.TicketTypeID);
                            cmdB.Parameters.AddWithValue("@se", item.SeatID);
                            cmdB.Parameters.AddWithValue("@amt", item.Price + (addonTotal / cart.Count));
                            bId = Convert.ToInt32(cmdB.ExecuteScalar());
                        }

                        List<int> selAddons = Session["SelectedAddons"] as List<int>;
                        if (selAddons != null)
                        {
                            foreach (int aId in selAddons)
                            {
                                object ap = Db.Scalar("SELECT Price FROM AddonMaster WHERE AddonID=?", aId);
                                using (SqlCommand cmdA = new SqlCommand("INSERT INTO BookingAddon (BookingID, AddonID, Quantity, Price) VALUES (@b, @a, 1, @p)", con, tx))
                                {
                                    cmdA.Parameters.AddWithValue("@b", bId);
                                    cmdA.Parameters.AddWithValue("@a", aId);
                                    cmdA.Parameters.AddWithValue("@p", ap);
                                    cmdA.ExecuteNonQuery();
                                }
                            }
                        }

                        using (SqlCommand cmdP = new SqlCommand("INSERT INTO Payment (BookingID, PaymentRef, PaymentMode, CardOrUpiNumber, AmountPaid, PaymentStatus) VALUES (@b, @pr, @mo, @card, @paid, 'Success')", con, tx))
                        {
                            cmdP.Parameters.AddWithValue("@b", bId);
                            cmdP.Parameters.AddWithValue("@pr", "PAY-" + bref);
                            cmdP.Parameters.AddWithValue("@mo", ddlMode.SelectedValue);
                            cmdP.Parameters.AddWithValue("@card", txtRef.Text);
                            cmdP.Parameters.AddWithValue("@paid", item.Price);
                            cmdP.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    Session.Remove("BookingCart");
                    Session.Remove("SelectedAddons");
                    Session["LastBookingRef"] = string.Join(", ", refs);
                    Response.Redirect("MyBookings.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            lblMsg.CssClass = "notice error";
            lblMsg.Text = "Payment failed: " + ex.Message;
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Addons.aspx");
    }
}
