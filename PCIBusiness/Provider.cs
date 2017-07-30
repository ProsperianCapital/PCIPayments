using System;
using System.Text;

namespace PCIBusiness
{
	public class Provider : BaseData
	{
		private string  bureauCode;
		private string  bureauName;
		private byte    bureauStatus;
		private string  urlGoTo;
		private string  urlReturn;
		private string  userID;
		private string  password;

		public  string  BureauCode
		{
			get { return Tools.NullToString(bureauCode); }
		}
		public  string  BureauName
		{
			get { return Tools.NullToString(bureauName); }
		}
		public  byte    StatusCode
		{
			get { return bureauStatus; }
		}
		public  string  StatusName
		{
			get { return ""; }
		}

		public string ConnectionDetails(byte mode,string separator="")
		{
			if ( mode == 1 ) // HTML
				return "<table>"
					  + "<tr><td>Payment Provider</td><td class='Red'> : " + BureauName + "</td></tr>"
					  + "<tr><td>Bureau Code</td><td class='Red'> : " + BureauCode + "</td></tr>"
					  + "<tr><td>Status</td><td class='Red'> : " + StatusName + "</td></tr>"
					  + "<tr><td colspan='2'><hr /></td></tr>"
					  + "<tr><td>Go To URL</td><td> : " + "" + "</td></tr>"
					  + "<tr><td>Return To URL</td><td> : " + "" + "</td></tr>"
					  + "<tr><td>User ID</td><td> : " + "" + "</td></tr>"
					  + "<tr><td>Password</td><td> : " + "" + "</td></tr>"
					  + "</table>";

			if ( Tools.NullToString(separator).Length < 1 )
				separator = Environment.NewLine;

			return "Payment Provider : " + BureauName + separator
			     + "Bureau Code : " + BureauCode + separator
			     + "URL : " + separator
			     + "User ID : " + separator
			     + "Password : ";
		}

		public override void LoadData(DBConn dbConn)
		{
			dbConn.SourceInfo = "Provider.LoadData";
		}
	}
}
