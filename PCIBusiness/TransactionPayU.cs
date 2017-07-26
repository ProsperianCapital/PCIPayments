﻿using System;
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

		static string url      = "https://staging.payu.co.za";
		static string userID   = "200239";
		static string password = "5AlTRPoD";

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

		public  bool   Successful
		{
			get { return Tools.NullToString(resultSuccessful).ToUpper() == "TRUE"; }
		}

		public int Process(Payment payment)
		{
			string soapXML = "";
			int    ret     = 10;
			payRef         = "";

			Tools.LogInfo("TransactionPayU.Process/10","Started ... " + payment.MerchantReference,100);

			try
			{
				soapXML = "<ns1:doTransaction>"
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
			            +	"<merchantReference>" + payment.MerchantReference + "</merchantReference>"
			            + "</AdditionalInformation>"
				        + "<Customer>"
				        +   "<merchantUserId>" + payment.MerchantUserId + "</merchantUserId>"
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
			            +	"<amountInCents>" + payment.PaymentAmount.ToString() + "</amountInCents>";
				if ( payment.PaymentToken.Length > 1 ) // Card is tokenized
					soapXML = soapXML
						     + "<pmId>" + payment.PaymentToken + "</pmId>";
				else
					soapXML = soapXML
			              + "<cardNumber>" + payment.CardNumber + "</cardNumber>"
			              + "<cardExpiry>" + payment.CardExpiry + "</cardExpiry>"
			              + "<cvv>" + payment.CardCVV + "</cvv>";
				soapXML = soapXML
			            + "</Creditcard>"
				        + "</ns1:doTransaction>";

				Tools.LogInfo("TransactionPayU.Process/20","XML = " + soapXML,100);

			// Construct soap object
				ret = 20;
				XmlDocument soapEnvelopeXml = CreateSoapEnvelope(soapXML);

			// Create username and password namespace
				ret = 30;
				XmlNamespaceManager mgr = new XmlNamespaceManager(soapEnvelopeXml.NameTable);
				mgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
				XmlNode userName        = soapEnvelopeXml.SelectSingleNode("//wsse:Username",mgr);
				userName.InnerText      = userID;
				XmlNode userPassword    = soapEnvelopeXml.SelectSingleNode("//wsse:Password",mgr);
				userPassword.InnerText  = password;

			// Construct web request object
				ret = 40;
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url+"/service/PayUAPI?wsdl");
				webRequest.Headers.Add(@"SOAP:Action");
				webRequest.ContentType = "text/xml;charset=\"utf-8\"";
				webRequest.Accept      = "text/xml";
				webRequest.Method      = "POST";

			// Insert soap envelope into web request
				ret = 50;
				using (Stream stream = webRequest.GetRequestStream())
				{
					soapEnvelopeXml.Save(stream);
				}

			// Get the PayU reference number from the completed web request.
				ret = 60;
				string soapResult;

				using (WebResponse webResponse = webRequest.GetResponse())
					using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
					{
						soapResult = rd.ReadToEnd();
					}

				Tools.LogInfo("TransactionPayU.Process/30","soapResult = " + soapResult,100);

			// Create an empty soap result object
				ret = 70;
				XmlDocument soapResultXml = new XmlDocument();
				soapResultXml.LoadXml(soapResult.ToString());

			//	Get data from result XML
				ret              = 80;
				resultSuccessful = Tools.XMLNode(soapResultXml,"successful");
				resultCode       = Tools.XMLNode(soapResultXml,"resultCode");
				resultMsg        = Tools.XMLNode(soapResultXml,"resultMessage");
				payRef           = Tools.XMLNode(soapResultXml,"payUReference");
				payToken         = Tools.XMLNode(soapResultXml,"pmId");
				if ( Successful && payRef.Length > 0 )
					ret = 0;
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionPayU.Process/500","Ret="+ret.ToString()+" / "+soapXML,100);
				Tools.LogException("TransactionPayU.Process/510","Ret="+ret.ToString()+" / "+soapXML,ex);
			}

			Tools.LogInfo("TransactionPayU.Process/520","Ret="+ret.ToString(),100);
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
	}
}
