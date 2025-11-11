using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Bussiness;
using Bussiness.CenterService;
using Bussiness.Interface;
using log4net;
using SqlDataProvider.Data;

namespace Tank.Request
{
	public class SendMailAndItem : Page
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static string GetChargeIP => ConfigurationSettings.AppSettings["AdminIP"];

		protected void Page_Load(object sender, EventArgs e)
		{
			int num = 1;
			try
			{
				string userHostAddress = Context.Request.UserHostAddress;
				if (ValidLoginIP(userHostAddress))
				{
					string content = HttpUtility.UrlDecode(base.Request["content"]);
					string title = HttpUtility.UrlDecode(base.Request["title"]);
					string userID = HttpUtility.UrlDecode(base.Request["user_id"]);
					string gold = HttpUtility.UrlDecode(base.Request["gold"]);
					string money = HttpUtility.UrlDecode(base.Request["money"]);
					string str = HttpUtility.UrlDecode(base.Request["str"]);
					PlayerBussiness pb = new PlayerBussiness();
					if (pb.SendMailAndItem(title, content, Int32.Parse(userID), Int32.Parse(gold), Int32.Parse(money), str) == 0)
					{
						base.Response.Write(num);
						return;
					}
				}
				else
				{
					num = 5;
				}
			}
			catch (Exception ex)
			{
				log.Error("SendMailAndItem:", ex);
			}
			base.Response.Write(num + Context.Request.UserHostAddress);
		}

		public static bool ValidLoginIP(string ip)
		{
			string getChargeIp = GetChargeIP;
			int num = (string.IsNullOrEmpty(getChargeIp) ? 1 : (getChargeIp.Split('|').Contains(ip) ? 1 : 0));
			return num != 0;
		}
	}
}
