using System;
using System.Web;
using System.Data;
using System.IO;
using System.Data.OleDb;

/// <summary>
/// Worksheet handles the OLE data connection and imports the spreadsheet into memory.
/// </summary>
[Serializable]
public class Worksheet
{
    protected HttpPostedFile _file;
    protected string _tableName = "Sheet1$";
    protected bool _hasColumnNamesInFirstRow = false;
    protected string _localFilePath;

    public DataTable Schema = null;

    /// <summary>
    /// Gets the name of the Excel tab from which the data is to be pulled.
    /// </summary>
    public string TabName
    {
        get
        {
            return this._tableName;
        }
        set
        {
            this._tableName = value;
        }
    }

    /// <summary>
    /// Gets the pathname to the Excel spread sheet used for mapping
    /// </summary>
    public string LocalFilePath
    {
        get
        {
            return this._localFilePath;
        }
        set
        {
            this._localFilePath = value;
        }
    }

    public bool HasColumnNamesInFirstRow
    {
        get
        {
            return _hasColumnNamesInFirstRow;
        }
        set
        {
            _hasColumnNamesInFirstRow = value;
        }
    }

    public Worksheet(HttpPostedFile file)
    {
        _file = file;
        _localFilePath = SaveWorksheet(_file);
    }

    public Worksheet(HttpPostedFile file, bool hasColumnNamesInFirstRow)
    {
        _file = file;
        _hasColumnNamesInFirstRow = hasColumnNamesInFirstRow;
        _localFilePath = SaveWorksheet(_file);
    }

    public Worksheet() { }

    public void GetSchema()
    {
        string ConnectionString = String.Format("provider=Microsoft.ACE.OLEDB.12.0; data source='{0}'; Extended Properties=\"Excel 12.0;HDR=No;IMEX=1;\"", _localFilePath);

        using (OleDbConnection cn = new OleDbConnection(ConnectionString))
        {
            cn.Open();
            this.Schema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            cn.Close();
        }
    }

    public void GetExcelWorksheets()
    {
        //Do While Not objRS.EOF 
        //    strTable = objRS.Fields("table_name").Value
        //    objRS.MoveNext
        //Loop
    }

    /// <summary>
    /// Class to hold the DataTable return value for GetDataTable(). The return value can be 
    /// a DataTable, an Exception, or possibly both.
    /// </summary>
    public class TableData
    {
        public DataTable dt;
        public Exception ex;

        public TableData(DataTable dt)
        {
            this.dt = dt;
            this.ex = null;
        }

        public TableData(Exception ex)
        {
            this.dt = null;
            this.ex = ex;
        }
    }

    /// <summary>
    /// Uses Jet/ACE to connect directly to the file and select the rows into memory as a DataTable.
    /// Once the table is created, loop through the rows and eliminate any that are completely blank.
    /// Finally, check to see if the user has checked "Has Column Names in First Row" -- if true,
    /// move the data in Row 1 to the column headers.
    /// 
    /// 2009-06-29 MM
    /// Found something very unusual that I haven't found documented anywhere else. During debugging,
    /// I opened the spreadsheet being uploaded, kept it open in Excel during this function, and noticed 
    /// that the date fields were being converted to numeric. It took me a few tries to figure out
    /// why but when I closed the file they came back in as regular dates. Apparently either ACE or Excel
    /// is triggering a different algorithm when working with an open file. This happens reliably with 
    /// both XLS and XLSX files. Will add a message to the help text asking the user to close the file
    /// before uploading -- not sure what else can be done.
    /// </summary>
    /// <returns></returns>
    public virtual TableData GetDataTable()
    {
        //2008-06-13 MM
        //Neither the JET nor the ACE library are standard in the .NET framework. ACE is required for Excel 2007.
        //If the DLLs are not installed on the server you will get this exception when trying to connect:
        //"The 'Microsoft.ACE.OLEDB.12.0' provider is not registered on the local machine."
        //http://www.microsoft.com/downloads/details.aspx?FamilyID=7554F536-8C28-4598-9B72-EF94E038C891&displaylang=en
        //is a link to download a file called AccessDatabaseEngine.exe with the ACE 12.0 DLLs.  It must be installed
        //locally on the web server.

        //Notes about the ConnectionString
        //	- HDR=No/Yes is the header row tag. If the file will have the column names in the header, set this to yes.
        //	- The Jet connection string's data source goes to the directory. The ACE goes to the file itself.
        string fileName = Path.GetFileName(_localFilePath);
        string extension = Path.GetExtension(fileName);
        string filePath = Path.GetDirectoryName(_localFilePath) + "\\";

        string ConnectionString = string.Empty;
        if (extension.ToLower() == ".csv" || extension.ToLower() == ".txt")
            ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source='{0}'; Extended Properties='text; HDR=Yes;FMT=Delimited'", filePath);
        else
            ConnectionString = String.Format("provider=Microsoft.ACE.OLEDB.12.0; data source='{0}'; Extended Properties=\"Excel 12.0;HDR=No;IMEX=1;\"", _localFilePath);


        DataTable dtOrig = null; //original data rows as imported
        DataTable dt = null; //data rows minus all blank rows
        using (OleDbConnection cn = new OleDbConnection(ConnectionString))
        {
            try
            {
                cn.Open();
            }
            catch (OleDbException ex)
            {
                return new TableData(ex);
            }

            string firstSheetName = string.Empty;
            if (extension != ".csv")
            {
                DataTable dbSchema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dbSchema == null || dbSchema.Rows.Count < 1)
                {
                    throw new Exception("Error: Could not determine the name of the first worksheet.");
                }
                firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
            }
            else
                firstSheetName = fileName;

            string SQL = String.Format(@"SELECT * FROM [{0}]", firstSheetName);

            using (OleDbCommand cmd = new OleDbCommand(SQL, cn))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //create and fill the main table (dt) and a copy (dtOrig)
                dtOrig = new DataTable();
                dt = new DataTable();

                try
                {
                    da.Fill(dtOrig);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    //Log.WriteUploadLog(SessionHandler.Read("UserID").ToString(), this._file.FileName, ex.Message, "Error");
                    return new TableData(ex);
                }

                //2009-06-16 MM
                //Loop through all the data to find rows that are completely blank. If a blank row is found
                //in dtOrig, remove the corresponding row index from dt. Creating a duplicate of the table
                //and looping through each cell adds substantial processing overhead here but it prevents
                //needing to load all the rows into session. There is probably a faster and more elegant
                //solution but I'd rather not having possibly hundreds of extra blank rows sitting around
                //in the session.
                for (int idx = dtOrig.Rows.Count - 1; idx >= 0; idx--)
                {
                    DataRow dr = dtOrig.Rows[idx];
                    bool HasData = false;
                    for (int c = 0; c < dtOrig.Columns.Count; c++)
                    {
                        string Value = Convert.ToString(dr[c]);
                        if (Value != "") //at least one cell has data
                        {
                            System.Diagnostics.Debug.Print(Value);
                            HasData = true;
                            break;
                        }
                    }

                    if (HasData == false) //remove the row from dt
                    {
                        dt.Rows.RemoveAt(idx);
                    }
                }

                //now if this is the audit spreadsheet, we want to also remove the 
                //	first row and make the second the first 
                //	(the second row actually contains the headers)
                if (this._tableName == "Data-ALL$")
                {
                    dt.Rows.RemoveAt(0);
                }

                if (_hasColumnNamesInFirstRow)
                {
                    //convert the 1st row to the header
                    DataRow dr = dt.Rows[0];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn dc = dt.Columns[i];
                        string ColumnName = Convert.ToString(dr[i]);
                        if (ColumnName != "") //preserve the default name if no other name is available
                        {
                            //ensure the new column name is unique
                            int idx = 1;
                            while (dt.Columns.Contains(ColumnName))
                            {
                                //generate a new, unique column name
                                ColumnName = String.Format("{0}{1}", Convert.ToString(dr[i]), idx);
                                idx++;
                            }
                            dc.ColumnName = ColumnName; //assign the new name
                        }
                    }
                    dt.Rows.RemoveAt(0);
                }
            }

            cn.Close();
        }

        return new TableData(dt);
    }

    /// <summary>
    /// Saves a file from its current path to the configuration uploadPath location. Prepends the date to the 
    /// filename to keep them sorted. If the file already exists, appends _1, _2, _3, etc. to the file until
    /// it finds a unique name that doesn't exist.
    /// </summary>
    /// <param name="file">an uploaded file</param>
    /// <returns>the full server mapped path of the newly saved file</returns>
    protected string SaveWorksheet(HttpPostedFile file)
    {
        string FileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        string Extension = Path.GetExtension(file.FileName);
        string FolderPath = System.Web.Configuration.WebConfigurationManager.AppSettings["UploadPath"].ToString();

        //try
        //{
        FileNameWithoutExtension = String.Format("{0}_{1}", DateTime.Today.ToString("yyyy-MM-dd"), FileNameWithoutExtension);
        string FileName = String.Format("{0}{1}", FileNameWithoutExtension, Extension);
        string FilePath = Path.Combine(FolderPath, FileName);
        int idx = 0;
        while (File.Exists(FilePath))
        {
            string NewFileNameWithoutExtension = String.Format("{0}_{1}", FileNameWithoutExtension, ++idx);
            FileName = String.Format("{0}{1}", NewFileNameWithoutExtension, Extension);
            FilePath = Path.Combine(FolderPath, FileName);
        }
        file.SaveAs(FilePath);
        return FilePath;
        //}
        //catch (Exception ex)
        //{
        //    string s = string.Format("Cannot save the file {0} due to: {1}", FileName, ex.Message);
        //    Log.WriteUploadLog(string.Empty, FileName, s, "Error");
        //    return null;
        //}

    }
}


///<summary>
///This class extends Worksheet to add just the needed functionality
///for processing the Vancouver Log data
///</summary>
[Serializable]
public class VancouverLogWorksheet : Worksheet
{
    public VancouverLogWorksheet(HttpPostedFile postedFile, bool hasHeaderColumns)
        : base(postedFile, hasHeaderColumns)
    {
        dt = getDataTableSchema();
    }

    public VancouverLogWorksheet(HttpPostedFile postedFile)
        : base(postedFile)
    {
        dt = getDataTableSchema();
    }

    public VancouverLogWorksheet() { dt = getDataTableSchema(); }
    public override TableData GetDataTable()
    {
        string fileName = Path.GetFileName(_localFilePath);
        string extension = Path.GetExtension(fileName);
        string filePath = Path.GetDirectoryName(_localFilePath) + "\\";

        string ConnectionString = string.Empty;
        if (extension.ToLower() == ".csv" || extension.ToLower() == ".txt")
            ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source='{0}'; Extended Properties='text; HDR=Yes;FMT=Delimited'", filePath);
        else
            ConnectionString = String.Format("provider=Microsoft.ACE.OLEDB.12.0; data source='{0}'; Extended Properties=\"Excel 12.0;HDR=No;IMEX=1;\"", _localFilePath);


        DataTable dtOrig = null; //original data rows as imported
        using (OleDbConnection cn = new OleDbConnection(ConnectionString))
        {
            try
            {
                cn.Open();
            }
            catch (OleDbException ex)
            {
                return new TableData(ex);
            }

            string firstSheetName = string.Empty;
            if (extension.ToLower() != ".csv" && extension.ToLower() != ".txt")
            {
                DataTable dbSchema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dbSchema == null || dbSchema.Rows.Count < 1)
                {
                    throw new Exception("Error: Could not determine the name of the first worksheet.");
                }
                firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
            }
            else
                firstSheetName = fileName;

            string SQL = String.Format(@"SELECT * FROM [{0}]", firstSheetName);

            using (OleDbCommand cmd = new OleDbCommand(SQL, cn))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //fill temp table (dtOrig) with uploaded data for processing
                dtOrig = new DataTable();
                try
                {
                    da.Fill(dtOrig);
                }
                catch (Exception ex)
                {
                    //Log.WriteUploadLog(SessionHandler.Read("UserID").ToString(), this._file.FileName, ex.Message, "Error");
                    return new TableData(ex);
                }

                //Loop through all the data passing each row for processing and then insertion into main table. This process includes 
                //finding rows that are completely blank. If a blank row is found in dtOrig, skip it and move on to the next one.
                for (int idx = dtOrig.Rows.Count - 1; idx >= 0; idx--)
                {
                    DataRow dr = dtOrig.Rows[idx];
                    bool HasData = false;
                    for (int c = 0; c < dtOrig.Columns.Count; c++)
                    {
                        string Value = Convert.ToString(dr[c]);
                        if (Value != "") //at least one cell has data
                        {
                            HasData = true;
                            break;
                        }
                    }

                    if (HasData)
                    {
                        //pass data row to method for processing
                        parseDataRow(dr);
                    }
                }

                if (_hasColumnNamesInFirstRow)
                {
                    //convert the 1st row to the header
                    DataRow dr = dt.Rows[0];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn dc = dt.Columns[i];
                        string ColumnName = Convert.ToString(dr[i]);
                        if (ColumnName != "") //preserve the default name if no other name is available
                        {
                            //ensure the new column name is unique
                            int idx = 1;
                            while (dt.Columns.Contains(ColumnName))
                            {
                                //generate a new, unique column name
                                ColumnName = String.Format("{0}{1}", Convert.ToString(dr[i]), idx);
                                idx++;
                            }
                            dc.ColumnName = ColumnName; //assign the new name
                        }
                    }
                    dt.Rows.RemoveAt(0);
                }
            }

            cn.Close();
        }

        return new TableData(dt);
    }

    public static DataTable GetDataTableSchema()
    {
        VancouverLogWorksheet vw = new VancouverLogWorksheet();
        return vw.dt;
    }
    protected virtual DataTable getDataTableSchema()
    {
        DataTable table = new DataTable();
        DataColumn column;

        column = new DataColumn("WellID", typeof(int));
        table.Columns.Add(column);

        column = new DataColumn("WellLabel", typeof(string));
        table.Columns.Add(column);

        column = new DataColumn("Category", typeof(string));
        table.Columns.Add(column);

        column = new DataColumn("LogDate", typeof(DateTime));
        table.Columns.Add(column);

        column = new DataColumn("LogValue", typeof(Int64));
        table.Columns.Add(column);

        column = new DataColumn("Units", typeof(string));
        table.Columns.Add(column);

        column = new DataColumn("ModifiedDate", typeof(DateTime));
        table.Columns.Add(column);

        column = new DataColumn("Source", typeof(string));
        table.Columns.Add(column);

        column = new DataColumn("UploadedBy", typeof(string));
        table.Columns.Add(column);

        column = new DataColumn("UploadID", typeof(int));
        table.Columns.Add(column);

        return table;
    }

    protected virtual void parseDataRow(DataRow dr)
    {
        int WellID;
        DateTime LogDate;
        long LogValue;

        string[] elements;
        DataRow dataRow = dt.NewRow();

        //For parsing the column values
        string value = "";

        //first elelment contains the WellID, WellLabel and Category
        value = Convert.ToString(dr[0]);
        if (value.Length > 0)
        {
            elements = value.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (int.TryParse(elements[0].Replace("[", ""), out WellID))
                dataRow["WellID"] = WellID;

            dataRow["WellLabel"] = elements[1].Replace("]", "");

            dataRow["Category"] = elements[2];

            value = string.Empty;
        }

        //next column is the LogDate
        value = Convert.ToString(dr[1]);
        if (value.Length > 0)
        {
            if (DateTime.TryParse(value, out LogDate))
            {
                dataRow["LogDate"] = LogDate;
                value = string.Empty;
            }
        }

        //next column is LogValue
        value = Convert.ToString(dr[2]);
        if (value.Length > 0 && !value.Equals("65535"))
        {
            if (long.TryParse(value, out LogValue))
            {
                dataRow["LogValue"] = LogValue;
                value = string.Empty;
            }
        }

        //next column is Units
        value = Convert.ToString(dr[3]);
        if (value.Length > 0)
        {
            dataRow["Units"] = value;
            value = string.Empty;
        }

        //Add row to main table
        dt.Rows.Add(dataRow);
    }

    //The main table to hold the processed Vancouver data
    protected DataTable dt;
}

public class VancouverTideWorksheet : VancouverLogWorksheet
{
    public VancouverTideWorksheet(HttpPostedFile postedFile)
        : base(postedFile)
    {
        //base class will handle initialization
        //necessary methods have been overridden in this class
    }

    public VancouverTideWorksheet()
        : base()
    {
        //base class will handle initialization
        //necessary methods have been overridden in this class
    }



    protected override DataTable getDataTableSchema()
    {
        DataTable t = new DataTable();

        t.Columns.Add(new DataColumn("DateTime", typeof(string)));
        t.Columns.Add(new DataColumn("Height", typeof(float)));
        t.Columns.Add(new DataColumn("ModifiedDate", typeof(DateTime)));
        t.Columns.Add(new DataColumn("Source", typeof(string)));
        t.Columns.Add(new DataColumn("UploadedBy", typeof(string)));
        t.Columns.Add(new DataColumn("UploadID", typeof(int)));

        return t;
    }

    protected override void parseDataRow(DataRow dr)
    {
        int idxDate = 0, idxTime = 1, idxHeight = 2;

        string row;
        DataRow dataRow = dt.NewRow();
        string[] elements;

        row = Convert.ToString(dr[0]);

        //rows with all text start with # symbol
        if (!String.IsNullOrEmpty(row) && !row.Contains("#"))
        {
            elements = row.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (elements == null || elements.Length == 0)
                return;

            DateTime dateTime;
            if (DateTime.TryParse(String.Format("{0} {1}", elements[idxDate], elements[idxTime]), out dateTime))
                dataRow["DateTime"] = dateTime;
            else
                throw (new Exception("Error parsing date and time values."));

            float height;
            if (float.TryParse(elements[idxHeight], out height))
                dataRow["Height"] = height;
            else
                throw (new Exception("Error parsing height value."));

            dt.Rows.Add(dataRow);
        }
    }
}