using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class MeasurementCategory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)        
            SecurityCheck("Measurement Mgr.");                    

        //set the page title.
        string Category = Request.QueryString["category"];
        gvMain.Caption = string.Format("<b>Edit {0}</b>", Category);
    }

    protected void SecurityCheck(string PermissionName)
    {
        if (!Security.Permission.CanRead(PermissionName, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID")))
            Response.Redirect("~/index.aspx", false);

        Common.AuthorizeGrid(dvDetail, PermissionName);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        gvMain.DataBind();

        //select the first row or switch to insert mode
        if (gvMain.Rows.Count > 0)
        {
            if (gvMain.SelectedIndex == -1) gvMain.SelectedIndex = 0;
        }
        else
            if (dvDetail.CanInsert) dvDetail.ChangeMode(DetailsViewMode.Insert);
    }

    protected void DetailsView_DataBound(object sender, EventArgs e)
    {
        try
        {
            DetailsView dv = sender as DetailsView;
            if (dv == null) return;

            DataRowView drv = dv.DataItem as DataRowView;
            if (drv == null) return;

            string desc = String.Format("{0}", drv["ListValue"]).Replace("'", "");

            string DeleteConfirmation = String.Format("if(!confirm('Are you sure you want to remove {0}?')) return false;", desc);
            foreach (DetailsViewRow dvr in dv.Rows)
            {
                foreach (Control tc in dvr.Cells)
                {
                    foreach (Control c in tc.Controls)
                    {
                        Button lb = c as Button;
                        if (lb != null && lb.CommandName == "Delete")                        
                                lb.OnClientClick = DeleteConfirmation;                        
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    protected void HandleDataException(object sender, SqlDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            //display exception text in a nice way
            Common.Alert(this.Page, e.Exception.Message);
            e.ExceptionHandled = true;
            return;
        }
    }

    protected void DisplayTotalRecords(object sender, SqlDataSourceStatusEventArgs e)
    {
        HandleDataException(sender, e);

        litRecords.Text = String.Format("Total Records: {0}", e.AffectedRows);
    }

    protected void RefreshList(object sender, SqlDataSourceStatusEventArgs e)
    {
        HandleDataException(sender, e);

        if (e.AffectedRows > 0)
        {
            gvMain.DataBind();
        }
    }
    protected void DetailsView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
    {
        SqlDataSource sds = sdsMain;
        GridView gv = gvMain;

        if (e.AffectedRows < 1) return;
        string ID = SessionHandler.Read("ListManagerID");
        if (ID == "") return;

        SessionHandler.Remove("ListManagerID");

        //select the newly-added grid row
        int idx = -1;

        DataView dv = sds.Select(new DataSourceSelectArguments()) as DataView;
        for (int i = 0; i < dv.Table.Rows.Count; i++)
        {
            DataRow thisRow = dv.Table.Rows[i];
            if (thisRow["ListManagerID"].ToString() == ID)
            {
                idx = i;
                break;
            }
        }

        if (idx > -1)
        {
            int page = idx / gv.PageSize;
            int rowonpage = idx % gv.PageSize;
            gv.Sort("", SortDirection.Ascending);
            gv.PageIndex = page;
            gv.SelectedIndex = rowonpage;
        }
    }
    protected void sdsDetail_Inserted(object sender, SqlDataSourceStatusEventArgs e)
    {
        RefreshList(sender, e);
        if (e.Command.Parameters["@ID"].Value == DBNull.Value) return;
        string ID = e.Command.Parameters["@ID"].Value.ToString();
        SessionHandler.Write("ListManagerID", ID);
    }
}