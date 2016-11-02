using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Web.Configuration;

public partial class users : BetterPage
{
	protected bool USE_SSO = false;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//first, see if the user is supposed to be here
			if (!Security.Permission.CanRead("Users", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
				Response.Redirect("~/index.aspx", false);
		}

		if (!Security.Permission.CanWrite("Users", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
		{
			pnlPermission.Visible = false;
			pnlInstructions.Visible = false;
			pnlDetail.Visible = false;
		}
		pnlNewUser.Visible = Security.Permission.CanCreate("Users", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID"));


		//DataView dv = (DataView)sdsHomeDirectory.Select(new DataSourceSelectArguments());
		//if (dv != null && dv.Table != null && dv.Table.Rows.Count > 0)
		//{
		//	if (dv.Table.Rows[0][0] == DBNull.Value)
		//		txtHomeDirectory.Text = String.Empty;
		//	else
		//		txtHomeDirectory.Text = dv.Table.Rows[0][0].ToString();
		//}

	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		gvUsers.DataBind();

		//select the first row
		if (gvUsers.Rows.Count > 0)
		{
			if (gvUsers.SelectedIndex == -1) gvUsers.SelectedIndex = 0;
		}
		
		Form.DefaultButton = btnSearch.UniqueID;
		txtSearch.Focus();
	}

	protected void btnInsert_Click(object sender, EventArgs e)
	{
		if (!Security.Permission.CanWrite("Users", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			return;


		int result = sdsPermission.Insert();
		if (result < 1)
			lblError.Text = "User already has this permission.";
	}

	protected void HandleDataException(object sender, SqlDataSourceStatusEventArgs e)
	{
		if (e.Exception != null)
		{
			lblError.Text = "Could not connect to database server";
			e.ExceptionHandled = true;
		}
	}

	//protected void btnReset_Click(object sender, EventArgs e)
	//{
	//	if(gvUsers.SelectedRow == null || gvUsers.SelectedRow.Cells == null && gvUsers.SelectedRow.Cells.Count < 2) return;
	//	string username = gvUsers.SelectedRow.Cells[1].Text;

	//	if (Security.User.ResetPassword(username, txtPass.Text))
	//		lblMessage.Text = String.Format("Successfully reset password for {0}", username);
	//	else
	//		lblError.Text = String.Format("Failed to reset password for {0}", username);
	//}

	protected string GeneratePassword()
	{
		const int PASSWORD_LENGTH = 8;

		//generate a random password of hex digits
		string password = String.Empty;

		int digit;
		string hex;
		Random r = new Random();
		for (int i = 0; i < PASSWORD_LENGTH; i++)
		{
			digit = r.Next(1, 16);
			hex = String.Format("{0:x}", digit);
			password += hex;
		}

		return password;
	}
	/// <summary>
	/// Sends an e-mail message to a user alerting them that they have been added to the site.
	/// The message includes the new username and password.
	/// </summary>
	/// <param name="password">string</param>
	protected void SendNewUserMail(string password)
	{
		string MAIL_SERVER_ADDR = Convert.ToString(WebConfigurationManager.AppSettings["MailServer"]);
		const string SUBJECT = "Web Portal Account";
		const string REPLY_EMAIL = "Portal Administrator <no-reply@lfr.com>";

		//initialize SMTP client
		SmtpClient c = null;
		try
		{
			c = new SmtpClient(MAIL_SERVER_ADDR);
			c.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
		}
		catch (Exception ex)
		{
			throw new Exception("Could not initialize SMTP client.", ex);
		}
		if (c == null) throw new Exception("Could not initialize SMTP client.");

		lblError.Text = "";

		//create feedback message
		string HOST_URL = Request.Url.Scheme + "://" + Request.Url.Authority;
		string SITE_URL = HOST_URL + Request.ApplicationPath;

		string BodyText = null;

		if (password == null)
			BodyText = String.Format(@"
				<div style='font:normal normal 12px/1.4em Arial,sans-serif;'>
				An account has been created for you on LFR's web portal.
				The web portal is a service of LFR to keep track of progress and share information
				across the duration of a project.
				Your username is below:<br>
				<br>
				Username: <b>{0}</b><br>
				Please use your existing Portal password.<br>
				<br>
				The website is located at <a href='{1}'>{1}</a>.<br>
				<br>
				Thanks,<br>
				LFR<br>
				<br>
				</div>
			", txtEmail.Text, SITE_URL);
		else
			BodyText = String.Format(@"
				<div style='font:normal normal 12px/1.4em Arial,sans-serif;'>
				An account has been created for you on LFR's web portal.
				The web portal is a service of LFR to keep track of progress and share information
				across the duration of a project.
				Your username and password are below:<br>
				<br>
				Username: <b>{0}</b><br>
				Password: <b>{1}</b><br>
				<br>
				The website is located at <a href='{2}'>{2}</a>. Please change your password when you 
				login.<br>
				<br>
				Thanks,<br>
				LFR<br>
				<br>
				</div>
			", txtEmail.Text, password, SITE_URL);

		//create feedback mail
		MailMessage msg = null;
		try
		{
			msg = new MailMessage(REPLY_EMAIL, txtEmail.Text);
			msg.Subject = SUBJECT;
			msg.Body = BodyText;
			msg.SubjectEncoding = Encoding.ASCII;
			msg.IsBodyHtml = true;
		}
		catch (Exception ex)
		{
			msg.Dispose();
			msg = null;
			throw new Exception("Could not create mail message.", ex);
		}

		//send feedback mail
		try
		{
			c.Send(msg);
		}
		catch (Exception ex)
		{
			msg.Dispose();
			msg = null;
			throw new Exception("Could not deliver mail.", ex);
		}

		//cleanup feedback mail
		msg.Dispose();
		msg = null;
	}

	protected void btnCreate_Click(object sender, EventArgs e)
	{
		if (!Page.IsValid) return;
		if (!Security.Permission.CanCreate("Users", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			return;


		if (Security.User.Exists(txtEmail.Text))
		{
			if (Security.User.IsMember(txtEmail.Text, SessionHandler.Read("SiteID")))
				lblError.Text = String.Format("{0} is already a member of this site.", txtEmail.Text);
			else
			{
				pnlAddUser.Visible = true;
				lblUserExists.Text = String.Format("{0} already exists in the database. Give access to this site?", txtEmail.Text);
			}
			return;
		}

		//generate a random password
		string password = GeneratePassword();

		//attempt to create the user record in the database
		Security.User u = null;
		u = Security.User.Create(txtEmail.Text, password, txtEmail.Text, txtFirstName.Text, txtLastName.Text,
			txtCompany.Text, String.Empty, String.Empty, String.Empty);
		if (u == null) //error
		{
			lblError.Text = String.Format("Error: Could not create {0}.", txtEmail.Text);
			return;
		}

		//then add the user to the site
		if (!Security.User.AddToSite(txtEmail.Text, SessionHandler.Read("SiteID")))
		{
			lblError.Text = String.Format("Error: Could not add {0} to this site.", txtEmail.Text);
			return;
		}

		string s = String.Format("Account created for {0}", txtEmail.Text);
		Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, s, Request.UserHostAddress);
		

		lblError.Text = "";
		lblMessage.Text = String.Format("Successfully created {0} and sent login information. Use the grid above to add permissions to the new user.", txtEmail.Text);

		SendNewUserMail(password);

		foreach (Control ct in pnlNewUser.Controls)
			if (ct is TextBox)
				((TextBox)ct).Text = "";

		//update the grid
		gvUsers.DataBind();
	}

	protected void btnOK_Click(object sender, EventArgs e)
	{
		pnlAddUser.Visible = false;
		if (Security.User.AddToSite(txtEmail.Text, SessionHandler.Read("SiteID")))
		{
			SendNewUserMail(null);

			lblMessage.Text = String.Format("Successfully added {0} to this site.", txtEmail.Text);
			string s = String.Format("Account created for {0}", txtEmail.Text);
			Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, s, Request.UserHostAddress);

			foreach (Control ct in pnlNewUser.Controls)
				if (ct is TextBox)
					((TextBox)ct).Text = "";

			//update the grid
			gvUsers.DataBind();
		}
		else
			lblMessage.Text = String.Format("Error: Could not add {0} to this site.", txtEmail.Text);
	}

	protected void btnCancel_Click(object sender, EventArgs e)
	{
		pnlAddUser.Visible = false;
	}

	protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		try
		{
			//DataRowView drv = e.Row.DataItem as DataRowView;

			foreach (TableCell tc in e.Row.Cells)
			{
				foreach (Control c in tc.Controls)
				{
					LinkButton lb = c as LinkButton;
					if (lb != null && lb.CommandName == "Delete")
						lb.OnClientClick = "return confirm('Are you sure you want to remove this row? You can add it again later.');";
				}
			}
		}
		catch (Exception)
		{
		}
	}

	//protected void btnSetDirectory_Click(object sender, EventArgs e)
	//{
	//	int count = sdsHomeDirectory.Update();

	//	string username = String.Empty;
	//	if (gvUsers.SelectedRow != null && gvUsers.SelectedRow.Cells != null && gvUsers.SelectedRow.Cells.Count > 0)
	//		username = gvUsers.SelectedRow.Cells[1].Text;

	//	if (count < 1)
	//		lblError.Text = String.Format("Error: Could not set home directory to {0} for {1}", txtHomeDirectory.Text, username);
	//	else
	//		lblMessage.Text = String.Format("Home directory set to {0} for {1}", txtHomeDirectory.Text, username);
	//}

	protected void dvUser_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
	{
		if (e.AffectedRows < 1) return;

		string username = String.Empty;
		string password = String.Empty;

		foreach (string Key in e.NewValues.Keys)
		{
			if (Key.ToLower() == "username")
			{
				username = e.NewValues[Key].ToString();
				break;
			}
		}

		string s = String.Format("User {0} updated", username);
		Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, s, Request.UserHostAddress);

		TextBox t = dvUser.FindControl("txtPasswordReset") as TextBox;
		if (t == null) return;
		password = t.Text;

		//no update needed if either are empty
		if (username == String.Empty || password == String.Empty) return;

		if (!Security.User.ResetPassword(username, password))
			lblError.Text = String.Format("Could not reset password for {0}", username);

		s = String.Format("Password reset for user {0}", username);
		Log.WriteAppLog(SessionHandler.Read("UserID"), Request.Url.OriginalString, s, Request.UserHostAddress);

	}

	protected void sdsUserDetail_Status(object sender, SqlDataSourceStatusEventArgs e)
	{
		if (e.AffectedRows > 0) gvUsers.DataBind();

		if (e.Exception != null)
		{
			lblError.Text = "Could not connect to database server";
			e.ExceptionHandled = true;
		}
	}

	protected void DetailsView_DataBound(object sender, EventArgs e)
	{
		try
		{
			DetailsView dv = sender as DetailsView;
			if (dv == null) return;

			DataRowView drv = dv.DataItem as DataRowView;
			if (drv == null) return;

			string name = String.Format("{0} {1} ({2})", drv["firstname"], drv["lastname"], drv["username"]);

			foreach (DetailsViewRow dvr in dv.Rows)
			{
				foreach (Control tc in dvr.Cells)
				{
					foreach (Control c in tc.Controls)
					{
						LinkButton lb = c as LinkButton;
						if (lb != null && lb.CommandName == "Delete")
							lb.OnClientClick = String.Format("return confirm('{0} will no longer be able to access this website. Are you sure you want to do this?');", name);
						else if (lb != null && lb.CommandName == "New")
						{
							if (drv["lockedout"] == DBNull.Value || drv["lockedout"].ToString() == "False")
							{
								lb.Text = "Lock";
								lb.CommandName = "Lock";
								lb.CommandArgument = drv["username"].ToString();
								lb.ToolTip = "Lock this account";
								lb.OnClientClick = String.Format("return confirm('{0} will no longer be able to access this website while locked out. If unlocked later, an e-mail {1}will be sent to the user. Are you sure you want to do this?');", name, USE_SSO ? "" : "with a new password ");
							}
							else
							{
								lb.Text = "Unlock";
								lb.CommandName = "Unlock";
								lb.CommandArgument = drv["username"].ToString();
								lb.ToolTip = "Unlock this account";
								lb.OnClientClick = String.Format("return confirm('{0} will receive an e-mail {1}and be permitted to access this website again. Are you sure you want to do this?');", name, USE_SSO ? "" : "with a new password ");
							}
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	protected void dvUser_ItemCommand(object sender, DetailsViewCommandEventArgs e)
	{
		string username = "";
		if (e.CommandArgument != null) username = e.CommandArgument.ToString();
		if (username == "")
		{
			lblError.Text = String.Format("Error: No username is selected.");
			return;
		}

		switch (e.CommandName)
		{
			case "Lock":
				if (!Security.User.Lock(username))
					lblError.Text = String.Format("Error: Could not lock account for {0}.", username);
				else
				{
					dvUser.DataBind();
					gvUsers.DataBind();
					lblMessage.Text = String.Format("Successfully locked out {0}.", username);
					//Log.Write(SessionHandler.Read(Constants.LOGIN_INFO), Request.Url.OriginalString, String.Format("Account locked by administrator for: {0}", username), Request.UserHostAddress);
				}
				break;
			case "Unlock":
				if (!Security.User.Unlock(username))
					lblError.Text = String.Format("Error: Could not unlock account for {0}.", username);
				else
				{
					dvUser.DataBind();
					gvUsers.DataBind();
					lblMessage.Text = String.Format("Successfully unlocked account for {0}.", username);
					//Log.Write(SessionHandler.Read(Constants.LOGIN_INFO), Request.Url.OriginalString, String.Format("Account unlocked by administrator for: {0}", username), Request.UserHostAddress);
				}
				break;
		}
	}

	protected void DisplayTotalRecords(object sender, SqlDataSourceStatusEventArgs e)
	{
		HandleDataException(sender, e);

		//pnlSummary.Visible = e.AffectedRows > 0;
		litRecords.Text = String.Format("Total Records: {0}", e.AffectedRows);
	}

	#region " Text Search "

	protected void sdsTextSearch_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
	{
		string arg = txtSearch.Text;

		//2007-07-21 MM
		//This will guard against SQL injection in a limited way. The desired use of the
		//search box to return all records (restore the grid to its original form) when 
		//searching for an empty string doesn't work if the query encloses a parameter
		//in the form: like '%'+@arg+'%'. Of course, if there's another way to do this
		//and achieve the same result while making the data more secure, I'm all for it.
		arg = arg.Replace("'", "''");
		arg = arg.Replace("--", "");

		//replace wildcards with string to search for
		e.Command.CommandText = String.Format(e.Command.CommandText, arg);
	}

	protected void btnSearch_Click(object sender, EventArgs e)
	{
		gvUsers.SelectedIndex = -1;
		gvUsers.PageIndex = 0;
		gvUsers.DataSourceID = (txtSearch.Text == "" ? "sdsUsers" : "sdsTextSearch");
		dvUser.ChangeMode(DetailsViewMode.ReadOnly);
	}

	#endregion
}
