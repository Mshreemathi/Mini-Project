using System;
using System.Data;
using System.Data.SqlClient;

public partial class MyBookingsPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack)
        {
            if (Session["LastBookingRef"] != null)
            {
                lblMsg.CssClass = "notice success";
                lblMsg.Text = "Booking confirmed for reference(s): " + Session["LastBookingRef"];
                Session.Remove("LastBookingRef");
            }
            BindTickets();
        }
    }

    void BindTickets()
    {
        string sql = "";
        if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
        {
            sql = "SELECT b.BookingID, b.SeatID, b.BookingRef, (m.Team1 + ' vs ' + m.Team2) AS MatchName, st.StandName, s.SeatNumber, b.TotalAmount, b.BookingDate FROM Booking b INNER JOIN MatchDetails m ON b.MatchID=m.MatchID INNER JOIN StandDetails st ON b.StandID=st.StandID INNER JOIN SeatDetails s ON b.SeatID=s.SeatID ORDER BY b.BookingID DESC";
            gvTickets.DataSource = Db.GetTable(sql);
        }
        else
        {
            int cId = Convert.ToInt32(Session["CustomerID"]);
            sql = "SELECT b.BookingID, b.SeatID, b.BookingRef, (m.Team1 + ' vs ' + m.Team2) AS MatchName, st.StandName, s.SeatNumber, b.TotalAmount, b.BookingDate FROM Booking b INNER JOIN MatchDetails m ON b.MatchID=m.MatchID INNER JOIN StandDetails st ON b.StandID=st.StandID INNER JOIN SeatDetails s ON b.SeatID=s.SeatID WHERE b.CustomerID=? ORDER BY b.BookingID DESC";
            gvTickets.DataSource = Db.GetTable(sql, cId);
        }
        gvTickets.DataBind();
    }

    protected void gvTickets_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "CancelTicket")
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            int bookingId = Convert.ToInt32(args[0]);
            int seatId = Convert.ToInt32(args[1]);

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    using (SqlCommand cmdUpd = new SqlCommand("UPDATE SeatDetails SET Status='Available' WHERE SeatID=@s", con, tx))
                    {
                        cmdUpd.Parameters.AddWithValue("@s", seatId);
                        cmdUpd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmdDel = new SqlCommand("DELETE FROM Booking WHERE BookingID=@b", con, tx))
                    {
                        cmdDel.Parameters.AddWithValue("@b", bookingId);
                        cmdDel.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }

            lblMsg.CssClass = "notice success";
            lblMsg.Text = "Ticket reservation cancelled and seat released back to available inventory.";
            BindTickets();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}
