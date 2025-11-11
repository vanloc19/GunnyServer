using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using log4net;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class AdvanceQuestTime : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			XElement xElement = new XElement("Result");
			xElement.Add(new XAttribute("value", flag));
			xElement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write($"0,{DateTime.Now},0");
		}
	}
}
