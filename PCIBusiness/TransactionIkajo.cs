using System;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class TransactionIkajo : Transaction
	{
		static string url      = "https://staging.payu.co.za";
		static string userID   = "Staging Enterprise Integration Store 1";
		static string password = "j3w8swi5";

//		public override string ConnectionDetails(byte mode,string separator="")
//		{
//			if ( mode == 1 ) // HTML
//				return "<table>"
//					  + "<tr><td>Payment Provider</td><td class='Red'> : Ikajo</td></tr>"
//					  + "<tr><td>Status</td><td class='Red'> : In development</td></tr>"
//					  + "<tr><td colspan='2'><hr /></td></tr>"
//					  + "<tr><td>Bureau Code</td><td> : " + bureauCode + "</td></tr>"
//					  + "<tr><td>URL</td><td> : " + url + "</td></tr>"
//					  + "<tr><td>User ID</td><td> : " + userID + "</td></tr>"
//					  + "<tr><td>Password</td><td> : " + password + "</td></tr>"
//					  + "</table>";
//
//			if ( Tools.NullToString(separator).Length < 1 )
//				separator = Environment.NewLine;
//
//			return "Payment Provider : Ikajo" + separator
//			     + "Bureau Code : " + bureauCode + separator
//			     + "URL : " + url + separator
//			     + "User ID : " + userID + separator
//			     + "Password : " + password;
//		}

//		public  string  BureauStatus
//		{
//			get { return "Development"; }
//		}
//		public  string  URL
//		{
//			get { return ""; }
//		}

		public override int ProcessPayment(Payment payment)
		{
			return 0;
		}

		public override int GetToken(Payment payment)
		{
			return 0;
		}

		public TransactionIkajo() : base()
		{
			bureauCode = Tools.BureauCode(Constants.PaymentProvider.Ikajo);
		}
	}
}
