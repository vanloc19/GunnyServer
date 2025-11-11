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
	public class MapServerList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write(Bulid(context));
		}

		public static string Bulid(HttpContext context)
		{
			bool flag = false;
			string str = "Fail";
			XElement result = new XElement("Result");
			try
			{
				using (MapBussiness mapBussiness = new MapBussiness())
				{
					ServerMapInfo[] allServerMap = mapBussiness.GetAllServerMap();
					foreach (ServerMapInfo allServer in allServerMap)
					{
						result.Add(FlashUtils.CreateMapServer(allServer));
					}
				}
				flag = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("MapServerList", ex);
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "MapServerList", isCompress: true);
		}
	}
}
