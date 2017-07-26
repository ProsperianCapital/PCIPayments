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

		public int Process(Payment payment)
		{
			return 0;
		}
	}
}
