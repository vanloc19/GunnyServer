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
	public class ConsortiaEquipControlList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			XElement xelement = new XElement("Result");
			int num1 = 0;
			try
			{
				int num2 = 1;
				int num3 = 10;
				int num4 = 1;
				int num5 = int.Parse(context.Request["consortiaID"]);
				int num6 = int.Parse(context.Request["level"]);
				int num7 = int.Parse(context.Request["type"]);
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					ConsortiaEquipControlInfo[] consortiaEquipControlPage = consortiaBussiness.GetConsortiaEquipControlPage(num2, num3, ref num1, num4, num5, num6, num7);
					foreach (ConsortiaEquipControlInfo equipControlInfo in consortiaEquipControlPage)
					{
						xelement.Add(FlashUtils.CreateConsortiaEquipControlInfo(equipControlInfo));
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("ConsortiaList", ex);
			}
			xelement.Add(new XAttribute("total", num1));
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
