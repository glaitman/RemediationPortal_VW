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
using System.Text;

public partial class logviewer : BetterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//if (!Security.Permission.CanRead("Application Log", SessionHandler.Read(Constants.LOGIN_INFO)))
			//    Response.Redirect("~/index.aspx", false);
		}

		Form.DefaultButton = btnSearch.UniqueID;

		txtSearch.Attributes["onFocus"] = String.Format(@"javascript:selrange({0}, {1}, {2})", txtSearch.ClientID, 0, txtSearch.Text.Length);
		txtSearch.Focus();

		gvLog.DataBind();
	}

	protected void HandleDataException(object sender, SqlDataSourceStatusEventArgs e)
	{
		if (e.Exception != null)
		{
			//display exception text in a nice way
			lblError.Text = String.Format("Error: {0}", e.Exception.Message);
			e.ExceptionHandled = true;
			return;
		}
	}

	protected void DisplayTotalRecords(object sender, SqlDataSourceStatusEventArgs e)
	{
		HandleDataException(sender, e);

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
		gvLog.SelectedIndex = -1;
		gvLog.DataSourceID = (txtSearch.Text == "" ? "sdsLog" : "sdsTextSearch");
	}

	#endregion

}
