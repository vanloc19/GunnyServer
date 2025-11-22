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
	public class CelebByAchievementPointList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			// Prevent caching to ensure fresh data
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			context.Response.ContentType = "application/octet-stream";

			// Return compressed XML directly to client (always fresh from database)
			XElement result = BuildXml();
			if (result != null)
			{
				context.Response.BinaryWrite(StaticFunction.Compress(result.ToString(check: false)));
			}
			else
			{
				context.Response.Write("CelebByAchievementPointList Fail!");
			}
		}

		public static string Build(HttpContext context)
		{
			return Build();
		}

		public static string Build()
		{
			// Order 11 = AchievementPoint desc (total AchievementPoint ranking)
			XElement result = BuildXml();
			return csFunction.CreateCompressXml(result, "CelebByAchievementPointList", isCompress: true);
		}

		public static XElement BuildXml()
		{
			// Build XML directly from database for client response (order 11 = AchievementPoint desc)
			return csFunction.BuildCelebUsersXml(11);
		}
	}
}
