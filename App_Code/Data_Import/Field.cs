using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

/// <summary>
/// Represents one field in the datamap schema. The DataMap object contains a list of fields, 
/// which are loaded from the schema file.
/// </summary>
namespace DataLayer
{
	[Serializable]
	public class Field
	{
		public string FieldID;
		public string Destination;
		public string DataType;
		public int Length;
		public string Default;
		public string Source;
		public bool Required;

		public Field(string Destination, string Default, string DataType)
		{
			this.Destination = Destination;
			this.DataType = DataType;
			this.Default = Default;
			this.Required = false;
		}

		public Field(string Destination, string Default, string DataType, string Required)
		{
			this.Destination = Destination;
			this.DataType = DataType;
			this.Default = Default;
			this.Required = false;
			Boolean.TryParse(Required, out this.Required); //just use the default if this fails
		}

		public Field(string Destination, string Default, string DataType, string Length, string Required)
		{
			this.Destination = Destination;
			this.DataType = DataType;
			this.Length = 0;
			Int32.TryParse(Length, out this.Length);
			this.Default = Default;
			this.Required = false;
			Boolean.TryParse(Required, out this.Required); //just use the default if this fails
		}

		/// <summary>
		/// Instantiator function to create a Field object from an XML node.
		/// If the node does not contain the required attributes an XmlException will
		/// be thrown. Called from DataLayer.Create().
		/// </summary>
		/// <returns>The Field object populated from the schema node.</returns>
		/// <param name="node">The node of the schema XML file containing the field.</param>
		public static Field Create(XmlNode node)
		{
			XmlAttribute _Default = node.Attributes["Default"];
			XmlAttribute _DataType = node.Attributes["DataType"];
			XmlAttribute _Length = node.Attributes["Length"];
			XmlAttribute _Required = node.Attributes["Required"];
			XmlAttribute _Destination = node.Attributes["Destination"];

			//check for required members
			if (_Default == null) throw new XmlException("Field must include the attribute 'Default'.");
			if (_DataType == null) throw new XmlException("Field must include the attribute 'DataType'.");
			if (_Destination == null) throw new XmlException("Field must include the attribute 'Destination'.");

			Field f = null;
			if (_Required == null)
				f = new Field(_Destination.Value, _Default.Value, _DataType.Value);
			else if (_Length == null)
				f = new Field(_Destination.Value, _Default.Value, _DataType.Value, _Required.Value);
			else
				f = new Field(_Destination.Value, _Default.Value, _DataType.Value, _Length.Value, _Required.Value);

			return f;
		}

		public override string ToString()
		{
			return this.Destination;
		}
	}
}