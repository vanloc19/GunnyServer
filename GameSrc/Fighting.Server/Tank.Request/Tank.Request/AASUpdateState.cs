using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Bussiness.CenterService;
using log4net;

namespace Tank.Request
{
	public class AASUpdateState : Page
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
			int num = 2;
			try
			{
				bool flag = bool.Parse(base.Request["state"]);
				string userHostAddress = Context.Request.UserHostAddress;
				if (ValidLoginIP(userHostAddress))//(context.Request.UserHostAddress;))
				{
					using (CenterServiceClient centerServiceClient = new CenterServiceClient())
					{
						num = ((!centerServiceClient.AASUpdateState(flag)) ? 1 : 0);
					}
				}
			}
			catch (Exception ex)
			{
				log.Error("ASSUpdateState:", ex);
			}
			base.Response.Write(num);
		}
	}
}
