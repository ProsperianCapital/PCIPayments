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
		static string merchantURL     = "https://payment.ccp.boarding.transact24.com/Recurring";
		static string returnURL       = "http://www.paulkilfoil.co.za/Prosperian/PaymentSucceed.aspx";
		static string partnerControl  = "b0148b62531a9311f52560a2a88ba70f";
		static string merchantAccount = "567654452";
		static string postHTML = 
			@"<html><head></head><body>
			  <form>
			  <input type='hidden' id='version'               value='2' />
			  <input type='hidden' id='merchant_id'           value='merchant_id' />
			  <input type='hidden' id='merchant_account'      value='merchant_account' />
			  <input type='hidden' id='merchant_order'        value='merchant_order' />
			  <input type='hidden' id='merchant_product_desc' value='merchant_product_desc' />
			  <input type='hidden' id='email'                 value='x@y.com' />
			  <input type='hidden' id='amount'                value='amount' />
			  <input type='hidden' id='currency'              value='USD' />
			  <input type='hidden' id='credit_card_type'      value='credit_card_type' />
			  <input type='hidden' id='credit_card_number'    value='credit_card_number' />
			  <input type='hidden' id='expire_month'          value='expire_month' />
			  <input type='hidden' id='expire_year'           value='expire_year' />
			  <input type='hidden' id='cvv2'                  value='cvv2' />
			  <input type='hidden' id='server_return_url'     value='server_return_url' />
			  <input type='hidden' id='control'               value='control' />
			  </form>
			  </body></html>";

		public override string ConnectionDetails(byte mode,string separator="")
		{
			if ( mode == 1 ) // HTML
				return "<table>"
					  + "<tr><td>Payment Provider</td><td class='Red'> : Transact24</td></tr>"
					  + "<tr><td>Status</td><td class='Red'> : In development</td></tr>"
					  + "<tr><td colspan='2'><hr /></td></tr>"
					  + "<tr><td>Bureau Code</td><td> : " + bureauCode + "</td></tr>"
					  + "<tr><td>Go to URL</td><td> : " + merchantURL + "</td></tr>"
					  + "<tr><td>Return URL</td><td> : " + returnURL + "</td></tr>"
					  + "<tr><td>Partner Control</td><td> : " + partnerControl + "</td></tr>"
					  + "<tr><td>Merchant Account</td><td> : " + merchantAccount + "</td></tr>"
					  + "</table>";

			if ( Tools.NullToString(separator).Length < 1 )
				separator = Environment.NewLine;

			return "Payment Provider : Transact24" + separator
			     + "Bureau Code : " + bureauCode + separator
				  + "Go to URL : " + merchantURL + separator
				  + "Return URL : " + returnURL + separator
				  + "Partner Control : " + partnerControl + separator
				  + "Merchant Account : " + merchantAccount;
		}

		public int Process(Payment payment)
		{
			int ret = 10;

			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			// Construct web request object
				ret = 20;
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(merchantURL);
//				webRequest.Headers.Add(@"SOAP:Action");
//				webRequest.ContentType    = "text/xml;charset=\"utf-8\"";
//				webRequest.Accept         = "text/xml";
				webRequest.KeepAlive      = false;
				webRequest.Method         = "POST";
				webRequest.ContentType    = "application/x-www-form-urlencoded;charset=\"utf-8\"";

			//	Set up this payment's details
				ret = 30;
				postHTML = postHTML.Replace("value='merchant_account'"     ,"value='"+merchantAccount+"'")
					                .Replace("value='merchant_id'"          ,"value='"+merchantAccount+"'")
					                .Replace("value='server_return_url'"    ,"value='"+returnURL+"'")
				                   .Replace("value='merchant_order'"       ,"value='"+payment.MerchantReference+"'")
				                   .Replace("value='merchant_product_desc'","value='RTR Payment Code "+payment.MerchantReference+"'")
				                   .Replace("value='amount'"               ,"value='"+payment.PaymentAmount.ToString()+"'")
				                   .Replace("value='credit_card_type'"     ,"value='"+payment.CardType.ToString()+"'")
				                   .Replace("value='credit_card_number'"   ,"value='"+payment.CardNumber+"'")
				                   .Replace("value='expire_month'"         ,"value='"+payment.CardExpiryMonth+"'")
				                   .Replace("value='expire_year'"          ,"value='"+payment.CardExpiryYY+"'")
				                   .Replace("value='cvv2'"                 ,"value='"+payment.CardCVV+"'");

			//	Checksum (SHA1)
				ret = 40;
				string chk = merchantAccount
					        + payment.MerchantReference
							  + "RTR Payment Code "+payment.MerchantReference
							  + payment.PaymentAmount.ToString()
							  + "USD"
							  + payment.CardType.ToString()
							  + payment.CardNumber.ToString()
							  + payment.CardExpiryMonth.ToString()
							  + payment.CardExpiryYY.ToString()
							  + payment.CardExpiryYY.ToString()
							  + returnURL
							  + partnerControl;
				ret = 50;  
				postHTML   = postHTML.Replace("value='control'","value='"+HashSHA1(chk)+"'");

				ret = 60;
				byte[] page = Encoding.ASCII.GetBytes(postHTML);

			// Insert encoded HTML into web request
				ret = 70;
				using (Stream stream = webRequest.GetRequestStream())
				{
					stream.Write(page, 0, page.Length);
					stream.Flush();
					stream.Close();
				}

			// Get the XML response
				ret = 80;
				string xmlResult;

				using (WebResponse webResponse = webRequest.GetResponse())
					using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
						xmlResult = rd.ReadToEnd();

//				using (WebResponse response = webRequest.GetResponse())
//				{
//					if (response == null) Console.WriteLine("Response is null");
//					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
//					{
//						Console.WriteLine(reader.ReadToEnd().Trim());
//					}
//				}

				ret = 0;
			}
			catch (Exception ex)
			{
				Tools.LogException("TransactionT24.Process","Ret="+ret.ToString()+" / "+postHTML,ex);
			}
			return ret;
		}

		private string HashSHA1(string x)
		{
			byte[] hash;
			using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
				hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(x));
			return System.Text.Encoding.UTF8.GetString(hash);
		}

		public TransactionT24() : base()
		{
			bureauCode = Tools.BureauCode(Constants.PaymentProvider.T24);
		}

	}
}
