using System;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class TransactionMyGate : Transaction
	{
		static string soapEnvelope = "";

		private int SendXML(string url,string userID,string password)
		{
			int    ret         = 10;
			string xmlReceived = "";
			payRef             = "";

			try
			{
				Tools.LogInfo("TransactionPayU.SendXML/10","XML Sent=" + xmlSent,30);

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
				Tools.LogInfo("TransactionPayU.SendXML/30","URL=" + url + "/service/PayUAPI?wsdl"
					                                    + ", UserID=" + userID
					                                    + ", Password=" + password,30);
				ret = 40;
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://www.mygate.co.za/Collections/1x0x0/pinManagement.cfc?wsdl");
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

				Tools.LogInfo("TransactionPayU.SendXML/40","XML Received=" + xmlReceived,30);

			// Create an empty soap result object
				ret       = 70;
				xmlResult = new XmlDocument();
				xmlResult.LoadXml(xmlReceived.ToString());

//			//	Get data from result XML
				ret              = 80;
//				resultSuccessful = Tools.XMLNode(xmlResult,"successful");
				resultCode       = Tools.XMLNode(xmlResult,"resultCode");
				resultMsg        = Tools.XMLNode(xmlResult,"resultMessage");
//				payRef           = Tools.XMLNode(xmlResult,"payUReference");
//				payToken         = Tools.XMLNode(xmlResult,"pmId");

//				if ( Successful )
//					return 0;
//				else
//					Tools.LogInfo("TransactionPayU.SendXML/50","Ret="+ret.ToString()+", XML Received="+xmlReceived,120);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.SendXML/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionPayU.SendXML/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}

		public override int GetToken(Payment payment)
		{
			int ret = 300;
			xmlSent = "";

			Tools.LogInfo("TransactionMyGate.GetToken/10","Merchant Ref=" + payment.MerchantReference,30);

/*
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.mygate.co.za/Collections/1x0x0/pinManagement.cfc?wsdl");
XmlDocument soapEnvelopeXml = new XmlDocument();
soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<soap:Body>
<HelloWorld3 xmlns=""http://tempuri.org/"">
<parameter1>test</parameter1>
<parameter2>23</parameter2>
<parameter3>test</parameter3>
</HelloWorld3>
</soap:Body>
</soap:Envelope>");
using (Stream stream = request.GetRequestStream()) 
{ 
soapEnvelopeXml.Save(stream); 
}
using (WebResponse response = request.GetResponse())
{
using (StreamReader rd = new StreamReader(response.GetResponseStream())) 
{ 
string soapResult = rd.ReadToEnd();
Console.WriteLine(soapResult);
} 

			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.GetToken/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionPayU.GetToken/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
*/

			return ret;
		}


		public override int ProcessPayment(Payment payment)
		{
			int ret = 600;
			xmlSent = "";

			Tools.LogInfo("TransactionPayU.ProcessPayment/10","Merchant Ref=" + payment.MerchantReference,30);

//		   +   "<secure3d>false</secure3d>"
//       +   "<storePaymentMethod>true</storePaymentMethod>"

			try
			{
				xmlSent = "<ns1:doTransaction>"
				        + "<Safekey>" + payment.ProviderKey + "</Safekey>"
				        + "<Api>ONE_ZERO</Api>"
				        + "<TransactionType>PAYMENT</TransactionType>"
				        + "<AuthenticationType>TOKEN</AuthenticationType>"
				        + "<Customfield>"
				        +   "<key>processingType</key>"
				        +   "<value>REAL_TIME_RECURRING</value>"
				        + "</Customfield>"
				        + "<AdditionalInformation>"
				        +   "<merchantReference>" + payment.MerchantReference + "</merchantReference>"
				        + "</AdditionalInformation>"
				        + "<Customer>"
				        +   "<merchantUserId>" + payment.ProviderUserID + "</merchantUserId>"
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

				Tools.LogInfo("TransactionPayU.ProcessPayment/20","XML Sent=" + xmlSent,30);

				ret    = SendXML(payment.ProviderURL,payment.ProviderUserID,payment.ProviderPassword);
				payRef = Tools.XMLNode(xmlResult,"payUReference");
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.ProcessPayment/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionPayU.ProcessPayment/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}


		private static XmlDocument CreateSoapEnvelope(string content)
		{
			StringBuilder str = new StringBuilder(soapEnvelope);
			str.Insert(str.ToString().IndexOf("</SOAP-ENV:Body>"), content);

		//	Create an empty soap envelope
			XmlDocument soapEnvelopeXml = new XmlDocument();
			soapEnvelopeXml.LoadXml(str.ToString());
			return soapEnvelopeXml;
		}

		public TransactionMyGate() : base()
		{
			bureauCode = Tools.BureauCode(Constants.PaymentProvider.MyGate);
		}
	}
}
