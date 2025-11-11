using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ConsortiaEquipControl : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			bool flag = false;
			string str = "Fail!";
			XElement xelement = new XElement("Result");
			int num1 = 0;
			try
			{
				int num2 = int.Parse(context.Request["consortiaID"]);
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					for (int index1 = 1; index1 < 3; index1++)
					{
						for (int index2 = 1; index2 < 11; index2++)
						{
							ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(num2, index2, index1);
							if (consortiaEuqipRiches != null)
							{
								xelement.Add(new XElement("Item", new XAttribute("type", consortiaEuqipRiches.Type), new XAttribute("level", consortiaEuqipRiches.Level), new XAttribute("riches", consortiaEuqipRiches.Riches)));
								num1++;
							}
						}
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("ConsortiaEventList", ex);
			}
			xelement.Add(new XAttribute("total", num1));
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
