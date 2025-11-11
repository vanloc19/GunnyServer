using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Bussiness.CenterService;
using log4net;

namespace Tank.Request
{
	public class SystemNotice : Page
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static string GetChargeIP => ConfigurationManager.AppSettings["AdminIP"];

		public static bool ValidLoginIP(string ip)
		{
			string getChargeIp = GetChargeIP;
			if (!string.IsNullOrEmpty(getChargeIp) && !getChargeIp.Split('|').Contains(ip))
			{
				return false;
			}
			return true;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			int num = 1;
			try
			{
				string userHostAddress = Context.Request.UserHostAddress;;
				if (ValidLoginIP(userHostAddress))//(context.Request.UserHostAddress;))
				{
					string str = HttpUtility.UrlDecode(base.Request["content"]);
					if (!string.IsNullOrEmpty(str))
					{
						using (CenterServiceClient centerServiceClient = new CenterServiceClient())
						{
							if (centerServiceClient.SystemNotice(str))
							{
								num = 0;
							}
						}
					}
				}
				else
				{
					num = 2;
				}
			}
			catch (Exception ex)
			{
				log.Error("SystemNotice:", ex);
			}
			base.Response.Write(num);
		}
	}
}
