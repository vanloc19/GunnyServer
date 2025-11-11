using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Xml.Linq;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class IMListLoad : IHttpHandler, IRequiresSessionState
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			XElement xelement1 = new XElement("Result");
			try
			{
				int num = Convert.ToInt32(context.Request["id"]);
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					FriendInfo[] friendsAll = playerBussiness.GetFriendsAll(num);
					XElement xelement2 = new XElement("customList", new XAttribute("ID", 0), new XAttribute("Name", "Lista de Amigos"));
					xelement1.Add(xelement2);
					FriendInfo[] array = friendsAll;
					foreach (FriendInfo friendInfo in array)
					{
						XElement xelement3 = new XElement("Item", new XAttribute("ID", friendInfo.FriendID), new XAttribute("NickName", friendInfo.NickName), new XAttribute("Birthday", DateTime.Now), new XAttribute("ApprenticeshipState", 0), new XAttribute("LoginName", friendInfo.UserName), new XAttribute("Style", friendInfo.Style), new XAttribute("Sex", friendInfo.Sex == 1), new XAttribute("Colors", friendInfo.Colors), new XAttribute("Grade", friendInfo.Grade), new XAttribute("Hide", friendInfo.Hide), new XAttribute("ConsortiaName", friendInfo.ConsortiaName), new XAttribute("TotalCount", friendInfo.Total), new XAttribute("EscapeCount", friendInfo.Escape), new XAttribute("WinCount", friendInfo.Win), new XAttribute("Offer", friendInfo.Offer), new XAttribute("Relation", friendInfo.Relation), new XAttribute("Repute", friendInfo.Repute), new XAttribute("State", (friendInfo.State == 1) ? 1 : 0), new XAttribute("Nimbus", friendInfo.Nimbus), new XAttribute("DutyName", friendInfo.DutyName));
						xelement1.Add(xelement3);
					}
				}
				flag = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("IMListLoad", ex);
			}
			xelement1.Add(new XAttribute("value", flag));
			xelement1.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement1.ToString(check: false));
		}
	}
}
