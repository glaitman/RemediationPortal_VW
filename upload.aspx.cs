using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using System.IO;
using System.Xml;
using DataLayer;

public partial class upload : BetterPage
{
    Worksheet w;
	protected void Page_Init(object sender, EventArgs e)
	{
		//2008-11-06 MM
		//Create the table every postback, and make sure it is done early enough in the page lifecycle 
		//to receive ViewState information on postback.
		if (IsPostBack)
		{
			DataTable dt = CacheHandler.Read(SessionHandler.Read("MappingDataID")) as DataTable;
			if (dt != null)
			{
				LoadPanels(dt, pnlMapFields);
			}
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//foreach(ListItem li in ddlCategory.Items)
			//{
			//    if (li.Value != "")
			//    {
			//        if (!Security.Permission.CanWrite(li.Text, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			//            li.Enabled = false;
			//    }
			//}
		}
	}

	protected void ib_Command(object sender, CommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "Back":
				MultiView1.ActiveViewIndex--;
				//TODO: we may need to discard or update session objects here depending on the ActiveViewIndex
				return;
			case "BackMapDocs":
				//
                DataMap dm = CreateDataMap(SessionHandler.Read("UploadCategory"));
				ResetMapDocs(dm);
				PopulateMapDocFields(dm);
				return;
			case "Skip":
				MultiView1.ActiveViewIndex--;
				//TODO: we may need to discard or update session objects here depending on the ActiveViewIndex
				return;
		}
	}

	protected DataMap CreateDataMap(string ID)
	{
		xdsDataSchema.XPath = "Table[@ID='" + ID + "']";
		XmlDocument xmlCriteriaSchema = (XmlDocument)xdsDataSchema.GetXmlDocument();
		XmlNode DataRoot = xmlCriteriaSchema.DocumentElement;

		////TODO: check that the path exists in SchemaFiles before opening it
		//xdsFileSchema.XPath = "Table[@ID='" + ID + "']";
		//XmlDocument xmlCriteriaFiles = (XmlDocument)xdsFileSchema.GetXmlDocument();
		XmlNode FileRoot = null;

		DataMap dm = DataMap.Create(DataRoot, FileRoot, ID);
		return dm;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="dt"></param>
	/// <param name="p"></param>
	/// <returns>returns true if all fields match the default values</returns>
	protected bool LoadPanels(DataTable dt, Panel p)
	{
		string Category = String.Empty;
		Table t = null;
		TableRow tr = null;
		TableCell tc = null;
		Literal l = null;
		DropDownList ddl = null;
		CheckBox chk = null;

		if (p.Controls.Count > 0)
			p.Controls.Clear();

		//create new table
		t = new Table();
		t.ID = "Table1";
		t.CellPadding = 2;
		t.CssClass = "grid";

		//header row
		tr = new TableRow();
		tr.CssClass = "gridheader";
		l = new Literal();
		l.Text = String.Format("Source *");
		tc = new TableCell();
		tc.Width = Unit.Percentage(40);
		tc.Controls.Add(l);
		tr.Cells.Add(tc);

		l = new Literal();
		l.Text = String.Format("Destination");
		tc = new TableCell();
		tc.Width = Unit.Percentage(30);
		tc.Controls.Add(l);
		tr.Cells.Add(tc);

		l = new Literal();
		l.Text = String.Format("Data Type");
		tc = new TableCell();
		tc.Width = Unit.Percentage(20);
		tc.Controls.Add(l);
		tr.Cells.Add(tc);

		l = new Literal();
		l.Text = String.Format("Required");
		tc = new TableCell();
		tc.Width = Unit.Percentage(10);
		tc.Controls.Add(l);
		tr.Cells.Add(tc);

		t.Rows.Add(tr);

		int count = 0;

		//create the data structure for the selected category
		DataMap dm = CreateDataMap(SessionHandler.Read("UploadCategory"));
		//loop through the structure's columns to create a page that asks the user
		//	to match the columns of the spreadsheet to those that it expects in the schema.
		for (int i = 0; i < dm.Fields.Count; i++)
		{
			DataLayer.Field f = dm.Fields[i];
			//now clean up all those funky characters. Using a string, because cannot manipulate the f.Default obj
			string fDefault = f.Default.Replace("\\n", "\n");

			tr = new TableRow();
			if (i % 2 == 1) tr.CssClass = "gridrowhiglight";

			//create a single dropdown list of all the columns in the spreadsheet. 
			//	this gives the user a ddl to select a spreadsheet column from.
			//	This must occur within the loop or only 1 will get created.
			ddl = new DropDownList();
			ddl.Items.Add(new ListItem("Please select...", ""));
			for (int j = 0; j < dt.Columns.Count; j++)
			{
				DataColumn dc = dt.Columns[j];
				ddl.Items.Add(new ListItem(dc.ColumnName));
			}

			if (ddl.Items.FindByValue(fDefault) != null)
			{
				ddl.SelectedValue = fDefault;
				count++;
			}
			else
			{
				Console.Write("");
			}

			//there is a ddl for each row so it should have its own ID
			ddl.ID = String.Format("ddl{0}", i);

			//create the cell with the ddl
			tc = new TableCell();
			tc.Controls.Add(ddl);
			//clean up old ones.
			if (tc.BackColor == System.Drawing.Color.Salmon)
				tc.BackColor = System.Drawing.Color.Empty;
			//let the user know which ones they need to update by coloring it differently
			if (ddl.SelectedIndex == 0)
				tc.BackColor = System.Drawing.Color.Salmon;
			tr.Cells.Add(tc);

			l = new Literal();
			l.Text = f.Destination;
			tc = new TableCell();
			tc.Controls.Add(l);
			tr.Cells.Add(tc);

			l = new Literal();
			if (f.Length > 0)
				l.Text = String.Format("{0}({1})", f.DataType, f.Length);
			else
				l.Text = f.DataType;
			tc = new TableCell();
			tc.Controls.Add(l);
			tr.Cells.Add(tc);

			chk = new CheckBox();
			chk.Checked = f.Required;
			chk.Enabled = false;
			tc = new TableCell();
			tc.Controls.Add(chk);
			tr.Cells.Add(tc);

			t.Rows.Add(tr);
		}

		//we're going to indicate in session whether supporting documents section should be shown.
		SessionHandler.Write("DisableSupportingDocuments", dm.DisableSupportingDocuments.ToString());

		p.Controls.Add(t);

		return (count == dm.Fields.Count);
	}

	/// <summary>
	/// Gets the Map Docs to Issues form ready for detailed entry about files. It pulls the next available
	/// file record in the database that has the same uploadID and source file that also is not yet linked.
	/// Requires a Session "UploadID" from the files that were uploaded and need to be linked.
	/// </summary>
	/// <param name="dm">Fully populated DataMap object. Needs: dm.DataFiles.SelectInventoryQuery, dm.DataFiles.SelectFileQuery</param>
	/// <returns>-1 if there are no more files to update. 0 or greater if there are still files to be linked.</returns>
	protected int PopulateMapDocFields(DataMap dm)
	{
		if (dm == null || dm.DataFile == null) 
			return -1;

		DataMap.DataFiles df = dm.DataFile;

		//need the queries to go on.
		if (df.SelectInventoryQuery == null || df.SelectInventoryQuery == string.Empty
			|| df.SelectFileQuery == null || df.SelectFileQuery == string.Empty) 
			return -1;

		//need a source filename.
		string SourceFileName = SessionHandler.Read("FileName");
		SourceFileName = Path.GetFileName(SourceFileName);
		if (SourceFileName == "")
			return -1;

		//start at -1 so that if the query below is never ran, we exist with an indication that there was an issue.
		int row = -1;

		//clean out the existing data if its there.
		hFileID.Value = "";
		litMailCode.Text = "";
		litFileName.Text = "";
		tbDateCreated.Text = "";
		tbPreparedFor.Text = "";
		tbPreparedBy.Text = "";
		tbNotes.Text = "";
		ddlCategoryInsert.SelectedIndex = 0;

		//then populate the fields with this file's details
		using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(df.SelectFileQuery, cn))
			{
				cmd.Parameters.AddWithValue("@Source", SourceFileName);
				//the uploadid session is set when the form is initially submitted to the db.
				//	and use of this method needs to set the ID as well.
				cmd.Parameters.AddWithValue("@UploadID", SessionHandler.Read("UploadID"));
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						row++;

						string mailCode = Convert.ToString(reader["MailCode"]);
						if (mailCode == null || mailCode == "")
						{
							row = -1;
							break;
						}

						hFileID.Value = Convert.ToString(reader["FileID"]);
						litMailCode.Text = mailCode;
						litFileName.Text = Convert.ToString(reader["Filename"]);
						tbTitle.Text = Convert.ToString(reader["Title"]);
						tbDateCreated.Text = Convert.ToString(reader["DateCreated"]);
						tbPreparedBy.Text = Convert.ToString(reader["PreparedBy"]);
						tbPreparedFor.Text = Convert.ToString(reader["PreparedFor"]);
						tbNotes.Text = Convert.ToString(reader["Description"]);
						ddlCategoryInsert.SelectedValue = Convert.ToString(reader["Category"]);
						DateTime DateCreated = new DateTime();
						if (DateTime.TryParse(Convert.ToString(reader["DateCreated"]), out DateCreated))
							tbDateCreated.Text = DateCreated.ToShortDateString();

						//we only want the first row. 
						break;
					}
				}
			}
		}

		if (litMailCode.Text == "" || row == -1) return -1;

		//populate the dropdown after clearing it.
		rblInventory.Items.Clear();
		rblInventory.Items.Add(new ListItem("None", ""));
		//Caitlin asks that "None" always be the selected item in the list.
		rblInventory.SelectedIndex = 0;
		//pull the session uploadID instead of the datamap object because the dm value keeps changing the and session stays the same.
		string uploadID = SessionHandler.Read("UploadID");
		using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(df.SelectInventoryQuery, cn))
			{
				cmd.Parameters.AddWithValue("@Source", SourceFileName);
				cmd.Parameters.AddWithValue("@UploadID", uploadID);
				cmd.Parameters.AddWithValue("@MailCode", litMailCode.Text);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						//ListItem li = new ListItem(Convert.ToString(reader["Name"]), Convert.ToString(reader["InventoryID"]));
						//ddlInventory.Items.Add(li);
						String desc = string.Format("{0}", Convert.ToString(reader["Name"]));
						ListItem li = new ListItem(desc, Convert.ToString(reader["InventoryID"]));
						rblInventory.Items.Add(li);
					}
				}
			}
		}
		return row;
	}

	/// <summary>
	/// Clears all the mapped IDs for the files that were uploaded in this session, based on the UploadID and file Source.
	/// Requires a Session "UploadID" from the files that were uploaded and need to be linked.
	/// </summary>
	/// <param name="dm">Fully populated DataMap object. Needs: dm.DataFiles.RemoveFileAttachment</param>
	private void ResetMapDocs(DataMap dm)
	{
		try
		{
			DataMap.DataFiles df = dm.DataFile;
			using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(df.RemoveFileAttachment, cn))
				{
					//the uploadid session is set when the form is initially submitted to the db.
					//	and use of this method needs to set the ID as well.
					cmd.Parameters.AddWithValue("@UploadID", SessionHandler.Read("UploadID"));
					int results = cmd.ExecuteNonQuery();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	protected void btnUpload_Click(object sender, EventArgs e)
	{
		//read the contents of the file into a DataTable
		HttpPostedFile postedFile = fu1.PostedFile;

		string filename = Path.GetFullPath(postedFile.FileName);
		string extension = Path.GetExtension(postedFile.FileName);
		int contentLength = postedFile.ContentLength;

		SessionHandler.Write("FileName", Path.GetFileName(postedFile.FileName));

		if (extension.ToLower() != ".xls" && extension.ToLower() != ".xlsx" && extension.ToLower() != ".csv" && extension.ToLower() != ".txt")
		{
			lblError.Text = "Error: Only XLS, XLSX, CSV or TXT files are supported.";
			return;
		}
		if (contentLength <= 2)
		{
			lblError.Text = "Error: Selected file contains no data.";
			return;
		}

		//we have to know from here on out what type of data the program is
		//	working with. Use the session to determine throughout. 
		//	If nothing's selected, don't go on.
        string category = ddlCategory.SelectedValue;

		if (category != "")
		{
			//Category must go into the session so it can be accessed in Page_Init. Control values
			//won't be available this early in the page lifecycle since ViewState hasn't been restored yet.
			SessionHandler.Write("UploadCategory", category);			
		}
		else
		{
			Common.Alert(this, "Error: You must select a Category to continue.");
			return;
		}

        //Find out what type of object we are working with
        switch (category)
        {
            case "vwpump$":
                w = new VancouverLogWorksheet(fu1.PostedFile, chkColumnNames.Checked);
                break;
            case "vwtide$":
                w = new VancouverTideWorksheet(fu1.PostedFile);
                break;
            default:
                w = new Worksheet(fu1.PostedFile, chkColumnNames.Checked);
                break;

        }
		w.TabName = category;

		Worksheet.TableData td = w.GetDataTable();
		DataTable dt = td.dt;

		//If dt is null then there was an error generating the table data.
		//	In that case, the ex object of the TableData struct should be filled in.
		if (dt == null)
		{
			string s = string.Empty;

			if (td.ex == null)
				s = "Error: Selected file is not supported.";
			else
			{
				string tabName = w.TabName.EndsWith("$") ? w.TabName.Remove(w.TabName.Length - 1) : w.TabName;
				if (td.ex.GetType().Name == "OleDbException" && td.ex.Message.Contains(w.TabName))
				{
					s = string.Format("A tab labeled '{0}' cannot be found. The tab containing data to upload must be named '{0}'", tabName);
				}
				else
				{
					s = td.ex.Message;
				}
			}

			s = string.Format("Error: {0}", s);
			lblError.Text = s;
			return;
		}
		//if the datatable has 0 rows, then this is an empty template. Return it to the users.
		if (dt.Rows.Count == 0)
		{
			lblError.Text = "Error: The spreadsheet contains no rows of data.";
			return;
		}
		//The generated LocalFilePath will serve as the cacheID for the data table
		SessionHandler.Write("MappingDataID", w.LocalFilePath);		
		CacheHandler.Write(SessionHandler.Read("MappingDataID"), dt);

		MultiView1.SetActiveView(vwPreview);

        switch (category.ToLower())
        {
            case "vwpump$":
                DetailsView1.DataSource = dt.DefaultView.ToTable(false, new string[] { "WellID", "WellLabel", "Category", "LogDate", "LogValue", "Units" });
                break;
            case "vwtide$":
                DetailsView1.DataSource = dt.DefaultView.ToTable(false, new string[] { "DateTime", "Height" });
                break;
            default:
                DetailsView1.DataSource = dt;
                break;
        }

		DetailsView1.DataBind();
		litCount.Text = String.Format("Displaying 1 of {0} data rows", dt.Rows.Count);

		pnlMapFields.Controls.Clear();
		bool MappingComplete = LoadPanels(dt, pnlMapFields);
		if (MappingComplete)
		{
			btnContinue3.Visible = true;
			btnContinue.Visible = false;
		}


		//LoadPanels(dt);
	}

	protected void btnContinue_Click(object sender, EventArgs e)
	{
		MultiView1.SetActiveView(vwMapFields);

		DataTable dt = CacheHandler.Read(SessionHandler.Read("MappingDataID")) as DataTable;
		if (dt == null)
			dt = RefreshData(SessionHandler.Read("MappingDataID"));
		bool MappingComplete = LoadPanels(dt, pnlMapFields);
	}

	protected void btnContinue2_Click(object sender, EventArgs e)
	{
		//get the enclosing table
		Table t = Common.GetControl(pnlMapFields, "Table1") as Table;
		if (t == null)
		{
			Common.Alert(this, "Error: Can't find table mapping.");
			return;
		}

		for (int i = 1; i < t.Rows.Count; i++)
		{
			TableRow tr = t.Rows[i];

			if (tr.Cells[0].HasControls() && tr.Cells[0].Controls.Count > 0)
			{
				DropDownList ddl = tr.Cells[0].Controls[0] as DropDownList;
				//if (ddl == null) continue; //not a data row
				if (ddl.SelectedValue == "")
				{
					Common.Alert(this, "Error: One or more destination fields has no source value.");
					return;
				}
			}
		}

		if (SessionHandler.Read("DisableSupportingDocuments").ToLower() == "true")
		{
			btnSubmit_Click(sender, e);
		}
		else
		{
			MultiView1.SetActiveView(vwAttachDocs);
			bool isWaste = (SessionHandler.Read("UploadCategory").Contains("Waste"));

			pnlFileUploadWaste.Visible = isWaste;
			pnlFileUploadGeneral.Visible = !isWaste;
		}
	}

	/// <summary>
	/// This submit method is called after all files and spreadsheets have been selected. It is the
	/// submit that will warn the user that files are being sent to the server and then it calls those 
	/// methods to do all initial inserts.
	/// </summary>
	/// <param name="sender">Button object</param>
	/// <param name="e"></param>
	protected void btnSubmit_Click(object sender, EventArgs e)
	{
		//get the enclosing table
		Table t = Common.GetControl(pnlMapFields, "Table1") as Table;
		if (t == null)
		{
			Common.Alert(this, "Error: Could not update source table.");
			return;
		}

        DataMap dm = CreateDataMap(SessionHandler.Read("UploadCategory"));
		dm.ColumnNamesIn1stRow = chkColumnNames.Checked;


		//map fields
		for (int i = 0; i < dm.Fields.Count; i++)
		{
			DataLayer.Field f = dm.Fields[i];
			TableRow tr = t.Rows[i + 1];

			if (tr.Cells[0].HasControls() && tr.Cells[0].Controls.Count > 0)
			{
				DropDownList ddl = tr.Cells[0].Controls[0] as DropDownList;
				//if (ddl == null) continue; //not a data row

				string Source = ddl.SelectedValue;
				dm.MapField(Source, f.Destination);
			}
		}
		
		DataTable dt = CacheHandler.Read(SessionHandler.Read("MappingDataID")) as DataTable;
		dm.UploadData = dt;
		if (fu2.HasFile)
		{
			Document d = Document.Create(dm, fu2.PostedFile, SessionHandler.Read("UploadCategory"));
			dm.AttachedFile = d;
		}

		bool Valid = dm.Validate(); //critical validation step
		DataMap.DataCounts dc = null; //capture the number of rows inserted
		if (Valid)
		{
			dc = dm.Insert(); //insert the data
			CacheHandler.Remove(SessionHandler.Read("MappingDataID"));			
		}

		//make sure to save the uploadid for this upload session
		SessionHandler.Write("UploadID", dm.UploadID);

		//Check the validaty of the data thus far. If there's not a problem here, go on to data mapping if needed.
		//If there is a problem, exit out now with the info.
		if (Valid && dc != null && String.IsNullOrEmpty(dm.Errors))
		{
			DataMap.DataFiles df = dm.DataFile;
			//if there was a file uploaded AND the data object has a query in place for mapping docs to
			//  inventory, then we're going to allow the user to 
			//	map the docs to inventory records. If not, then just finish up.
			if (fu2 != null && fu2.FileName != null && fu2.FileName != "" && 
				df != null && df.SelectFileQuery != null && df.SelectFileQuery != "")
			{
				MultiView1.SetActiveView(vwMapDocs);
				int success = PopulateMapDocFields(dm);
				Valid = (success >= 0);
				if (Valid)
					Session.Add("DataCount", dc);	//Save the session with counts
			}
			else
			{
				MultiView1.SetActiveView(vwComplete);
				//display success text and log the upload
				litCompleteHeader.Text = "Successful Upload";

				//if this session doesn't allow file updates, then don't show a count for it.
				string files = string.Empty;
				if (SessionHandler.Read("DisableSupportingDocuments").ToLower() != "true")
				{
					files = string.Format(" and {0} supporting file{1}", dc.FilesInserted, dc.FilesInserted == 1 ? "" : "s"); 
				}
				string s = String.Format("{0} data row{1}{2} {3} been uploaded successfully.",
					dc.DataRowsInserted, dc.DataRowsInserted == 1 ? "" : "s",
					files, dc.FilesInserted == 1 ? "has" : "have");
				lblSuccess.Text = s;
                if (dc.DataRowsInserted == 0)
                {   //Let user know that spreadsheet contained duplicates
                    string info = @"<br /><br /><br />
                                    Note: If zero records were uploaded, then your spreadsheet contained duplicate data. If you feel 
                                    this is an error, you can report the issue to your system administrator or to the development team 
                                    by using the <a href=""feedback.aspx"">feed back</a> menu.";
                    lblSuccess.Text += info;
                }
				Log.WriteUploadLog(SessionHandler.Read("UserID"), SessionHandler.Read("FileName"), s, "Success");
				Log.WriteAppLog(s);
				divErrs.Visible = false;
			}
		}

		if (!Valid || dc == null || !String.IsNullOrEmpty(dm.Errors))
		{
			MultiView1.SetActiveView(vwComplete);
			litCompleteHeader.Text = "Failed Upload";
			string s = String.Format("An error has occurred while uploading the file {0}", SessionHandler.Read("FileName"));
			Log.WriteAppLog(s);
			//errors have already been logged at this point -- display them
			litErrors.Text = dm.Errors;
			divErrs.Visible = true;
		}

		//SessionHandler.Remove("FileName");
	}

	/// <summary>
	/// This submit is from the area that allows users to link documents to mailcodes inthe spreadsheet. 
	/// It will be clicked multiple times for each document the user has loaded (multiple if they have loaded
	/// a zip file with multiple docs or once if just a single doc). It determines that it is done linking docs
	/// to spreadsheet rows if the PopulateMapDocFields method doesn't return any more rows to deal with.
	/// </summary>
	/// <param name="sender">Button object</param>
	/// <param name="e"></param>
	protected void btnSubmitMapDocs_Click(object sender, EventArgs e)
	{
		//first save the data that was entered.
        DataMap dm = CreateDataMap(SessionHandler.Read("UploadCategory"));
		DataMap.DataFiles df = dm.DataFile;
		int results = 0;

		using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
		{
			cn.Open();
			using (SqlCommand cmd = new SqlCommand(df.UpdateFileQuery, cn))
			{
				cmd.Parameters.AddWithValue("@InventoryID", rblInventory.SelectedValue);
				cmd.Parameters.AddWithValue("@FileID", hFileID.Value);
				cmd.Parameters.AddWithValue("@Title", tbTitle.Text);
				cmd.Parameters.AddWithValue("@PreparedBy", tbPreparedBy.Text);
				cmd.Parameters.AddWithValue("@PreparedFor", tbPreparedFor.Text);
				cmd.Parameters.AddWithValue("@Description", tbNotes.Text);
				cmd.Parameters.AddWithValue("@DateCreated", tbDateCreated.Text);
				cmd.Parameters.AddWithValue("@Category", ddlCategoryInsert.SelectedValue);
				results = cmd.ExecuteNonQuery();
			}
		}
		//if nothing was updated or if there's an issue, then let the user know and try again.
		if (results <= 0)
		{
			Common.Alert(this.Page, "Error: Could not update file table.");
			return;
		}

		//try to populate the fields with the next file. If there is one, it will return > 0.
		int success = PopulateMapDocFields(dm);

		//if there are no more files to match, then move on.
		if (success < 0)
		{
			//switch to the completed view
			MultiView1.SetActiveView(vwComplete);

			//get the stored counts from the data insert prior to mapping.
			DataMap.DataCounts dc = Session["DataCount"] as DataMap.DataCounts;
			Session.Remove("DataCount"); //remove the table from the session as quickly as possible

			//show the relevant results
			if (dc != null && dc.DataRowsInserted > 0)
			{
				//display success text and log the upload
				litCompleteHeader.Text = "Successful Upload";

				//if this session doesn't allow file updates, then don't show a count for it.
				string files = string.Empty;
				if (SessionHandler.Read("DisableSupportingDocuments").ToLower() != "true")
				{
					files = string.Format(" and {0} supporting file{1}", dc.FilesInserted, dc.FilesInserted == 1 ? "" : "s");
				}
				string s = String.Format("{0} data row{1}{2} {3} been uploaded successfully.",
					dc.DataRowsInserted, dc.DataRowsInserted == 1 ? "" : "s",
					files, dc.FilesInserted == 1 ? "has" : "have");
				lblSuccess.Text = s;
				Log.WriteUploadLog(SessionHandler.Read("UserID"), SessionHandler.Read("FileName"), s, "Success");
				Log.WriteAppLog(s);
				divErrs.Visible = false;
			}
			else
			{
				litCompleteHeader.Text = "Failed Upload";
				string s = String.Format("An error has occurred while uploading the file {0}", SessionHandler.Read("FileName"));
				Log.WriteAppLog(s);
				//errors have already been logged at this point -- display them
				litErrors.Text = dm.Errors;
				divErrs.Visible = true;
			}

			SessionHandler.Remove("FileName");
		}

	}

	protected void btnUploadAnother_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/upload.aspx");
		//MultiView1.SetActiveView(vwUpload);
	}

	private DataTable RefreshData(string localFilePath)
	{
		Worksheet w = new Worksheet();
		w.TabName = SessionHandler.Read("UploadCategory");
		w.LocalFilePath = localFilePath;
		w.HasColumnNamesInFirstRow = chkColumnNames.Checked;

		DataTable dt = w.GetDataTable().dt;
		CacheHandler.Write(SessionHandler.Read("MappingDataID"), dt);

		return dt;
	}

}
