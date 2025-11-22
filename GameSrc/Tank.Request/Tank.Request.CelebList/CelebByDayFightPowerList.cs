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
	public class CelebByDayFightPowerList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			// Return compressed XML directly to client (always fresh from database)
			XElement result = BuildXml();
			if (result != null)
			{
				// Use ToString(check: false) extension method from Bussiness.XmlExtends
				context.Response.BinaryWrite(StaticFunction.Compress(result.ToString(check: false)));
			}
			else
			{
				context.Response.Write("CelebByDayFightPowerList Fail!");
			}
		}

		public static string Build(HttpContext context)
		{
			// Removed IP check to allow public access from game client
			// if (!csFunction.ValidAdminIP(context.Request.UserHostAddress))
			// {
			//     return "CelebByDayFightPowerList Fail!";
			// }
			return Build();
		}

		public static string Build()
		{
			// Order 6 = FightPower desc (total FightPower ranking)
			// Create both compressed and uncompressed versions
			return csFunction.BuildCelebUsers("CelebByDayFightPowerList", 6, "CelebByDayFightPowerList_Out");
		}

		public static XElement BuildXml()
		{
			// Build XML directly from database for client response (order 6 = FightPower desc)
			return csFunction.BuildCelebUsersXml(6);
		}
	}
}
