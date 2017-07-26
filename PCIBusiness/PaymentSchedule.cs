using System;
using System.Text;

namespace PCIBusiness
{
	public class PaymentSchedule : BaseList
	{
		private string bureauCode;

		public override BaseData NewItem()
		{
			return (BaseData)(new Object());
		}

		public int ProcessCards(string bureau)
		{
			bureauCode = Tools.NullToString(bureau);
			Tools.LogInfo("PaymentSchedule.ProcessCards","BureauCode="+bureauCode,100);
//			sql        = "exec sp_Get_CardToToken " + Tools.DBString(bureauCode);

			sql = "select '{542595FF-78EC-4A42-996D-18F8790393E5}' as Safekey,"
			    +        "'1351079862'          as MerchantUserId,"
			    +        "'27'                  as CountryCode,"
			    +        "'PeteSmith@yahoo.com' as email,"
			    +        "'Pete'                as firstName,"
			    +        "'Smith'               as lastName,"
			    +        "'084 333 7777'        as mobile,"
			    +        "'2345679111'          as regionalId,"
			    +        "'3451'                as merchantReference,"
			    +        "10599                 as amountInCents,"
			    +        "'ZAR'                 as currencyCode,"
			    +        "'RTR 3451'            as description,"
			    +        "'Pete Smith'          as nameOnCard,"
			    +        "'4901222233334444'    as cardNumber,"
			    +        "'082020'              as cardExpiry,"
			    +        "'123'                 as cvv";
			return ProcessPayments();
		}

		public int ProcessTokens(string bureau)
		{
			bureauCode = Tools.NullToString(bureau);
			Tools.LogInfo("PaymentSchedule.ProcessTokens","BureauCode="+bureauCode,100);
			sql        = "exec sp_Get_CardToToken " + Tools.DBString(bureauCode);
			return ProcessPayments();
		}

		private int ProcessPayments()
		{
			int k = 0;

			Tools.LogInfo("PaymentSchedule.ProcessPayments/5","Started ... " + Tools.NullToString(sql),100);

			try
			{
				int err = ExecuteSQL(null,false,false);
				if ( err > 0 )
					Tools.LogException("PaymentSchedule.ProcessPayments/10",sql + " failed, return code " + err.ToString());
				else
					using (PCIBusiness.Payment payment = new PCIBusiness.Payment(bureauCode))
					{
						while ( ! dbConn.EOF )
						{
							k++;
							payment.LoadData(dbConn);
							err = payment.Process();
							dbConn.NextRow();
						}
//						sql = "exec PaymentScheduleEnd " + payment.PaymentAuditCode.ToString() + ",0,0";
//						Tools.LogInfo("PaymentSchedule.ProcessPayments/15","Ended (" + sql +")",100);
//						err = ExecuteSQL(null,true,false);
//						if ( err > 0 )
//							Tools.LogException("PaymentSchedule.ProcessPayments/20",sql + " failed, return code " + err.ToString());
					}
			}
			catch (Exception ex)
			{
				Tools.LogException("PaymentSchedule.ProcessPayments/25","Payment " + k.ToString(),ex);
			}
			finally
			{
				Tools.CloseDB(ref dbConn);
				Tools.LogInfo("PaymentSchedule.ProcessPayments/30","Finished (" + k.ToString() + " payment(s) processed)",100);
			}
			return k;
		}
	}
}
