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
using System.Text;

public partial class MasterPage_master : System.Web.UI.MasterPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			lblLastLogin.Text = SessionHandler.Read("LastLogin");
		}
	}

	protected string WriteMenu()
	{
		string page = Request.Url.Segments[Request.Url.Segments.Length - 1];

		StringBuilder sb = new StringBuilder(1024);
		sb.Append(GetMenuString("index.aspx", "Home", page));

		sb.Append(GetMenuString("upload.aspx", "Upload Data", page));

        //sb.Append(GetMenuString("uploadfiles.aspx", "Upload Files", page));

		if (Security.Permission.CanRead("Reviewer", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			sb.Append(GetMenuString("reviewdata.aspx", "Review Data", page));

        //if (Security.Permission.CanRead("Reviewer", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
        //    sb.Append(GetMenuString("reviewfiles.aspx", "Review Files", page));

        sb.Append(GetMenuString("AnalyticalCharts.aspx", "Analytical Charts", page));

        if (Security.Permission.CanRead("Measurement Mgr.", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
            sb.Append(GetMenuString("MeasurementCategory.aspx?category=Measurement", "Measurement Mgr.", page));

		if (Security.Permission.CanRead("Users", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
			sb.Append(GetMenuString("users.aspx", "Users", page));

		if (Security.Permission.CanRead("Application Log", SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
            sb.Append(GetMenuString("logviewer.aspx", "Application Log", page));

        sb.Append(GetMenuString("preferences.aspx", "Preferences", page));
        sb.Append(GetMenuString("feedback.aspx", "Feedback", page));

		return sb.ToString();
	}

	protected string GetMenuString(string LinkURL, string Caption, string CurrentURL)
	{
		return String.Format("<a href='{0}' class='{1}'>{2}</a>", LinkURL, 
			(LinkURL == CurrentURL ? "Level1Selected" : "Level1"), Caption);
	}

	//protected void SetDates()
	//{
	//    _mastercal.SelectedDates.Clear();

	//    DataView dv = (DataView)sdsBlog.Select(new DataSourceSelectArguments());
	//    if (dv != null && dv.Table != null)
	//    {
	//        foreach (DataRow dr in dv.Table.Rows)
	//        {
	//            _mastercal.SelectedDates.Add(Convert.ToDateTime(dr["EntryDate"]));
	//        }
	//    }

	//}

	//protected void _mastercal_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
	//{
	//    SetDates();
	//}

	//protected void _mastercal_SelectionChanged(object sender, EventArgs e)
	//{
	//}
}
