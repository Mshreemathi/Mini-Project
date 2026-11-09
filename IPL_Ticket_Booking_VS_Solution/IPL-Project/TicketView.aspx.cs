using System;
using System.Data;

public partial class TicketViewPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IsAuthenticated"] == null) { Response.Redirect("Login.aspx"); return; }
        if (!IsPostBack && Request.QueryString["BookingID"] != null)
        {
            int bId = Convert.ToInt32(Request.QueryString["BookingID"]);
            string sql = "SELECT b.BookingRef, c.CustomerName, (m.Team1 + ' vs ' + m.Team2) AS MatchName, m.Stadium, m.City, (CAST(m.MatchDate AS VARCHAR) + ' ' + m.MatchTime) AS Timing, st.StandName, s.SeatNumber FROM Booking b INNER JOIN Customer c ON b.CustomerID=c.CustomerID INNER JOIN MatchDetails m ON b.MatchID=m.MatchID INNER JOIN StandDetails st ON b.StandID=st.StandID INNER JOIN SeatDetails s ON b.SeatID=s.SeatID WHERE b.BookingID=?";
            DataTable dt = Db.GetTable(sql, bId);
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                lblRef.Text = r["BookingRef"].ToString();
                lblName.Text = r["CustomerName"].ToString();
                lblMatch.Text = r["MatchName"].ToString();
                lblStadium.Text = r["Stadium"].ToString() + ", " + r["City"].ToString();
                lblTime.Text = r["Timing"].ToString();
                lblStand.Text = r["StandName"].ToString();
                lblSeat.Text = r["SeatNumber"].ToString();
                lblQR.Text = "VALID-IPL2026-REF-" + r["BookingRef"].ToString();
            }
        }
    }
}
