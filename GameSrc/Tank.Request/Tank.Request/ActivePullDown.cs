using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Bussiness.CenterService;
using log4net;
using Road.Flash;

namespace Tank.Request
{
	public class ActivePullDown : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
			int int32_1 = Convert.ToInt32(context.Request["selfid"]);
			int int32_2 = Convert.ToInt32(context.Request["activeID"]);
			_ = context.Request["key"];
			string src = context.Request["activeKey"];
			bool flag = false;
			string msg = "ActivePullDownHandler.Fail";
			string awardID = "";
			XElement node = new XElement("Result");
			if (src != "")
			{
				byte[] bytes = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, src);
				awardID = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			}
			try
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					if (playerBussiness.PullDown(int32_2, awardID, int32_1, ref msg) == 0)
					{
						using (CenterServiceClient centerServiceClient = new CenterServiceClient())
						{
							centerServiceClient.MailNotice(int32_1);
						}
					}
				}
				flag = true;
				msg = LanguageMgr.GetTranslation(msg);
			}
			catch (Exception ex)
			{
				log.Error("ActivePullDown", ex);
			}
			node.Add(new XAttribute("value", flag));
			node.Add(new XAttribute("message", msg));
			context.Response.ContentType = "text/plain";
			context.Response.Write(node.ToString(check: false));
		}
	}
}
