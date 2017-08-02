using System;
using System.Configuration;
using System.Text;
using System.Web.UI.WebControls;
using PCIBusiness;

namespace PCIWeb
{
	public partial class RTR : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblTest.Text    = "";
			lblError.Text   = "";
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
			if ( lstProvider.SelectedValue == PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.PayU) )
				lblProvider.Text = (new PCIBusiness.TransactionPayU()).ConnectionDetails(1);
			else if ( lstProvider.SelectedValue == PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.T24) )
				lblProvider.Text = (new PCIBusiness.TransactionT24()).ConnectionDetails(1);
			else if ( lstProvider.SelectedValue == PCIBusiness.Tools.BureauCode(PCIBusiness.Constants.PaymentProvider.Ikajo) )
				lblProvider.Text = (new PCIBusiness.TransactionIkajo()).ConnectionDetails(1);
			else
				lblProvider.Text = "";
		}

		protected void btnProcess1_Click(Object sender, EventArgs e)
		{
			Process(1);
		}

		protected void btnProcess2_Click(Object sender, EventArgs e)
		{
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
					lblError.Text = k.ToString() + " payment(s) completed : " + payments.CountSucceeded.ToString() + " succeeded, " + payments.CountFailed.ToString() + " failed";
				//	paymentSchedule.ProcessTokens(provider,mode);
				}
				PCIBusiness.Tools.LogInfo("RTR.Process/10","Finished");
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("RTR.Process/15","",ex);
			}
		}

		protected void btnSQL_Click(Object sender, EventArgs e)
		{
			lblTest.Text = Tools.SQLDebug(txtTest.Text);
		}

		protected void btnConfig_Click(Object sender, EventArgs e)
		{
			try
			{
				string folder  = "<hr />System Version = " + PCIBusiness.SystemDetails.AppVersion + "<br />"
				               + "Server.MapPath = " + Server.MapPath("") + "<br />"
				               + "Request.Url.AbsoluteUri = " + Request.Url.AbsoluteUri + "<br />"
				               + "Request.Url.AbsolutePath = " + Request.Url.AbsolutePath + "<br />"
				               + "Request.Url.LocalPath = " + Request.Url.LocalPath + "<br />"
				               + "Request.Url.PathAndQuery = " + Request.Url.PathAndQuery + "<br />"
				               + "Request.RawUrl = " + Request.RawUrl + "<br />"
				               + "Request.PhysicalApplicationPath = " + Request.PhysicalApplicationPath + "<br />"
				               + "Error Logs folder/file = " + PCIBusiness.Tools.ConfigValue("LogFileErrors") + "<br />"
				               + "Info Logs folder/file = " + PCIBusiness.Tools.ConfigValue("LogFileInfo") + "<br />";
				System.Configuration.ConnectionStringSettings db  = System.Configuration.ConfigurationManager.ConnectionStrings["TestDB"];
				folder         = folder + "DB Connection [DBConn] = " + ( db == null ? "" : db.ConnectionString ) + "<br />";
				lblTest.Text   = folder;
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("RTR.btnProcess_Click/15","",ex);
			}
		}
	}
}
