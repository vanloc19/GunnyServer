using System.Reflection;
using System.Web;
using log4net;

namespace Tank.Request
{
	public class CreatShortCut : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			_ = context.Request["gameurl"];
			context.Response.Write("Not support right now");
		}
	}
}
