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
	public class MarryInfoPageList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag1 = false;
			string str1 = "Fail!";
			int num1 = 0;
			XElement xelement = new XElement("Result");
			try
			{
				int num2 = int.Parse(context.Request["page"]);
				string str2 = null;
				if (context.Request["name"] != null)
				{
					str2 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
				}
				bool flag2 = bool.Parse(context.Request["sex"]);
				int num3 = 12;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					MarryInfo[] marryInfoPage = playerBussiness.GetMarryInfoPage(num2, str2, flag2, num3, ref num1);
					for (int i = 0; i < marryInfoPage.Length; i++)
					{
						XElement marryInfo2 = FlashUtils.CreateMarryInfo(marryInfoPage[i]);
						xelement.Add(marryInfo2);
					}
					flag1 = true;
					str1 = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("MarryInfoPageList", ex);
			}
			xelement.Add(new XAttribute("total", num1));
			xelement.Add(new XAttribute("value", flag1));
			xelement.Add(new XAttribute("message", str1));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
