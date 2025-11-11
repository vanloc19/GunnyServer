using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class AccountRegister : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			XElement xelement = new XElement("Result");
			bool flag1 = false;
			try
			{
				string str1 = HttpUtility.UrlDecode(context.Request["username"]);
				string str2 = HttpUtility.UrlDecode(context.Request["password"]);
				string str3 = HttpUtility.UrlDecode(context.Request["password"]);
				bool flag2 = false;
				int num1 = 100;
				int num2 = 100;
				int num3 = 100;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					flag1 = playerBussiness.RegisterUser(str1, str2, str3, flag2, num1, num2, num3);
				}
			}
			catch (Exception ex)
			{
				log.Error("RegisterResult", ex);
			}
			finally
			{
				xelement.Add(new XAttribute("value", "vl"));
				xelement.Add(new XAttribute("message", flag1));
				context.Response.ContentType = "text/plain";
				context.Response.Write(xelement.ToString(check: false));
			}
		}
	}
}
