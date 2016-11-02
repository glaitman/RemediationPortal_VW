using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Security
{
	public class User : IDisposable
	{
		public string Username;
		public int UserID;
		public string Email;
		public int SiteID;
		public string Theme;
		public string LastLogin;
		public string LastIPAddress;
		private bool disposed = false;

		protected static bool USE_SSO = false;

		//private constructor
		private User()
		{
			UserID = -1;
			SiteID = -1;
			//LastLogin = DateTime.MinValue;
		}

		public void Dispose()
		{
			// dispose of the managed and unmanaged resources
			Dispose(true);

			// tell the GC that the Finalize process no longer needs
			// to be run for this object.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			// process only if mananged and unmanaged resources have
			// not been disposed of.
			if (!this.disposed)
			{
				if (disposeManagedResources)
				{
					this.Username = null;
					this.UserID = -1;
					this.SiteID = -1;
					//this.LastLogin = DateTime.MinValue;
				}

				this.disposed = true;
			}
		}

		public void Logout() { }

		public static User Create(string username, string password, string email, string firstName, string lastName, string company, string passwordQuestion, string passwordAnswer, string theme)
		{
			User u = null;
//            string SQL = @"
//				INSERT INTO Membership
//				(Username, Password, Salt1, Salt2, Email, PasswordQuestion, PasswordAnswer, 
//				FirstName, LastName, Company, CreationDate, Approved, LockedOut, Theme)
//				VALUES (@username, @password, @salt1, @salt2, @email, @passwordQuestion, @passwordAnswer, 
//				@firstname, @lastname, @company, @creationdate, @approved, @lockedOut, @theme)
//			";

			//stored procedure ensures a unique username
			string SQL = "sp_InsertMembership";

			//must also create Person record here

			//----perform checking all the relevant checks here
			// and set the status of the error accordingly, e.g.:
			//status = MembershipCreateStatus.InvalidPassword
			//status = MembershipCreateStatus.InvalidAnswer
			//status = MembershipCreateStatus.InvalidEmail

			username = username.ToLower().Trim();
			password = password.Trim();
			if (username == "" || password == "") return null;

			//generate encrypted password
			string salt1 = Authentication.GenerateSalt();
			string salt2 = Authentication.GenerateSalt();
			string SaltedHash = Authentication.Encode(username, password, salt1, salt2);

			//bounds checking
			if (salt1.Length != 12 || salt2.Length != 12 || SaltedHash.Length != 88) return null;

			int result = 0;
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					DateTime creationDate = DateTime.Now;
					bool approved = true;
					bool lockedOut = false;

					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@username", username);
						cmd.Parameters.AddWithValue("@password", SaltedHash);
						cmd.Parameters.AddWithValue("@salt1", salt1);
						cmd.Parameters.AddWithValue("@salt2", salt2);
						cmd.Parameters.AddWithValue("@email", email);
						cmd.Parameters.AddWithValue("@passwordQuestion", passwordQuestion);
						cmd.Parameters.AddWithValue("@passwordAnswer", passwordAnswer);
						cmd.Parameters.AddWithValue("@firstname", firstName);
						cmd.Parameters.AddWithValue("@lastname", lastName);
						cmd.Parameters.AddWithValue("@company", company);
						cmd.Parameters.AddWithValue("@creationDate", creationDate);
						cmd.Parameters.AddWithValue("@approved", approved); //use Convert.ToInt32 if necessary
						cmd.Parameters.AddWithValue("@lockedOut", lockedOut);
						cmd.Parameters.AddWithValue("@theme", theme);
						result = cmd.ExecuteNonQuery();
					}
					cn.Close();
					if (result == 0)
					{
						//error or user exists
						u = null;
					}
					else
					{
						u = new User();
						u.Username = username;
						u.Email = email;
					}
				}
			}
			catch (Exception)
			{
				u = null;
			}

			return u;
		}

		public static User Login(string username, string password) 
		{
			string salt1 = String.Empty;
			string salt2 = String.Empty;
			string SQL;
			User u = null;

			//standardize data
			username = username.ToLower().Trim();
			password = password.Trim();
			if (username == "" || password == "") return null;

			//first query: get salt values and verify user's existence
			SQL = @"
				SELECT UserID, Salt1, Salt2
				FROM Membership
				WHERE Username=@username
				AND (LockedOut IS NULL OR LockedOut = 0)
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								u = new User();
								u.Username = username;
								u.UserID = Convert.ToInt32(reader["UserID"]);
								salt1 = Convert.ToString(reader["Salt1"]);
								salt2 = Convert.ToString(reader["Salt2"]);
							}
							reader.Close();
						}
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				if (u != null) u.Dispose();
				return null;
			}


			if (u == null) return null; //user doesn't exist

			//get encoded string
			string SaltedHash = Authentication.Encode(username, password, salt1, salt2);

			//second query: compare password and retrieve other data
			SQL = @"
				SELECT Email,PasswordQuestion,CreationDate,LastLogin,
				LastActivity,LastPasswordChange,LastLockout,
				Approved,LockedOut,Theme
				FROM Membership
				WHERE UserID=@userid
				AND Password=@password
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@userid", u.UserID);
						cmd.Parameters.AddWithValue("@password", SaltedHash);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								//TODO: add lots of fields here
								u.LastLogin = Convert.ToString(reader["LastLogin"]);
								u.Theme = Convert.ToString(reader["Theme"]);
								u.Email = Convert.ToString(reader["Email"]);
							}
							else
							{
								u.Dispose();
								u = null;
							}
							reader.Close();
						}
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				u.Dispose();
				return null;
			}

			return u;
		}

		public static void SetLastLogin(string username)
		{
			string IPAddress = HttpContext.Current.Request.UserHostAddress;

			string SQL = @"
				UPDATE Membership SET
				LastLogin = GetDate(),
				LastIPAddress = @ipaddress
				WHERE Username=@username
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						cmd.Parameters.AddWithValue("@ipaddress", IPAddress);
						cmd.ExecuteNonQuery();
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
			}
		}

		public static string GetEmailAddress(string username)
		{
			string Email = "";
			string SQL = @"
				SELECT Email
				FROM Membership
				WHERE Username=@username
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						Email = Convert.ToString(cmd.ExecuteScalar());
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				Email = "";
			}

			return Email;
		}

		public static bool SetPassword(string username, string old_password, string new_password)
		{
			int userid = 0;
			string salt1 = String.Empty;
			string salt2 = String.Empty;
			bool success = false;
			string SQL;

			//standardize data
			username = username.ToLower().Trim();
			old_password = old_password.Trim();
			new_password = new_password.Trim();
			if (username == "" || old_password == "" || new_password == "") return false;

			//first query: get salt values and verify user's existence
			SQL = @"
				SELECT UserID, Salt1, Salt2
				FROM Membership
				WHERE Username=@username
			";
			using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(SQL, cn))
				{
					cmd.Parameters.AddWithValue("@username", username);
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							userid = Convert.ToInt32(reader["UserID"]);
							salt1 = Convert.ToString(reader["Salt1"]);
							salt2 = Convert.ToString(reader["Salt2"]);
						}
						reader.Close();
					}
				}
				cn.Close();
			}

			//get encoded string for old password
			string OldSaltedHash = Authentication.Encode(username, old_password, salt1, salt2);

			//generate encoded string for new password
			salt1 = Authentication.GenerateSalt();
			salt2 = Authentication.GenerateSalt();
			string NewSaltedHash = Authentication.Encode(username, new_password, salt1, salt2);

			SQL = @"
				UPDATE Membership SET
				Password=@newpassword,
				Salt1=@salt1,
				Salt2=@salt2,
				LastPasswordChange=getdate(),
				LastActivity=getdate()
				WHERE UserID=@userid
				AND Password=@oldpassword
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@userid", userid);
						cmd.Parameters.AddWithValue("@oldpassword", OldSaltedHash);
						cmd.Parameters.AddWithValue("@newpassword", NewSaltedHash);
						cmd.Parameters.AddWithValue("@salt1", salt1);
						cmd.Parameters.AddWithValue("@salt2", salt2);
						if (cmd.ExecuteNonQuery() > 0)
							success = true;
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

		public static bool ResetPassword(string username, string new_password)
		{
			int userid = 0;
			string salt1 = String.Empty;
			string salt2 = String.Empty;
			bool success = false;
			string SQL;

			//standardize data
			username = username.ToLower().Trim();
			new_password = new_password.Trim();
			if (username == "" || new_password == "") return false;

			//first query: get username and verify user's existence
			SQL = @"
				SELECT UserID
				FROM Membership
				WHERE Username=@username
			";
			using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
			{
				cn.Open();
				using (SqlCommand cmd = new SqlCommand(SQL, cn))
				{
					cmd.Parameters.AddWithValue("@username", username);
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							userid = Convert.ToInt32(reader["UserID"]);
						}
						reader.Close();
					}
				}
				cn.Close();
			}

			//generate encoded string for new password
			salt1 = Authentication.GenerateSalt();
			salt2 = Authentication.GenerateSalt();
			string NewSaltedHash = Authentication.Encode(username, new_password, salt1, salt2);

			SQL = @"
				UPDATE Membership SET
				Password=@newpassword,
				Salt1=@salt1,
				Salt2=@salt2,
				LastActivity=getdate()
				WHERE UserID=@userid
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@userid", userid);
						cmd.Parameters.AddWithValue("@newpassword", NewSaltedHash);
						cmd.Parameters.AddWithValue("@salt1", salt1);
						cmd.Parameters.AddWithValue("@salt2", salt2);
						if (cmd.ExecuteNonQuery() > 0)
							success = true;
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

		public static bool IsMember(string username, string siteID)
		{
			int result = 0;
			string SQL = @"
				select count(*) 
				from membershipsite ms
				join membership m on m.userid = ms.userid
				where m.username = @username
				and ms.siteid = @siteid
			";

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						cmd.Parameters.AddWithValue("@siteid", siteID);
						result = (int)cmd.ExecuteScalar();
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				result = 0;
			}

			return (result > 0);
		}

		public static bool AddToSite(string username, string siteID)
		{
			int userID = 0;
			string SQL = @"
				select userid 
				from membership
				where username = @username
			";

			//first ensure 
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						userID = (int) cmd.ExecuteScalar();
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				userID = 0;
			}

			if (userID < 1) return false;

			SQL = @"
				insert into membershipsite
				(userid, siteid)
				values
				(@userid, @siteid)
			";

			bool success = false;
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@userid", userID);
						cmd.Parameters.AddWithValue("@siteid", siteID);
						success = cmd.ExecuteNonQuery() > 0;
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

		public bool CanWrite(string PermissionName) 
		{
			Permission p = Permission.Get(PermissionName, UserID, 1);
			return (p == null) ? false : p.CanWrite();
		}

		public bool CanRead(string PermissionName)
		{
			Permission p = Permission.Get(PermissionName, UserID, 1);
			return (p == null) ? false : p.CanRead();
		}

		public bool CanCreate(string PermissionName)
		{
			Permission p = Permission.Get(PermissionName, UserID, 1);
			return (p == null) ? false : p.CanCreate();
		}

		public static bool Unlock(string username)
		{
			bool success = false;
			string email = String.Empty;
			string SQL = @"
				select m.email
				from membership m
				--join vw_activeusers au on au.userid = m.userid
				where m.username = @username
			";

			//if (!LicenseAvailable()) return false;

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						email = Convert.ToString(cmd.ExecuteScalar());
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				success = false;
			}

			SQL = String.Format(@"
				UPDATE Membership
				SET LockedOut=0,
				--InvalidLogins=0,
				--MustChangePass={0},
				LastActivity=getdate()
				WHERE Username=@username
			", USE_SSO ? "0" : "1");

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						success = cmd.ExecuteNonQuery() > 0;
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				success = false;
			}

			if (success)
			{
				string new_password = null;

				if (!USE_SSO)
				{
					//generate a new password for the user
					new_password = Security.Authentication.GenerateRandomPassword();
					//sanity check
					if (new_password.Length < 1) return false;
					//reset password for the user
					if (!Security.User.ResetPassword(username, new_password)) return false;
				}

				//send out e-mail
				if (!Security.User.SendUnlockEmail(username, new_password, email)) return false;
			}

			return success;
		}

		public static bool Lock(string username)
		{
			bool success = false;
			string SQL = @"
				UPDATE Membership
				SET LockedOut=1,
				LastLockout=getdate(),
				LastActivity=getdate()
				WHERE Username=@username
			";

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						success = cmd.ExecuteNonQuery() > 0;
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

		public static bool SendUnlockEmail(string Username, string Password, string Email)
		{
			bool DEBUG = bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["Debug"]);
			const string SUBJECT = "Web Portal Account";
			const string REPLY_EMAIL = "Portal Administrator <no-reply@gateway.lfr.com>";
			string SUPPORT_NAME = System.Web.Configuration.WebConfigurationManager.AppSettings["SupportName"];
			string SUPPORT_PHONE = System.Web.Configuration.WebConfigurationManager.AppSettings["SupportPhone"];
			string SUPPORT_EMAIL = System.Web.Configuration.WebConfigurationManager.AppSettings["SupportEmail"];
			string SUPPORT_EMAIL_DISPLAY = System.Web.Configuration.WebConfigurationManager.AppSettings["SupportEmailDisplay"];

			//create feedback message
			string HOST_URL = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
			string SITE_URL = HOST_URL + HttpContext.Current.Request.ApplicationPath;
			string UsePassword = String.Format(@"
				Your web portal account has been activated with a temporary password. When you login,
				please click on Preferences to change this password to something
				you'll remember.
				Please take care to safeguard this account information:<br>
				<br>
				Username: <b>{0}</b><br>
				Password: <b>{1}</b><br>", Username, Password);

			string BodyText = String.Format(@"
				<div style='font:normal normal 12px/1.4em Arial,sans-serif;'>
				{0}
				<br>
				The website is located at <a href='{1}'>{1}</a>. 
				If you have further questions, please contact {2} by e-mailing <a href='mailto:{3}'>{4}</a> 
				or calling {5}.<br>
				<br>
				</div>
			", Password == null ? "Your web portal account has been reactivated." : UsePassword
			 , SITE_URL, SUPPORT_NAME, SUPPORT_EMAIL, SUPPORT_EMAIL_DISPLAY, SUPPORT_PHONE);



			Email em = new Email(SUBJECT, BodyText, Email);
			em.ReplyEmail = REPLY_EMAIL;

			try
			{
				em.SendMail();
			}
			catch (Exception ex)
			{
				////Make sure to log the error in the errorLog table.
				//Log.WriteError(HttpContext.Current.User.Identity.Name
				//    , Request.Url.ToString()
				//    , Request.UserHostAddress.ToString()
				//    , ex);

				throw new Exception("Could not deliver mail.", ex);
			}
			return true;
		}

		public static bool Exists(string username)
		{
			string SQL;
			bool found = false;

			//standardize data
			username = username.ToLower().Trim();
			if (username == "") return false;

			//verify user's existence
			SQL = @"
				SELECT Count(*)
				FROM Membership
				WHERE Username=@username
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								if (Convert.ToInt32(reader[0]) > 0) found = true;
							}
							reader.Close();
						}
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				return false;
			}

			return found;
		}

		public static string GetUserID(string Username)
		{
			object result = null;
			string SQL = @"
				select userid from membership 
				where username = @username
			";

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{

					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", Username);
						result = cmd.ExecuteScalar();
					}
					cn.Close();

					return (result == null ? String.Empty : result.ToString());
				}
			}
			catch (Exception)
			{
				return String.Empty;
			}

		}

		public static string GetTheme(string Username)
		{
			object result = null;
			string SQL = @"
				select theme from membership 
				where username = @username
			";

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{

					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", Username);
						result = cmd.ExecuteScalar();
					}
					cn.Close();

					return (result == null ? String.Empty : result.ToString());
				}
			}
			catch (Exception)
			{
				return String.Empty;
			}

		}

		//get a user's data (used to return to the site with a cookie)
		public static User Rez(string username)
		{
			string SQL;
			User u = null;

			//standardize data
			username = username.ToLower().Trim();
			if (username == "") return null;

			SQL = @"
				SELECT UserID,Email,PasswordQuestion,CreationDate,LastLogin,
				LastActivity,LastPasswordChange,LastLockout,
				Approved,LockedOut,Theme
				FROM Membership
				WHERE Username=@username
				AND (LockedOut IS NULL OR LockedOut = 0)
			";
			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", username);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								u = new User();
								u.Username = username;
								u.UserID = Convert.ToInt32(reader["UserID"]);
								u.LastLogin = Convert.ToString(reader["LastLogin"]);
								u.Theme = Convert.ToString(reader["Theme"]);
								u.Email = Convert.ToString(reader["Email"]);
							}
							reader.Close();
						}
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				u.Dispose();
				return null;
			}

			return u;
		}

		public static string GetHomeDirectory(string Username, int SiteID)
		{
			object result = null;
			string SQL = @"
				select homedirectory from membershipsite
				where username = @username
				and siteid = @siteid
			";

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{

					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@username", Username);
						cmd.Parameters.AddWithValue("@siteid", SiteID);
						result = cmd.ExecuteScalar();
					}
					cn.Close();

					return (result == null ? String.Empty : result.ToString());
				}
			}
			catch (Exception)
			{
				return String.Empty;
			}

		}


	}

	//a "role" defines a commonly-grouped set of permissions used when creating a user
	public class Role
	{
		public int ID;
		public string Name;

		//private constructor
		private Role()
		{
			ID = 0;
		}

		public void Add(User u) { }
		public void Remove(User u) { }

		//shared constructors
		public static Role Create(DataRow dr) { return null; }
		public static Role Create(string name)
		{
			Role r = new Role();
			r.Name = name;
			return r;
		}


		//TODO: add project code for accounting table
		public static int GetSiteID(int UserID, string Rolename)
		{
			int SiteID = -1;
			string SQL = @"
				select s.siteid from membershipsite ms
				join site s on s.siteid = ms.siteid
				where ms.userid = @userid
				and s.url = @url
			";

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@userid", UserID);
						cmd.Parameters.AddWithValue("@url", Rolename);
						SiteID = (int)cmd.ExecuteScalar();
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				SiteID = -1;
			}

			return SiteID;
		}


	}

	///A "Permission" is the declarative ability to do something with a part of the system.
	///By default, everyone has no access to anything that is protected by a permission.
	///To save time and space, permissions need only be added to the database for parts of
	///the system a user needs access to.  Thus, if only 1 permission applies to a single
	///user, just 1 record is created.
	///2006-06-29 MM
	public class Permission
	{
		private const int MAX_LENGTH = 8;

		//constant arrays to compare to
		private BitArray BIT_CREATE;	//8
		private BitArray BIT_READ;		//4
		private BitArray BIT_WRITE;		//2
		private BitArray BIT_RESERVED;	//1

		private int m_PermissionID;
		private int m_UserID;
		private string m_PermissionName;

		//main array
		private BitArray m_Permissions = null;

		//private constructor
		private Permission()
		{
			m_PermissionID = m_UserID = -1;

			//initialize "constant" arrays:
			BIT_RESERVED = new BitArray(MAX_LENGTH);
			BIT_RESERVED.Set(0, true);

			BIT_WRITE = new BitArray(MAX_LENGTH);
			BIT_WRITE.Set(1, true);

			BIT_READ = new BitArray(MAX_LENGTH);
			BIT_READ.Set(2, true);

			BIT_CREATE = new BitArray(MAX_LENGTH);
			BIT_CREATE.Set(3, true);

			m_Permissions = new BitArray(MAX_LENGTH);
		}

		public bool CanWrite()
		{
			BitArray ba = m_Permissions.And(BIT_WRITE);
			for (int i = 0; i < ba.Length; i++)
				if (ba[i] != BIT_WRITE[i]) return false;

			return true;
		}

		public bool CanRead()
		{
			BitArray ba = m_Permissions.And(BIT_READ);
			for (int i = 0; i < ba.Length; i++)
				if (ba[i] != BIT_READ[i]) return false;

			return true;
		}

		public bool CanCreate()
		{
			BitArray ba = m_Permissions.And(BIT_CREATE);
			for (int i = 0; i < ba.Length; i++)
				if (ba[i] != BIT_CREATE[i]) return false;

			return true;
		}

		//shared constructors
		public static Permission Get(string PermissionName, int UserID, int SiteID)
		{
//            string SQL = @"
//				SELECT PermissionID,CanWrite,CanRead,CanCreate
//				FROM Permission
//				WHERE PermissionName=@PermissionName
//				AND UserID=@UserID
//			";

			string SQL = @"
				select sp.sitepermissionid,msp.canwrite,msp.canread,msp.cancreate
				from membershipsitepermission msp
				join sitepermission sp on sp.sitepermissionid = msp.sitepermissionid
				join membershipsite ms on ms.membershipsiteid = msp.membershipsiteid
				where sp.permissionname = @permissionname
				and ms.userid = @userid
				and ms.siteid = @siteid
			";

			Permission p = null;

			try
			{
				using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MembershipProvider"].ConnectionString))
				{
					cn.Open();
					using (SqlCommand cmd = new SqlCommand(SQL, cn))
					{
						cmd.Parameters.AddWithValue("@PermissionName", PermissionName);
						cmd.Parameters.AddWithValue("@UserID", UserID);
						cmd.Parameters.AddWithValue("@SiteID", SiteID);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								p = new Permission();
								p.m_PermissionName = PermissionName;
								p.m_UserID = UserID;
								if (reader["SitePermissionID"] != DBNull.Value) p.m_PermissionID = Convert.ToInt32(reader["SitePermissionID"]);
								if (reader["CanCreate"] != DBNull.Value && Convert.ToBoolean(reader["CanCreate"]) == true) p.m_Permissions.Or(p.BIT_CREATE);
								if (reader["CanRead"] != DBNull.Value && Convert.ToBoolean(reader["CanRead"]) == true) p.m_Permissions.Or(p.BIT_READ);
								if (reader["CanWrite"] != DBNull.Value && Convert.ToBoolean(reader["CanWrite"]) == true) p.m_Permissions.Or(p.BIT_WRITE);
							}
							reader.Close();
						}
					}
					cn.Close();
				}
			}
			catch (Exception)
			{
				p = null;
			}

			return p;
		}

		public static bool CanWrite(string PermissionName, string UserID, string SiteID)
		{
			int uid, sid;
			if (!Int32.TryParse(UserID, out uid)) return false;
			if (!Int32.TryParse(SiteID, out sid)) return false;

			Permission p = Permission.Get(PermissionName, uid, sid);
			return (p == null) ? false : p.CanWrite();
		}

		public static bool CanRead(string PermissionName, string UserID, string SiteID)
		{
			int uid, sid;
			if (!Int32.TryParse(UserID, out uid)) return false;
			if (!Int32.TryParse(SiteID, out sid)) return false;

			Permission p = Permission.Get(PermissionName, uid, sid);
			return (p == null) ? false : p.CanRead();
		}

		public static bool CanRead(string PermissionName, string UserID)
		{
			string SiteID = SessionHandler.Read("SiteID");

			int uid, sid;
			if (!Int32.TryParse(UserID, out uid)) return false;
			if (!Int32.TryParse(SiteID, out sid)) return false;

			Permission p = Permission.Get(PermissionName, uid, sid);
			return (p == null) ? false : p.CanRead();
		}

		public static bool CanCreate(string PermissionName, string UserID, string SiteID)
		{
			int uid, sid;
			if (!Int32.TryParse(UserID, out uid)) return false;
			if (!Int32.TryParse(SiteID, out sid)) return false;

			Permission p = Permission.Get(PermissionName, uid, sid);
			return (p == null) ? false : p.CanCreate();
		}


	}

	public static class Authentication
	{
		//public static string CreateRandomPassword(int PasswordLength) { return null; }

		public static string GenerateSalt()
		{
			RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
			rc2.GenerateIV();
			return Convert.ToBase64String(rc2.IV);
		}


		public static string GenerateRandomPassword()
		{
			const int PASSWORD_LENGTH = 8;

			//generate a random password of hex digits
			string password = String.Empty;

			int digit;
			string hex;
			Random r = new Random();
			for (int i = 0; i < PASSWORD_LENGTH; i++)
			{
				digit = r.Next(1, 16);
				hex = String.Format("{0:x}", digit);
				password += hex;
			}

			return password;
		}

		public static string GenerateRandomImprovedPassword()
		{
			const int PASSWORD_LENGTH = 8;

			//generate a random password
			string password = String.Empty;

			int digit;
			string hex;
			Random r = new Random();
			for (int i = 0; i < PASSWORD_LENGTH - 1; i++)
			{
				digit = r.Next(1, 16);
				hex = String.Format("{0:x}", digit);
				password += hex;
			}

			//generate one special character and add it at a random position
			//quotes and backslash characters should be ok here
			switch (r.Next(1, 3))
			{
				case 3:
					digit = r.Next(91, 95);
					break;
				case 2:
					digit = r.Next(58, 64);
					break;
				default:
					digit = r.Next(33, 47);
					break;
			}

			int pos = r.Next(0, password.Length);
			hex = Convert.ToString((char)digit);
			password = password.Insert(pos, hex);

			return password;
		}

		public static string Encode(string Username, string Password, string Salt1, string Salt2)
		{
			//changing this string will invalidate all existing passwords
			//good security to change when moving to a new database and resetting all passwords
			const string FormatString = "{0}${1}+{2}^{3}.{4}";

			SHA512Managed sha = new SHA512Managed();
			UTF8Encoding encoder = new UTF8Encoding();

			if (sha == null || encoder == null) return null;

			string s = String.Format(FormatString, Username, Password, Salt1, Salt2, Password.Length);
			byte[] hash = sha.ComputeHash(encoder.GetBytes(s));
			return Convert.ToBase64String(hash);
		}

		public static string Encode(string Password, string Salt1, string Salt2)
		{
			//changing this string will invalidate all existing passwords
			const string FormatString = "{0}+{1}^{2}${3}";

			SHA512Managed sha = new SHA512Managed();
			UTF8Encoding encoder = new UTF8Encoding();

			if (sha == null || encoder == null) return null;

			string s = String.Format(FormatString, Password, Salt1, Salt2, Password.Length);
			byte[] hash = sha.ComputeHash(encoder.GetBytes(s));
			return Convert.ToBase64String(hash);
		}

		public static string Encode(string Password, string Salt)
		{
			//changing this string will invalidate all existing passwords
			const string FormatString = "{0}>{1}:{2}";

			SHA512Managed sha = new SHA512Managed();
			UTF8Encoding encoder = new UTF8Encoding();

			if (sha == null || encoder == null) return null;

			string s = String.Format(FormatString, Password, Salt, Password.Length);
			byte[] hash = sha.ComputeHash(encoder.GetBytes(s));
			return Convert.ToBase64String(hash);
		}

	}

	//This class bridges the gap between our own user definition and .NET's built-in support for
	//certain controls.  Only needed items are implemented.
	//class LocalMembershipProvider : MembershipProvider
	//{
	//    public override string ApplicationName
	//    {
	//        get
	//        {
	//            throw new Exception("The method or operation is not implemented.");
	//        }
	//        set
	//        {
	//            throw new Exception("The method or operation is not implemented.");
	//        }
	//    }

	//    public override bool ChangePassword(string username, string oldPassword, string newPassword)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool DeleteUser(string username, bool deleteAllRelatedData)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool EnablePasswordReset
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override bool EnablePasswordRetrieval
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override int GetNumberOfUsersOnline()
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string GetPassword(string username, string answer)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override MembershipUser GetUser(string username, bool userIsOnline)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string GetUserNameByEmail(string email)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override int MaxInvalidPasswordAttempts
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override int MinRequiredNonAlphanumericCharacters
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override int MinRequiredPasswordLength
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override int PasswordAttemptWindow
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override MembershipPasswordFormat PasswordFormat
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override string PasswordStrengthRegularExpression
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override bool RequiresQuestionAndAnswer
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override string ResetPassword(string username, string answer)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool RequiresUniqueEmail
	//    {
	//        get { throw new Exception("The method or operation is not implemented."); }
	//    }

	//    public override bool UnlockUser(string userName)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override void UpdateUser(MembershipUser user)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool ValidateUser(string username, string password)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }
	//}

	//class LocalRoleProvider : RoleProvider
	//{
	//    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string ApplicationName
	//    {
	//        get
	//        {
	//            throw new Exception("The method or operation is not implemented.");
	//        }
	//        set
	//        {
	//            throw new Exception("The method or operation is not implemented.");
	//        }
	//    }

	//    public override void CreateRole(string roleName)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string[] GetAllRoles()
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string[] GetRolesForUser(string username)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override string[] GetUsersInRole(string roleName)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool IsUserInRole(string username, string roleName)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }

	//    public override bool RoleExists(string roleName)
	//    {
	//        throw new Exception("The method or operation is not implemented.");
	//    }
	//}

}

