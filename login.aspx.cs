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

public partial class login : System.Web.UI.Page
{
	public override string StyleSheetTheme
	{
		get { return ThemeHandler.GetTheme(); }
	}

	protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
			Login1.Focus();
    }

	protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
	{
		//string domain = "compliance.lfr.com";
		string domain = Request.Url.Host;
		if (domain.StartsWith("www.")) //remove www.
		{
			int pos = domain.IndexOf(".");
			if (pos > -1) domain = domain.Substring(pos + 1);
		}
		if ((domain == "gateway.lfr.com" || domain == "outreach.lfr.com" || domain == "upload.lfr.com") && Request.Url.Segments.Length > 1)
		{
			domain = String.Format("{0}{1}{2}", domain, Request.Url.Segments[0], Request.Url.Segments[1]);
			if (domain.EndsWith("/")) domain = domain.Remove(domain.Length - 1);
		}
		Security.User u = Security.User.Login(Login1.UserName, Login1.Password);
		if (u != null)
		{
			int SiteID = Security.Role.GetSiteID(u.UserID, domain);
			if (SiteID > -1)
			{
				DateTime dt = DateTime.MinValue;
				if (DateTime.TryParse(u.LastLogin, out dt))
				{
					SessionHandler.Write("LastLogin", dt.ToString("MM/dd/yyyy h:mm tt"));
				}
				SessionHandler.Write("UserID", u.UserID.ToString());
				SessionHandler.Write("SiteID", SiteID.ToString());
				SessionHandler.Write("UserEmail", u.Email);
				//TODO: ::
				//SessionHandler.Write("ProjectID", "00109359");

				Session.Remove("HomeDirectory");
				DataView dv = (DataView)sdsHomeDirectory.Select(new DataSourceSelectArguments());
				if(dv != null && dv.Table.Rows.Count > 0)
					if(dv.Table.Rows[0][0] != DBNull.Value)
						SessionHandler.Write("HomeDirectory", "documents/" + dv.Table.Rows[0][0].ToString());

				Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, "Logged in from login page", Request.UserHostAddress);
				e.Authenticated = true;
				return;
			}
		}

		Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, String.Format("Invalid username {0} at login page", Login1.UserName), Request.UserHostAddress);
		e.Authenticated = false;
	}

	protected void Login1_LoggedIn(object sender, EventArgs e)
	{
		Security.User.SetLastLogin(Login1.UserName);
	}
}
