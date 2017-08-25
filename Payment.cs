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
		private string   firstName;
		private string   lastName;
		private string   address1;
		private string   address2;
		private string   postalCode;
		private string   provinceCode;
		private string   regionalId;
		private string   email;
		private string   phoneCell;
		private int      paymentAmount;
//		private byte     paymentStatus;
		private string   ccToken;
		private string   paymentDescription;
		private string   currencyCode;
		private string   ccNumber;
		private string   ccType;
		private string   ccExpiryMonth;
		private string   ccExpiryYear;
		private string   ccName;
		private string   ccCVV;
		private string   bureauCode;

//		private string   providerAccount;
		private string   providerKey;
		private string   providerUserID;
		private string   providerPassword;
		private string   providerURL;

		private Provider    provider;
		private Transaction transaction;


//		Payment Provider stuff
		public string    ProviderKey
		{
			get { return  Tools.NullToString(providerKey); }
		}
//		public string    ProviderAccount
//		{
//			get 
//			{
//				if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
//					return "";
//				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
//					return "567654452";
//				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
//					return "";
//				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.MyGate) )
//					return "MY014473";
//				return "";
//			}
//		}
		public string    ProviderUserID
		{
			get { return  Tools.NullToString(providerUserID); }
		}
		public string    ProviderPassword
		{
			get { return  Tools.NullToString(providerPassword); }
		}
		public string    ProviderURL
		{
			get { return  Tools.NullToString(providerURL); }

//		Testing ...

//			get { return  "https://payment.ccp.boarding.transact24.com/PaymentCard";           } // T24
//			get { return  "https://www.mygate.co.za/Collections/1x0x0/pinManagement.cfc?wsdl"; } MyGate
		}

//		public string    MerchantUserId
//		{
//			get { return  Tools.NullToString(merchantUserId); }
//		}
//		public string    Address(byte line)
//		{
//			if ( address == null || line < 1 || ( line > address.Length && line < 255 ) )
//				return "";
//			if ( line == 255 ) // Last non-blank address
//			{
//				for ( int k = address.Length ; k > 0 ; k-- )
//					if ( address[k-1].Length > 0 )
//						return address[k-1];
//				return "";
//			}
//			return address[line-1];
////			while ( line < address.Length )
////			{
////				if ( address[line].Length > 0 )
////					return address[line];
////				line++;
////			}
////			return "";
//		}

//		Customer stuff
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
		public string    Address1
		{
			get { return  Tools.NullToString(address1); }
		}
		public string    Address2
		{
			get { return  Tools.NullToString(address2); }
		}
		public string    PostalCode
		{
			get { return  Tools.NullToString(postalCode); }
		}
		public string    ProvinceCode
		{
			get { return  Tools.NullToString(provinceCode); }
		}
		public string    State // Province, but only if USA
		{
			get
			{
				string x = Tools.NullToString(countryCode).ToUpper();
				if ( x.Length > 1 && x.StartsWith("US") )
					return ProvinceCode;
				return "";
			}
		}
		public string    CountryCode
		{
			get { return  Tools.NullToString(countryCode); }
		}

//		payment stuff
		public string    MerchantReference
		{
			get { return  Tools.NullToString(merchantReference); }
		}
		public string    CurrencyCode
		{
			get { return  Tools.NullToString(currencyCode); }
		}
		public string    PaymentDescription
		{
			get { return  Tools.NullToString(paymentDescription); }
		}
		public  int      PaymentAmount
		{
//			get { return (paymentAmount > 0 ? paymentAmount : 0); }
			get { return  paymentAmount; }
		}
//		public  byte     PaymentStatus
//		{
//			get { return  paymentStatus; }
//		}

//		Card stuff
		public  string   CardToken
		{
			get { return  Tools.NullToString(ccToken); }
		}
		public  string   CardType
		{
			get { return  ccType; }
		}
		public  string   CardNumber
		{
			get { return  Tools.NullToString(ccNumber); }
		}
		public  string   CardExpiryMM
		{
			get { return  Tools.NullToString(ccExpiryMonth).PadLeft(2,'0'); }
		}
		public  string   CardExpiryYY
		{
			get
			{
				ccExpiryYear = Tools.NullToString(ccExpiryYear);
				if ( ccExpiryYear.Length == 4 )
					return ccExpiryYear.Substring(2,2);
				if ( ccExpiryYear.Length == 2 )
					return ccExpiryYear;
				return "";
			}
		}
		public  string   CardExpiryYYYY
		{
			get
			{
				ccExpiryYear = Tools.NullToString(ccExpiryYear);
				if ( ccExpiryYear.Length == 4 )
					return ccExpiryYear;
				if ( ccExpiryYear.Length == 2 )
					return "20" + ccExpiryYear;
				return "";
			}
		}


//		public  string   CardExpiry
//		{
//			get { return  Tools.NullToString(ccExpiry); }
//		}
//		public  string   CardExpiryMonth
//		{
//			get
//			{
//				if ( CardExpiry.Length >= 4 )
//					return ccExpiry.Substring(0,2);
//				return "";
//			}
//		}
//		public  string   CardExpiryYYYY // 4 digits
//		{
//			get
//			{
//				if ( CardExpiry.Length == 6 )
//					return ccExpiry.Substring(2,4);
//				return "";
//			}
//		}
//		public  string   CardExpiryYY // 2 digits
//		{
//			get
//			{
//				if ( CardExpiry.Length == 6 )
//					return ccExpiry.Substring(4,2);
//				if ( CardExpiry.Length == 4 )
//					return ccExpiry.Substring(2,2);
//				return "";
//			}
//		}
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
			Tools.LogInfo("Payment.GetToken/10","Merchant Ref=" + merchantReference,10);

			if ( transaction == null || transaction.BureauCode != bureauCode )
			{
				if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
					transaction = new TransactionPayU();
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
					transaction = new TransactionIkajo();
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
					transaction = new TransactionT24();
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.MyGate) )
					transaction = new TransactionMyGate();
				else
					return ret;
			}
			ret = transaction.GetToken(this);
			sql = "exec sp_Upd_CardTokenVault @MerchantReference = "           + Tools.DBString(merchantReference) // nvarchar(20),
				                           + ",@PaymentBureauCode = "           + Tools.DBString(bureauCode)        // char(3),
			                              + ",@PaymentBureauToken = "          + Tools.DBString(transaction.PaymentToken)
			                              + ",@BureauSubmissionSoap = "        + Tools.DBString(transaction.XMLSent,3)
			                              + ",@BureauResultSoap = "            + Tools.DBString(transaction.XMLResult,3)
			                              + ",@TransactionStatusCode = "       + Tools.DBString(transaction.ResultCode)
		                                 + ",@CardTokenisationStatusCode = '" + ( ret == 0 ? "007'" : "001'" );
			Tools.LogInfo("Payment.GetToken/20","SQL=" + sql,30);
			int k = ExecuteSQLUpdate();
			Tools.LogInfo("Payment.GetToken/90","Ret=" + ret.ToString(),30);
			return ret;
		}

		public int ProcessPayment()
		{
			int ret = 37020;
			int k;
			Tools.LogInfo("Payment.ProcessPayment/10","Merchant Ref=" + merchantReference,10);

			if ( transaction == null || transaction.BureauCode != bureauCode )
			{
				if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.PayU) )
					transaction = new TransactionPayU();
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.Ikajo) )
					transaction = new TransactionIkajo();
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.T24) )
					transaction = new TransactionT24();
				else if ( bureauCode == Tools.BureauCode(Constants.PaymentProvider.MyGate) )
					transaction = new TransactionMyGate();
				else
					return ret;
			}
			sql = "exec sp_Upd_CardPayment @MerchantReference = " + Tools.DBString(merchantReference)
			                           + ",@TransactionStatusCode = '77'";
			Tools.LogInfo("Payment.ProcessPayment/20","SQL 1=" + sql,199);
			k   = ExecuteSQLUpdate();
			Tools.LogInfo("Payment.ProcessPayment/30","SQL 1 complete",199);
			ret = transaction.ProcessPayment(this);
			sql = "exec sp_Upd_CardPayment @MerchantReference = " + Tools.DBString(merchantReference)
			                           + ",@TransactionStatusCode = " + Tools.DBString(transaction.ResultCode);
			Tools.LogInfo("Payment.ProcessPayment/40","SQL 2=" + sql,199);
			k   = ExecuteSQLUpdate();
			Tools.LogInfo("Payment.ProcessPayment/50","SQL 2 complete",199);
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
			dbConn.SourceInfo  = "Payment.LoadData";

		//	Payment Provider
			providerKey        = dbConn.ColString("Safekey");
			providerURL        = dbConn.ColString("url");
			providerUserID     = dbConn.ColString("MerchantUserId"); // Also used as merchant account
			providerPassword   = dbConn.ColString("MerchantUserPassword");

		//	Customer
			firstName          = dbConn.ColString("firstName");
			lastName           = dbConn.ColString("lastName");
			email              = dbConn.ColString("email");
			phoneCell          = dbConn.ColString("mobile");
			regionalId         = dbConn.ColString("regionalId",0);
			address1           = dbConn.ColString("address1",0);
			address2           = dbConn.ColString("city",0);
			postalCode         = dbConn.ColString("zip_code",0);
			provinceCode       = dbConn.ColString("State",0);
			countryCode        = dbConn.ColString("CountryCode");

		//	Payment
			merchantReference  = dbConn.ColString("merchantReference");
			paymentAmount      = dbConn.ColLong  ("amountInCents");
			currencyCode       = dbConn.ColString("currencyCode");
			paymentDescription = dbConn.ColString("description");

		//	Card/token details, not always present, don't log errors
			ccName             = dbConn.ColString("nameOnCard",0);
			ccNumber           = dbConn.ColString("cardNumber",0);
			ccExpiryMonth      = dbConn.ColString("cardExpiryMonth",0);
			ccExpiryYear       = dbConn.ColString("cardExpiryYear",0);
			ccType             = dbConn.ColString("cardType",0);
			ccCVV              = dbConn.ColString("cvv",0);
			ccToken            = dbConn.ColString("token",0);
		}

		public override void CleanUp()
		{
			provider    = null;
			transaction = null;
		}

		public Payment(string bureau) : base()
		{
			bureauCode = Tools.NullToString(bureau);
		//	Load provider info here ... it will be passed to the Transaction
		//	if ( bureauCode.Length > 0 )
		//		provider = (new Providers()).LoadOne(bureauCode);
		}
	}
}