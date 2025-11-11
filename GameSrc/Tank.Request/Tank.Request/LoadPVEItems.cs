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
	public class LoadPVEItems : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
			{
				context.Response.Write(Build(context));
			}
			else
			{
				context.Response.Write("IP is not valid!");
			}
		}

		public static string Build(HttpContext context)
		{
			bool flag = false;
			string str = "Fail";
			XElement result = new XElement("Result");
			try
			{
				using (PveBussiness pveBussiness = new PveBussiness())
				{
					PveInfo[] allPveInfos = pveBussiness.GetAllPveInfos();
					foreach (PveInfo allPveInfo in allPveInfos)
					{
						result.Add(FlashUtils.CreatePveInfo(allPveInfo));
					}
				}
				flag = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("LoadPVEItems", ex);
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "LoadPVEItems", isCompress: true);
		}
	}
}
