using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Services;
using log4net;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class PayTransit : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string site = "";

		public string PayURL => ConfigurationManager.AppSettings["PayURL_" + site];

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			string str = "";
			string format = string.Empty;
			try
			{
				if (!string.IsNullOrEmpty(context.Request["username"]))
				{
					str = HttpUtility.UrlDecode(context.Request["username"].Trim());
				}
				site = ((context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]).ToLower());
				if (!string.IsNullOrEmpty(site))
				{
					format = PayURL;
					int num = str.IndexOf('_');
					if (num != -1)
					{
						str = str.Substring(num + 1, str.Length - num - 1);
					}
				}
				if (string.IsNullOrEmpty(format))
				{
					format = ConfigurationManager.AppSettings["PayURL"];
				}
				context.Response.Redirect(string.Format(format, str, site), endResponse: false);
			}
			catch (Exception ex)
			{
				log.Error("PayTransit:", ex);
			}
		}
	}
}
