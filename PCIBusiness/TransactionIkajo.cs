﻿using System;
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

		public override string ConnectionDetails(byte mode,string separator="")
		{
			if ( mode == 1 ) // HTML
				return "<table>"
					  + "<tr><td>Payment Provider</td><td style='color:red'> : Ikajo</td></tr>"
					  + "<tr><td>Status</td><td style='color:red'> : In development</td></tr>"
					  + "<tr><td colspan='2'><hr /></td></tr>"
					  + "<tr><td>Bureau Code</td><td> : " + PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.Ikajo) + "</td></tr>"
					  + "<tr><td>URL</td><td> : " + url + "</td></tr>"
					  + "<tr><td>User ID</td><td> : " + userID + "</td></tr>"
					  + "<tr><td>Password</td><td> : " + password + "</td></tr>"
					  + "</table>";

			if ( Tools.NullToString(separator).Length < 1 )
				separator = Environment.NewLine;

			return "Payment Provider : Ikajo" + separator
			     + "Bureau Code : " + PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.Ikajo) + separator
			     + "URL : " + url + separator
			     + "User ID : " + userID + separator
			     + "Password : " + password;
		}


		public int Process(Payment payment)
		{
			return 0;
		}
	}
}
