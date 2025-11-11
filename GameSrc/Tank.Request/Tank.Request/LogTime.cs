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
	public class LogTime : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			XElement xelement = new XElement("Result");
			int num1 = 0;
			try
			{
				int num2 = int.Parse(context.Request["page"]);
				int num3 = int.Parse(context.Request["size"]);
				int num4 = int.Parse(context.Request["order"]);
				int num5 = int.Parse(context.Request["consortiaID"]);
				int num6 = int.Parse(context.Request["state"]);
				string str = csFunction.ConvertSql(HttpUtility.UrlDecode((context.Request["name"] == null) ? "" : context.Request["name"]));
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					ConsortiaAllyInfo[] consortiaAllyPage = consortiaBussiness.GetConsortiaAllyPage(num2, num3, ref num1, num4, num5, num6, str);
					foreach (ConsortiaAllyInfo consortiaAllyInfo in consortiaAllyPage)
					{
						xelement.Add(FlashUtils.CreateConsortiaAllyInfo(consortiaAllyInfo));
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
