using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

public partial class AnalyticalCharts : BetterPage
{
    PumpLogDataChart dataChart;
    DateTime? startDate = DateTime.MinValue, endDate = DateTime.MinValue;
    int sHours = 00, eHours = 00, sMinutes = 00, eMinutes = 00, Seconds = 00;
    string categoryY1 = "", categoryY2 = "";
    List<Well> wells;
    GraphType? graphType;

    
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {        
        //Get input parameters from session
        wells = Session[ChartParameters.Wells] as List<Well>;
        startDate = Session[ChartParameters.StartDate] as DateTime?;
        endDate = Session[ChartParameters.EndDate] as DateTime?;
        graphType = Session[ChartParameters.GraphType] as GraphType?;
        switch (graphType)
        {
            case GraphType.SingleGraph:
                //Add a placeholder list containing one well
                fvChart.DataSource = new List<Well>() { new Well() { WellID = 0, Label = "Multiple" } };
                fvChart.DataKeyNames = new string[] { "WellID" };
                fvChart.DataBind();
                break;
            default:
                fvChart.DataSource = wells;
                fvChart.DataKeyNames = new string[] { "WellID" };
                fvChart.DataBind();
                break;
        }
    }

    protected void fvChart_PageIndexChanging(object sender, FormViewPageEventArgs e)
    {
        FormView fv = sender as FormView;
        if (fv == null) return;

        fv.PageIndex = e.NewPageIndex;
    }

    protected void fvChart_DataBound(object sender, EventArgs e)
    {
        FormView fv = sender as FormView;
        if (fv == null || fv.DataItem == null) return;

        //Set Measurment Categories
        setCategories();

        /*
         * Create Chart and generate ChartArea and Series
         * for all of the data items in the DataChart object
         */
        Chart chart = Common.GetControl(fv, "Chart1") as Chart;        
        if (chart == null) return;
        

        //Generate graphs based on GraphType
        if (graphType.Value == GraphType.PagedGraph)
        {
            Well well = (Well)fv.DataItem;
            dataChart = new PumpLogDataChart(categoryY1, categoryY2)
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                ID = well.WellID,
                Label = well.Label
            };
            if (dataChart.GetDataPoints() > 0)
            {
                //Create & add Series
                foreach (string category in dataChart.Categories)
                {
                    Series s = new Series(dataChart.GetCategoryDescription(category));
                    s.ChartArea = chart.ChartAreas[0].Name;
                    s.XValueType = ChartValueType.DateTime;
                    s.ChartType = SeriesChartType.FastLine;
                    s.BorderWidth = 3;
                    s.YValueType = ChartValueType.Double;
                    s.ToolTip = s.Name;
                    s.Points.DataBind(dataChart.DataPoints[category], "Date", "Value", string.Empty);
                    SetAxisType(category, s, chart.ChartAreas[0]);
                    ChartHanlder.FormatEmptySeries(s);
                    chart.Series.Add(s);
                }
            }
        }
        else
        {
            foreach (Well well in wells)
            {
                dataChart = new PumpLogDataChart(categoryY1, categoryY2)
                {
                    StartDate = startDate.Value,
                    EndDate = endDate.Value,
                    ID = well.WellID,
                    Label = well.Label
                };
                if (dataChart.GetDataPoints() > 0)
                {
                    //Create & add Series
                    foreach (string category in dataChart.Categories)
                    {
                        Series s = new Series(String.Format("W{0} {1}", dataChart.WellNo, dataChart.GetCategoryDescription(category)));
                        s.ChartArea = chart.ChartAreas[0].Name;
                        s.XValueType = ChartValueType.DateTime;
                        s.ChartType = SeriesChartType.FastLine;
                        s.BorderWidth = 3;
                        s.YValueType = ChartValueType.Double;
                        s.ToolTip = s.Name;
                        s.Points.DataBind(dataChart.DataPoints[category], "Date", "Value", string.Empty);
                        SetAxisType(category, s, chart.ChartAreas[0]);
                        ChartHanlder.FormatEmptySeries(s);
                        chart.Series.Add(s);
                    }
                }

            }
        }

        #region Pager Row Logic
        //Get pager row.
        FormViewRow fvr = fv.FooterRow;
        if (fvr != null)
        {
            Label lblPageNo = fvr.Cells[0].FindControl("lblPageNumber") as Label;
            Label lblTotal = fvr.Cells[0].FindControl("lblTotal") as Label;
            LinkButton lbFirst = fvr.Cells[0].FindControl("lbFirst") as LinkButton;
            LinkButton lbPrev = fvr.Cells[0].FindControl("lbPrev") as LinkButton;
            LinkButton lbLast = fvr.Cells[0].FindControl("lbLast") as LinkButton;
            LinkButton lbNext = fvr.Cells[0].FindControl("lbNext") as LinkButton;
            if (lblPageNo != null && lblTotal != null)
            {
                lblPageNo.Text = String.Format("{0}", fv.PageIndex + 1);
                lblTotal.Text = String.Format("{0}", fv.PageCount);

                if (lbFirst != null && lbPrev != null && lbLast != null && lbNext != null)
                {
                    if (fv.PageIndex == 0 && fv.PageIndex == fv.PageCount - 1)
                    {
                        //get label first/previous and disable
                        lbFirst.Enabled = false;
                        lbFirst.ToolTip = String.Empty;
                        lbFirst.CssClass = "atfirstlast";

                        lbPrev.Enabled = false;
                        lbPrev.ToolTip = String.Empty;
                        lbPrev.CssClass = "atfirstlast";

                        //get label last/next and disable
                        lbLast.Enabled = false;
                        lbLast.ToolTip = String.Empty;
                        lbLast.CssClass = "atfirstlast";

                        lbNext.Enabled = false;
                        lbNext.ToolTip = String.Empty;
                        lbNext.CssClass = "atfirstlast";
                    }
                    else if (fv.PageIndex == 0)
                    {
                        //get label first/previous and disable
                        lbFirst.Enabled = false;
                        lbFirst.ToolTip = String.Empty;
                        lbFirst.CssClass = "atfirstlast";

                        lbPrev.Enabled = false;
                        lbPrev.ToolTip = String.Empty;
                        lbPrev.CssClass = "atfirstlast";
                    }
                    else if (fv.PageIndex == fv.PageCount - 1)
                    {
                        //get label last/next and disable
                        lbLast.Enabled = false;
                        lbLast.ToolTip = String.Empty;
                        lbLast.CssClass = "atfirstlast";

                        lbNext.Enabled = false;
                        lbNext.ToolTip = String.Empty;
                        lbNext.CssClass = "atfirstlast";
                    }
                }
            }
        }
        #endregion
    }

    private void setCategories()
    {        
        categoryY1 = SessionHandler.Read(ChartParameters.TidePredictionY1);
        if (String.IsNullOrEmpty(categoryY1))
            categoryY1 = SessionHandler.Read(ChartParameters.CategoryY1);

        categoryY2 = SessionHandler.Read(ChartParameters.TidePredictionY2);
        if (String.IsNullOrEmpty(categoryY2))
            categoryY2 = SessionHandler.Read(ChartParameters.CategoryY2);
    }

    private void SetAxisType(string category, Series s, ChartArea chartArea)
    {
        s.YAxisType = (AxisType)Session[category];
    }

    protected void Chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
    {
        Chart chart = sender as Chart;
        if (chart == null) return;

        /* For some reason this event fires twice for each Chart Legend.
         * If the Legend has already been fomratted from the previous time
         * round then we don't want to try and re-add the same columns twice
         * as that will throw an error. So this check is to make sure that the
         * Legend hasn't been formatted already. I have researched this extensively,
         * but still haven't found out why this happens...
         */
        if (chart.Legends[e.LegendName].CellColumns.Count > 0) return;

        //Add formatting to Legend
        ChartHanlder.FormatLegend(chart.Legends[e.LegendName]);
    }

    protected void Chart_Customize(object sender, EventArgs e)
    {
        Chart chart = sender as Chart;
        if (chart == null || dataChart == null) return;

        TitleCollection tc = chart.Titles;
        tc.Add(new Title()
        {
            Alignment = ContentAlignment.TopCenter,
            Text = String.Format("Well #{0}", graphType.Value == GraphType.PagedGraph ? dataChart.WellNo.ToString() : " (Multiple)"),
            TextOrientation = TextOrientation.Horizontal
        });

        //Create and format ChartArea
        ChartArea chartArea = chart.ChartAreas[0] as ChartArea;
        if (chartArea != null)
        {
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            chartArea.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.BorderDashStyle = ChartDashStyle.NotSet;
            chartArea.AxisX.Minimum = dataChart.StartDate.ToOADate();
            chartArea.AxisX.Maximum = dataChart.EndDate.ToOADate();
            chartArea.AxisY.Title = dataChart.GetCategoryDescription(categoryY1);
            chartArea.AxisY2.Title = dataChart.GetCategoryDescription(categoryY2);
        }
    }

    protected void chk_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox cb = sender as CheckBox;
        if (cb == null) return;

        CheckBox cbY1 = fvChart.FooterRow.Cells[0].FindControl("chkTideY1") as CheckBox;
        CheckBox cbY2 = fvChart.FooterRow.Cells[0].FindControl("chkTideY2") as CheckBox;

        if (cb.ID == cbY1.ID)
        {
            cbY1.Checked = cb.Checked;
            cbY2.Checked = false;

            //Add session values for Primary Axis
            Session.Add(ChartParameters.Tide, AxisType.Primary);
            SessionHandler.Write(ChartParameters.TidePredictionY1, ChartParameters.Tide);

            //Remove any existing session values for Secondary Axis
            SessionHandler.Remove(ChartParameters.TidePredictionY2);
        }
        else
        {
            cbY1.Checked = false;
            cbY2.Checked = cb.Checked;

            //Add session values for Secondary Axis
            Session.Add(ChartParameters.Tide, AxisType.Secondary);
            SessionHandler.Write(ChartParameters.TidePredictionY2, ChartParameters.Tide);

            //Remove any existing session values for Primary Axis
            SessionHandler.Remove(ChartParameters.TidePredictionY1);
        }

        //Remove tide session data if not selected
        if (!cbY1.Checked && !cbY2.Checked)
        {
            SessionHandler.Remove(ChartParameters.TidePredictionY1);
            SessionHandler.Remove(ChartParameters.TidePredictionY2);
        }

        Session.Add(cbY1.ID, cbY1.Checked);
        Session.Add(cbY2.ID, cbY2.Checked);
    }

    protected void chk_PreRender(object sender, EventArgs e)
    {
        CheckBox cb = sender as CheckBox;
        if (cb == null) return;

        bool? bTest = Session[cb.ID] as bool?;
        if (bTest.HasValue)
            cb.Checked = bTest.Value;
    }

    protected void ib_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Export":
                //Set Y1 & Y2 Axis for export
                setCategories();
                SessionHandler.Write(ChartParameters.ExportY1, categoryY1);
                SessionHandler.Write(ChartParameters.ExportY2, categoryY2);
                Response.Redirect(e.CommandArgument.ToString());
                break;
            default:
                break;
        }
    }

    protected void lbParameters_Click(object sender, EventArgs e)
    {
        mpeChartParameters.Show();
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (validate())
        {
            pnlErrors.Visible = false;
            try
            {
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, sHours, sMinutes, Seconds);
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, eHours, eMinutes, Seconds);

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

                //Get the graph type
                int gType;
                int.TryParse(rblGraphType.SelectedValue, out gType);
                graphType = (GraphType)gType;
                Session.Add(ChartParameters.GraphType, graphType);

                mpeChartParameters.Hide();
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
            mpeChartParameters.Show();
        }
    }

    protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox cb = sender as CheckBox;
        if (cb == null) return;

        foreach (ListItem li in chkWellID.Items)
            li.Selected = cb.Checked;

        mpeChartParameters.Show();
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

        if (!Common.TryParseNullableDate(cbStartDt.SelectedValue, out startDate))
            blErrors.Items.Add("You must enter a value for field 'Date' in section 'Start Date/Time'");

        //Parse the values for end date and time
        if (!String.IsNullOrEmpty(txtEHours.Text) && !int.TryParse(txtEHours.Text, out eHours) || eHours > 23)
            blErrors.Items.Add("Value entered for field 'Hours' in 'End Date/Time' section is incorrect format.");

        if (!String.IsNullOrEmpty(txtEMinutes.Text) && !int.TryParse(txtEMinutes.Text, out eMinutes) || eMinutes > 59)
            blErrors.Items.Add("Value entered for field 'Minutes' in 'End Date/Time' section is incorrect format.");

        if (!Common.TryParseNullableDate(cbEndDt.SelectedValue, out endDate))
            blErrors.Items.Add("You must enter a value for field 'Date' in section 'End Date/Time'");

        if (DateTime.Compare(startDate.Value, endDate.Value) > 0)
            blErrors.Items.Add("Start Date cannot come after End Date.");

        return blErrors.Items.Count <= 0;
    }

    protected void ibExport_AddTrigger(object sender, EventArgs e)
    {
        ImageButton ib = sender as ImageButton;
        if (ib == null) return;

        ToolkitScriptManager1.RegisterPostBackControl(ib);
    }

    protected void lbExport_AddTrigger(object sender, EventArgs e)
    {
        LinkButton lb = sender as LinkButton;
        if (lb == null) return;

        ToolkitScriptManager1.RegisterPostBackControl(lb);
    }

    private void clearChartParameters()
    {
        Session.Remove(ChartParameters.CategoryY1);
        Session.Remove(ChartParameters.CategoryY2);
        Session.Remove(ChartParameters.EndDate);
        Session.Remove(ChartParameters.ExportY1);
        Session.Remove(ChartParameters.ExportY2);
        Session.Remove(ChartParameters.StartDate);
        Session.Remove(ChartParameters.Tide);
        Session.Remove(ChartParameters.TidePredictionY1);
        Session.Remove(ChartParameters.TidePredictionY2);
        Session.Remove(ChartParameters.Wells);
    }
}