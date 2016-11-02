using System;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Caching;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DataLayer
{
	/// <summary>
	/// DataMap is the base data object for importing data from the portal into IDEA.
	/// It contains the schema, the DataTable imported from the spreadsheet, a mapping
	/// of columns to destination fields, and an attached file to load into IDEA's 
	/// file library.
	/// </summary>
	[Serializable]
	public class DataMap
	{
		protected string _ID;
		protected string _DisplayName;
		protected bool _DisableSupportingDocuments;
		protected string _UploadID;
		//protected string _WorksheetName;
		//protected string _Category;
		protected string Query;
        protected string CommandType;
		//protected string _FileQuery;
		protected DataTable _UploadData;
		protected Hashtable _Mapping;
		public List<Field> Fields = null;
		//public Dictionary<string, Field> Fields2 = null;
		protected string _Errors;
		public Document AttachedFile = null;
		public bool ColumnNamesIn1stRow;
		public DataFiles DataFile;
		public DataCounts DataCount;

		public string ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}
		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
			set
			{
				_DisplayName = value;
			}
		}
		public bool DisableSupportingDocuments
		{
			get
			{
				return _DisableSupportingDocuments;
			}
			set
			{
				_DisableSupportingDocuments = value;
			}
		}
		public string UploadID
		{
			get
			{
				return _UploadID;
			}
		}
		//public string WorksheetName
		//{
		//    get
		//    {
		//        return _WorksheetName;
		//    }
		//    set
		//    {
		//        _WorksheetName = value;
		//    }
		//}
		//public string Category
		//{
		//    get
		//    {
		//        return _Category;
		//    }
		//    set
		//    {
		//        _Category = value;
		//    }
		//}
		public DataTable UploadData
		{
			get
			{
				return _UploadData;
			}
			set
			{
				_UploadData = value;
			}
		}
		public Hashtable Mapping
		{
			get
			{
				return _Mapping;
			}
			set
			{
				_Mapping = value;
			}
		}
		public string Errors
		{
			get
			{
				return _Errors;
			}
			set
			{
				_Errors = value;
			}
		}

		/// <summary>
		/// Used to return the number of data rows and the number of files inserted.
		/// </summary>
		[Serializable]
		public class DataCounts
		{
			public int DataRowsInserted;
			public int FilesInserted;

			public DataCounts(int DataRowsInserted, int FilesInserted)
			{
				this.DataRowsInserted = DataRowsInserted;
				this.FilesInserted = FilesInserted;
			}
		}

		[Serializable]
		public class DataFiles
		{
			protected string _ID;
			protected string _DisplayName;
			protected bool _DisableSupportingDocuments;
			protected string _InsertQuery;
			protected string _SelectInventoryQuery;
			protected string _SelectFileQuery;
			protected string _UpdateFileQuery;
			protected string _RemoveFileAttachment;
			protected Hashtable _Mapping;
			public List<Field> Fields = null;
			protected string _Errors;

			public string ID
			{
				get
				{
					return _ID;
				}
				set
				{
					_ID = value;
				}
			}
			public string DisplayName
			{
				get
				{
					return _DisplayName;
				}
				set
				{
					_DisplayName = value;
				}
			}
			public bool DisableSupportingDocuments
			{
				get
				{
					return _DisableSupportingDocuments;
				}
				set
				{
					_DisableSupportingDocuments = value;
				}
			}
			public Hashtable Mapping
			{
				get
				{
					return _Mapping;
				}
				set
				{
					_Mapping = value;
				}
			}
			public string Errors
			{
				get
				{
					return _Errors;
				}
				set
				{
					_Errors = value;
				}
			}
			public string InsertQuery
			{
				get
				{
					return _InsertQuery;
				}
				set
				{
					_InsertQuery = value;
				}
			}
			public string SelectInventoryQuery
			{
				get
				{
					return _SelectInventoryQuery;
				}
				set
				{
					_SelectInventoryQuery = value;
				}
			}
			public string SelectFileQuery
			{
				get
				{
					return _SelectFileQuery;
				}
				set
				{
					_SelectFileQuery = value;
				}
			}
			public string UpdateFileQuery
			{
				get
				{
					return _UpdateFileQuery;
				}
				set
				{
					_UpdateFileQuery = value;
				}
			}
			public string RemoveFileAttachment
			{
				get
				{
					return _RemoveFileAttachment;
				}
				set
				{
					_RemoveFileAttachment = value;
				}
			}

			public DataFiles()
			{
				this._Mapping = new Hashtable(40);
				this.Fields = new List<Field>(50);
			}

			public static DataFiles Create(XmlNode root, string ID)
			{
				if (root == null) return null;

				//locate correct report
				XmlNode nodeID = root.SelectSingleNode("Table[@ID='" + ID + "']");
				XmlNode nodeSQL = root.SelectSingleNode("Table[@ID='" + ID + "']/SQL");
				if (nodeSQL == null || nodeID == null) return null;

				XmlAttribute _ID = nodeID.Attributes["ID"];
				XmlAttribute _DisplayName = nodeID.Attributes["DisplayName"];
				XmlAttribute _DisableSupportingDocuments = nodeID.Attributes["DisableSupportingDocuments"];
				XmlAttribute _InsertQuery = nodeSQL.Attributes["InsertQuery"];
				XmlAttribute _SelectInventoryQuery = nodeSQL.Attributes["SelectInventoryQuery"];
				XmlAttribute _SelectFileQuery = nodeSQL.Attributes["SelectFileQuery"];
				XmlAttribute _UpdateFileQuery = nodeSQL.Attributes["UpdateFileQuery"];
				XmlAttribute _RemoveFileAttachment = nodeSQL.Attributes["RemoveFileAttachment"];

				//check for required members
				if (_ID == null) throw new XmlException("Table must include the attribute 'ID'.");
				if (_DisplayName == null) throw new XmlException("Table must include the attribute 'Display Name'.");
				if (_InsertQuery == null) throw new XmlException("Table must include the attribute 'SQL'.");

				DataFiles df = new DataFiles();
				df.ID = _ID.Value;
				df.DisplayName = _DisplayName.Value;
				df.InsertQuery = _InsertQuery.Value;
				if (_SelectInventoryQuery != null)
					df.SelectInventoryQuery = _SelectInventoryQuery.Value;
				if (_DisableSupportingDocuments != null)
					df.DisableSupportingDocuments = bool.Parse(_DisableSupportingDocuments.Value);
				if (_SelectFileQuery != null)
					df.SelectFileQuery = _SelectFileQuery.Value;
				if (_UpdateFileQuery != null)
					df.UpdateFileQuery = _UpdateFileQuery.Value;
				if (_RemoveFileAttachment != null)
					df.RemoveFileAttachment = _RemoveFileAttachment.Value;

				//create fields
				XmlNodeList lstFields = root.SelectNodes("Table[@ID='" + ID + "']/Field");
				for (int i = 0; i < lstFields.Count; i++)
				{
					Field f = Field.Create(lstFields[i]);
					f.FieldID = i.ToString();
					if (f != null)
					{
						df.Fields.Add(f);
					}
					//if (f != null) dm.Fields2[f.Destination] = f;
				}

				return df;

			}
		}

		public DataMap()
		{
			this._UploadData = null;
			//Set the unique upload id to be unique based on time of now. This single value will be applied to all inserts that occur inthis session
			this._UploadID = ((new TimeSpan((DateTime.Now.Ticks - (new DateTime(2010, 1, 1)).Ticks)).Ticks) / 1000000).ToString();
			this._Mapping = new Hashtable(40);
			this.Fields = new List<Field>(50);
			//this.Fields2 = new Dictionary<string, Field>(50);
			this.ColumnNamesIn1stRow = false;
		}

		/// <summary>
		/// Instantiator. Parses the XML schema file, creates the DataLayer object, sets its properties, 
		/// and adds the list of associated fields. Throws an XmlException if any required
		/// attributes are not present in the schema.
		/// </summary>
		/// <returns>The DataLayer object populated from the schema node.</returns>
		/// <param name="root">The root node of the schema XML file containing the destination table structure.</param>
		/// <param name="ID">The ID of the Table element in the schema to load into the DataLayer.</param>
		public static DataMap Create(XmlNode DataRoot, XmlNode FileRoot, string ID)
		{
			if (DataRoot == null) return null;

			//locate correct report
			XmlNode nodeID = DataRoot.SelectSingleNode("Table[@ID='" + ID + "']");
			XmlNode nodeSQL = DataRoot.SelectSingleNode("Table[@ID='" + ID + "']/SQL");
			if (nodeSQL == null || nodeID == null) return null;

			XmlAttribute _ID = nodeID.Attributes["ID"];
			XmlAttribute _DisplayName = nodeID.Attributes["DisplayName"];
			XmlAttribute _DisableSupportingDocuments = nodeID.Attributes["DisableSupportingDocuments"];
			//XmlAttribute _DefaultWorksheetName = nodeID.Attributes["DefaultWorksheetName"];
			XmlAttribute _Query = nodeSQL.Attributes["InsertQuery"];
            XmlAttribute _CommandType = nodeSQL.Attributes["CommandType"];

			//check for required members
			if (_ID == null) throw new XmlException("Table must include the attribute 'ID'.");
			if (_DisplayName == null) throw new XmlException("Table must include the attribute 'Display Name'.");
			//if (_DefaultWorksheetName == null) throw new XmlException("Table must include the attribute 'Default Worksheet Name'.");
			//if (_Category == null) throw new XmlException("Table must include the attribute 'Category'.");
			if (_Query == null) throw new XmlException("Table must include the attribute 'SQL'.");

			DataMap dm = new DataMap();
			dm.ID = _ID.Value;
			dm.DisplayName = _DisplayName.Value;
			if (_DisableSupportingDocuments != null)
				dm.DisableSupportingDocuments = bool.Parse(_DisableSupportingDocuments.Value);
			//dm.WorksheetName = _DefaultWorksheetName.Value;
			dm.Query = _Query.Value;
            dm.CommandType = _CommandType == null ? "" : _CommandType.Value;

			//create fields from schema
			XmlNodeList lstFields = DataRoot.SelectNodes("Table[@ID='" + ID + "']/Field");
			for (int i = 0; i < lstFields.Count; i++)
			{
				Field f = Field.Create(lstFields[i]);
				f.FieldID = i.ToString();
				if (f != null)
				{
					dm.Fields.Add(f);
				}
				//if (f != null) dm.Fields2[f.Destination] = f;
			}

			if (FileRoot != null)
			{
				//if the table ID also exists in the file schema, create a class for it
				XmlNode nodeFile = FileRoot.SelectSingleNode("Table[@ID='" + ID + "']");
				if (nodeFile != null)
				{
					DataFiles df = new DataFiles();
					df = DataMap.DataFiles.Create(FileRoot, nodeFile.Attributes["ID"].Value);
					dm.DataFile = df;

					//XmlNode nodeFileSQL = root.SelectSingleNode("Table[@ID='" + FileTableID + "']/SQL");
					//if (nodeFileSQL.Attributes["InsertQuery"] != null)
					//    dm._FileQuery = nodeSQL.Attributes["InsertQuery"].Value;
				}
			}


			return dm;
		}

		/// <summary>
		/// Write errors to screen and log them in the db.
		/// </summary>
		/// <param name="FileName">The name of the file being uploaded.</param>
		/// <param name="SheetName">The selected worksheet name.</param>
		/// <param name="Row">The row number of the error.</param>
		/// <param name="SourceField">The name of the field in the source file (spreadsheet).</param>
		/// <param name="DestinationField">The name of the field in the destination table.</param>
		/// <param name="ErrorMessage">The details of the error.</param>
		public void HandleError(string FileName, string SheetName, int Row, string SourceField,
			string DestinationField, string ErrorMessage)
		{
			if (this.ColumnNamesIn1stRow) ++Row;

			//add text to local error log
			this._Errors += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>",
				Row, SourceField, DestinationField, ErrorMessage);

			//add text to application log
			string s = String.Format("{0} on {1}, Row {2}, Field {3}/{4}",
				ErrorMessage, FileName, Row, SourceField, DestinationField);
			Log.WriteUploadLog(SessionHandler.Read("UserID").ToString(), FileName, s, "Error");
		}

		/// <summary>
		/// Scans through all the data rows and attached files to make sure they are in
		/// the correct format and use the right conventions.
		/// </summary>
		/// <returns>True if all the data and files are valid.</returns>
		public bool Validate()
		{
			bool DataIsValid = true;
			if (_UploadData != null) DataIsValid = ValidateData();

			bool FileIsValid = true;
			if (AttachedFile != null) FileIsValid = AttachedFile.ValidateAll();

			return (DataIsValid && FileIsValid);
		}

		/// <summary>
		/// Loop through the data rows and check each one for:
		/// - required fields are not blank
		/// - field values do not exceed the maximum length (when specified)
		/// - data respects the destination field's data type
		/// </summary>
		/// <returns>True if the data table is valid</returns>
		protected bool ValidateData()
		{
			bool IsValid = true;

			for (int idx = 0; idx < _UploadData.Rows.Count; idx++)
			{
				DataRow dr = _UploadData.Rows[idx];

				//Dictionary<string, object> dict = new Dictionary<string, object>(50);
				foreach (Field f in this.Fields) //loop through each field
				{
					string Source = GetHashValue(f.Destination);
					object Value = dr[Source];
					//dict.Add(f.Destination, Value);
					string ValueText = Convert.ToString(Value); //used for error checking

					//check for required fields
					if (f.Required && ValueText == "")
					{
						//error condition: a required field is blank
						HandleError(SessionHandler.Read("FileName"), this.ID, idx + 1, Source, f.Destination,
							"Required field is empty");
						IsValid = false;
					}					

					//check for field lengths
					if (f.Length > 0 && ValueText.Length > f.Length)
					{
						//error condition: data is too long to fit in the field
						HandleError(SessionHandler.Read("FileName"), this.DisplayName, idx + 1, Source, f.Destination,
							String.Format("Field is {0} characters too long", ValueText.Length - f.Length));
						IsValid = false;
					}

					//check data types
					bool CorrectType = true;
					if (ValueText.Trim() != "")
					{
						switch (f.DataType)
						{
							case "date":
								DateTime dt1;
								CorrectType = DateTime.TryParse(ValueText, out dt1);
								break;
							case "int":
								int i1;
								ValueText = ValueText.Replace(",", ""); //strip commas
								CorrectType = Int32.TryParse(ValueText, out i1);
								if (CorrectType) dr[Source] = ValueText;
								break;
							case "float":
								float f1;
								ValueText = ValueText.Replace(",", ""); //strip commas
								CorrectType = Single.TryParse(ValueText, out f1);
								if (CorrectType) dr[Source] = ValueText;
								break;
							case "money":
								float m1;
								ValueText = ValueText.Replace(",", ""); //strip commas
								ValueText = ValueText.Replace("$", ""); //strip dollar signs
								CorrectType = Single.TryParse(ValueText, out m1);
								if (CorrectType) dr[Source] = ValueText;
								break;
						}
					}
					if (!CorrectType)
					{
						//error condition: data in an incorrect data type
						HandleError(SessionHandler.Read("FileName"), this.DisplayName, idx + 1, Source, f.Destination,
							String.Format("{0} is not a valid {1} data type", ValueText, f.DataType));
						IsValid = false;
					}

				} //end for each field		
			} //end for each row
			return IsValid;
		}

		/// <summary>
		/// Loop through the data rows and insert each one into the staging
		/// database. If there is an error with the data or an exception, log
		/// this and don't insert anything.
		/// </summary>
		/// <returns>The number of rows inserted.</returns>
		public DataCounts Insert()
		{
			string SQL = GetInsertQuery();

			int DataRowsInserted = 0, FilesInserted = 0;
			int current_idx = 0;
			if (_UploadData != null)
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
				{
					cn.Open();
					using (SqlTransaction tr = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
					{
						try
						{
							using (SqlCommand cmd = new SqlCommand(SQL, cn, tr))
							{
                                if (String.IsNullOrEmpty(this.CommandType))
                                {
                                    for (int idx = 0; idx < _UploadData.Rows.Count; idx++)
                                    {
                                        current_idx = idx;
                                        DataRow dr = _UploadData.Rows[idx];
                                        cmd.Parameters.Clear();

                                        //hardcoded parameters that are common to all queries
                                        cmd.Parameters.AddWithValue("@SiteID", SessionHandler.Read("SiteID"));
                                        string FileName = SessionHandler.Read("FileName");
                                        FileName = Path.GetFileName(FileName);
                                        cmd.Parameters.AddWithValue("@Source", FileName);
                                        cmd.Parameters.AddWithValue("@UploadedBy", SessionHandler.Read("UserEmail"));
                                        cmd.Parameters.AddWithValue("@UploadID", this._UploadID);

                                        //Dictionary<string, object> dict = new Dictionary<string, object>(50);
                                        foreach (Field f in this.Fields)
                                        {
                                            string Source = GetHashValue(f.Destination);
                                            object Value = dr[Source];
                                            string ParameterName = String.Format("@{0}", f.Destination);
                                            cmd.Parameters.AddWithValue(ParameterName, Value);
                                        }

                                        DataRowsInserted += cmd.ExecuteNonQuery();
                                    } //end for
                                }
                                else
                                {
                                    foreach (DataRow dr in _UploadData.Rows)
                                    {
                                        dr["ModifiedDate"] = DateTime.Now;
                                        dr["Source"] = Path.GetFileName(SessionHandler.Read("FileName"));
                                        dr["UploadedBy"] = SessionHandler.Read("UserEmail");
                                        dr["UploadID"] = this._UploadID;
                                    }
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure; //Currently, this should always be StoredProcedure
                                    cmd.Parameters.Add("@Data", SqlDbType.Structured).Value = _UploadData;
                                    DataRowsInserted += cmd.ExecuteNonQuery();
                                }
							} //end command

							//TODO: move AttachedFile.InsertAll() here (it is below) so it can use the
							//same transaction.  I'm afraid this will block the database for a long time
							//while the files upload unless the weakest transaction IsolationLevel is used.

							tr.Commit(); //execute the transaction if the code reaches this point
						}
						catch (Exception ex)
						{
							HandleError(SessionHandler.Read("FileName"), this.DisplayName, current_idx + 1, "", "",
								String.Format("Exception during import: {0}", ex.Message));
							tr.Rollback(); //rollback the transaction if there's an exception
							DataRowsInserted = 0;
						}
					} //end transaction
					cn.Close();
				} //end connection
			}

			if (DataRowsInserted > 0)
				FilesInserted = InsertFile();

			DataCounts dc = new DataCounts(DataRowsInserted, FilesInserted);
			return dc;
		}

		public int InsertFile()
		{
			if (AttachedFile == null) return 0;
			int count = AttachedFile.InsertAll();
			return count;
		}

		protected string GetHashValue(string key)
		{
			if (_Mapping.Contains(key))
			{
				return Convert.ToString(_Mapping[key]);
			}
			else
				return "";
		}

		public void MapField(string Source, string Destination)
		{
			_Mapping.Add(Destination, Source);
		}

		/// <summary>
		/// Generate an insert query using the base query provided in the schema,
		/// and substitute the list of fields and field parameters. If there is a
        /// value for CommandType property (only value is StoredProcedure) than just
        /// return Query property, which should be the name of the stored procedure.
		/// </summary>
		/// <returns>The complete insert query to be sent to the database.</returns>
		private string GetInsertQuery()
		{
			StringBuilder des = new StringBuilder(2048);
			StringBuilder src = new StringBuilder(2048);
			int count = 0;

                for (int i = 0; i < this.Fields.Count; i++)
                {
                    string s = this.Fields[i].ToString();
                    if (s != "")
                    {
                        des.Append(String.Format("{0}{1}", (count == 0 ? "" : " , "), this.Fields[i].Destination));
                        src.Append(String.Format("{0}@{1}", (count == 0 ? "" : " , "), this.Fields[i].Destination));
                        ++count;
                    }
                }

                return String.IsNullOrEmpty(this.CommandType) ? String.Format(this.Query, des, src) : this.Query;
		}		
	}
}
