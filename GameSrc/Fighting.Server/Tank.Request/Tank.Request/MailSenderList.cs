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
	public class MailSenderList : IHttpHandler
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
				int num = int.Parse(context.Request.QueryString["selfID"]);
				if (num != 0)
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						MailInfo[] mailBySenderID = playerBussiness.GetMailBySenderID(num);
						foreach (MailInfo mailInfo in mailBySenderID)
						{
							xelement.Add(FlashUtils.CreateMailInfo(mailInfo, "Item"));
						}
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("MailSenderList", ex);
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.BinaryWrite(StaticFunction.Compress(xelement.ToString(check: false)));
		}
	}
}
