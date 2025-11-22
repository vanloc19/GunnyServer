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
	public class CelebByDayGPList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			// Prevent caching to ensure fresh data
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			context.Response.ContentType = "application/octet-stream";

			// Return same data as CelebByGpList (Tổng) - always fresh from database
			XElement result = CelebByGpList.BuildXml();
			if (result != null)
			{
				context.Response.BinaryWrite(StaticFunction.Compress(result.ToString(check: false)));
			}
			else
			{
				context.Response.Write("CelebByDayGPList Fail!");
			}
		}

		public static string Build(HttpContext context)
		{
			// Removed IP check to allow public access from game client
			// if (!csFunction.ValidAdminIP(context.Request.UserHostAddress))
			// {
			//     return "CelebByDayGPList Fail!";
			// }
			return Build();
		}

		public static string Build()
		{
			// Use same data as CelebByGpList (Tổng) but create CelebByDayGPList.xml file
			XElement result = CelebByGpList.BuildXml();
			return csFunction.CreateCompressXml(result, "CelebByDayGPList", isCompress: true);
		}

		public static XElement BuildXml()
		{
			// Use same data as CelebByGpList (Tổng)
			return CelebByGpList.BuildXml();
		}
	}
}
