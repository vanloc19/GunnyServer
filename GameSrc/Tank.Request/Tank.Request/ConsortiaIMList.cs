using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ConsortiaIMList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			int num1 = 0;
			XElement xelement = new XElement("Result");
			try
			{
				int num2 = int.Parse(context.Request["id"]);
				using (ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness())
				{
					ConsortiaInfo consortiaSingle = consortiaBussiness2.GetConsortiaSingle(num2);
					if (consortiaSingle != null)
					{
						xelement.Add(new XAttribute("Level", consortiaSingle.Level));
						xelement.Add(new XAttribute("Repute", consortiaSingle.Repute));
					}
				}
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					ConsortiaUserInfo[] consortiaUsersPage = consortiaBussiness.GetConsortiaUsersPage(1, 1000, ref num1, -1, num2, -1, -1);
					foreach (ConsortiaUserInfo consortiaUserInfo in consortiaUsersPage)
					{
						xelement.Add(FlashUtils.CreateConsortiaIMInfo(consortiaUserInfo));
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("ConsortiaIMList", ex);
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
