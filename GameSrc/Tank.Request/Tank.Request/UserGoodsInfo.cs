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
	public class UserGoodsInfo : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			XElement xelement = new XElement("Result");
			try
			{
				int num = int.Parse(context.Request.Params["ID"]);
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					ItemInfo userItemSingle = playerBussiness.GetUserItemSingle(num);
					xelement.Add(FlashUtils.CreateGoodsInfo(userItemSingle));
				}
				flag = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("UserGoodsInfo", ex);
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
