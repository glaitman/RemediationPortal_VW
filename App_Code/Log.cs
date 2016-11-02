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
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// Summary description for Log
/// </summary>
public static class Log
{
	/// <summary>
	/// Writes a log in the Application Log table.
	/// </summary>
	/// <param name="username">ID of the user to associate the log record with</param>
	/// <param name="url">URL from which the message is coming</param>
	/// <param name="message">Text to be placed in the message of the log</param>
	/// <param name="ipaddress">The IP Address of the system that generated the log</param>
	public static void WriteAppLog(string username, string url, string message, string ipaddress)
	{
		try
		{
			using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand("sp_WriteAppLog", cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@username", username);
					cmd.Parameters.AddWithValue("@url", url);
					cmd.Parameters.AddWithValue("@msg", message);
					cmd.Parameters.AddWithValue("@ip", ipaddress);
					cmd.Parameters.AddWithValue("@siteid", SessionHandler.Read("SiteID"));

					cmd.ExecuteNonQuery();
				}
				cn.Close();
			}
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Writes an entry in the ApplicationLog table.
	/// Assumes; username=Session("UserID"), url=Request.Url, ipaddress=Request.UserHostAddress
	/// </summary>
	/// <param name="message">Text to be placed in the message of the log</param>
	public static void WriteAppLog(string message)
	{
		Log.WriteAppLog(SessionHandler.Read("UserID"), HttpContext.Current.Request.Url.OriginalString, message, HttpContext.Current.Request.UserHostAddress);
	}

	/// <summary>
	/// Write a log to the upload Log table regarding status of an upload.
	/// </summary>
	/// <param name="userID">The id of the user performing the upload. Defaults to the session Userid if empty</param>
	/// <param name="fileName">The filename of the file being uploaded. Strips out any paths to simple filename</param>
	/// <param name="message">Detailed message to accompany the log record</param>
	/// <param name="status">Current status of the log item. EG: 'Error', 'Failed', 'Success', etc</param>
	public static void WriteUploadLog(string userID, string fileName, string message, string status)
	{
		if (userID == string.Empty) userID = SessionHandler.Read("UserID");
		fileName = System.IO.Path.GetFileName(fileName);

		try
		{
			using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand("sp_WriteUploadLog", cn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@userID", userID);
					cmd.Parameters.AddWithValue("@message", message);
					cmd.Parameters.AddWithValue("@status", status);
					cmd.Parameters.AddWithValue("@fileName", fileName);
					cmd.Parameters.AddWithValue("@siteid", SessionHandler.Read("SiteID"));

					cmd.ExecuteNonQuery();
				}
				cn.Close();
			}
		}
		catch (Exception)
		{
		}
	}
}
