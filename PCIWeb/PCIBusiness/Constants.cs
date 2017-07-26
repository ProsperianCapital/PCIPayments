﻿using System;

namespace PCIBusiness
{
	public static class Constants
	{

	//	General stand-alone constants
	//	-----------------------------
		public static DateTime C_NULLDATE()
		{
			return System.Convert.ToDateTime("1799/12/31");
		}
		public static string C_HTMLBREAK()
		{
			return "<br />";
		}
		public static string C_TEXTBREAK()
		{
			return Environment.NewLine; // "\n";
		}
		public static short C_MAXSQLROWS()
		{
			return 100;
		}
		public enum DBColumnStatus : byte
		{
			InvalidColumn = 1,
			EOF           = 2,
			ValueIsNull   = 3,
			ColumnOK      = 4
		}

		public enum PaymentStatus : byte
		{
			WaitingToPay     =   1,
			FailedTryAgain   =  21,
			FailedDoNotRetry =  22,
			BusyProcessing   =  51,
			Successful       = 101
		}

		public enum PaymentProvider : int
		{
			PayU  =  12,
			Ikajo = 203,
			T24   =  34
		}

		public enum CreditCardType : byte
		{
			Visa            = 1,
			MasterCard      = 2,
			AmericanExpress = 3,
			DinersClub      = 4
		}

		public enum PagingMode : byte
		{
			None              = 0,
			AllowScreenPaging = 209,
			DoNotReadNextRow  = 244
		}
	}
}