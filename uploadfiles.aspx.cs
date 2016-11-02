using System;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using DataLayer;

public partial class uploadfiles : BetterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			foreach (ListItem li in ddlCategory.Items)
			{
				if (li.Value != "")
				{
					if (!Security.Permission.CanWrite(li.Text, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
						li.Enabled = false;
				}
			}
		}
	}

	protected void btnSubmit_Click(object sender, EventArgs e)
	{
		bool Valid = false;
		//it depends on which type of file is being uploaded
		if (ddlCategory.SelectedValue == "")
		{
			Common.Alert(this, "Error: You must select a Category to continue.");
			ddlCategory.Focus();
			return;
		}
		DataMap dm = new DataMap();

		if (fu2.HasFile)
		{
			Document d = Document.Create(dm, fu2.PostedFile, ddlCategory.SelectedValue);
			dm.AttachedFile = d;
		}

		Valid = dm.Validate(); //critical validation step

		int count = 0;
		if (Valid)
			count = dm.InsertFile();

		MultiView1.SetActiveView(vwComplete);
		if (Valid && count > 0)
		{
			//display success text and log the upload
			litCompleteHeader.Text = "Successful Upload";
			string s = String.Format("{0} supporting file{1} {2} been uploaded successfully.",
				count, count == 1 ? "" : "s",
				count == 1 ? "has" : "have");
			lblSuccess.Text = s;
			Log.WriteUploadLog(SessionHandler.Read("UserID"), SessionHandler.Read("FileName"), s, "Success");
			Log.WriteAppLog(s);
			divErrs.Visible = false;
		}
		else
		{
			litCompleteHeader.Text = "Failed Upload";
			string s = String.Format("An error has occurred while uploading supporting documents");
			Log.WriteAppLog(s);
			//errors have already been logged at this point -- display them
			litErrors.Text = dm.Errors;
			divErrs.Visible = true;
		}
	}

	protected void btnUploadAnother_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/uploadfiles.aspx");
	}

	protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
	{
		bool isWaste = (ddlCategory.SelectedValue == "Waste & Recycling$");

		pnlWasteInstructions.Visible = isWaste;
		pnlGeneralInstructions.Visible = !isWaste;
	}

}
