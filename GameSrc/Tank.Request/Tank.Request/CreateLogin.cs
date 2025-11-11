using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Bussiness.Interface;
using log4net;

namespace Tank.Request
{
	public class CreateLogin : Page
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static string GetLoginIP => ConfigurationManager.AppSettings["LoginIP"];

		public static bool ValidLoginIP(string ip)
		{
			string getLoginIp = GetLoginIP;
			if (!string.IsNullOrEmpty(getLoginIp) && !getLoginIp.Split('|').Contains(ip))
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
				string str1 = HttpUtility.UrlDecode(base.Request["content"]);
				string str2 = ((base.Request["site"] == null) ? "" : HttpUtility.UrlDecode(base.Request["site"]).ToLower());
				string[] strArray = BaseInterface.CreateInterface().UnEncryptLogin(str1, ref num, str2);
				if (strArray.Length > 3)
				{
					string lower1 = strArray[0].Trim().ToLower();
					string lower2 = strArray[1].Trim().ToLower();
					if (!string.IsNullOrEmpty(lower1) && !string.IsNullOrEmpty(lower2))
					{
						PlayerManager.Add(BaseInterface.GetNameBySite(lower1, str2), lower2);
						num = 0;
					}
					else
					{
						num = -91010;
					}
				}
				else
				{
					num = -1900;
				}
			}
			catch (Exception ex)
			{
				log.Error("CreateLogin:", ex);
			}
			base.Response.Write(num);
		}
	}
}
