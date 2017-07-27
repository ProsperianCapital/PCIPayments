using System;
using System.Text;
using System.Web.UI.WebControls;

namespace PCIWeb
{
	public partial class RTR : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblConfig.Text  = "";
			lblError.Text   = "";
			lblVersion.Text = "Version " + PCIBusiness.SystemDetails.AppVersion;
		}

		protected void btnProcess1_Click(Object sender, EventArgs e)
		{
            Process(1);
        }

		protected void btnProcess2_Click(Object sender, EventArgs e)
		{
            Process(2);
        }

		protected void btnProcess3_Click(Object sender, EventArgs e)
		{
            Process(3);
        }
		private void Process(byte mode)
		{
			try
			{
				string provider = lstProvider.SelectedValue;
				PCIBusiness.Tools.LogInfo("Proces/5","Started, provider '" + provider + "'");

				using (PCIBusiness.PaymentSchedule paymentSchedule = new PCIBusiness.PaymentSchedule())
				{
					int k         = paymentSchedule.ProcessCards(provider,mode);
					lblError.Text = k.ToString() + " payment(s) completed : " + paymentSchedule.CountSucceeded.ToString() + " succeeded, " + paymentSchedule.CountFailed.ToString() + " failed";
				//	paymentSchedule.ProcessTokens(provider,mode);
				}
				PCIBusiness.Tools.LogInfo("Process/10","Finished");
			}
			catch (Exception ex)
			{
				PCIBusiness.Tools.LogException("Process/15","",ex);
			}
		}

		protected void btnConfig_Click(Object sender, EventArgs e)
		{
			try
			{
                string folder  = "<hr />Server.MapPath = " + Server.MapPath("") + "<br />"
                               + "Request.Url.AbsoluteUri = " + Request.Url.AbsoluteUri + "<br />"
                               + "Request.Url.AbsolutePath = " + Request.Url.AbsolutePath + "<br />"
                               + "Request.Url.LocalPath = " + Request.Url.LocalPath + "<br />"
                               + "Request.Url.PathAndQuery = " + Request.Url.PathAndQuery + "<br />"
                               + "Request.RawUrl = " + Request.RawUrl + "<br />"
                               + "Request.PhysicalApplicationPath = " + Request.PhysicalApplicationPath + "<br />";
                System.Configuration.ConnectionStringSettings db  = System.Configuration.ConfigurationManager.ConnectionStrings["TestDB"];
                folder         = folder + "DB Connection [TestDB] = " + db.ConnectionString + "<br />";
                db             = System.Configuration.ConfigurationManager.ConnectionStrings["LiveDB"];
                folder         = folder + "DB Connection [LiveDB] = " + db.ConnectionString + "<br />";
                lblConfig.Text = folder;
            }
            catch (Exception ex)
            {
				PCIBusiness.Tools.LogException("RTR.btnProcess_Click/15","",ex);
            }
		}
	}
}
