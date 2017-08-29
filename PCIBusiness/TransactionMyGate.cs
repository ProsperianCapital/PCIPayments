using System;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace PCIBusiness
{
	public class TransactionMyGate : Transaction
	{
//	v1
//		static string soapEnvelope =
//			@"<SOAP-ENV:Envelope
//				xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/http' 
//				xmlns:ns1='PinManagement' 
//				xmlns:ns2='http://rpc.xml.coldfusion'>
//				<SOAP-ENV:Header>
//				</SOAP-ENV:Header>
//				<SOAP-ENV:Body>
//				</SOAP-ENV:Body>
//			</SOAP-ENV:Envelope>";

//	v2
//		static string soapEnvelope =
//			@"<SOAP-ENV:Envelope
//          SOAP-ENV:encodingStyle='http://schemas.xmlsoap.org/soap/encoding'
//				xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope'
//				xmlns:ns1='PinManagement'> 
//				<SOAP-ENV:Body>
//				</SOAP-ENV:Body>
//			</SOAP-ENV:Envelope>";

//	v3
		static string soapEnvelope =
			@"<SOAP-ENV:Envelope
				xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope'
				xmlns:ns1='PinManagement'
				xmlns:xsd='http://www.w3.org/2001/XMLSchema'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
				xmlns:SOAP-ENC='http://schemas.xmlsoap.org/soap/encoding'
				SOAP-ENV:encodingStyle='http://schemas.xmlsoap.org/soap/encoding'>
				<SOAP-ENV:Body> 
				</SOAP-ENV:Body> 
			</SOAP-ENV:Envelope>";

		private int SendXML(string url,string userID,string password)
		{
			int    ret         = 10;
			string xmlReceived = "";
			payRef             = "";

			try
			{
				if ( ! url.ToUpper().EndsWith("WSDL") )
					url = url + "?wsdl";

				Tools.LogInfo("TransactionMyGate.SendXML/10","URL=" + url + ", XML Sent=" + xmlSent,30);

//			// Construct soap object
//				ret = 20;
//				XmlDocument soapEnvelopeXml = new XmlDocument();
//				soapEnvelopeXml.LoadXml(xmlSent);

			// Construct soap object
				ret = 20;
				XmlDocument soapEnvelopeXml = CreateSoapEnvelope(xmlSent);

//			// Create username and password namespace
//				ret = 30;
//				XmlNamespaceManager mgr = new XmlNamespaceManager(soapEnvelopeXml.NameTable);
//				mgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
//				XmlNode userName        = soapEnvelopeXml.SelectSingleNode("//wsse:Username",mgr);
//				userName.InnerText      = userID;
//				XmlNode userPassword    = soapEnvelopeXml.SelectSingleNode("//wsse:Password",mgr);
//				userPassword.InnerText  = password;

			// Construct web request object
				Tools.LogInfo("TransactionMyGate.SendXML/30","Create/set up web request, URL=" + url,199);
				ret = 40;
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
				webRequest.Headers.Add(@"SOAP:Action");
				webRequest.ContentType = "text/xml;charset=\"utf-8\"";
				webRequest.Accept      = "text/xml";
				webRequest.Method      = "POST";

			// Insert soap envelope into web request
				Tools.LogInfo("TransactionMyGate.SendXML/35","Save web request",199);
				ret = 50;
				using (Stream stream = webRequest.GetRequestStream())
					soapEnvelopeXml.Save(stream);

			// Get the completed web request XML
				Tools.LogInfo("TransactionMyGate.SendXML/40","Get web response",199);
				ret = 60;

				using (WebResponse webResponse = webRequest.GetResponse())
				{
					ret = 63;
					Tools.LogInfo("TransactionMyGate.SendXML/45","Read web response stream",199);
					using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
					{
						ret         = 66;
						xmlReceived = rd.ReadToEnd();
					}
				}

				Tools.LogInfo("TransactionMyGate.SendXML/50","XML Received=" + xmlReceived,199);

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

				Tools.LogInfo("TransactionMyGate.SendXML/80","URL=" + url + ", XML Sent=" + xmlSent+", XML Received="+xmlReceived,200);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionMyGate.SendXML/85","Ret="+ret.ToString()+", URL=" + url + ", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionMyGate.SendXML/90","Ret="+ret.ToString()+", URL=" + url + ", XML Sent="+xmlSent,ex);
			}
			return ret;
		}


		public override int GetToken(Payment payment)
		{
			int    ret         = 300;
//			string xmlReceived = "";
			xmlSent = "";

			try
			{
//	v3
				xmlSent = "<ns1:fLoadPinCC>"
				        +    "<ClientID xsi:type='xsd:string'>MY014473</ClientID>"
				        +    "<ApplicationID xsi:type='xsd:string'>65F1573E-F8F9-46C6-B2C4-C88B86737E16</ApplicationID>"
				        +    "<CardNumber xsi:type='xsd:string'>4000000000000004</CardNumber>"
				        +    "<CardHolder xsi:type='xsd:string'>P Smith</CardHolder>"
				        +    "<ExpiryMonth xsi:type='xsd:string'>11</ExpiryMonth>"
				        +    "<ExpiryYear xsi:type='xsd:string'>2019</ExpiryYear>"
				        +    "<CardType xsi:type='xsd:string'>7</CardType>"
				        +    "<ClientPin xsi:type='xsd:string'>1156</ClientPin>"
				        +    "<ClientUCI xsi:type='xsd:string'>1123</ClientUCI>"
				        + "</ns1:fLoadPinCC>";

//	v2
//				xmlSent = "<ns1:fLoadPinCC>"
//				        +    "<ClientID>MY014473</ClientID>"
//				        +    "<ApplicationID>65F1573E-F8F9-46C6-B2C4-C88B86737E16</ApplicationID>"
//				        +    "<CardNumber>4000000000000004</CardNumber>"
//				        +    "<CardHolder>P Smith</CardHolder>"
//				        +    "<ExpiryMonth>11</ExpiryMonth>"
//				        +    "<ExpiryYear>2019</ExpiryYear>"
//				        +    "<CardType>7</CardType>"
//				        +    "<ClientPin>1156</ClientPin>"
//				        +    "<ClientUCI>1123</ClientUCI>"
//				        + "</ns1:fLoadPinCC>";

//	v1
//				xmlSent = "<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>"
//				        + "<soap:Body>"
//				        + "<fLoadPinCC>" // xmlns='https://www.mygate.co.za/Collections/1x0x0/pinManagement.cfc'>"
//				        +    "<ClientID>1</ClientID>"
//				        +    "<ApplicationID>1</ApplicationID>"
//				        +    "<CardNumber>4000000000000004</CardNumber>"
//				        +    "<CardHolder>P Smith</CardHolder>"
//				        +    "<ExpiryMonth>11</ExpiryMonth>"
//				        +    "<ExpiryYear>2019</ExpiryYear>"
//				        +    "<CardType>7</CardType>"
//				        +    "<ClientPin>1156</ClientPin>"
//				        +    "<ClientUCI>1123</ClientUCI>"
//				        + "</fLoadPinCC>"
//				        + "</soap:Body>"
//				        + "</soap:Envelope>";

//    object[] fLoadPinCC(string ClientID, string ApplicationID, string CardNumber, string CardHolder, string ExpiryMonth, string ExpiryYear, string CardType, string ClientPin, string ClientUCI);

				ret = SendXML(payment.ProviderURL,payment.ProviderUserID,payment.ProviderPassword);

				Tools.LogInfo("TransactionMyGate.GetToken/10","Merchant Ref=" + payment.MerchantReference,30);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionMyGate.GetToken/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionMyGate.GetToken/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}

		public override int ProcessPayment(Payment payment)
		{
			int ret = 600;
			xmlSent = "";

			try
			{
				Tools.LogInfo("TransactionMyGate.ProcessPayment/20","XML Sent=" + xmlSent,30);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionMyGate.ProcessPayment/85","Ret="+ret.ToString()+", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionMyGate.ProcessPayment/90","Ret="+ret.ToString()+", XML Sent="+xmlSent,ex);
			}
			return ret;
		}
		private static XmlDocument CreateSoapEnvelope(string xmlBody)
		{
			StringBuilder str = new StringBuilder(soapEnvelope);
			str.Insert(str.ToString().IndexOf("</SOAP-ENV:Body>"), xmlBody);

		//	Create an empty soap envelope
			XmlDocument soapEnvelopeXml = new XmlDocument();
			soapEnvelopeXml.LoadXml(str.ToString());
			return soapEnvelopeXml;
		}
	}
}
