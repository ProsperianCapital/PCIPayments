using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using Stripe;

namespace PCIBusiness
{
	public class TransactionStripe : Transaction
	{
		private string customerId;
		private string paymentMethodId;
		private string err;

		public string CustomerId
		{
			get { return Tools.NullToString(customerId); }
		}
		public string PaymentMethodId
		{
			get { return Tools.NullToString(paymentMethodId); }
		}

		public override int GetToken(Payment payment)
		{
			int ret         = 10;
			err             = "";
			payToken        = "";
			customerId      = "";
			paymentMethodId = "";
			strResult       = "";
			resultCode      = "991";
			resultMsg       = "failed";

			Tools.LogInfo("GetToken/10","Merchant Ref=" + payment.MerchantReference,10,this);

			try
			{
				ret                        = 20;
				StripeConfiguration.ApiKey = payment.ProviderPassword; // Secret key

				ret                        = 30;
				var tokenOptions           = new TokenCreateOptions
				{
					Card = new TokenCardOptions
					{
						Number   = payment.CardNumber,
						ExpMonth = payment.CardExpiryMonth,
						ExpYear  = payment.CardExpiryYear,
						Cvc      = payment.CardCVV
					}
				};
				ret              = 40;
				var tokenService = new TokenService();
				var token        = tokenService.Create(tokenOptions);
				payToken         = token.Id;
				err              = err + ", tokenId="+Tools.NullToString(payToken);

				ret                      = 50;
				var paymentMethodOptions = new PaymentMethodCreateOptions
				{
					Type = "card",
					Card = new PaymentMethodCardOptions
					{
						Token = token.Id
					}
				};
				ret                      = 60;
				var paymentMethodService = new PaymentMethodService();
				var paymentMethod        = paymentMethodService.Create(paymentMethodOptions);
				paymentMethodId          = paymentMethod.Id;
				err                      = err + ", paymentMethodId="+Tools.NullToString(paymentMethodId);

				ret                 = 70;
				var customerOptions = new CustomerCreateOptions
				{
					Name          = payment.CardName, // (payment.FirstName + " " + payment.LastName).Trim(),
					Email         = payment.EMail,
					Phone         = payment.PhoneCell,
					PaymentMethod = paymentMethod.Id,
				};
				ret                  = 80;
				var customerService  = new CustomerService();
				var customer         = customerService.Create(customerOptions);
//				customer.Currency    = payment.CurrencyCode.ToLower();
				customerId           = customer.Id;
				err                  = err + ", customerId="+Tools.NullToString(customerId);

				ret                  = 100;
				strResult            = customer.StripeResponse.Content;
//				resultMsg            = Tools.JSONValue(strResult,"status");
				resultCode           = customer.StripeResponse.ToString();
				int k                = resultCode.ToUpper().IndexOf(" STATUS=");
				ret                  = 110;
				err                  = err + ", StripeResponse="+Tools.NullToString(resultCode);

//	customer.StripeResponse.ToString() is as follows:
//	<Stripe.StripeResponse status=200 Request-Id=req_bI0B5glG6r6DNe Date=2021-05-28T09:35:23>

				if ( k > 0 )
				{
					resultCode = resultCode.Substring(k+8).Trim();
					k          = resultCode.IndexOf(" ");
					if ( k > 0 )
						resultCode = resultCode.Substring(0,k);
				}
				else
					resultCode = "999";

				err                  = err + ", strResult="+Tools.NullToString(strResult);
				ret                  = 120;
				customer             = null;
				customerService      = null;
				customerOptions      = null;
				paymentMethod        = null;
				paymentMethodService = null;
				paymentMethodOptions = null;
				token                = null;
				tokenService         = null;
				tokenOptions         = null;

				if ( resultCode.StartsWith("2") && payToken.Length > 0 && paymentMethodId.Length > 0 && customerId.Length > 0 )
				{
					resultMsg = "succeeded";
					ret       = 0;
				}
				else
					Tools.LogInfo ("GetToken/197",err.Substring(2),231,this);
			}
			catch (Exception ex)
			{
			//	resultMsg = "Ret="+ret.ToString() + ", tokenId="+Tools.NullToString(payToken) + ", paymentMethodId="+Tools.NullToString(paymentMethodId) + ", customerId="+Tools.NullToString(customerId) + ", strResult="+Tools.NullToString(strResult);
				err       = "Ret=" + ret.ToString() + err;
				Tools.LogInfo     ("GetToken/198",err,255,this);
				Tools.LogException("GetToken/199",err,ex ,this);
			}
			return ret;
		}

		public override int TokenPayment(Payment payment)
		{
			if ( ! EnabledFor3d(payment.TransactionType) )
				return 590;

			int ret    = 10;
			payRef     = "";
			strResult  = "";
			err        = "";
			resultMsg  = "failed";
			resultCode = "981";

			Tools.LogInfo("TokenPayment/10","Merchant Ref=" + payment.MerchantReference,10,this);

			try
			{
				ret                        = 20;
				StripeConfiguration.ApiKey = payment.ProviderPassword; // Secret key

				ret                        = 30;
				var paymentIntentOptions   = new PaymentIntentCreateOptions
				{
 
					Amount              = payment.PaymentAmount,
					Currency            = payment.CurrencyCode.ToLower(), // Must be "usd" not "USD"
					StatementDescriptor = payment.PaymentDescription,
					Customer            = payment.CustomerID,
					PaymentMethod       = payment.PaymentMethodID,
					Description         = payment.MerchantReference,
					ConfirmationMethod  = "manual"
//					SetupFutureUsage    = "off_session",
//					Confirm             = true,
//					PaymentMethodData   = new PaymentIntentPaymentMethodDataOptions
//					{
//						Type = "card"
//					},
				};
				ret                      = 40;
				var paymentIntentService = new PaymentIntentService();
				var paymentIntent        = paymentIntentService.Create(paymentIntentOptions);	
				err                      = err + ", paymentIntentId="+Tools.NullToString(paymentIntent.Id);

				ret                  = 50;
				var confirmOptions   = new PaymentIntentConfirmOptions
				{
					PaymentMethod     = payment.PaymentMethodID
				};
				ret                  = 60;
				var paymentConfirm   = paymentIntentService.Confirm(paymentIntent.Id,confirmOptions);
				payRef               = paymentConfirm.Id;
				err                  = err + ", paymentConfirmId="+Tools.NullToString(payRef);

				ret                  = 70;
				strResult            = paymentConfirm.StripeResponse.Content;
				resultMsg            = Tools.JSONValue(strResult,"status");
				resultCode           = paymentConfirm.StripeResponse.ToString();
				int k                = resultCode.ToUpper().IndexOf(" STATUS=");
				ret                  = 80;
				err                  = err + ", StripeResponse="+Tools.NullToString(resultCode);

				if ( k > 0 )
				{
					resultCode = resultCode.Substring(k+8).Trim();
					k          = resultCode.IndexOf(" ");
					if ( k > 0 )
						resultCode = resultCode.Substring(0,k);
				}
				else
					resultCode = "989";

				err                  = err + ", strResult="+Tools.NullToString(strResult);
				ret                  = 100;
				paymentIntentService = null;
				paymentIntent        = null;
				confirmOptions       = null;
				paymentConfirm       = null;

				if ( resultCode.StartsWith("2") && payRef.Length > 0 )
					ret = 0;
				else
					Tools.LogInfo ("TokenPayment/197",err.Substring(2),231,this);
			}
			catch (Exception ex)
			{
				err = "Ret=" + ret.ToString() + err;
				Tools.LogInfo     ("TokenPayment/198",err,255,this);
				Tools.LogException("TokenPayment/199",err,ex ,this);
			}
			return ret;
		}

		public override int CardPayment(Payment payment)
		{
			int ret = 10;

			try
			{
				Tools.LogInfo("CardPayment/10","Merchant Ref=" + payment.MerchantReference,10,this);
			}
			catch (Exception ex)
			{
				Tools.LogInfo     ("CardPayment/198","Ret="+ret.ToString(),255,this);
				Tools.LogException("CardPayment1/99","Ret="+ret.ToString(),ex ,this);
			}
			return ret;
		}

		private int TestService(byte live=0)
      {
			try
			{
			}
			catch (Exception ex)
			{
				Tools.LogException("TestService/199","",ex,this);
			}
			return 99040;
		}

		public TransactionStripe() : base()
		{
			base.LoadBureauDetails(Constants.PaymentProvider.Stripe);
			err       = "";
			xmlSent   = "";
			strResult = "";
			xmlResult = null;
		}
	}
}
