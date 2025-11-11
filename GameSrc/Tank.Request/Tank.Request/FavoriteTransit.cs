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
	public class FavoriteTransit : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static string GetFavoriteUrl => ConfigurationManager.AppSettings["FavoriteUrl"];

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			try
			{
				string str1 = ((context.Request["username"] == null) ? "" : HttpUtility.UrlDecode(context.Request["username"]));
				string str2 = ((context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]).ToLower());
				string format = string.Empty;
				if (!string.IsNullOrEmpty(str2))
				{
					format = ConfigurationManager.AppSettings[$"FavoriteUrl_{str2}"];
					int num = str1.IndexOf('_');
					if (num != -1)
					{
						str1 = str1.Substring(num + 1, str1.Length - num - 1);
					}
				}
				if (string.IsNullOrEmpty(format))
				{
					format = GetFavoriteUrl;
				}
				context.Response.Redirect(string.Format(format, str1, str2), endResponse: false);
			}
			catch (Exception ex)
			{
				log.Error("FavoriteTransit:", ex);
			}
		}
	}
}
