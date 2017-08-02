using System;
using System.Xml;


namespace PCIBusiness
{
	public abstract class Transaction : StdDisposable
	{
		protected string payRef;
		protected string payToken;
		protected string resultCode;
		protected string resultMsg;
		protected string xmlSent;
//		protected string xmlReceived;
		protected string bureauCode;
		protected XmlDocument xmlResult;

		public  string  PaymentReference
		{
			get { return Tools.NullToString(payRef); }
		}
		public  string  PaymentToken
		{
			get { return Tools.NullToString(payToken); }
		}
		public  string  ResultCode
		{
			get { return Tools.NullToString(resultCode); }
		}
		public  string  ResultMessage
		{
			get { return Tools.NullToString(resultMsg); }
		}
		public  string      XMLSent
		{
			get { return     Tools.NullToString(xmlSent); }
		}
		public  XmlDocument XMLResult
		{
			get { return     xmlResult; }
		}
		public  string      BureauCode
		{
			get { return     Tools.NullToString(bureauCode); }
		}

      public abstract string ConnectionDetails(byte mode,string separator="");

      public override void Close()
		{
			xmlResult = null;
		}

		public Transaction()
		{
			payRef      = "";
			payToken    = "";
			resultCode  = "";
			resultMsg   = "";
			xmlSent     = "";
			bureauCode  = "";
		}
	}
}
