using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Bussiness;
using Bussiness.Interface;
using log4net;

namespace Tank.Request
{
	public class UserNameCheck : Page
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected void Page_Load(object sender, EventArgs e)
		{
			int num = 1;
			try
			{
				string str1 = HttpUtility.UrlDecode(base.Request["username"]);
				string str2 = ((base.Request["site"] == null) ? "" : HttpUtility.UrlDecode(base.Request["site"]));
				if (!string.IsNullOrEmpty(str1))
				{
					string nameBySite = BaseInterface.GetNameBySite(str1, str2);
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						num = ((playerBussiness.GetUserSingleByUserName(nameBySite) == null) ? 2 : 0);
					}
				}
			}
			catch (Exception ex)
			{
				log.Error("UserNameCheck:", ex);
			}
			base.Response.Write(num);
		}
	}
}
