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
	}
}
