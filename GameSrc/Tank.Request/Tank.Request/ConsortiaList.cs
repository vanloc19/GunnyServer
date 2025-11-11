using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ConsortiaList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			XElement node = new XElement("Result");
			int total = 0;
			try
			{
				int page = int.Parse(context.Request["page"]);
				int size = int.Parse(context.Request["size"]);
				int order = int.Parse(context.Request["order"]);
				int consortiaID = int.Parse(context.Request["consortiaID"]);
				string name = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
				int level = int.Parse(context.Request["level"]);
				int openApply = int.Parse(context.Request["openApply"]);
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					ConsortiaInfo[] consortiaPage = consortiaBussiness.GetConsortiaPage(page, size, ref total, order, name, consortiaID, level, openApply);
					foreach (ConsortiaInfo info in consortiaPage)
					{
						node.Add(FlashUtils.CreateConsortiaInfo(info));
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("ConsortiaList", ex);
			}
			node.Add(new XAttribute("total", total));
			node.Add(new XAttribute("value", flag));
			node.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.BinaryWrite(StaticFunction.Compress(node.ToString(check: false)));
		}
	}
}
