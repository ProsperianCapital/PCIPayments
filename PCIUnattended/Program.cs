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
				PCIBusiness.Tools.LogInfo("PCIUnattended.Main/5","Started ...",210);
				
				int    k;
				int    rows     = 1;
				byte   mode     = 0;
				string provider = "";
				string argValue;

				for ( k = 0 ; k < arg.Length ; k++ )
				{
					argValue = arg[k].ToUpper().Trim();
					PCIBusiness.Tools.LogInfo("PCIUnattended.Main/10","Arg[" + k.ToString() + "] = " + argValue,210);

					if ( argValue.StartsWith("MODE=") )
						mode = System.Convert.ToByte(argValue.Substring(5));
					else if ( argValue.StartsWith("ROWS=") )
						rows = System.Convert.ToInt32(argValue.Substring(5));
					else if ( argValue.StartsWith("PROVIDER=") )
						provider = argValue.Substring(9);
				}

				using (PCIBusiness.Payments payments = new PCIBusiness.Payments())
				{
					PCIBusiness.Tools.LogInfo("PCIUnattended.Main/15","Processing ...",210);
					k = payments.ProcessCards(provider,mode,rows);
					PCIBusiness.Tools.LogInfo("PCIUnattended.Main/20","Finished, k="+k.ToString() + ", " + (payments.CountSucceeded+payments.CountFailed).ToString() + " payment(s) completed : " + payments.CountSucceeded.ToString() + " succeeded, " + payments.CountFailed.ToString() + " failed",210);
				}
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("PCIUnattended.Main/25","",ex);
			}
		}
	}
}
