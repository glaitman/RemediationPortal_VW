using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class preferences : BetterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

	protected void Page_PreRender(object sender, EventArgs e)
	{
		string s = ThemeHandler.GetTheme();
		if (s != "" && ddlTheme.Items.FindByValue(s) != null)
			ddlTheme.SelectedValue = s;
	}

	protected void btnChange_Click(object sender, EventArgs e)
	{
		if (HttpContext.Current == null || HttpContext.Current.User == null || HttpContext.Current.User.Identity == null)
			return;

		string username = HttpContext.Current.User.Identity.Name;
		string oldpass = txtPassword.Text;
		string newpass = txtNewPassword.Text;

		if (Security.User.SetPassword(username, oldpass, newpass))
		{
			Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, String.Format("Password changed successfully for {0}", username), Request.UserHostAddress);
			lblMessage.Text = "Your password has been changed successfully.";
			lblMessage.CssClass = "message";
		}
		else
		{
			Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, String.Format("{0} tried to change password unsuccessfully", username), Request.UserHostAddress);
			lblMessage.Text = "Incorrect password.  No change has been made.";
			lblMessage.CssClass = "error";
		}

	}
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/index.aspx", false);
	}

	protected void ddlTheme_SelectedIndexChanged(object sender, EventArgs e)
	{
		//TODO: update membershipsite record here

		if (ddlTheme.SelectedValue != "")
		{
			ThemeHandler.SetTheme(ddlTheme.SelectedValue);
			try
			{
				//Response.Redirect(Request.ServerVariables["URL"], true);
				Response.Redirect(Request.Url.OriginalString, false);
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
		}
	}


}
