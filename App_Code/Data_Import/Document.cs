using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

/// <summary>
/// The upload portal allows an arbitrary file to be attached to the DataMap
/// object. This file is added to an IDEA library associated with the upload.
/// e.g. for Waste & Recycling, the file is inserted into the Disposal 
/// Documentation folder. The file may be any kind of file, including a zipfile.
/// If the file is a zipfile, it is unzipped (just at the outermost level) and
/// everything inside is added to the IDEA library.
/// </summary>
namespace DataLayer
{
	public class Document
	{
		public DataMap Parent;
		public string DocumentCategory;
		public string FileName;
		public int ContentLength;
		public string ContentType;
		List<byte> FileImage = new List<byte>();
		public List<Document> Entries; //a list of zipped files contained in this document
		public string Category;
		public string TrackingNumber;
		public string ShippedDate;
		public string DateCreated;
		public string MailCode;

		public Document(DataMap Parent, string FileName, int ContentLength, string ContentType)
		{
			this.Parent = Parent;
			this.DocumentCategory = Parent.ID;
			this.FileName = FileName;
			this.ContentLength = ContentLength;
			this.ContentType = ContentType;
			this.FileImage = null;
			this.Entries = null;
			this.Category = String.Empty;
			this.TrackingNumber = String.Empty;
			this.ShippedDate = String.Empty;
			this.DateCreated = String.Empty;
			this.MailCode = String.Empty;
		}

		/// <summary>
		/// Returns a valid MIME type for the given filename, based on the extension.
		/// Uses the prevalent application/octet-stream for most types of files.
		/// </summary>
		/// <param name="FileName">The name of the file whose extension will be used to determine the MIME type.</param>
		/// <returns>The MIME type of the file.</returns>
		public static string GetContentType(string FileName)
		{
			string ext = Path.GetExtension(FileName);
			switch (ext.ToLower())
			{
				case ".pdf":
					return "application/pdf";
				case ".doc":
					return "application/msword";
				case ".xls":
					return "application/vnd.ms-excel";
				case ".gif":
					return "image/gif";
				case ".jpg":
					return "images/pjpeg";
				case ".txt":
					return "text/plain";
				case ".zip":
					return "application/zip";
				default:
					return "application/octet-stream";
			}
		}

		/// <summary>
		/// Instantiator. Given a file, creates a new Document object. If the file is a zipfile, it unzips any
		/// files inside, ignoring paths, creates Document objects for them, and and adds them to the parent 
		/// Document's list of Entries.
		/// </summary>
		/// <param name="Parent">The parent DataMap object is needed for reporting errors.</param>
		/// <param name="AttachedFile">The original file uploaded.</param>
		/// <param name="Category">The category of the file. IE: Remediation, Waste, Asbestos, etc</param>
		/// <returns>The new document, or the parent document in the case of a zipfile.</returns>
		public static Document Create(DataMap Parent, HttpPostedFile AttachedFile, string Category)
		{
			string FileName = Path.GetFileName(AttachedFile.FileName); //strip local path
			Document d = new Document(Parent, FileName, AttachedFile.ContentLength, AttachedFile.ContentType);

			string ext = Path.GetExtension(FileName);
			if (ext.ToLower() == ".zip") //unzip the files and add them to the list of entries
			{
				d.Entries = new List<Document>(6);

				using (ICSharpCode.SharpZipLib.Zip.ZipInputStream zis = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(AttachedFile.InputStream))
				{
					ICSharpCode.SharpZipLib.Zip.ZipEntry ze = zis.GetNextEntry();
					while (ze != null)
					{
						if (ze.IsFile)
						{
							int FileSize = (int)ze.Size;
							Document zd = new Document(Parent, Path.GetFileName(ze.Name), FileSize,
								GetContentType(ze.Name));

							zd.FileImage = new List<byte>();
							byte[] data = new byte[FileSize];
							zis.Read(data, 0, data.Length);
							zd.FileImage.AddRange(data);
							//important to know where to place the file when doing an insert
							zd.DocumentCategory = Category;

							d.Entries.Add(zd); //attach the unzipped file to the document
						}
						ze = zis.GetNextEntry();
					}
					zis.Close();
				}


			}
			else //create the file image (there will be no entries for this file)
			{
				d.FileImage = new List<byte>();
				byte[] data = new byte[AttachedFile.InputStream.Length];

				AttachedFile.InputStream.Read(data, 0, data.Length);
				d.FileImage.AddRange(data);
			}

			//set the documentcategory type if its null, based on session as a backup
			if (d.DocumentCategory == null || d.DocumentCategory == "")
				d.DocumentCategory = Category;

			return d;
		}

		/// <summary>
		/// Scans the document for errors. If a zipfile is attached, scan through each entry for 
		/// errors and ignore the parent document.
		/// </summary>
		/// <returns>True if the files are valid.</returns>
		public bool ValidateAll()
		{
			int count = 0;
			
			if (this.Entries != null) //zipfile
			{
				foreach (Document zd in this.Entries)
					count += zd.ValidateGeneral();
			}
			else
				count += this.ValidateGeneral();			

			return (count == 0);
		}

		/// <summary>
		/// Ensures the given file matches the correct file naming convention.
		/// If any errors are found, it increments the number of errors and
		/// returns the total. Called from ValidateAll(). 
		/// Expects filename in MAILCODE_TITLE_YYYY-MM-DD format.
		/// </summary>
		/// <returns>The total number of errors found.</returns>
		protected int ValidateGeneral()
		{
			int NumErrors = 0;

			//validate filename
			char[] delim = { '_' };
			string FileNameWithoutExt = Path.GetFileNameWithoutExtension(this.FileName);
			string[] Segments = FileNameWithoutExt.Split(delim);
			if (Segments.Length == 3)
			{
				this.MailCode = Segments[0];
				this.Category = Segments[1];
				this.DateCreated = Segments[2]; //YYYY-MM-DD
			}
			else if (Segments.Length == 5)
			{
				this.MailCode = Segments[0];
				this.Category = Segments[1];
				this.DateCreated = String.Format("{0}-{1}-{2}", Segments[2], Segments[3], Segments[4]); //put YYYY_MM_DD into YYYY-MM-DD
			}
			else
			{
				//error - invalid filename
				Parent.HandleError(Parent.ID, Parent.DisplayName, -1, "Attached File", "Attached File",
					String.Format("{0} is not in the format MAILCODE_TITLE_YYYY-MM-DD", this.FileName));
				NumErrors++;
				return NumErrors; //return early only in this case -- otherwise it'll trigger all the other errors
			}

			DateTime dt;
			if (!DateTime.TryParse(this.DateCreated, out dt)) //check for a valid date format
			{
				bool InvalidDate = true;
				if (this.DateCreated.Length == 8) //if not, make sure the date isn't in YYYYMMDD before giving up
				{
					string t = String.Format("{0}-{1}-{2}", this.DateCreated.Substring(0, 4), this.DateCreated.Substring(4, 2), this.DateCreated.Substring(6, 2));
					if (DateTime.TryParse(t, out dt))
					{
						this.DateCreated = t;
						InvalidDate = false;
					}
				}

				if (InvalidDate)
				{
					//error - invalid date format
					Parent.HandleError(Parent.ID, Parent.DisplayName, -1, "Attached File", "Attached File",
						String.Format("{0} has an invalid file date: Filename should be MAILCODE_TITLE_YYYY-MM-DD", this.FileName));
					NumErrors++;
				}
			}

			return NumErrors;
		}

		/// <summary>
		/// Insert the document into the staging database. If the document is a zipfile, 
		/// insert each entry instead. (The original zipfile is discarded.)
		/// </summary>
		/// <returns>The number of rows inserted.</returns>
		public int InsertAll()
		{
			int count = 0;

			if (this.Entries != null) //zipfile
			{
				foreach (Document zd in this.Entries)
				{
					count += zd.Insert();
				}
			}
			else
			{
				count += this.Insert();
			}

			return count;
		}

		/// <summary>
		/// Inserts this document into the Files database table. Called from InsertAll().
		/// </summary>
		/// <returns>The number of rows inserted (1 or 0).</returns>
		protected int Insert()
		{
			//get the file insert query from the schema
			string category = this.DocumentCategory;
			string SQL = "";
			if (category == null || category == "")
			{
				Parent.HandleError(Parent.ID, Parent.DisplayName, -1, "Attached File", "Attached File",
					String.Format("A category was not selected. It is unknown what type of document [{0}] is.", this.FileName));
				return -1;
			}

			DataMap.DataFiles df = new DataMap.DataFiles();
			if (this.Parent.DataFile == null)
			{
				//if there is no access to the schema before this, create it so we can access the query.
				DataMap dm = CreateDataMap(category);
				df = dm.DataFile;
			}
			else
				df = this.Parent.DataFile;

			if (df == null || df.InsertQuery == "")
			{
				Parent.HandleError(Parent.ID, Parent.DisplayName, -1, "Attached File", "Attached File",
					"No Insert Query was found. Cannot complete the insert.");
				return -1;
			}

			System.Text.StringBuilder des = new System.Text.StringBuilder(2048);
			System.Text.StringBuilder src = new System.Text.StringBuilder(2048);
			int cnt = 0;
			for (int i = 0; i < df.Fields.Count; i++)
			{
				string s = df.Fields[i].ToString();
				if (s != "")
				{
					des.Append(String.Format("{0}{1}", (cnt == 0 ? "" : " , "), df.Fields[i].Destination));
					src.Append(String.Format("{0}@{1}", (cnt == 0 ? "" : " , "), df.Fields[i].Destination));
					++cnt;
				}
			}

			SQL = String.Format(df.InsertQuery, des, src);

			int count = 0;
			using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(SQL, cn))
				{
					//hardcoded parameters that are common to all queries
					cmd.Parameters.AddWithValue("@SiteID", SessionHandler.Read("SiteID"));
					string SourceFileName = SessionHandler.Read("FileName");
					SourceFileName = Path.GetFileName(SourceFileName);
					string Title = Path.GetFileNameWithoutExtension(this.FileName);
					//Waste uses "category" in a different way then most modules. Most modules set the 
					//	category as a type from a dropdown. Waste uses it for description.
					string docCategory = (category == "Waste & Recycling$" ? this.Category : string.Empty);

					cmd.Parameters.AddWithValue("@Source", SourceFileName);
					cmd.Parameters.AddWithValue("@UploadedBy", SessionHandler.Read("UserEmail"));
					cmd.Parameters.AddWithValue("@Filename", this.FileName);
					cmd.Parameters.AddWithValue("@Category", docCategory);
					cmd.Parameters.AddWithValue("@FileImage", this.FileImage.ToArray());
					cmd.Parameters.AddWithValue("@ContentType", this.ContentType);
					cmd.Parameters.AddWithValue("@FileSize", this.ContentLength);
					cmd.Parameters.AddWithValue("@UploadID", Parent.UploadID);
					//only pass in the affected title value if the query is inserting it
					if (df.InsertQuery.Contains("@Title"))
						cmd.Parameters.AddWithValue("@Title", Title);

					//for every extra field in the object, set the name as the destination from the
					//	schema and set the value from an evalutated interpretation of the Default name.
					//	This needs to be redesigned though... Because evaluating the property of this object
					//	is an akward way of being dynamic.
					foreach (Field f in df.Fields)
					{
						string Source = f.Destination;
						String propName = f.Default;
						System.Reflection.FieldInfo fi = this.GetType().GetField(propName);
						if (fi != null && fi.GetValue(this) != null)
						{
							string Value = fi.GetValue(this).ToString();

							string ParameterName = String.Format("@{0}", f.Destination);
							cmd.Parameters.AddWithValue(ParameterName, Value);
						}
					}
					count = cmd.ExecuteNonQuery();
				}

				cn.Close();
			}

			return count;
		}

		private DataMap CreateDataMap(string ID)
		{
			System.Web.UI.WebControls.XmlDataSource xdsData = new System.Web.UI.WebControls.XmlDataSource();
			xdsData.ID = "xdsData";
			xdsData.DataFile = "~/XML/schema.config";
			xdsData.XPath = "Table[@ID='" + ID + "']";
			XmlDocument xmlCriteriaSchema = (XmlDocument)xdsData.GetXmlDocument();
			XmlNode DataRoot = xmlCriteriaSchema.DocumentElement;

			System.Web.UI.WebControls.XmlDataSource xdsFiles = new System.Web.UI.WebControls.XmlDataSource();
			xdsFiles.ID = "xdsFiles";
			xdsFiles.DataFile = "~/XML/schema_files.config";
			xdsFiles.XPath = "Table[@ID='" + ID + "']";
			XmlDocument xmlCriteriaFile = (XmlDocument)xdsFiles.GetXmlDocument();
			XmlNode FileRoot = xmlCriteriaFile.DocumentElement;

			DataMap dm = DataMap.Create(DataRoot, FileRoot, ID);

			return dm;
		}
	}
}