using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Http;
using System.IO;

namespace PCIBusiness
{
	public class TransactionT24 : Transaction
	{
//		static string partnerControl = "b0148b62531a9311f52560a2a88ba70f";
//		static string merchantID     = "567654452";

		static string postHTML = 
			@"<html><head></head><body>
			  <form>
			  <input type='hidden' id='version'               value='2' />
			  <input type='hidden' id='merchant_account'      value='merchant_account' />
			  <input type='hidden' id='merchant_order'        value='merchant_order' />
			  <input type='hidden' id='merchant_product_desc' value='merchant_product_desc' />
			  <input type='hidden' id='first_name'            value='first_name' />
			  <input type='hidden' id='last_name'             value='last_name' />
			  <input type='hidden' id='address1'              value='address1' />
			  <input type='hidden' id='city'                  value='city' />
			  <input type='hidden' id='state'                 value='state' />
			  <input type='hidden' id='zip_code'              value='zip_code' />
			  <input type='hidden' id='country'               value='country' />
			  <input type='hidden' id='phone'                 value='phone' />
			  <input type='hidden' id='email'                 value='email' />
			  <input type='hidden' id='amount'                value='amount' />
			  <input type='hidden' id='currency'              value='USD' />
			  <input type='hidden' id='credit_card_type'      value='credit_card_type' />
			  <input type='hidden' id='credit_card_number'    value='credit_card_number' />
			  <input type='hidden' id='expire_month'          value='expire_month' />
			  <input type='hidden' id='expire_year'           value='expire_year' />
			  <input type='hidden' id='cvv2'                  value='cvv2' />
			  <input type='hidden' id='control'               value='control' />
			  </form>
			  </body></html>";

//			  <input type='hidden' id='return_url'            value='return_url' />
//			  <input type='hidden' id='server_return_url'     value='server_return_url' />

		public  bool   Successful
		{
			get
			{
				resultCode = Tools.NullToString(resultCode).ToUpper();
				return ( resultCode.Length > 1 && resultCode.Substring(0,1) == "A" );
			}
		}

		private int PostHTML(string url)
		{
			int    ret         = 10;
			string xmlReceived = "";
			payRef             = "";

			try
			{
				Tools.LogInfo("TransactionT24.PostHTML/10","URL=" + url + ", XML Sent=" + xmlSent,177);

			// Construct web request object
				ret = 20;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
//				webRequest.Headers.Add(@"SOAP:Action");
//				webRequest.ContentType    = "text/xml;charset=\"utf-8\"";
//				webRequest.Accept         = "text/xml";
				webRequest.ContentType    = "application/x-www-form-urlencoded;charset=\"utf-8\"";
				webRequest.Method         = "POST";
				webRequest.KeepAlive      = false;

				ret = 30;
				byte[] page = Encoding.ASCII.GetBytes(xmlSent);

			// Insert encoded HTML into web request
				ret = 40;
				using (Stream stream = webRequest.GetRequestStream())
				{
					stream.Write(page, 0, page.Length);
//					stream.Flush();
					stream.Close();
				}

			// Get the XML response
				ret = 50;

				using (WebResponse webResponse = webRequest.GetResponse())
				{
					ret = 60;
					using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
					{
						ret         = 70;
						xmlReceived = rd.ReadToEnd();
					}
				}

				Tools.LogInfo("TransactionT24.PostHTML/70","XML Received=" + xmlReceived.ToString(),177);
				ret       = 80;
				xmlResult = new XmlDocument();
				xmlResult.LoadXml(xmlReceived.ToString());

//			//	Get data from result XML
				ret        = 90;
				resultCode = Tools.XMLNode(xmlResult,"status");
				resultMsg  = Tools.XMLNode(xmlResult,"message");

				if ( Successful )
					return 0;

				Tools.LogInfo("TransactionT24.SendXML/80","URL=" + url + ", XML Sent=" + xmlSent+", XML Received="+xmlReceived,177);
			}
			catch (Exception ex)
			{
				Tools.LogInfo("TransactionT24.PostHTML/85","Ret="+ret.ToString()+", URL=" + url + ", XML Sent="+xmlSent,255);
				Tools.LogException("TransactionT24.PostHTML/90","Ret="+ret.ToString()+", URL=" + url + ", XML Sent="+xmlSent,ex);
			}
			return ret;
		}

		public override int GetToken(Payment payment)
		{
			int ret = 10;

			try
			{
				xmlSent = "version=2"
				        + "&ipaddress="
				        + "&merchant_account="      + Tools.URLString(payment.ProviderUserID)
				        + "&first_name="            + Tools.URLString(payment.FirstName)
				        + "&last_name="             + Tools.URLString(payment.LastName)
				        + "&address1="              + Tools.URLString(payment.Address1)
				        + "&city="                  + Tools.URLString(payment.Address2)
				        + "&state="                 + Tools.URLString(payment.State) // USA Only
				        + "&zip_code="              + Tools.URLString(payment.PostalCode)
				        + "&country="               + Tools.URLString(payment.CountryCode)
				        + "&phone="                 + Tools.URLString(payment.PhoneCell)
				        + "&email="                 + Tools.URLString(payment.EMail)
				        + "&merchant_order="        + Tools.URLString(payment.MerchantReference)
				        + "&merchant_product_desc=" + Tools.URLString(payment.PaymentDescription)
				        + "&amount="                + Tools.URLString(payment.PaymentAmount.ToString())
				        + "&currency="              + Tools.URLString(payment.CurrencyCode)
				        + "&credit_card_type="      + Tools.URLString(payment.CardType)
				        + "&credit_card_number="    + Tools.URLString(payment.CardNumber)
				        + "&expire_month="          + Tools.URLString(payment.CardExpiryMM)
				        + "&expire_year="           + Tools.URLString(payment.CardExpiryYY)
				        + "&cvv2="                  + Tools.URLString(payment.CardCVV);

			//	Checksum (SHA1)
				ret = 20;
				string chk = payment.ProviderUserID
							  + payment.FirstName
							  + payment.LastName
							  + payment.Address1
							  + payment.Address2
							  + payment.State
							  + payment.PostalCode
							  + payment.CountryCode
							  + payment.PhoneCell
							  + payment.EMail
					        + payment.MerchantReference
					        + payment.PaymentDescription
							  + payment.PaymentAmount.ToString()
							  + payment.CurrencyCode
							  + payment.CardType
							  + payment.CardNumber
							  + payment.CardExpiryMM
							  + payment.CardExpiryYY
							  + payment.CardCVV;

//				ret        = 30;
//				xmlSent    = xmlSent + "&address2=";
//				if ( payment.Address(2).Length > 0 && payment.Address(2) != payment.Address(255) )
//				{
//					xmlSent = xmlSent + Tools.URLString(payment.Address(2));
//					chk     = chk + payment.Address(2);
//				}

				chk        = chk + payment.ProviderKey; // partnerControl;
				ret        = 40;  
				xmlSent    = xmlSent + "&control=" + HashSHA1(chk);

				Tools.LogInfo("TransactionT24.GetToken/10","(Reserve) POST="+xmlSent+", Key="+payment.ProviderKey,177);

				ret        = PostHTML(payment.ProviderURL);
				payToken   = Tools.XMLNode(xmlResult,"merchant_card_number");
				payRef     = Tools.XMLNode(xmlResult,"transaction_id");

				Tools.LogInfo("TransactionT24.GetToken/20","ResultCode="+ResultCode + ", payRef=" + payRef + ", payToken=" + payToken,177);

				if ( ret == 0 ) // Do refund
				{
					Tools.LogInfo("TransactionT24.GetToken/30","(Refund) Transaction Id=" + payRef,177);
					ret     = 50;
					xmlSent = "version=2"
					        + "&merchant_account=" + Tools.URLString(payment.ProviderUserID)
					        + "&order_id="         + Tools.URLString(payRef)
					        + "&amount="           + Tools.URLString(payment.PaymentAmount.ToString())
					        + "&currency="         + Tools.URLString(payment.CurrencyCode);
					ret     = 60;
					chk     = payment.ProviderUserID
					        + payRef
					        + payment.PaymentAmount.ToString()
					        + payment.CurrencyCode
					        + payment.ProviderKey;
					ret     = 70;  
					xmlSent = xmlSent + "&control=" + HashSHA1(chk);
					Tools.LogInfo("TransactionT24.GetToken/40","(Refund) POST="+xmlSent+", Key="+payment.ProviderKey,177);
					ret     = PostHTML("https://payment.ccp.transact24.com/Refund");
				}
			}
			catch (Exception ex)
			{
				Tools.LogException("TransactionT24.GetToken/90","Ret="+ret.ToString()+" / "+postHTML,ex);
			}
			return ret;
		}

		public override int ProcessPayment(Payment payment)
		{
			int ret = 10;

			try
			{
				xmlSent = "version=2"
				        + "&ipaddress="
				        + "&merchant_account="      + Tools.URLString(payment.ProviderUserID)
				        + "&first_name="            + Tools.URLString(payment.FirstName)
				        + "&last_name="             + Tools.URLString(payment.LastName)
				        + "&address1="              + Tools.URLString(payment.Address1)
				        + "&city="                  + Tools.URLString(payment.Address2)
				        + "&state="                 + Tools.URLString(payment.State) // USA Only
				        + "&zip_code="              + Tools.URLString(payment.PostalCode)
				        + "&country="               + Tools.URLString(payment.CountryCode)
				        + "&phone="                 + Tools.URLString(payment.PhoneCell)
				        + "&email="                 + Tools.URLString(payment.EMail)
				        + "&merchant_order="        + Tools.URLString(payment.MerchantReference)
				        + "&merchant_product_desc=" + Tools.URLString(payment.PaymentDescription)
				        + "&amount="                + Tools.URLString(payment.PaymentAmount.ToString())
				        + "&currency="              + Tools.URLString(payment.CurrencyCode)
				        + "&merchant_card_number="  + Tools.URLString(payment.CardToken);

			//	Checksum (SHA1)
				ret = 20;
				string chk = payment.ProviderUserID
							  + payment.FirstName
							  + payment.LastName
							  + payment.Address1
							  + payment.Address2
							  + payment.State
							  + payment.PostalCode
							  + payment.CountryCode
							  + payment.PhoneCell
							  + payment.EMail
					        + payment.MerchantReference
					        + payment.PaymentDescription
							  + payment.PaymentAmount.ToString()
							  + payment.CurrencyCode
							  + payment.CardToken;

				chk        = chk + payment.ProviderKey; // partnerControl;
				ret        = 40;  
				xmlSent    = xmlSent + "&control=" + HashSHA1(chk);

				Tools.LogInfo("TransactionT24.ProcessPayment/20","POST="+xmlSent+", Key="+payment.ProviderKey,177);

				ret        = PostHTML(payment.ProviderURL);
			}
			catch (Exception ex)
			{
				Tools.LogException("TransactionT24.ProcessPayment/90","Ret="+ret.ToString()+" / "+postHTML,ex);
			}
			return ret;
		}


		private string HashSHA1(string x)
		{
			StringBuilder hashStr = new StringBuilder();
			byte[]        hashArr;

//			x = "567654635samHartogh123 ABC StreetABC City1234ZA0839683725samantha.hartogh@prosperian.mu20161031x105025x2098Token0USD255236700317170170418000ba2e2640cc6d35615c14ccd42d5c4445";

			using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
				hashArr = sha1.ComputeHash(Encoding.UTF8.GetBytes(x));
			foreach (byte h in hashArr)
				hashStr.Append(h.ToString("x2"));

			Tools.LogInfo("TransactionT24.HashSHA1","Str In="+x+", Hash Out="+hashStr.ToString(),177);

			return hashStr.ToString();
		}

		public TransactionT24() : base()
		{
			bureauCode = Tools.BureauCode(Constants.PaymentProvider.T24);
		}
	}
}