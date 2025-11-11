using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Bussiness.CenterService;
using Bussiness.Interface;
using log4net;

namespace Tank.Request
{
	public class NoticeServerUpdate : Page
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
			int num1 = 2;
			try
			{
				int num2 = int.Parse(Context.Request["serverID"]);
				int num3 = int.Parse(Context.Request["type"]);
				string userHostAddress = Context.Request.UserHostAddress;;
				if (ValidLoginIP(userHostAddress))//(context.Request.UserHostAddress;))
				{
					using (CenterServiceClient centerServiceClient = new CenterServiceClient())
					{
						num1 = centerServiceClient.NoticeServerUpdate(num2, num3);
					}
					if (num3 == 5 && num1 == 0)
					{
						num1 = HandleServerMapUpdate();
					}
				}
				else
				{
					num1 = 5;
				}
			}
			catch (Exception ex)
			{
				log.Error("ExperienceRateUpdate:", ex);
				num1 = 4;
			}
			base.Response.Write(num1);
		}

		private int HandleServerMapUpdate()
		{
			if (!BaseInterface.RequestContent("http://" + HttpContext.Current.Request.Url.Authority.ToString() + "/MapServerList.ashx").Contains("Success"))
			{
				return 3;
			}
			return 0;
		}
	}
}
