using System;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class ThreeDSecure : BaseData
	{
		private WebClient client3d;
		private int       amount;      // No cents, SA Rands only
		private int       paymentCode; // Merchant reference, our primary key from "Payment" table
		private short     cardMM;      // 1-12
		private short     cardYY;      // 4 digits, eg. 2019
		private string    cardNumber; 

		private string    payLoad;
		private string    transactionId;
		private string    acsURL;
		private string    errNo;
		private string    errDesc;
		private string    enrolled;
		private string    cavv;
		private string    xid;
		private string    eciFlag;
		private string    parStatus;
		private string    sigVerify;

		private string    xml3d;
		private string    url3d;

		public  int    Amount
		{
			get { return amount; }
			set { amount = ( value > 0 ? value : 0 ); }
		}
		public  int    PaymentCode
		{
			get { return paymentCode; }
			set { paymentCode = value; }
		}
		public  string CardNumber
		{
			set { cardNumber = value.Trim(); }
		}
		public  short  CardMonth
		{
			set { cardMM = ( value > 0 && value < 13 ? value : (short)0 ); }
		}
		public  short  CardYear
		{
			set { cardYY = ( value > 2010 && value < 2999 ? value : (short)0 ); }
		}

//		BankServe stuff ...
		public  string PayLoad
		{
			get { return Tools.NullToString(payLoad); }
			set { payLoad = value; }
		}
		public  string TransactionId
		{
			get { return Tools.NullToString(transactionId); }
			set { transactionId = value; }
		}
		public  string RedirectURL
		{
			get { return Tools.NullToString(acsURL); }
		}
		public  string Enrolled
		{
			get { return Tools.NullToString(enrolled).ToUpper(); }
		}
		public  string ErrorNo
		{
			get { return Tools.NullToString(errNo); }
		}
		public  string Cavv
		{
			get { return Tools.NullToString(cavv); }
			set { cavv = value; }
		}
		public  string Xid
		{
			get { return Tools.NullToString(xid); }
			set { xid = value; }
		}
		public  string EciFlag
		{
			get { return Tools.NullToString(eciFlag); }
			set { eciFlag = value; }
		}
		public  string ParStatus
		{
			get { return Tools.NullToString(parStatus); }
			set { parStatus = value; }
		}
		public  string SignatureVerification
		{
			get { return Tools.NullToString(sigVerify); }
			set { sigVerify = value; }
		}

		private System.Xml.XmlDocument Send3dMessage(string xmlMsg)
		{
			if ( client3d == null ) client3d = new System.Net.WebClient();
			returnMessage = "Internal error checking this card's 3d Secure status";
			returnCode    = 110;

			try
			{
				client3d.QueryString.Add("cmpi_msg",xmlMsg);
				returnCode = 120;
				System.IO.Stream       data     = client3d.OpenRead(url3d);
				returnCode = 130;
				System.IO.StreamReader reader   = new System.IO.StreamReader(data);
				returnCode = 140;
				string                 pageData = reader.ReadToEnd().Trim();
				returnCode = 150;
				try
				{
					reader.Close();
					returnCode = 155;
					data.Close();
				}
				finally
				{
					reader  = null;
				}
				returnCode = 160;
				System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
				returnCode = 170;
				xml.LoadXml(pageData);
				returnCode = 180;
				errNo      = xml.DocumentElement.SelectSingleNode("/CardinalMPI/ErrorNo").InnerText;
				errDesc    = xml.DocumentElement.SelectSingleNode("/CardinalMPI/ErrorDesc").InnerText;
				returnCode = 0;
				return xml;
			}
			catch (Exception ex)
			{
				Tools.LogException("ThreeDSecure.Send3dMessage","ReturnCode=" + returnCode.ToString() + ", url3d="+url3d,ex);
			}
			return null;
		}

		public int CardLookup()
		{
			try
			{
				returnCode    = 310;
				string xmlMsg = "<CardinalMPI>"
				              + "<MsgType>cmpi_lookup</MsgType>"
				              + xml3d
				              + "<TransactionType>C</TransactionType>"
				              + "<Amount>" + amount.ToString() + "00</Amount>" // Our amounts are SA RAND only (no cents)
				              + "<CurrencyCode>710</CurrencyCode>"             // The code for "SA Rand" = 710 (US$ = 840)
				              + "<OrderNumber>" + paymentCode.ToString() + "</OrderNumber>"
				              + "<CardNumber>" + cardNumber.Replace(" ","") + "</CardNumber>"
				              + "<CardExpMonth>" + ( cardMM < 10 ? "0" : "" ) + cardMM.ToString() + "</CardExpMonth>"
				              + "<CardExpYear>" + cardYY.ToString() + "</CardExpYear>"
				              + "</CardinalMPI>";

				returnCode                 = 320;
				System.Xml.XmlDocument xml = Send3dMessage(xmlMsg);

				if ( returnCode == 0 && xml != null && errNo == "0" && errDesc.Length == 0 )
				{
					returnCode    = 330;
					returnMessage = "";
					enrolled      = xml.DocumentElement.SelectSingleNode("/CardinalMPI/Enrolled").InnerText.ToUpper(); // 3d-enabled (Y/N/U)
					payLoad       = xml.DocumentElement.SelectSingleNode("/CardinalMPI/Payload").InnerText;
					transactionId = xml.DocumentElement.SelectSingleNode("/CardinalMPI/TransactionId").InnerText;
					acsURL        = xml.DocumentElement.SelectSingleNode("/CardinalMPI/ACSUrl").InnerText;
					eciFlag       = xml.DocumentElement.SelectSingleNode("/CardinalMPI/EciFlag").InnerText;
					returnCode    = 340;

					if ( enrolled == "Y" ) // 3d-enabled
						returnCode = (int)Constants.ThreeDStatus.Enabled;
					else if ( enrolled == "N" ) // Not 3d-enabled
						returnCode = (int)Constants.ThreeDStatus.NotEnabled;
					else if ( enrolled == "U" ) // 3d-status not available
						returnCode = (int)Constants.ThreeDStatus.Unavailable;
					else
						returnMessage = "Invalid 3d Secure lookup status received ; please try again";
				}
				else if ( errDesc.Length > 0 )
					returnMessage = errDesc;
				else
					returnMessage = "Unable to check this card's 3d Secure status";
			}
			catch (Exception ex)
			{
				Tools.LogException("ThreeDSecure.CardLookup","ReturnCode=" + returnCode.ToString(),ex);
			}
			return returnCode;
		}

		public int CardAuthenticate()
		{
			try
			{
				returnCode    = 510;
				string xmlMsg = "<CardinalMPI>"
				              + "<MsgType>cmpi_authenticate</MsgType>"
				              + xml3d
				              + "<TransactionType>C</TransactionType>"
				              + "<TransactionId>" + System.Net.WebUtility.UrlEncode(transactionId) + "</TransactionId>"
				              + "<PAResPayload>" + System.Net.WebUtility.UrlEncode(payLoad) + "</PAResPayload>"
				              + "</CardinalMPI>";
				returnCode                 = 520;
				System.Xml.XmlDocument xml = Send3dMessage(xmlMsg);

				if ( returnCode == 0 && xml != null && errNo == "0" && errDesc.Length == 0 )
				{
					returnCode = 530;
					cavv       = xml.DocumentElement.SelectSingleNode("/CardinalMPI/Cavv").InnerText;
					xid        = xml.DocumentElement.SelectSingleNode("/CardinalMPI/Xid").InnerText;
					eciFlag    = xml.DocumentElement.SelectSingleNode("/CardinalMPI/EciFlag").InnerText;
					parStatus  = xml.DocumentElement.SelectSingleNode("/CardinalMPI/PAResStatus").InnerText.ToUpper();
					sigVerify  = xml.DocumentElement.SelectSingleNode("/CardinalMPI/SignatureVerification").InnerText.ToUpper();
					returnCode = 540;

					if ( errNo == "0" )
					{
						returnMessage  = "";
						returnCode     = 550;
						if ( parStatus == "Y" )
							returnCode  = (int)Constants.ThreeDStatus.Success1;
						else if ( parStatus == "N" )
							returnCode  = (int)Constants.ThreeDStatus.Failed;
						else if ( parStatus == "U" )
							returnCode  = (int)Constants.ThreeDStatus.Incomplete;
						else if ( parStatus == "A" )
							returnCode  = (int)Constants.ThreeDStatus.Success2;
						else
							returnMessage = "Invalid 3d Secure transaction status received ; please try again";
					}
					else
						returnMessage = errDesc;
				}
			}
			catch (Exception ex)
			{
				Tools.LogException("ThreeDSecure.CardAuthenticate","ReturnCode=" + returnCode.ToString(),ex);
			}
			return returnCode;
		}

		public override void LoadData(DBConn dbConn)
		{ }

		public override void CleanUp()
		{
			client3d = null;
		}

		public ThreeDSecure(byte payMode) : base()
		{
			url3d = PCIBusiness.Tools.ConfigValue("3dURL");
			xml3d = "<Version>"        + PCIBusiness.Tools.ConfigValue("3dVersion"  ) + "</Version>"
			      + "<ProcessorId>"    + PCIBusiness.Tools.ConfigValue("3dProcessor") + "</ProcessorId>"
			      + "<MerchantId>"     + PCIBusiness.Tools.ConfigValue("3dMerchant" ) + "</MerchantId>"
			      + "<TransactionPwd>" + PCIBusiness.Tools.ConfigValue("3dPassword" ) + "</TransactionPwd>";
		}
	}
}
