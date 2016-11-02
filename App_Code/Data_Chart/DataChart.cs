using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DataChart is a base class used to hold the data that will be used
/// to generate a Chart. Any derived class will have the logic specific
/// to the type of data that will be used.
/// </summary>
public class DataChart
{
    /// <summary>
    /// Class Constructor
    /// </summary>
	public DataChart()
	{
        dataPoints = new Dictionary<string, List<DataPoint>>();
        categories = new List<string>();
	}

    /// <summary>
    /// Class Constructor
    /// </summary>
    /// <param name="category1">Data name for Primary Y-Axis</param>
    /// <param name="category2">Data name for Secondary Y-Axis</param>
    public DataChart(string category1, string category2)
    {
        dataPoints = new Dictionary<string, List<DataPoint>>();
        categories = new List<string>();

        if (!String.IsNullOrEmpty(category1))
            categories.Add(category1);

        if (!String.IsNullOrEmpty(category2))
            categories.Add(category2);
    }

    #region properties
    int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    string label;
    public string Label
    {
        get { return label; }
        set { label = value; }
    }

    DateTime startDate;
    public DateTime StartDate
    {
        get { return startDate; }
        set { startDate = value; }
    }

    DateTime endDate;
    public DateTime EndDate
    {
        get { return endDate; }
        set { endDate = value; }
    }

    List<string> categories;
    public List<string> Categories
    {
        get { return categories; }
    }

    Dictionary<string, List<DataPoint>> dataPoints;
    public Dictionary<string, List<DataPoint>> DataPoints
    {
        get { return dataPoints; }
    }
    #endregion

    /// <summary>
    /// Data Point object.
    /// </summary>
    public class DataPoint
    {
        public DataPoint()
        {

        }

        public DataPoint(DateTime date, int value)
        {
            this.Date = date;
            this.Value = value;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public double Value
        {
            get;
            set;
        }
    }
}


[Serializable]
public struct Well
{
    public int WellID
    {
        get;
        set;
    }

    public string Label
    {
        get;
        set;
    }
}