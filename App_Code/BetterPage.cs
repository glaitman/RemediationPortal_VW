using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO.Compression;

/// <summary>
/// BetterPage does the following:
/// - Overrides the stylesheet property (StyleSheetTheme)
/// - Injects a script that performs an asynchronous callback 30 seconds before the session timeout (OnInit)
/// - Compresses the HTTP response according to the browser's capabilities (OnLoad)
/// - Redirects to the login page if the session is invalid (OnLoad)
/// </summary>
public class BetterPage : System.Web.UI.Page
{
	public BetterPage()
	{
	}

	public override string StyleSheetTheme
	{
		get { DoAuthentication(); return ThemeHandler.GetTheme(); }
	}

	protected void DoAuthentication()
	{
		if (SessionHandler.Read("UserID") == "")
		{
			if (HttpContext.Current == null || HttpContext.Current.User == null || HttpContext.Current.User.Identity == null)
			{
				FormsAuthentication.RedirectToLoginPage();
				return;
			}

			string domain = Request.Url.Host;
			if (domain.StartsWith("www.")) //remove www.
			{
				int pos = domain.IndexOf(".");
				if (pos > -1) domain = domain.Substring(pos + 1);
			}

			Security.User u = Security.User.Rez(HttpContext.Current.User.Identity.Name);
			if (u == null) //user doesn't exist
			{
				Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, String.Format("Cannot rez invalid user {0}", HttpContext.Current.User.Identity.Name), Request.UserHostAddress);
				FormsAuthentication.RedirectToLoginPage();
				return;
			}
			else //user exists
			{
				SessionHandler.Remove("LastLogin");
				SessionHandler.Remove("HomeDirectory");

				int SiteID = Security.Role.GetSiteID(u.UserID, domain);
				if (SiteID > -1) //valid member of this site
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

					string HomeDirectory = Security.User.GetHomeDirectory(u.Username, u.SiteID);
					if (HomeDirectory != "") SessionHandler.Write("HomeDirectory", "documents/" + HomeDirectory);

					Security.User.SetLastLogin(u.Username);
					//Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, String.Format("User {0} has rezzed", HttpContext.Current.User.Identity.Name), Request.UserHostAddress);
				}
				else //valid user but shouldn't be at this site
				{
					Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, String.Format("Cannot rez {0} / no site access", HttpContext.Current.User.Identity.Name), Request.UserHostAddress);
					FormsAuthentication.RedirectToLoginPage();
					return;
				}
			}
		}
	}

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		if (this.Context.Session != null && this.Context.Session.Timeout > 0)
		{
			//For debugging, use 1 minute:
			//int TimeOutMilliseconds = (1 * 60000) - 30000;

			int TimeOutMilliseconds = (this.Context.Session.Timeout * 60000) - 30000;
			if (TimeOutMilliseconds <= 0) return;

			string str_Script = @"
			<script type='text/javascript'>
			//Number of Reconnects
			var count=0;
			//Maximum reconnects setting
			var max = 60;

			function getObjectHTTP() {
				var xmlhttp = null;
				if (navigator.userAgent.indexOf('MSIE')>=0) {
				   xmlhttp = new ActiveXObject('Microsoft.XMLHTTP');
				} else {
				   xmlHttp = new XMLHttpRequest();
				}
				return xmlhttp;
			}

			//Just open an empty page (with a unique URL to prevent caching)
			function Reconnect() {
				count++;
				if (count < max) {
					window.status = 'Link to server re-established (' + count.toString()+'x)';
					var url='reconnect.aspx?' + escape(new Date().toString());
					xmlHttp=getObjectHTTP();
					xmlHttp.open('GET', url , true);
					xmlHttp.send(null);
				}
			}

			//Callback after (int_MilliSecondsTimeOut / 1000) seconds to keep connection alive
			window.setInterval('Reconnect()'," + TimeOutMilliseconds.ToString() + @"); //Set to length required

			</script>
			";

			ClientScript.RegisterStartupScript(GetType(), "Reconnect", str_Script);

		}
	}

	protected override void OnLoad(EventArgs e)
	{
		////2006-03-13 MM, if the user isn't authenticated, go back and login
		//if (!SessionHandler.IsValid() || SessionHandler.Read("UserID") == "")
		//{
		//    //It would be nice to save the path in the QueryString here, and then return to it
		//    //after logging in.



		//    try
		//    {
		//        if(!Page.IsCallback)
		//            //make sure parameter 2 is true -- this calls Response.End() automatically
		//            Response.Redirect(Constants.ASPX_LOGIN, true);
		//        //Response.End();
		//    }
		//    catch (System.Threading.ThreadAbortException)
		//    {
		//        //might be a good idea to write this to a log
		//    }
		//}

		try
		{
			//don't compress the response if the user agent doesn't allow it
			if (Request.Headers["Accept-encoding"] == null) return;

			//ignore compression for Konqueror since it causes a warning message
			if (Request.UserAgent != null && Request.UserAgent.ToLower().Contains("konqueror")) return;

			if (Request.Headers["Accept-encoding"].Contains("gzip")) //compress using gzip
			{
				Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress, true);
				Response.AppendHeader("Content-encoding", "gzip");
			}
			else if (Request.Headers["Accept-encoding"].Contains("deflate")) //compress using deflate
			{
				Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress, true);
				Response.AppendHeader("Content-encoding", "deflate");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally //always do this
		{
			base.OnLoad(e);
		}

		//string identity = String.Empty;
		//string ip_address = Context.Request.UserHostAddress;
		//if (Context.User != null && Context.User.Identity != null) identity = Context.User.Identity.Name;
		//Log.WriteAppLog(String.Format("{0}\t{1}\t{2}\t{3}", DateTime.Now, Request.Url, ip_address, identity));

		//Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, "Page load", Request.UserHostAddress);

	}

}
