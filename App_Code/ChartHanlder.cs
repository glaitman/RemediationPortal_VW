using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;

/// <summary>
/// Summary description for ChartHanlder
/// </summary>
public static class ChartHanlder
{
    /// <summary>
    /// Applies custom formatting for a chart series object
    /// which may contain empty data points.
    /// </summary>
    /// <param name="s">Chart.Series object</param>
    public static void FormatEmptySeries(Series s)
    {
        s.EmptyPointStyle.Color = Color.Transparent;
        s.EmptyPointStyle.BorderWidth = 0;
        s.EmptyPointStyle.BorderDashStyle = ChartDashStyle.NotSet;
        s.EmptyPointStyle.MarkerSize = 7;
        s.EmptyPointStyle.MarkerStyle = MarkerStyle.None;
        s.EmptyPointStyle.MarkerBorderColor = Color.Black;
        s.EmptyPointStyle.MarkerColor = Color.LightGray;
    }

    /// <summary>
    /// Applies custom formatting for a Chart Legend object
    /// </summary>
    /// <param name="l">Chart.Legend object</param>
    public static void FormatLegend(Legend l)
    {
        //Add first cell column
        l.CellColumns.Add(new LegendCellColumn()
        {
            ColumnType = LegendCellColumnType.SeriesSymbol,
            HeaderText = "Color",
            HeaderBackColor = Color.WhiteSmoke
        });

        //Add second cell column
        l.CellColumns.Add(new LegendCellColumn()
        {
            ColumnType = LegendCellColumnType.Text,
            HeaderText = "Category",
            Text = "#SERIESNAME",
            HeaderBackColor = Color.WhiteSmoke
        });

        //Add header separator of type line
        l.HeaderSeparator = LegendSeparatorStyle.Line;
        l.HeaderSeparatorColor = Color.FromArgb(64, 64, 64, 64);

        //Add item column separator of type line
        l.ItemColumnSeparator = LegendSeparatorStyle.Line;
        l.ItemColumnSeparatorColor = Color.FromArgb(64, 64, 64, 64);

        //Set Avg cell column attributes
        l.CellColumns.Add(new LegendCellColumn()
        {
            Text = "#AVG{N1}",
            HeaderText = "Average",
            Name = "AvgColumn",
            HeaderBackColor = Color.WhiteSmoke,
            ColumnType = LegendCellColumnType.Text
        });

    }
}

public enum GraphType
{
    PagedGraph,
    SingleGraph
};