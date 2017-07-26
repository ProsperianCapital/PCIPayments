using System;
using System.Text;
using System.Web.UI.WebControls;

namespace PCIWeb
{
	public partial class RTR : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void btnProcess_Click(Object sender, EventArgs e)
		{
			try
			{
                string provider = lstProvider.SelectedValue;
				PCIBusiness.Tools.LogInfo("RTR.btnProcess_Click/5","Started, provider '" + provider + "'");

				using (PCIBusiness.PaymentSchedule paymentSchedule = new PCIBusiness.PaymentSchedule())
				{
					paymentSchedule.ProcessCards (provider);
					paymentSchedule.ProcessTokens(provider);
				}
				PCIBusiness.Tools.LogInfo("RTR.btnProcess_Click/10","Finished");
            }
            catch (Exception ex)
            {
				PCIBusiness.Tools.LogException("RTR.btnProcess_Click/15","",ex);
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
                System.Configuration.ConnectionStringSettings db = System.Configuration.ConfigurationManager.ConnectionStrings["TestDB"];
                folder         = folder + "DB Connection = " + db.ConnectionString + "<br />";
                lblConfig.Text = folder;
            }
            catch (Exception ex)
            {
				PCIBusiness.Tools.LogException("RTR.btnProcess_Click/15","",ex);
            }
		}
	}
}
