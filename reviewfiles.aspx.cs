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
using System.Data.SqlClient;
using System.Web.Configuration;

public partial class reviewfiles : BetterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			SecurityCheck("Reviewer");

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

	protected void SecurityCheck(string PermissionName)
	{
		if (!Security.Permission.CanRead(PermissionName, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			Response.Redirect("~/index.aspx", false);

		Common.AuthorizeGrid(dvFile, PermissionName);
	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		gvFile.DataBind();
		gvRemediation.DataBind();

		//select the first row
		if (gvFile.Rows.Count > 0)
		{
			if (gvFile.SelectedIndex == -1) gvFile.SelectedIndex = 0;
		}

		//select the first row
		if (gvRemediation.Rows.Count > 0)
		{
			if (gvRemediation.SelectedIndex == -1) gvRemediation.SelectedIndex = 0;
		}
	}

	protected void test(object sender, SqlDataSourceCommandEventArgs e)
	{
		Console.Write("");
	}

	//2007-01-20 MM
	//For a reason I can't determine, these events are not automatically fired by the enclosing GridView's
	//RowCommand event (gvFile_RowCommand).
	protected void ib_Command(object sender, CommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "View":
				DownloadFile(e.CommandArgument.ToString());
				break;
		}
	}

	protected void DownloadFile(string FileId)
	{

		string SQL = string.Empty;
		string category = ddlCategory.SelectedValue;

		switch (category)
		{
			case "Waste & Recycling$":
				SQL = @"
					select f.fileimage, f.contenttype, f.filename
					from up_WasteFiles f
					where f.upWasteFilesID = @fileid
				";
				break;
			case "Regulated Waste$":
				SQL = @"
					select f.fileimage, f.contenttype, f.filename
					from up_WasteFiles f
					where f.upWasteFilesID = @fileid
				";
				break;
			case "Site Remediation$":
				SQL = @"
					select f.fileimage, f.contenttype, f.filename
					from up_RemediationFiles f
					where f.upRemediationFilesID = @fileid
				";
				break;
			default:
				mvMain.Visible = false;
				break;
		}

		string disposition = null;
		byte[] image = null;
		string content_type = null;
		string filename = null;

		if (FileId != null && FileId != "")
		{
			using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand(SQL, conn))
				{
					command.Parameters.AddWithValue("@fileid", FileId);

					using (SqlDataReader sdr = command.ExecuteReader())
					{
						if (sdr.Read())
						{
							if (!sdr.IsDBNull(0))
								image = (byte[])sdr[0];
							if (!sdr.IsDBNull(1))
								content_type = (string)sdr[1];
							if (!sdr.IsDBNull(2))
								filename = (string)sdr[2];
						}
					}
				}
			}
		}

		if (image == null || content_type == null || image.Length == 0 || content_type.Length == 0)
		{
			return;
		}
		else
		{
			//if (content_type.Contains("pdf") || content_type.Contains("octet"))
			//    disposition = "inline";
			//else
			disposition = "attachment";


			Response.ClearHeaders();
			if (Response.Filter != null) Response.Filter = null;
			Response.Clear();
			Response.AddHeader("Content-Disposition", String.Format("{0}; filename={1}", disposition, filename));
			Response.AddHeader("Content-Length", image.Length.ToString());
			Response.ContentType = content_type;

			//Response.Filter = null;

			//Response.OutputStream.Write(image, 0, image.Length);
			Response.BinaryWrite(image);
			Response.End();


		}
	}

	protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
	{
		string category = ddlCategory.SelectedValue;
		switch (category)
		{
			case "Regulated Waste$":
				mvMain.Visible = true;
				mvMain.SetActiveView(vwWaste);
				break;
			case "Waste & Recycling$":
				mvMain.Visible = true;
				mvMain.SetActiveView(vwWaste);
				break;
			case "Site Remediation$":
				mvMain.Visible = true;
				mvMain.SetActiveView(vwRemediation);
				break;
			default:
				mvMain.Visible = false;
				break;
		}
	}
}
