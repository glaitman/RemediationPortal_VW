using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
/// <summary>
///This class handles all session variables. Don't reference session
///variables directly from the code; use this class instead.
/// </summary>
public class SessionHandler
{
	public static bool IsValid()
	{
		HttpContext context = HttpContext.Current;
		if (context == null || context.Session == null) return false;
		if (context.Session.IsNewSession) return false;

		return true;
	}

	//get value by key
	public static string Read(string key)
	{
		HttpContext context = HttpContext.Current;
		if (context == null || context.Session == null) return "";

		if (context.Session[key] == null) return "";
		return Convert.ToString(context.Session[key]);
	}

	//store value by key
	//return true if successful
	public static bool Write(string key, string value)
	{
		HttpContext context = HttpContext.Current;
		if (context == null || context.Session == null) return false;

		if (key == null || key.Length == 0) return false;
		context.Session[key] = value;
		return true;
	}

	//store value by key
	//if overwrite is false, do only if current value is null or empty
	//return true if successful
	public static bool Write(string key, string value, bool overwrite)
	{
		HttpContext context = HttpContext.Current;
		if (context == null || context.Session == null) return false;

		if (key == null || key.Length == 0) return false;
		if (!overwrite)
		{
			if (context.Session[key] == null || context.Session[key].ToString() == "")
			{
				context.Session[key] = value;
				return true;
			}
			else return false;
		}
		else
		{
			context.Session[key] = value;
			return true;
		}
	}


	public static void Remove(string key)
	{
		HttpContext context = HttpContext.Current;
		if (context == null || context.Session == null) return;

		if (key == null || key.Length == 0) return;
		if (context.Session[key] != null) context.Session.Remove(key);
	}




}
