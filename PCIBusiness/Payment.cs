using System;
using System.Text;

namespace PCIBusiness
{
	public class Payment : BaseData
	{
//		private int     paymentCode;
//		private int     paymentAuditCode;
		private string  merchantReference;
		private string  safeKey;
		private string  merchantUserId;
		private string  countryCode;
		private string  email;
		private string  firstName;
		private string  lastName;
		private string  phoneCell;
		private string  regionalId;
		private int     paymentAmount;
		private byte    paymentStatus;
		private string  paymentToken;
		private string  paymentDescription;
		private string  currencyCode;
		private string  ccNumber;
		private byte    ccType;
		private string  ccExpiry;
		private string  ccName;
		private string  ccCVV;
		private string  bureauCode;
//		private int     providerCode;

//		public  int     PaymentCode
//		{
//			get { return paymentCode; }
//		}
//		public  int     PaymentAuditCode
//		{
//			get { return paymentAuditCode; }
//		}
		public string   MerchantReference
		{
			get { return Tools.NullToString(merchantReference); }
		}
		public string   MerchantUserId
		{
			get { return Tools.NullToString(merchantUserId); }
		}
		public string   CountryCode
		{
			get { return Tools.NullToString(countryCode); }
		}
		public string   CurrencyCode
		{
			get { return Tools.NullToString(currencyCode); }
		}
		public string   PaymentDescription
		{
			get { return Tools.NullToString(paymentDescription); }
		}
		public string   SafeKey
		{
			get { return Tools.NullToString(safeKey); }
		}
		public string   FirstName
		{
			get { return Tools.NullToString(firstName); }
		}
		public string   LastName
		{
			get { return Tools.NullToString(lastName); }
		}
		public string   EMail
		{
			get { return Tools.NullToString(email); }
		}
		public string   PhoneCell
		{
			get { return Tools.NullToString(phoneCell); }
		}
		public string   RegionalId
		{
			get { return Tools.NullToString(regionalId); }
		}
		public  int     PaymentAmount
		{
			get { return paymentAmount; }
		}
		public  byte    PaymentStatus
		{
			get { return paymentStatus; }
		}
		public  string  PaymentToken
		{
			get { return Tools.NullToString(paymentToken); }
		}
		public  byte    CardType
		{
			get { return ccType; }
		}
		public  string  CardNumber
		{
			get { return Tools.NullToString(ccNumber); }
		}
		public  string  CardExpiry
		{
			get { return Tools.NullToString(ccExpiry); }
		}
		public  string  CardExpiryMonth
		{
			get
			{
				if ( CardExpiry.Length == 6 )
					return ccExpiry.Substring(0,2);
				return "";
			}
		}
		public  string  CardExpiryYYYY // 4 digits
		{
			get
			{
				if ( CardExpiry.Length == 6 )
					return ccExpiry.Substring(2,4);
				return "";
			}
		}
		public  string  CardExpiryYY // 2 digits
		{
			get
			{
				if ( CardExpiry.Length == 6 )
					return ccExpiry.Substring(4,2);
				return "";
			}
		}
		public  string  CardName
		{
			get { return Tools.NullToString(ccName); }
		}
		public  string  CardCVV
		{
			get { return Tools.NullToString(ccCVV); }
		}

		public int Process()
		{
//			if ( paymentAuditCode < 1 || paymentCode < 1 )
//			{
//				Tools.LogException("Payment.Process/1","Invalid payment code and/or audit code");
//				return 64010;
//			}

			int ret = 64020;
			sql = "";
			Tools.LogInfo("Payment.Process/2","Start processing MerchantReference=" + merchantReference,10);

			if ( System.Convert.ToInt32(bureauCode) == (int)Constants.PaymentProvider.PayU )
				ret = ProcessPayU();
			else if ( System.Convert.ToInt32(bureauCode) == (int)Constants.PaymentProvider.Ikajo )
				ret = ProcessIkajo();
			else if ( System.Convert.ToInt32(bureauCode) == (int)Constants.PaymentProvider.T24 )
				ret = ProcessT24();

//			if ( sql.Length > 3 )
//			{
//				sql = "exec PaymentUpdate " + paymentAuditCode.ToString()
//			                         + "," + paymentCode.ToString()
//											 + "," + sql;
//				ExecuteSQLUpdate();
//			}

			Tools.LogInfo("Payment.Process/3","End processing PaymentCode=" + merchantReference + ", Ret=" + ret.ToString(),10);
			return ret;
		}

		private int ProcessT24()
		{
			TransactionT24 transaction = new TransactionT24();
			int            ret         = transaction.Process(this);
		//	Blah
		//	Blah
			transaction = null;
			return 0;
		}

		private int ProcessIkajo()
		{
			TransactionIkajo transaction = new TransactionIkajo();
			int              ret         = transaction.Process(this);
		//	Blah
		//	Blah
			transaction = null;
			return 0;
		}

		private int ProcessPayU()
		{
			TransactionPayU transaction = new TransactionPayU();
			int             ret         = transaction.Process(this);


//			sp_Upd_CardTokenVault
	

		//	These are SQL parameters that will be used in stored proc "sp_Upd_CardTokenVault"
			sql = (byte)( transaction.Successful ? Constants.PaymentStatus.Successful : Constants.PaymentStatus.FailedTryAgain )
			    + "," + Tools.DBString(transaction.PaymentReference)
			    + "," + Tools.DBString(transaction.PaymentToken)
			    + "," + Tools.DBString(transaction.ResultCode)
			    + "," + Tools.DBString(transaction.ResultMessage);
			transaction = null;
			return ret;
		}

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

		//	Prosperian
			safeKey            = dbConn.ColString("Safekey");
			merchantUserId     = dbConn.ColString("MerchantUserId");
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
			ccName             = dbConn.ColString("nameOnCard");
		//	ccType             = dbConn.ColByte  ("x");
			ccNumber           = dbConn.ColString("cardNumber");
			ccExpiry           = dbConn.ColString("cardExpiry");
			ccCVV              = dbConn.ColString("cvv");
		}

		public Payment(string bureau)
		{
			bureauCode = Tools.NullToString(bureau);
		}
	}
}
