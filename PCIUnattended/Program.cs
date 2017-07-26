using System;
using System.Collections.Generic;
using System.Text;

namespace PCIUnattended
{
	class Program
	{
		static void Main(string[] arg)
		{
			try
			{
				PCIBusiness.Tools.LogInfo("PCIUnattended.Main/5","Started ...");

				using (PCIBusiness.PaymentSchedule paymentSchedule = new PCIBusiness.PaymentSchedule())
				{
					paymentSchedule.ProcessCards("1");
					paymentSchedule.ProcessTokens("1");
				}
				
//				int    k;
//				string argValue;
//
//				for ( k = 0 ; k < arg.Length ; k++ )
//				{
//					argValue = arg[k].ToUpper().Trim();
//					PCIBusiness.Tools.LogInfo("PCIUnattended.Main/10","Arg[" + k.ToString() + "] = " + argValue);
//
//					if ( argValue.Contains("PAYMENT") )
//						using (PCIBusiness.PaymentSchedule paymentSchedule = new PCIBusiness.PaymentSchedule())
//						{
//							paymentSchedule.ProcessCards();
//							paymentSchedule.ProcessTokens();
//						}
//				}

				PCIBusiness.Tools.LogInfo("PCIUnattended.Main/15","Finished ...");
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("PCIUnattended.Main/20","",ex);
			}
		}
	}
}
