using System;
using System.Configuration;
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
	public class VisualizeItemLoad : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag1 = false;
			string str = "Fail!";
			bool flag2 = bool.Parse(context.Request["sex"]);
			XElement xelement = new XElement("Result");
			try
			{
				string appSetting = ConfigurationManager.AppSettings[flag2 ? "BoyVisualizeItem" : "GrilVisualizeItem"];
				xelement.Add(new XAttribute("content", appSetting));
				flag1 = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("VisualizeItemLoad", ex);
			}
			xelement.Add(new XAttribute("value", flag1));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
