using System;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class TransactionMyGate : Transaction
	{
		private int SendXML(string url,string userID,string password)
		{
			int    ret         = 10;
			string xmlReceived = "";
			payRef             = "";

			try
			{
				Tools.LogInfo("TransactionMyGate.SendXML/10","XML Sent=" + xmlSent,30);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionMyGate.SendXML/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionMyGate.SendXML/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}

		public override int GetToken(Payment payment)
		{
			int    ret         = 300;
			string xmlReceived = "";
			xmlSent = "";

			try
			{
				Tools.LogInfo("TransactionMyGate.GetToken/10","Merchant Ref=" + payment.MerchantReference,30);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionMyGate.GetToken/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionMyGate.GetToken/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}

		public override int ProcessPayment(Payment payment)
		{
			int ret = 600;
			xmlSent = "";

			try
			{
				Tools.LogInfo("TransactionMyGate.ProcessPayment/20","XML Sent=" + xmlSent,30);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionMyGate.ProcessPayment/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionMyGate.ProcessPayment/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}
	}
}
