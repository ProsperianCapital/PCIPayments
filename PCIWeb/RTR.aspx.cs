using System;
using System.IO;
using System.Configuration;
using System.Text;
using System.Web.UI.WebControls;
using PCIBusiness;

namespace PCIWeb
{
	public partial class RTR : System.Web.UI.Page
	{
		private byte systemStatus;
		private int  timeOut;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				systemStatus = System.Convert.ToByte(Tools.ConfigValue("SystemStatus"));
			}
			catch
			{
				systemStatus = 0;
			}
			lblSStatus.Text     = ( systemStatus == 0 ? "Active" : "Disabled" );
			btnProcess1.Enabled = ( systemStatus == 0 );
			btnProcess2.Enabled = ( systemStatus == 0 );
			lblTest.Text        = "";
			lblError.Text       = "";

			ProviderDetails();

			if ( ! Page.IsPostBack )
			{
				lblVersion.Text = "Version " + PCIBusiness.SystemDetails.AppVersion;

				PCIBusiness.DBConn       conn       = null;
				ConnectionStringSettings db         = ConfigurationManager.ConnectionStrings["DBConn"];
				string[]                 connString = PCIBusiness.Tools.NullToString(db.ConnectionString).Split(';');
				int                      k;

				lblSQLServer.Text = "";
				lblSQLDB.Text     = "";
				lblSQLUser.Text   = "";
				lblSQLStatus.Text = "";
				lblSMode.Text     = Tools.ConfigValue("SystemMode");

				foreach ( string x in connString )
				{
					k = x.ToUpper().IndexOf("SERVER=");
					if ( k >= 0 )
					{
						lblSQLServer.Text = x.Substring(k+7);
						continue;
					}
					k = x.ToUpper().IndexOf("DATABASE=");
					if ( k >= 0 )
					{
						lblSQLDB.Text = x.Substring(k+9);
						continue;
					}
					k = x.ToUpper().IndexOf("UID=");
					if ( k >= 0 )
						lblSQLUser.Text = x.Substring(k+4);
				}
				if ( PCIBusiness.Tools.OpenDB(ref conn) )
					lblSQLStatus.Text = "Connected";
				else
					lblSQLStatus.Text = "<span class='Red'>Cannot connect</span>";
				PCIBusiness.Tools.CloseDB(ref conn);
				conn = null;
			}			
		}

		private void ProviderDetails()
		{
			string bureau = lstProvider.SelectedValue.Trim();
			if ( bureau.Length > 0 )
				using (Payments payments = new Payments())
				{
					Provider provider    = payments.Summary(bureau);
					lblBureauCode.Text   = provider.BureauCode;
					lblBureauURL.Text    = provider.BureauURL;
					lblBureauStatus.Text = provider.BureauStatusName;
					lblMerchantKey.Text  = provider.MerchantKey;
					lblMerchantUser.Text = provider.MerchantUserID;
					lblCards.Text        = provider.CardsToBeTokenized.ToString();
					lblPayments.Text     = provider.PaymentsToBeProcessed.ToString();
				}

//			if ( lstProvider.SelectedValue == PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.PayU) )
//				using ( PCIBusiness.TransactionPayU x = new PCIBusiness.TransactionPayU() )
//				{
//	//				lblBureauCode.Text   = x.BureauCode;
//					lblBureauURL.Text    = x.URL;
//					lblBureauStatus.Text = x.BureauStatus;
//				}
//			else if ( lstProvider.SelectedValue == PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.T24) )
//				using ( PCIBusiness.TransactionT24 x = new PCIBusiness.TransactionT24() )
//				{
//					lblBureauCode.Text   = x.BureauCode;
//					lblBureauURL.Text    = x.URL;
//					lblBureauStatus.Text = x.BureauStatus;
//				}
//			else if ( lstProvider.SelectedValue == PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.Ikajo) )
//				using ( PCIBusiness.TransactionIkajo x = new PCIBusiness.TransactionIkajo() )
//				{
//					lblBureauCode.Text   = x.BureauCode;
//					lblBureauURL.Text    = x.URL;
//					lblBureauStatus.Text = x.BureauStatus;
//				}
//			else
//			{
//				lblBureauCode.Text   = "";
//				lblBureauURL.Text    = "";
//				lblBureauStatus.Text = "";
//			}
		}

		protected void btnProcess1_Click(Object sender, EventArgs e)
		{
			if ( systemStatus == 0 )
				Process(1);
		}

		protected void btnProcess2_Click(Object sender, EventArgs e)
		{
			if ( systemStatus == 0 )
				Process(2);
		}

		private void Process(byte mode)
		{
			try
			{
				string provider = lstProvider.SelectedValue;
				PCIBusiness.Tools.LogInfo("RTR.Process/5","Started, provider '" + provider + "'");

				using (PCIBusiness.Payments payments = new PCIBusiness.Payments())
				{
					int k         = payments.ProcessCards(provider,mode);
					lblError.Text = (payments.CountSucceeded+payments.CountFailed).ToString() + " payment(s) completed : " + payments.CountSucceeded.ToString() + " succeeded, " + payments.CountFailed.ToString() + " failed";
				//	payments.ProcessTokens(provider,mode);
				}
				PCIBusiness.Tools.LogInfo("RTR.Process/10","Finished");
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("RTR.Process/15","",ex);
			}
		}
		private void ShowFile(string fileName)
		{
			StreamReader fHandle = null;
			try
			{
				int k    = fileName.LastIndexOf(".");
				fileName = fileName.Substring(0,k) + "-" + PCIBusiness.Tools.DateToString(System.DateTime.Now,7) + fileName.Substring(k);
				fHandle  = File.OpenText(fileName);
				string h = fHandle.ReadToEnd().Trim().Replace("<","&lt;").Replace(">","&gt;");
				h        = h.Replace(Environment.NewLine,"</p><p>");
				if ( ! h.EndsWith("<p>") )
					h = h + "<p>";
				lblTest.Text = "<div class='Error'>Log File : " + fileName + "</div><p>" + h + "&nbsp;</p>";
			}
			catch
			{
				lblError.Text = "File " + fileName + " could not be found";
			}
			finally
			{
				if ( fHandle != null )
					fHandle.Close();
				fHandle = null;
			}
		}


		protected void btnSQL_Click(Object sender, EventArgs e)
		{
			lblTest.Text = Tools.SQLDebug(txtTest.Text) + "<p>&nbsp;</p>";
		}

		protected void btnInfo_Click(Object sender, EventArgs e)
		{
			ShowFile(PCIBusiness.Tools.ConfigValue("LogFileInfo"));
		}
		protected void btnError_Click(Object sender, EventArgs e)
		{
			ShowFile(PCIBusiness.Tools.ConfigValue("LogFileErrors"));
		}

		protected void btnConfig_Click(Object sender, EventArgs e)
		{
			try
			{
				string folder  = "System Version = " + PCIBusiness.SystemDetails.AppVersion + "<br />"
				               + "Server.MapPath = " + Server.MapPath("") + "<br />"
				               + "Request.Url.AbsoluteUri = " + Request.Url.AbsoluteUri + "<br />"
				               + "Request.Url.AbsolutePath = " + Request.Url.AbsolutePath + "<br />"
				               + "Request.Url.LocalPath = " + Request.Url.LocalPath + "<br />"
				               + "Request.Url.PathAndQuery = " + Request.Url.PathAndQuery + "<br />"
				               + "Request.RawUrl = " + Request.RawUrl + "<br />"
				               + "Request.PhysicalApplicationPath = " + Request.PhysicalApplicationPath + "<br />"
				               + "System Mode = " + PCIBusiness.Tools.ConfigValue("SystemMode") + "<br />"
				               + "Page timeout = " + Server.ScriptTimeout.ToString() + " seconds<br />"
				               + "Error Logs folder/file = " + PCIBusiness.Tools.ConfigValue("LogFileErrors") + "<br />"
				               + "Info Logs folder/file = " + PCIBusiness.Tools.ConfigValue("LogFileInfo") + "<br />";
				System.Configuration.ConnectionStringSettings db  = System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"];
				folder         = folder + "DB Connection [DBConn] = " + ( db == null ? "" : db.ConnectionString ) + "<p>&nbsp;</p>";
				lblTest.Text   = folder;
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("RTR.btnProcess_Click","",ex);
			}
		}

		public RTR() : base()
		{
			timeOut              = Server.ScriptTimeout;
			Server.ScriptTimeout = 1800; // 30 minutes
		}

		~RTR()
		{
			Server.ScriptTimeout = timeOut;
		}
	}
}
