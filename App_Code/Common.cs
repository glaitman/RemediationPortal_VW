using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class Common
{
    public static Control GetControl(Control Parent, string ControlID)
    {
        return Parent.FindControl(ControlID);
    }

    public static Control GetControl(Control Parent, string FirstControlID, string SecondControlID)
    {
        Control c = null;
        c = Parent.FindControl(FirstControlID);
        if (c == null) c = Parent.FindControl(SecondControlID);
        return c;
    }

	public static Control GetControl(Control Parent, string FirstControlID, string SecondControlID, string ThirdControlID)
	{
		Control c = null;
		c = Parent.FindControl(FirstControlID);
		if (c == null) c = Parent.FindControl(SecondControlID);
		if (c == null) c = Parent.FindControl(ThirdControlID);
		return c;
	}

	public static string GetMonth(string MonthID)
	{
		switch (MonthID)
		{
			case "1":
				return "January";
			case "2":
				return "February";
			case "3":
				return "March";
			case "4":
				return "April";
			case "5":
				return "May";
			case "6":
				return "June";
			case "7":
				return "July";
			case "8":
				return "August";
			case "9":
				return "September";
			case "10":
				return "October";
			case "11":
				return "November";
			case "12":
				return "December";
		}
		return "None";
	}

	public static void Alert(Page p, string ErrorMessage)
	{
		string js = String.Format("<script>alert('{0}')</script>\n", ErrorMessage.Replace("'", @"\'").Replace(Environment.NewLine, @"\n"));
		p.ClientScript.RegisterStartupScript(p.GetType(), "Error Message", js);
	}

	public static void AuthorizeGrid(SecureControls.SecureDetailsView dv, string PermissionName)
	{
		dv.CanDelete = dv.CanUpdate = Security.Permission.CanWrite(PermissionName, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID"));
		dv.CanInsert = Security.Permission.CanCreate(PermissionName, SessionHandler.Read("UserID"), SessionHandler.Read("SiteID"));
	}

	public static string CheckBoxList_SaveArray(CheckBoxList cbl, TextBox txt)
	{
		const string LIST_DELIMITER = "|";
		if (cbl == null) return "";

		StringBuilder sb = new StringBuilder(1024);
		foreach (ListItem li in cbl.Items) //create a delimited string of items
		{
			if (li.Selected) sb = sb.Append(li.Value).Append(LIST_DELIMITER);
		}

		if (txt != null) //append "other" value if present
			sb.Append(txt.Text).Append(LIST_DELIMITER);

		if (sb.Length > 0) sb = sb.Remove(sb.Length - 1, 1); //remove last delimiter

		return sb.ToString();
	}

	public static void CheckBoxList_LoadArray(CheckBoxList cbl, TextBox txt, CheckBox chk, string DelimitedText)
	{
		string Item;
		const string LIST_DELIMITER = "|";
		if (cbl == null) return;

		ArrayList al = new ArrayList(16);
		int Index = 0, LastIndex = -1;
		do
		{
			//load delimited strings into an array
			Index = DelimitedText.IndexOf(LIST_DELIMITER, LastIndex + 1);
			if (Index >= 0)
			Item = DelimitedText.Substring(LastIndex + 1, Index - LastIndex - 1);
			else
				Item = DelimitedText.Substring(LastIndex + 1, DelimitedText.Length - LastIndex - 1);

			if (Item != String.Empty) al.Add(Item);
			LastIndex = Index;
		} while (Index >= 0);

		ListItem li = null;
		foreach (string s in al)
		{
			li = cbl.Items.FindByValue(s);
			if (li != null)
				li.Selected = true; //select items that match the array
			else
			{
				if (chk != null && txt != null) //select "other" box and enter in textbox if present
				{
					chk.Checked = true;
					txt.Text = s;
				}
			}
		}
	}

    public static bool TryParseNullableDate(string value, out DateTime? result)
    {
        DateTime dt;
        if (DateTime.TryParse(value, out dt))
        {
            result = dt;
            return true;
        }
        else
        {
            result = DateTime.MinValue;
            return false;
        }
    }
}
