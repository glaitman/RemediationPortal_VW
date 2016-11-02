using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Email
/// </summary>
public class Email
{
	private string REPLY_EMAIL = "Portal Administrator <no-reply@gateway.lfr.com>";
	private bool DEBUG = bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["Debug"]);
	private string MAIL_SERVER_ADDR = Convert.ToString(WebConfigurationManager.AppSettings["MailServer"]);
	private static string EmailCurrentUser = Security.User.GetEmailAddress(HttpContext.Current.User.Identity.Name);

	private string TO_EMAIL = EmailCurrentUser;
	private string CC_EMAIL = string.Empty;
	private string BCC_EMAIL = string.Empty;
	private string SUBJECT = string.Empty;
	private string BODY = string.Empty;
	private Attachment ATTACHMENT;


	public string ReplyEmail
	{
		get
		{
			return REPLY_EMAIL;
		}
		set
		{
			this.REPLY_EMAIL = value;
		}
	}

	/// <summary>
	/// An e-mail address (or comman deliminated list) of the main recipient(s).
	/// </summary>
	public string ToEmail
	{
		get
		{
			return TO_EMAIL;
		}
		set
		{
			this.TO_EMAIL = value;
		}
	}

	/// <summary>
	/// An e-mail address (or comman deliminated list) of the CC'd recipient(s).
	/// </summary>
	public string CCEmail
	{
		get
		{
			return CC_EMAIL;
		}
		set
		{
			this.CC_EMAIL = value;
		}
	}

	/// <summary>
	/// An e-mail address (or comman deliminated list) of the Bcc'd recipient(s).
	/// </summary>
	public string BccEmail
	{
		get
		{
			return BCC_EMAIL;
		}
		set
		{
			this.BCC_EMAIL = value;
		}
	}

	public string MessageSubject
	{
		get
		{
			return SUBJECT;
		}
		set
		{
			this.SUBJECT = value;
		}
	}

	/// <summary>
	/// The body of the e-mail message. This accepts HTML and RTF text.
	/// </summary>
	public string MessageBody
	{
		get
		{
			return BODY;
		}
		set
		{
			this.BODY = value;
		}
	}


	public Attachment MessageAttachment
	{
		get
		{
			return ATTACHMENT;
		}
		set
		{
			this.ATTACHMENT = value;
		}
	}

	/// <summary>
	/// Creates a message object. It assumes the To address is 
	/// the logged on user, the sender address is the default 
	/// LFR IDEA email sender (no-reply@lfr.com). No Subject or
	/// Body for the message is set.
	/// </summary>
	public Email()
	{
		this.TO_EMAIL = EmailCurrentUser;
	}

	/// <summary>
	/// Creates a message object. It assumes the To address is 
	/// the logged on user, the sender address is the default 
	/// LFR IDEA email sender (no-reply@lfr.com).
	/// </summary>
	/// <param name="MsgSubject">The subject line of the message.</param>
	/// <param name="MsgBody">The body of the message. This accepts HTML and RTF text.</param>
	public Email(string MsgSubject, string MsgBody)
	{
		this.SUBJECT = MsgSubject;
		this.BODY = MsgBody;
		this.TO_EMAIL = EmailCurrentUser;
	}

	/// <summary>
	/// Creates a message object for a particular recipient. It assumes the 
	/// sender address is the default LFR IDEA email sender (no-reply@lfr.com).
	/// </summary>
	/// <param name="MsgSubject">The subject line of the message.</param>
	/// <param name="MsgBody">The body of the message. This accepts HTML and RTF text.</param>
	/// <param name="MsgTo">An e-mail address (or comman deliminated list) of the main recipient(s)</param>
	public Email(string MsgSubject, string MsgBody, string MsgTo)
	{
		this.SUBJECT = MsgSubject;
		this.BODY = MsgBody;
		this.TO_EMAIL = MsgTo;
	}


	/// <summary>
	/// Returns a comma seperated list of users who should receive certain specified emails.
	/// </summary>
	/// <param name="emailType">Either "Incident" for the 'Receive Event Emails' permission
	/// or "Review" for the 'Receive Reviewed Event Emails' permission.</param>
	/// <returns>comma seperated string of email addresses.</returns>
	public string GetEmailRecipients(string permissionName)
	{
		string sendTo = string.Empty;

		string SQL = @"
SELECT m.email
FROM membershipsitepermission msp
	INNER JOIN sitepermission sp on sp.sitepermissionid = msp.sitepermissionid
	INNER JOIN membershipsite ms on ms.membershipsiteid = msp.membershipsiteid
	INNER JOIN membership m on m.userid = ms.userid
WHERE sp.permissionname = @PermissionName
	AND ms.siteid = @SiteID
	AND msp.canread = 1
	AND m.email IS NOT NULL
		";

		using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(SQL, cn))
			{
				cmd.Parameters.AddWithValue("@PermissionName", permissionName);
				cmd.Parameters.AddWithValue("@SiteID", SessionHandler.Read("SiteID"));

				SqlDataReader sdr = null;
				try
				{
					sdr = cmd.ExecuteReader();
					while (sdr.Read())
					{
						if (!sdr.IsDBNull(0))
						{
							sendTo += String.Format("{0},", sdr.GetString(0));
						}
					}
					sdr.Close();
					cmd.Cancel();
				}
				catch (Exception ex)
				{
					string error = String.Format("An exception ({0}) occurred while attempting to process the query : {1}", ex, SQL);
					Exception newexp = new Exception(error);
					throw newexp;
				}
			}
			cn.Close();
		}

		return sendTo;

	}

	public void SendMail()
	{
		//initialize SMTP client
		SmtpClient c = null;
		try
		{
			c = new SmtpClient(MAIL_SERVER_ADDR);
			c.DeliveryMethod = DEBUG ? SmtpDeliveryMethod.Network : SmtpDeliveryMethod.PickupDirectoryFromIis;
		}
		catch (Exception ex)
		{
			throw new Exception("Could not initialize SMTP client.", ex);
		}
		if (c == null) throw new Exception("Could not initialize SMTP client.");

		const string header = @"
			<head>
				<style>
					body {
						font:normal normal 13px/1.2em Trebuchet MS,Verdana,Arial;
						color:#333;
						background:#fff;
					}
					td {
						font:normal normal 13px/1.2em Trebuchet MS,Verdana,Arial;
						color:#333;
						background:#ffffd5;
					}
				</style>
			</head>
		";


		string MsgBody;
		if (BODY.Contains("<html>"))
		{
			MsgBody = BODY;
		}
		else
		{
			MsgBody = String.Format(@"
			<html>
			{0}
			<body>
				{1}
			</body>
			</html>
		", header, BODY);
		}

		//create feedback mail
		MailMessage msg = null;
		try
		{
			msg = new MailMessage(REPLY_EMAIL, TO_EMAIL);
			msg.Subject = SUBJECT;
			msg.Body = MsgBody;
			if (CC_EMAIL != string.Empty) msg.CC.Add(CC_EMAIL);
			if (BCC_EMAIL != string.Empty) msg.Bcc.Add(BCC_EMAIL);
			if (ATTACHMENT != null) msg.Attachments.Add(ATTACHMENT);
			msg.SubjectEncoding = System.Text.Encoding.ASCII;
			msg.Priority = MailPriority.Normal;
			msg.IsBodyHtml = true;
		}
		catch (Exception ex)
		{
			if (msg != null) msg.Dispose();
			msg = null;
			throw new Exception("Could not create mail message.", ex);
		}

		if (msg == null) return;

		//send feedback mail
		try
		{
			if (DEBUG)
			{
				msg.Body = String.Format(@"
				<b>Message to be sent to:</b><br>{0}<br>
				<b>Message to be CC'd to:</b><br>{1}<br>
				<b>Message to be BCC'd to:</b><br>{2}
				<br><br><b>Message body:</b><br>{3}"
					, msg.To.ToString()
					, msg.CC.ToString()
					, msg.Bcc.ToString()
					, msg.Body);

				msg.To.Clear();
				msg.CC.Clear();
				msg.Bcc.Clear();

				if (EmailCurrentUser != null)
					msg.To.Add(EmailCurrentUser);
				else
					msg.To.Add("no-reply@gateway.lfr.com");
			}

			c.Send(msg);
		}
		catch (Exception ex)
		{
		//    //Make sure to log the error in the errorLog table.
		//    Log.WriteError(HttpContext.Current.User.Identity.Name
		//        , Request.Url.ToString()
		//        , Request.UserHostAddress.ToString()
		//        , ex);
			throw new Exception("Could not deliver mail.", ex);
		}

		//cleanup feedback mail
		msg.Dispose();
		msg = null;

		return;
	}
}
