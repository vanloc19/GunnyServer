using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class IMFriendsGood : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str1 = "Fail!";
			XElement xelement1 = new XElement("Result");
			try
			{
				string str2 = context.Request["UserName"];
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					ArrayList friendsGood = playerBussiness.GetFriendsGood(str2);
					for (int index = 0; index < friendsGood.Count; index++)
					{
						XElement xelement2 = new XElement("Item", new XAttribute("UserName", friendsGood[index].ToString()));
						xelement1.Add(xelement2);
					}
				}
				flag = true;
				str1 = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("IMFriendsGood", ex);
			}
			xelement1.Add(new XAttribute("value", flag));
			xelement1.Add(new XAttribute("message", str1));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement1.ToString(check: false));
		}
	}
}
