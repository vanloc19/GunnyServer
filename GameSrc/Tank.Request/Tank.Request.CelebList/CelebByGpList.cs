using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using log4net;
using Road.Flash;
using Bussiness;

namespace Tank.Request.CelebList
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class CelebByGpList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			log.WarnFormat("RANKING HANDLER - CelebByGpList.ProcessRequest called from IP: {0}", context.Request.UserHostAddress);
			// Prevent caching to ensure fresh data from database
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
			context.Response.Cache.SetMaxAge(TimeSpan.Zero);
			context.Response.ContentType = "application/octet-stream";
			context.Response.AddHeader("Pragma", "no-cache");
			context.Response.AddHeader("Expires", "0");

			// Return compressed XML directly to client (always fresh from database)
			XElement result = BuildXml();
			if (result != null)
			{
				log.WarnFormat("RANKING HANDLER - CelebByGpList result has {0} items", result.Elements().Count());
				// Add timestamp to ensure client knows this is fresh data
				result.Add(new XAttribute("lastUpdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
				// Use ToString(check: false) extension method from Bussiness.XmlExtends
				context.Response.BinaryWrite(StaticFunction.Compress(result.ToString(check: false)));
				log.WarnFormat("RANKING HANDLER - CelebByGpList response sent successfully");
			}
			else
			{
				log.Error("RANKING HANDLER - CelebByGpList BuildXml returned null!");
				context.Response.Write("CelebByGpList Fail!");
			}
		}

		public static string Build(HttpContext context)
		{
			// Removed IP check to allow public access from game client
			// if (!csFunction.ValidAdminIP(context.Request.UserHostAddress))
			// {
			//     return "CelebByGpList Fail!";
			// }
			return Build();
		}

		public static string Build()
		{
			// Order 0 = GP desc (total GP ranking)
			// Create both compressed and uncompressed versions
			return csFunction.BuildCelebUsers("CelebByGpList", 0, "CelebByGpList_Out");
		}

		public static XElement BuildXml()
		{
			// Build XML directly from database for client response (order 0 = GP desc)
			return csFunction.BuildCelebUsersXml(0);
		}
	}
}
