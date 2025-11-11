using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Bussiness.CenterService;
using log4net;

namespace Tank.Request
{
	public class ExperienceRate : Page
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected HtmlForm form1;

		public static string GetAdminIP => ConfigurationManager.AppSettings["AdminIP"];

		public static bool ValidLoginIP(string ip)
		{
			string getAdminIp = GetAdminIP;
			if (!string.IsNullOrEmpty(getAdminIp) && !getAdminIp.Split('|').Contains(ip))
			{
				return false;
			}
			return true;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			int num1 = 2;
			try
			{
				int num2 = int.Parse(Context.Request["serverId"]);
				string userHostAddress = Context.Request.UserHostAddress;;
				if (ValidLoginIP(userHostAddress))//(context.Request.UserHostAddress;))
				{
					using (CenterServiceClient centerServiceClient = new CenterServiceClient())
					{
						num1 = centerServiceClient.ExperienceRateUpdate(num2);
					}
				}
			}
			catch (Exception ex)
			{
				log.Error("ExperienceRateUpdate:", ex);
			}
			base.Response.Write(num1);
		}
	}
}
