using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCIBusiness
{
	public abstract class Transaction : StdDisposable
	{
		protected string payRef;
		protected string payToken;
		protected string resultCode;
		protected string resultMsg;

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

      public override void Close()
		{ }

		public Transaction()
		{
			payRef     = "";
			payToken   = "";
			resultCode = "";
			resultMsg  = "";
		}
	}
}
