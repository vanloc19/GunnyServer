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
	public class CelebByAchievementPointDayList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			// Prevent caching to ensure fresh data
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			context.Response.ContentType = "application/octet-stream";

			// Return same data as CelebByAchievementPointList (Tổng) - always fresh from database
			XElement result = CelebByAchievementPointList.BuildXml();
			if (result != null)
			{
				context.Response.BinaryWrite(StaticFunction.Compress(result.ToString(check: false)));
			}
			else
			{
				context.Response.Write("CelebByAchievementPointDayList Fail!");
			}
		}

		public static string Build(HttpContext context)
		{
			return Build();
		}

		public static string Build()
		{
			// Use same data as CelebByAchievementPointList (Tổng) but create CelebByAchievementPointDayList.xml file
			XElement result = CelebByAchievementPointList.BuildXml();
			return csFunction.CreateCompressXml(result, "CelebByAchievementPointDayList", isCompress: true);
		}

		public static XElement BuildXml()
		{
			// Use same data as CelebByAchievementPointList (Tổng)
			return CelebByAchievementPointList.BuildXml();
		}
	}
}
