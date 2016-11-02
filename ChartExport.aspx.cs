using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

public partial class ChartExport : System.Web.UI.Page
{
    //Chart data objects
    PumpLogDataChart dataChart;
    DateTime? startDate, endDate;
    string categoryY1 = "", categoryY2 = "";
    List<Well> wells;
    GraphType? graphType;
    Chart chart;

    //PDF elements
    Document doc;
    PdfWriter pdfWriter;

    protected void  Page_Init(object sender, EventArgs e)
    {        
        //Get Chart parameters from session.
        wells = Session[ChartParameters.Wells] as List<Well>;
        startDate = Session[ChartParameters.StartDate] as DateTime?;
        endDate = Session[ChartParameters.EndDate] as DateTime?;
        categoryY1 = SessionHandler.Read(ChartParameters.ExportY1);
        categoryY2 = SessionHandler.Read(ChartParameters.ExportY2);
        graphType = Session[ChartParameters.GraphType] as GraphType?;

        //Specify the Response content type and Header.
        Response.ContentType = "application/pdf";
        Response.AddHeader("Content-Disposition"
                            , String.Format("attachment; filename={0}-{1}_{2:yyyy-MM-dd}.pdf"
                            , categoryY1
                            , categoryY2
                            , DateTime.Today));

        //Initialize PDF objects
        doc = new Document();
        pdfWriter = PdfWriter.GetInstance(doc, Response.OutputStream);
        //doc.SetPageSize(PageSize.A4.Rotate());
        doc.Open();

        if (graphType.Value == GraphType.PagedGraph)
        {
            //Generate Charts
            foreach (Well well in wells)
            { 
                /* 
                 * Create a chart, get the data items to generate the series, 
                 * set all properties, add to document. 
                 */
                dataChart = new PumpLogDataChart(categoryY1, categoryY2)
                {
                    StartDate = startDate.Value,
                    EndDate = endDate.Value,
                    ID = well.WellID,
                    Label = well.Label
                };

                if (dataChart.GetDataPoints() > 0)
                {
                    /*
                     * Create Chart and generate ChartArea and Series
                     * for all of the data items in the DataChart object
                     */
                    chart = new Chart();
                    chart.ChartAreas.Add(new ChartArea("ChartArea1"));

                    //Add event handlers
                    chart.CustomizeLegend += new EventHandler<CustomizeLegendEventArgs>(Chart_CustomizeLegend);
                    chart.Customize += new EventHandler(Chart_Customize);

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
                        SetAxisType(category, s);
                        ChartHanlder.FormatEmptySeries(s);
                        chart.Series.Add(s);
                    }

                    //Add Styling
                    chart.SkinID = "Chart";
                    chart.ApplyStyleSheetSkin(this.Page);

                    //Add to PDF document
                    addToPDF(chart);
                }
            }
        }
        else
        {
            /*
             * Create Chart and generate ChartArea and Series
             * for all of the data items in the DataChart object
             */
            chart = new Chart();
            chart.ChartAreas.Add(new ChartArea("ChartArea1"));

            //Add event handlers
            chart.CustomizeLegend += new EventHandler<CustomizeLegendEventArgs>(Chart_CustomizeLegend);
            chart.Customize += new EventHandler(Chart_Customize);
            
            //Generate Charts
            foreach (Well well in wells)
            {
                /* 
                    * get the data items to generate the series, 
                    * set all properties, add to document. 
                    */
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
                        SetAxisType(category, s);
                        ChartHanlder.FormatEmptySeries(s);
                        chart.Series.Add(s);
                    }
                }                
            }

            //Add styling
            chart.SkinID = "Chart";
            chart.ApplyStyleSheetSkin(this.Page);

            //Add to PDF document
            addToPDF(chart);
        }
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        //Clean up
        if (doc != null && doc.IsOpen())
        {
            doc.Close();
            doc.Dispose();
        }
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

        //Add saved title to chart
        chart.Titles.Add(new Title()
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

    private void addToPDF(Chart chart)
    {
        if (doc != null && doc.IsOpen())
        {
            using (MemoryStream imgStream = new MemoryStream())
            {
                //Save chart image to memory stream (fires Chart.Customise event).
                chart.SaveImage(imgStream, ChartImageFormat.Jpeg);

                //Create an Image object from the MemoryStream data
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgStream.ToArray());

                //Scale the Image object to fit within the boundary of the PDF document and add it.
                img.ScaleToFit(doc.PageSize.Width - (doc.LeftMargin + doc.RightMargin),
                                doc.PageSize.Height - (doc.TopMargin + doc.BottomMargin));
                doc.Add(img);
                doc.NewPage();
            }
        }
    }

    private void SetAxisType(string category, Series s)
    {
        s.YAxisType = (AxisType)Session[category];
    }
}