using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;

/// <summary>
/// Summary description for ThemeHandler
/// </summary>
public static class ThemeHandler
{
	//return a user-defined theme if it exists, otherwise use the definition in web.config
	public static string GetTheme()
	{
		string theme = SessionHandler.Read("THEME");
		if (theme != "")
			return theme;
		else
			return WebConfigurationManager.AppSettings["Theme"];
	}

	public static void SetTheme(string value)
	{
		if (value != "")
			SessionHandler.Write("THEME", value);
	}

	public static void Init()
	{
		SessionHandler.Write("THEME", WebConfigurationManager.AppSettings["Theme"]);
	}

}
