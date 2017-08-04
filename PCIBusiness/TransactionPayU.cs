using System;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class TransactionPayU : Transaction
	{
//		static string url      = "https://staging.payu.co.za";
//		static string userID   = "Staging Enterprise Integration Store 1";
//		static string password = "j3w8swi5";

//		static string url      = "https://staging.payu.co.za";
//		static string userID   = "200239";
//		static string password = "5AlTRPoD";

//		private string url;
//		private string userID;
//		private string password;

		static string soapEnvelope =
			@"<SOAP-ENV:Envelope
				xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/' 
				xmlns:ns1='http://soap.api.controller.web.payjar.com/' 
				xmlns:ns2='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
				<SOAP-ENV:Header>
					<wsse:Security SOAP-ENV:mustUnderstand='1' xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
						<wsse:UsernameToken wsu:Id='UsernameToken-9' xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
							<wsse:Username></wsse:Username>
							<wsse:Password Type='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText'></wsse:Password>
						</wsse:UsernameToken>
					</wsse:Security>
				</SOAP-ENV:Header>
				<SOAP-ENV:Body>
				</SOAP-ENV:Body>
			</SOAP-ENV:Envelope>";

		private string resultSuccessful;

		public override string ConnectionDetails(byte mode,string separator="")
		{
			if ( mode == 1 ) // HTML
				return "<table>"
					  + "<tr><td>Payment Provider</td><td class='Red'> : PayU</td></tr>"
					  + "<tr><td>Status</td><td class='Red'> : Ready for testing</td></tr>"
					  + "<tr><td colspan='2'><hr /></td></tr>"
					  + "<tr><td>Bureau Code</td><td> : " + bureauCode + "</td></tr>"
//					  + "<tr><td>URL</td><td> : " + url + "</td></tr>"
//					  + "<tr><td>User ID</td><td> : " + userID + "</td></tr>"
//					  + "<tr><td>Password</td><td> : " + password + "</td></tr>"
					  + "</table>";

			if ( Tools.NullToString(separator).Length < 1 )
				separator = Environment.NewLine;

			return "Payment Provider : PayU" + separator
			     + "Bureau Code : " + bureauCode;
//			     + "URL : " + url + separator
//			     + "User ID : " + userID + separator
//			     + "Password : " + password;
		}


		public  bool   Successful
		{
			get { return Tools.NullToString(resultSuccessful).ToUpper() == "TRUE"; }
		}
				
		private int SendXML(string url,string userID,string password)
		{
			int    ret         = 10;
			string xmlReceived = "";
			payRef             = "";

			try
			{
				Tools.LogInfo("TransactionPayU.SendXML/10","XML = " + xmlSent,100);

			// Construct soap object
				ret = 20;
				XmlDocument soapEnvelopeXml = CreateSoapEnvelope(xmlSent);

			// Create username and password namespace
				ret = 30;
				XmlNamespaceManager mgr = new XmlNamespaceManager(soapEnvelopeXml.NameTable);
				mgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
				XmlNode userName        = soapEnvelopeXml.SelectSingleNode("//wsse:Username",mgr);
				userName.InnerText      = userID;
				XmlNode userPassword    = soapEnvelopeXml.SelectSingleNode("//wsse:Password",mgr);
				userPassword.InnerText  = password;

			// Construct web request object
				Tools.LogInfo("TransactionPayU.SendXML/30","URL = " + url + "/service/PayUAPI?wsdl"
					                                    + ", UserID = " + userID
					                                    + ", Password = " + password,100);
				ret = 40;
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url+"/service/PayUAPI?wsdl");
				webRequest.Headers.Add(@"SOAP:Action");
				webRequest.ContentType = "text/xml;charset=\"utf-8\"";
				webRequest.Accept      = "text/xml";
				webRequest.Method      = "POST";

			// Insert soap envelope into web request
				ret = 50;
				using (Stream stream = webRequest.GetRequestStream())
					soapEnvelopeXml.Save(stream);

			// Get the completed web request XML
				ret = 60;

				using (WebResponse webResponse = webRequest.GetResponse())
				{
					ret = 63;
					using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
					{
						ret         = 66;
						xmlReceived = rd.ReadToEnd();
					}
				}

				Tools.LogInfo("TransactionPayU.SendXML/40","XML Received = " + xmlReceived,100);

			// Create an empty soap result object
				ret       = 70;
				xmlResult = new XmlDocument();
				xmlResult.LoadXml(xmlReceived.ToString());

//			//	Get data from result XML
				ret              = 80;
				resultSuccessful = Tools.XMLNode(xmlResult,"successful");
				resultCode       = Tools.XMLNode(xmlResult,"resultCode");
				resultMsg        = Tools.XMLNode(xmlResult,"resultMessage");
//				payRef           = Tools.XMLNode(xmlResult,"payUReference");
//				payToken         = Tools.XMLNode(xmlResult,"pmId");

				if ( Successful )
					return 0;
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.SendXML/85","Ret="+ret.ToString()+" / "+xmlSent,100);
				Tools.LogException("TransactionPayU.SendXML/90","Ret="+ret.ToString()+" / "+xmlSent,ex);
			}

			Tools.LogInfo("TransactionPayU.SendXML/95","Ret="+ret.ToString(),100);
			return ret;
		}

		public override int GetToken(Payment payment)
		{
			int ret = 300;
			xmlSent = "";

			Tools.LogInfo("TransactionPayU.GetToken/10","Started ... " + payment.MerchantReference,100);

//        + "<AuthenticationType>NA</AuthenticationType>"

			try
			{
				xmlSent = "<ns1:doTransaction>"
				        + "<Safekey>" + payment.SafeKey + "</Safekey>"
				        + "<Api>ONE_ZERO</Api>"
				        + "<TransactionType>RESERVE</TransactionType>"
				        + "<Customfield>"
				        +   "<key>processingType</key>"
				        +   "<value>REAL_TIME_RECURRING</value>"
				        + "</Customfield>"
				        + "<AdditionalInformation>"
				        +   "<storePaymentMethod>true</storePaymentMethod>"
				        +   "<secure3d>false</secure3d>"
				        +   "<merchantReference>" + payment.MerchantReference + "</merchantReference>"
				        + "</AdditionalInformation>"
				        + "<Customer>"
				        +   "<merchantUserId>" + payment.UserID + "</merchantUserId>"
				        +   "<countryCode>" + payment.CountryCode + "</countryCode>"
				        +   "<email>" + payment.EMail + "</email>"
				        +   "<firstName>" + payment.FirstName + "</firstName>"
				        +   "<lastName>" + payment.LastName + "</lastName>"
				        +   "<mobile>" + payment.PhoneCell + "</mobile>"
				        +   "<regionalId>" + payment.RegionalId + "</regionalId>"
				        + "</Customer>"
				        + "<Basket>"
				        +	"<amountInCents>" + payment.PaymentAmount.ToString() + "</amountInCents>"
				        +	"<currencyCode>" + payment.CurrencyCode + "</currencyCode>"
				        +	"<description>" + payment.PaymentDescription + "</description>"
				        + "</Basket>"
				        + "<Creditcard>"
				        +	"<nameOnCard>" + payment.CardName + "</nameOnCard>"
				        +	"<amountInCents>" + payment.PaymentAmount.ToString() + "</amountInCents>"
			           +    "<cardNumber>" + payment.CardNumber + "</cardNumber>"
			           +    "<cardExpiry>" + payment.CardExpiry + "</cardExpiry>"
			           +    "<cvv>" + payment.CardCVV + "</cvv>"
				        + "</Creditcard>"
				        + "</ns1:doTransaction>";

				ret = SendXML(payment.URL,payment.UserID,payment.Password);
				if ( ret > 0 )
					return ret;

				payRef   = Tools.XMLNode(xmlResult,"payUReference");
				payToken = Tools.XMLNode(xmlResult,"pmId");

				Tools.LogInfo("TransactionPayU.GetToken/20","PayURef=" + payRef + ", pmId=" + payToken,100);

				xmlSent  = "<ns1:doTransaction>"
				         + "<Safekey>" + payment.SafeKey + "</Safekey>"
				         + "<Api>ONE_ZERO</Api>"
				         + "<TransactionType>RESERVE_CANCEL</TransactionType>"
				         + "<AdditionalInformation>"
				         +   "<merchantReference>" + payment.MerchantReference + "</merchantReference>"
				         +   "<payUReference>" + payRef + "</payUReference>"
				         + "</AdditionalInformation>"
				         + "<Basket>"
				         +	"<amountInCents>" + payment.PaymentAmount.ToString() + "</amountInCents>"
				         +	"<currencyCode>" + payment.CurrencyCode + "</currencyCode>"
				         + "</Basket>"
				         + "</ns1:doTransaction>";

				return SendXML(payment.URL,payment.UserID,payment.Password);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.GetToken/85","Ret="+ret.ToString()+" / "+xmlSent,100);
				Tools.LogException("TransactionPayU.GetToken/90","Ret="+ret.ToString()+" / "+xmlSent,ex);
			}

			Tools.LogInfo("TransactionPayU.GetToken/95","Ret="+ret.ToString(),100);
			return ret;
		}


		public override int ProcessPayment(Payment payment)
		{
			int ret = 600;
			xmlSent = "";

			Tools.LogInfo("TransactionPayU.ProcessPayment/10","Started ... " + payment.MerchantReference,100);

			try
			{
				xmlSent = "<ns1:doTransaction>"
				        + "<Safekey>" + payment.SafeKey + "</Safekey>"
				        + "<Api>ONE_ZERO</Api>"
				        + "<TransactionType>PAYMENT</TransactionType>"
				        + "<AuthenticationType>TOKEN</AuthenticationType>"
				        + "<Customfield>"
				        +   "<key>processingType</key>"
				        +   "<value>REAL_TIME_RECURRING</value>"
				        + "</Customfield>"
				        + "<AdditionalInformation>"
				        +   "<storePaymentMethod>true</storePaymentMethod>"
				        +   "<secure3d>false</secure3d>"
				        +   "<merchantReference>" + payment.MerchantReference + "</merchantReference>"
				        + "</AdditionalInformation>"
				        + "<Customer>"
				        +   "<merchantUserId>" + payment.UserID + "</merchantUserId>"
				        +   "<countryCode>" + payment.CountryCode + "</countryCode>"
				        +   "<email>" + payment.EMail + "</email>"
				        +   "<firstName>" + payment.FirstName + "</firstName>"
				        +   "<lastName>" + payment.LastName + "</lastName>"
				        +   "<mobile>" + payment.PhoneCell + "</mobile>"
				        +   "<regionalId>" + payment.RegionalId + "</regionalId>"
				        + "</Customer>"
				        + "<Basket>"
				        +	"<amountInCents>" + payment.PaymentAmount.ToString() + "</amountInCents>"
				        +	"<currencyCode>" + payment.CurrencyCode + "</currencyCode>"
				        +	"<description>" + payment.PaymentDescription + "</description>"
				        + "</Basket>"
				        + "<Creditcard>"
				        +	"<amountInCents>" + payment.PaymentAmount.ToString() + "</amountInCents>"
						  +   "<pmId>" + payment.CardToken + "</pmId>"
				        + "</Creditcard>"
				        + "</ns1:doTransaction>";

				Tools.LogInfo("TransactionPayU.ProcessPayment/20","XML = " + xmlSent,100);

				ret    = SendXML(payment.URL,payment.UserID,payment.Password);
				payRef = Tools.XMLNode(xmlResult,"payUReference");
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.ProcessPayment/85","Ret="+ret.ToString()+" / "+xmlSent,100);
				Tools.LogException("TransactionPayU.ProcessPayment/90","Ret="+ret.ToString()+" / "+xmlSent,ex);
			}

			Tools.LogInfo("TransactionPayU.ProcessPayment/95","Ret="+ret.ToString(),100);
			return ret;
		}

//		public int Process(string customerXML)
//		{
//			string soapXML = "";
//			int    ret     = 10;
//			payRef         = "";
//
//			try
//			{
//				soapXML = "<ns1:doTransaction>"
//				        + stdXML
//				        + customerXML
//				        + "</ns1:doTransaction>";
//
//			// Construct soap object
//				ret = 20;
//				XmlDocument soapEnvelopeXml = CreateSoapEnvelope(soapXML);
//
//			// Create username and password namespace
//				ret = 30;
//				XmlNamespaceManager mgr = new XmlNamespaceManager(soapEnvelopeXml.NameTable);
//				mgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
//				XmlNode userName        = soapEnvelopeXml.SelectSingleNode("//wsse:Username",mgr);
//				userName.InnerText      = userID;
//				XmlNode userPassword    = soapEnvelopeXml.SelectSingleNode("//wsse:Password",mgr);
//				userPassword.InnerText  = password;
//
//			// Construct web request object
//				ret = 40;
//				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url+"/service/PayUAPI?wsdl");
//				webRequest.Headers.Add(@"SOAP:Action");
//				webRequest.ContentType = "text/xml;charset=\"utf-8\"";
//				webRequest.Accept      = "text/xml";
//				webRequest.Method      = "POST";
//
//			// Insert soap envelope into web request
//				ret = 50;
//				using (Stream stream = webRequest.GetRequestStream())
//				{
//					soapEnvelopeXml.Save(stream);
//				}
//
//			// Get the PayU reference number from the completed web request.
//				ret = 60;
//				string soapResult;
//
//				using (WebResponse webResponse = webRequest.GetResponse())
//					using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
//					{
//						soapResult = rd.ReadToEnd();
//					}
//
//			// Create an empty soap result object
//				ret = 70;
//				XmlDocument soapResultXml = new XmlDocument();
//				soapResultXml.LoadXml(soapResult.ToString());
//
//			//	Get data from result XML
//				ret              = 80;
//				resultSuccessful = Tools.XMLNode(soapResultXml,"successful");
//				resultCode       = Tools.XMLNode(soapResultXml,"resultCode");
//				resultMsg        = Tools.XMLNode(soapResultXml,"resultMessage");
//				payRef           = Tools.XMLNode(soapResultXml,"payUReference");
//				if ( Successful && payRef.Length > 0 )
//					ret = 0;
//			}
//			catch (Exception ex)
//			{
//				Tools.LogException("TransactionPayU.Process","Ret="+ret.ToString()+" / "+soapXML,ex);
//			}
//			return ret;
//		}

		private static XmlDocument CreateSoapEnvelope(string content)
		{
			StringBuilder str = new StringBuilder(soapEnvelope);
			str.Insert(str.ToString().IndexOf("</SOAP-ENV:Body>"), content);

		//	Create an empty soap envelope
			XmlDocument soapEnvelopeXml = new XmlDocument();
			soapEnvelopeXml.LoadXml(str.ToString());
			return soapEnvelopeXml;
		}

		public TransactionPayU() : base()
		{
			bureauCode = Tools.BureauCode(Constants.PaymentProvider.PayU);
//			url        = Tools.ConfigValue(bureauCode+"/URL");
//			userID     = Tools.ConfigValue(bureauCode+"/UserID");
//			password   = Tools.ConfigValue(bureauCode+"/Password");
		}
	}
}
