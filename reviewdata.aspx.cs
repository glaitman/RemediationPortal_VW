using System;
using System.Web.UI.WebControls;
using Data_Access;

public partial class reviewdata : BetterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			SecurityCheck("Reviewer");

            //foreach (ListItem li in ddlCategory.Items)
            //{
            //    if (li.Value != "")
            //    {
            //        if (!Security.Permission.CanWrite(li.Text, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
            //            li.Enabled = false;
            //    }
            //}
		}
	}

	protected void SecurityCheck(string PermissionName)
	{
		if (!Security.Permission.CanRead(PermissionName, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			Response.Redirect("~/index.aspx", false);
	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
        //ImageButton btnSearch = new ImageButton();
        //btnSearch = btnSearchLogData;
	}

	protected void test(object sender, SqlDataSourceCommandEventArgs e)
	{
		Console.Write("");
	}

	protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
	{
        DropDownList ddl = sender as DropDownList;
        if (ddl == null) return;

        //ImageButton btnSearch = new ImageButton();
		switch (ddl.SelectedValue)
		{
			case "vwpump$":
				mvMain.Visible = true;
				mvMain.SetActiveView(vwLogData);
                gvLogData.DataBind();
                //btnSearch = btnSearchLogData;
				break;
            case "vwtide$":
                mvMain.Visible = true;
                mvMain.SetActiveView(vwTideData);
                gvTideData.DataBind();
                break;
			default:
				mvMain.Visible = false;
                //btnSearch = new ImageButton();
				break;
		}

        //if (btnSearch != new ImageButton())
        //    Form.DefaultButton = btnSearch.UniqueID;
	}

	#region " Text Search "

	protected void btnSearch_Click(object sender, CommandEventArgs e)
	{
		string sdsName = "";
		SqlDataSource sds = new SqlDataSource();
		TextBox txtSearch = new TextBox();
		GridView gv = new GridView();

		switch (e.CommandName)
		{
			default:
                //txtSearch = txtSearchLogData;
                //gv = gvLogData;
                //sdsName = "sdsPumpLogData";
                //sds = sdsPumpLogData;
				break;			
		}

		if (sdsName == "") return;

		//no matter what, clear out the prior search criteria
		gv.SelectedIndex = -1;
        sdsTextSearch.FilterExpression = "1=0 ";
        sdsTextSearch.FilterParameters.Clear();

		if (txtSearch.Text == "")
			gv.DataSourceID = sdsName;
		else
		{
            sdsTextSearch.SelectCommand = sds.SelectCommand;
			gv.DataSourceID = "sdsTextSearch";
			string arg = txtSearch.Text;
			//2007-07-21 MM
			//This will guard against SQL injection in a limited way. The desired use of the
			//search box to return all records (restore the grid to its original form) when 
			//searching for an empty string doesn't work if the query encloses a parameter
			//in the form: like '%'+@arg+'%'. Of course, if there's another way to do this
			//and achieve the same result while making the data more secure, I'm all for it.
			arg = arg.Replace("'", "''");
			arg = arg.Replace("--", "");

			foreach (DataControlField dcf in gv.Columns)
			{
				BoundField bf = dcf as BoundField;
				if (bf != null)
				{
					//if the field is a date or numeric do an = if not, do a LIKE
					if (bf.DataField == "ModifiedDate" || bf.DataField == "LogDate")
					{
						//only if the values are date types for these fields, do the search
						DateTime dt;
                        if (DateTime.TryParse(arg, out dt)) 
                            sdsTextSearch.FilterExpression += " OR " + bf.DataField + " = '{0}' ";
					}
					else if (bf.DataField == "WellID" || bf.DataField == "LogValue")
					{
						//only if the values are numeric types for these fields, do the search
						float f;
						if (float.TryParse(arg, out f))
							sdsTextSearch.FilterExpression += " OR " + bf.DataField + " = {0} ";	
					}
					else
						sdsTextSearch.FilterExpression += " OR " + bf.DataField + " LIKE '%{0}%' ";

					Parameter p = new Parameter(bf.DataField);
					sdsTextSearch.FilterParameters.Add(p);
				}
			}
			foreach (Parameter p in sdsTextSearch.FilterParameters)
			{
				p.DefaultValue = arg;
			}
		}
	}
	#endregion
    protected void sdsPumpLogData_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {

    }
    protected void gv_PreRender(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        if (gv == null) return;

        mvMain.Visible = gv.Rows.Count > 0;
    }
}
