using System;
using System.Configuration;

namespace TOMBusiness
{
	public static class Configuration
	{
/*
		private static byte paymentGatewayCode;
		public  static byte PaymentGatewayCode
		{
			get
			{
				if ( paymentGatewayCode == 0 )
					paymentGatewayCode = (byte)Constants.PaymentGateway.NedbankIveri;
				return paymentGatewayCode;
			}
			set 
			{
				foreach ( byte pgCode in Enum.GetValues(typeof(TOMBusiness.Constants.PaymentGateway)) )
					if ( pgCode == value )
					{
						paymentGatewayCode = value;
						break;
					}
			}
		}
*/
		private static byte chipType;
		public  static byte ChipType
		{
			get
			{
				if ( chipType == 0 )
					chipType = (byte)Constants.ChipType.RaceTec;
				return chipType;
			}
			set 
			{
				foreach ( byte pgCode in Enum.GetValues(typeof(TOMBusiness.Constants.ChipType)) )
					if ( pgCode == value )
					{
						chipType = value;
						break;
					}
			}
		}
	}
}