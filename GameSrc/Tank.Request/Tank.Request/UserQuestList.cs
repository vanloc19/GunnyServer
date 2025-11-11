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
	public class UserQuestList : IHttpHandler
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
				int num = int.Parse(context.Request["ID"]);
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					QuestDataInfo[] userQuest = playerBussiness.GetUserQuest(num);
					foreach (QuestDataInfo questDataInfo in userQuest)
					{
						xelement.Add(FlashUtils.CreateQuestDataInfo(questDataInfo));
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("UserQuestList", ex);
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
