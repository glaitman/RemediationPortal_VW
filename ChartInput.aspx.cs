using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.DataVisualization.Charting;

public partial class ChartInput : System.Web.UI.Page
{    
    DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;
    int sHours = 00, eHours = 00, sMinutes = 00, eMinutes = 00, Seconds = 00;
    string categoryY1 = "", categoryY2 = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {

    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (validate())
        {
            pnlErrors.Visible = false;
            try
            {
                startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, sHours, sMinutes, Seconds);
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, eHours, eMinutes, Seconds);

                //get selected wells
                List<Well> l = new List<Well>();
                foreach (ListItem li in chkWellID.Items)
                {
                    if (li.Selected)
                    {
                        l.Add(new Well()
                        {
                            WellID = int.Parse(li.Value),
                            Label = li.Text
                        });
                    }
                }

                /*
                 * add necessary parameters to session for later retrieval
                 */
                Session.Add(ChartParameters.Wells, l);
                Session.Add(ChartParameters.StartDate, startDate);
                Session.Add(ChartParameters.EndDate, endDate);

                //add primary Y-Axis parameters
                SessionHandler.Write(ChartParameters.CategoryY1, categoryY1);
                Session.Add(categoryY1, AxisType.Primary);

                //add secondary Y-Axis parameters
                SessionHandler.Write(ChartParameters.CategoryY2, categoryY2);
                Session.Add(categoryY2, AxisType.Secondary);

                Response.Redirect("~/AnalyticalCharts.aspx", false);
            }
            catch (Exception ex)
            {
                //log error
                Common.Alert(this.Page, ex.ToString());
            }
            finally
            {

            }
        }
        else
        {
            pnlErrors.Visible = true;
            Common.Alert(this.Page, "Some input parameters are invalid. Please review the error summary...");
        }
    }

    protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox cb = sender as CheckBox;
        if (cb == null) return;

        foreach (ListItem li in chkWellID.Items)
            li.Selected = cb.Checked;
    }

    private bool validate()
    {
        blErrors.Items.Clear();

        if (chkWellID.SelectedIndex < 0)
            blErrors.Items.Add("You must select at least 1 well.");        

        categoryY1 = ddlCategory.SelectedValue;
        categoryY2 = ddlCategory2.SelectedValue;
        if (String.IsNullOrEmpty(categoryY1) && String.IsNullOrEmpty(categoryY2))
            blErrors.Items.Add("You must select at least 1 Measurement Category.");
        else if (categoryY1 == categoryY2)
            blErrors.Items.Add("Each selected Measurement Category must be unique.");

        //Parse the values for start date and time
        if (!String.IsNullOrEmpty(txtSHours.Text) && !int.TryParse(txtSHours.Text, out sHours) || sHours > 23)
            blErrors.Items.Add("Value entered for field 'Hours' in 'Start Date/Time' section is incorrect format.");

        if (!String.IsNullOrEmpty(txtSMinutes.Text) && !int.TryParse(txtSMinutes.Text, out sMinutes) || sMinutes > 59)
            blErrors.Items.Add("Value entered for field 'Minutes' in 'Start Date/Time' section is incorrect format.");

        if (!DateTime.TryParse(cbStartDt.SelectedValue, out startDate))
            blErrors.Items.Add("You must enter a value for field 'Date' in section 'Start Date/Time'");
                  
        //Parse the values for end date and time
        if (!String.IsNullOrEmpty(txtEHours.Text) && !int.TryParse(txtEHours.Text, out eHours) || eHours > 23)
            blErrors.Items.Add("Value entered for field 'Hours' in 'End Date/Time' section is incorrect format.");

        if (!String.IsNullOrEmpty(txtEMinutes.Text) && !int.TryParse(txtEMinutes.Text, out eMinutes) || eMinutes > 59)
            blErrors.Items.Add("Value entered for field 'Minutes' in 'End Date/Time' section is incorrect format.");

        if (!DateTime.TryParse(cbEndDt.SelectedValue, out endDate))
            blErrors.Items.Add("You must enter a value for field 'Date' in section 'End Date/Time'");

        if (DateTime.Compare(startDate, endDate) > 0)
            blErrors.Items.Add("Start Date cannot come after End Date.");

        return blErrors.Items.Count <= 0;
    }
}