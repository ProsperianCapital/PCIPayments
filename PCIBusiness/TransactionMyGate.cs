using System;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class TransactionMyGate : Transaction
	{
		public override int ProcessPayment(Payment payment)
		{
			return 0;
		}

		public override int GetToken(Payment payment)
		{
			return 0;
		}

		public TransactionMyGate() : base()
		{
			bureauCode = Tools.BureauCode(Constants.PaymentProvider.MyGate);
		}
	}
}
