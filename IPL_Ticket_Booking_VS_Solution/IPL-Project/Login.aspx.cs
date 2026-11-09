using System;
using System.Data;

public partial class LoginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void SwitchTab_Click(object sender, EventArgs e)
    {
        string mode = ((System.Web.UI.WebControls.Button)sender).CommandArgument;
        if (mode == "Login")
        {
            pnlLogin.Visible = true;
            pnlSignup.Visible = false;
            btnTabLogin.CssClass = "button";
            btnTabSignup.CssClass = "button secondary";
        }
        else
        {
            pnlLogin.Visible = false;
            pnlSignup.Visible = true;
            btnTabLogin.CssClass = "button secondary";
            btnTabSignup.CssClass = "button";
        }
        lblMsg.Text = "";
    }

    protected void btnSignIn_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        string user = txtLoginUser.Text.Trim();
        string pass = txtLoginPass.Text.Trim();

        DataTable dt = Db.GetTable("SELECT * FROM Users WHERE Username=? AND Password=?", user, pass);
        if (dt.Rows.Count > 0)
        {
            DataRow r = dt.Rows[0];
            Session["IsAuthenticated"] = "true";
            Session["UserID"] = r["UserID"];
            Session["Username"] = user;
            Session["FullName"] = r["FullName"];
            Session["Email"] = r["Email"];
            Session["Mobile"] = r["Mobile"];
            Session["Role"] = r["Role"];

            DataTable dtCust = Db.GetTable("SELECT CustomerID FROM Customer WHERE UserID=?", r["UserID"]);
            if (dtCust.Rows.Count > 0) Session["CustomerID"] = dtCust.Rows[0]["CustomerID"];

            Response.Redirect("Default.aspx");
        }
        else
        {
            lblMsg.CssClass = "notice error";
            lblMsg.Text = "Invalid username or password.";
        }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        string name = txtRegName.Text.Trim();
        string email = txtRegEmail.Text.Trim();
        string mobile = txtRegMobile.Text.Trim();
        string user = txtRegUser.Text.Trim();
        string pass = txtRegPass.Text.Trim();

        try
        {
            object existing = Db.Scalar("SELECT UserID FROM Users WHERE Username=?", user);
            if (existing != null)
            {
                lblMsg.CssClass = "notice error";
                lblMsg.Text = "Username already exists. Please choose another.";
                return;
            }

            Db.Execute("INSERT INTO Users (Username, Password, FullName, Email, Mobile, Role) VALUES (?, ?, ?, ?, ?, 'User')", user, pass, name, email, mobile);
            object userId = Db.Scalar("SELECT UserID FROM Users WHERE Username=?", user);

            Db.Execute("INSERT INTO Customer (UserID, CustomerName, Email, Mobile, Address, City, State, Pincode, IdType, IdNumber, FavoriteTeam) VALUES (?, ?, ?, ?, 'Not Provided', 'Mumbai', 'Maharashtra', '400001', 'Aadhaar Card', '000000000000', 'Chennai Super Kings')", userId, name, email, mobile);

            lblMsg.CssClass = "notice success";
            lblMsg.Text = "Registration successful! You can now sign in using your credentials.";
            pnlLogin.Visible = true;
            pnlSignup.Visible = false;
        }
        catch (Exception ex)
        {
            lblMsg.CssClass = "notice error";
            lblMsg.Text = "Registration failed: " + ex.Message;
        }
    }
}
