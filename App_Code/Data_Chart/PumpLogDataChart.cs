using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Data_Access;

/// <summary>
/// Builds a DataChart object for Vancouver Wharves Pump Logger data.
/// </summary>
public class PumpLogDataChart : DataChart
{
    /// <summary>
    /// Class Constructor
    /// </summary>
	public PumpLogDataChart() : base()
	{

	}

    /// <summary>
    /// Class Constructor
    /// </summary>
    /// <param name="category1">Data name for Primary Y-Axis</param>
    /// <param name="category2">Data name for Secondary Y-Axis</param>
    public PumpLogDataChart(string category1, string category2)
        : base(category1, category2)
    {

    }

    /// <summary>
    /// Returns the ListValue field of the ListManager table where the Category
    /// is 'Measurement' and the ListKey is equal to the parameter key.
    /// </summary>
    /// <param name="key">Mathces the ListKey field of the ListManager table.</param>
    /// <returns></returns>
    public string GetCategoryDescription(string key)
    {
        PumpLogDataContext dtx = new PumpLogDataContext();
        var query = (from c in dtx.MeasurementCategories
                     where c.ListKey == key
                     select c.ListValue).FirstOrDefault();
        return query;
    }

    /// <summary>
    /// Fills the DataPoints member field with the data necessary
    /// to generate a Chart.
    /// </summary>
    /// <returns>DataItems.Count</returns>
    public int GetDataPoints()
    {        
        //get dataItems for each category
        foreach (string category in this.Categories)        
            getDataPoints(category);        

        return this.DataPoints.Count;
    }

    int offset = -31;
    public int WellNo
    {
        get { return this.ID + offset; }
    }

    /// <summary>
    /// Private member function that retreives data for the
    /// provided category.
    /// </summary>
    /// <param name="category"></param>
    private void getDataPoints(string category)
    {
       /* 
        * setup linq query using category, 
        * start/end date properties
        */

        PumpLogDataContext dtx = new PumpLogDataContext();

        //Pump Log data
        var qryPumpLog = from p in dtx.PumpLogDataItems
                         where p.Category == category
                         && p.LogDate >= this.StartDate
                         && p.LogDate <= this.EndDate
                         && p.WellID == this.ID
                         orderby p.LogDate ascending
                         select new
                         {
                             p.LogDate,
                             p.LogValue
                         };

        //Tidal Prediction data
        var qryTidalPrediction = from p in dtx.TidalPredictionItems
                                 where p.Date_Time >= this.StartDate
                                 && p.Date_Time <= this.EndDate
                                 orderby p.Date_Time ascending
                                 select new
                                 {
                                     p.Date_Time,
                                     p.Height
                                 };

        //Select data items and create a list of DataPoints             
        List<DataPoint> dataPoints = new List<DataPoint>();
        switch (category)
        {
            case "tide":
                foreach (var dataItem in qryTidalPrediction)
                {
                    dataPoints.Add(new DataPoint()
                    {
                        Date = dataItem.Date_Time.Value,
                        Value = dataItem.Height.HasValue ? (double)dataItem.Height.Value : double.NaN
                    });
                }
                break;
            default:
                foreach (var dataItem in qryPumpLog)
                {
                    dataPoints.Add(new DataPoint()
                    {
                        Date = dataItem.LogDate,
                        Value = dataItem.LogValue.HasValue ? (double)dataItem.LogValue : double.NaN
                    });
                }
                break;
        }

        //Add DataItems for Category
        this.DataPoints.Add(category, dataPoints);
    }
}