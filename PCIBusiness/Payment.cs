﻿using System;
using System.Text;

namespace PCIBusiness
{
	public class Payment : BaseData
	{
//		private int      paymentCode;
//		private int      paymentAuditCode;
		private string   merchantReference;
		private string   countryCode;
		private string   email;
		private string   firstName;
		private string   lastName;
		private string   phoneCell;
		private string   regionalId;
		private int      paymentAmount;
		private byte     paymentStatus;
		private string   ccToken;
		private string   paymentDescription;
		private string   currencyCode;
		private string   ccNumber;
		private byte     ccType;
		private string   ccExpiry;
		private string   ccName;
		private string   ccCVV;
		private string   bureauCode;

		private string   safeKey;
		private string   userID;
		private string   password;
		private string   url;

		private Provider    provider;
		private Transaction transaction;


//		Payment Provider stuff
		public string    SafeKey
		{
			get { return  Tools.NullToString(safeKey); }
		}
		public string    UserID
		{
			get { return  Tools.NullToString(userID); }
		}
		public string    Password
		{
			get { return  Tools.NullToString(password); }
		}
		public string    URL
		{
		//	get { return  Tools.NullToString(url); }
			get
			{
				if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
					return "https://www.payu.co.za";
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
					return "";
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
					return "";
				return "";
			}
		}

//		Payment/Customer stuff
		public string    MerchantReference
		{
			get { return  Tools.NullToString(merchantReference); }
		}
//		Renamed to "UserID"
//		public string    MerchantUserId
//		{
//			get { return  Tools.NullToString(merchantUserId); }
//		}
		public string    CountryCode
		{
			get { return  Tools.NullToString(countryCode); }
		}
		public string    CurrencyCode
		{
			get { return  Tools.NullToString(currencyCode); }
		}
		public string    PaymentDescription
		{
			get { return  Tools.NullToString(paymentDescription); }
		}
		public string    FirstName
		{
			get { return  Tools.NullToString(firstName); }
		}
		public string    LastName
		{
			get { return  Tools.NullToString(lastName); }
		}
		public string    EMail
		{
			get { return  Tools.NullToString(email); }
		}
		public string    PhoneCell
		{
			get { return  Tools.NullToString(phoneCell); }
		}
		public string    RegionalId
		{
			get { return  Tools.NullToString(regionalId); }
		}
		public  int      PaymentAmount
		{
			get { return  paymentAmount; }
		}
		public  byte     PaymentStatus
		{
			get { return  paymentStatus; }
		}
		public  string   CardToken
		{
			get { return  Tools.NullToString(ccToken); }
		}
		public  byte     CardType
		{
			get { return  ccType; }
		}
		public  string   CardNumber
		{
			get { return  Tools.NullToString(ccNumber); }
		}
		public  string   CardExpiry
		{
			get { return  Tools.NullToString(ccExpiry); }
		}
		public  string   CardExpiryMonth
		{
			get
			{
				if ( CardExpiry.Length == 6 )
					return ccExpiry.Substring(0,2);
				return "";
			}
		}
		public  string   CardExpiryYYYY // 4 digits
		{
			get
			{
				if ( CardExpiry.Length == 6 )
					return ccExpiry.Substring(2,4);
				return "";
			}
		}
		public  string   CardExpiryYY // 2 digits
		{
			get
			{
				if ( CardExpiry.Length == 6 )
					return ccExpiry.Substring(4,2);
				return "";
			}
		}
		public  string   CardName
		{
			get { return  Tools.NullToString(ccName); }
		}
		public  string   CardCVV
		{
			get { return  Tools.NullToString(ccCVV); }
		}
		public Provider  Provider
		{
			get { return  provider; }
		}

		public int GetToken()
		{
			int ret = 64020;
			sql     = "";
			Tools.LogInfo("Payment.GetToken/10","Start, Merchant Reference = " + merchantReference,10);

			if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
				transaction = new TransactionPayU();
			else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
				transaction = new TransactionIkajo();
			else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
				transaction = new TransactionT24();
			else
				return ret;

			ret = transaction.GetToken(this);
			sql = "exec sp_Upd_CardTokenVault @MerchantReference = "           + Tools.DBString(merchantReference) // nvarchar(20),
				                           + ",@PaymentBureauCode = "           + Tools.DBString(bureauCode)        // char(3),
		                                 + ",@CardTokenisationStatusCode = '" + ( ret == 0 ? "007'" : "001'" )
			                              + ",@PaymentBureauToken = "          + Tools.DBString(transaction.PaymentToken)
			                              + ",@BureauSubmissionSoap = "        + Tools.DBString(transaction.XMLSent,3)
			                              + ",@BureauResultSoap = "            + Tools.DBString(transaction.XMLResult.ToString(),3);
			int k = ExecuteSQLUpdate();
			Tools.LogInfo("Payment.GetToken/20","End, SQL = " + sql + " (Return code " + k.ToString() + ")",10);
			return ret;
		}

		public int ProcessPayment()
		{
			int ret = 37020;
			sql     = "";
			Tools.LogInfo("Payment.ProcessPayment/10","Start, Merchant Reference = " + merchantReference,10);

			if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
				ret = (new TransactionPayU()).ProcessPayment(this);
			else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
				ret = (new TransactionIkajo()).ProcessPayment(this);
			else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
				ret = (new TransactionT24()).ProcessPayment(this);
			else
				return ret;

			ret = transaction.ProcessPayment(this);
			sql = "exec sp_Upd_CardPayment @MerchantReference = " + Tools.DBString(merchantReference) // nvarchar(20),
			                           + ",@TransactionStatusCode = '" + ( ret == 0 ? "007'" : "001'" );
			int k = ExecuteSQLUpdate();
			Tools.LogInfo("Payment.ProcessPayment/20","End, SQL = " + sql + " (Return code " + k.ToString() + ")",10);
			return ret;
		}

/*
		public int GetTokenOLD()
		{
			int ret = 64020;
			sql     = "";
			Tools.LogInfo("Payment.GetToken/10","Start, Merchant Reference=" + merchantReference,10);

			if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
				ret = GetTokenPayU();
//			else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
//				ret = ProcessIkajo();
//			else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
//				ret = ProcessT24();

			if ( sql.Length > 3 )
			{
				sql = "exec sp_Upd_CardTokenVault @MerchantReference = " + Tools.DBString(merchantReference) // nvarchar(20),
					                           + ",@PaymentBureauCode = " + Tools.DBString(bureauCode)        // char(3),
			                                 + ",@CardTokenisationStatusCode = '" + ( ret == 0 ? "007'" : "001'" )
			                                 + sql;
				int k = ExecuteSQLUpdate();
				Tools.LogInfo("Payment.GetToken/20","SQL : " + sql + " (Return code " + k.ToString() + ")",10);
			}

			Tools.LogInfo("Payment.GetToken/30","End, Merchant Reference=" + merchantReference + ", Ret=" + ret.ToString(),10);
			return ret;
		}

		private int GetTokenPayU()
		{
			Tools.LogInfo("Payment.GetTokenPayU","Merchant Reference = " + merchantReference,100);

			using (TransactionPayU transaction = new TransactionPayU())
			{
				int ret = transaction.GetToken(this);

			//	These are SQL parameters that will be used in stored proc "sp_Upd_CardTokenVault"

				sql = ",@PaymentBureauToken = "   + Tools.DBString(transaction.PaymentToken)
				    + ",@BureauSubmissionSoap = " + Tools.DBString(transaction.XMLSent,3)
				    + ",@BureauResultSoap = "     + Tools.DBString(transaction.XMLResult.ToString(),3);

				return ret;
			}
		}
*/
		public override void LoadData(DBConn dbConn)
		{
			dbConn.SourceInfo = "Payment.LoadData";

		//	PK's test
		//	paymentCode       = dbConn.ColLong  ("PaymentCode");
		//	paymentAuditCode  = dbConn.ColLong  ("PaymentAuditCode",0);
		//	paymentAmount     = dbConn.ColLong  ("PaymentAmount");
		//	paymentStatus     = dbConn.ColByte  ("PaymentStatus");
		//	paymentToken      = dbConn.ColString("PaymentToken");
		//	ccNumber          = dbConn.ColString("CreditCardNumber");
		//	ccType            = dbConn.ColByte  ("CreditCardType");
		//	ccExpiry          = dbConn.ColString("CreditCardExpiry");
		//	ccName            = dbConn.ColString("CreditCardName");
		//	ccCVV             = dbConn.ColString("CreditCardCVV");
		//	providerCode      = dbConn.ColLong  ("ProviderCode");

		//	Payment Provider
			safeKey            = dbConn.ColString("Safekey");
		//	url                = dbConn.ColString("URL");
			userID             = dbConn.ColString("MerchantUserId");
			password           = dbConn.ColString("MerchantUserPassword");
		//	merchantUserId     = dbConn.ColString("MerchantUserId");

		//	Payment/Customer
			countryCode        = dbConn.ColString("CountryCode");
			email              = dbConn.ColString("email");
			firstName          = dbConn.ColString("firstName");
			lastName           = dbConn.ColString("lastName");
			phoneCell          = dbConn.ColString("mobile");
			regionalId         = dbConn.ColString("regionalId");
			merchantReference  = dbConn.ColString("merchantReference");
			paymentAmount      = dbConn.ColLong  ("amountInCents");
			currencyCode       = dbConn.ColString("currencyCode");
			paymentDescription = dbConn.ColString("description");

		//	Card/token details, not always present, don't log errors
			ccName             = dbConn.ColString("nameOnCard",0);
			ccNumber           = dbConn.ColString("cardNumber",0);
			ccExpiry           = dbConn.ColString("cardExpiry",0);
			ccCVV              = dbConn.ColString("cvv",0);
			ccToken            = dbConn.ColString("token",0);
		}

		public override void CleanUp()
		{
			provider    = null;
			transaction = null;
		}

		public Payment(string bureau)
		{
			bureauCode = Tools.NullToString(bureau);
		//	Load provider info here ... it will be passed to the Transaction
		//	if ( bureauCode.Length > 0 )
		//		provider = (new Providers()).LoadOne(bureauCode);
		}
	}
}
