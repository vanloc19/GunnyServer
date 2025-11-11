using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class LoadUsersSort : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag1 = false;
			string str = "Fail!";
			XElement xelement1 = new XElement("Result");
			int num1 = 0;
			try
			{
				int num2 = 1;
				int num3 = 10;
				int num4 = int.Parse(context.Request["order"]);
				int num5 = -1;
				bool flag2 = false;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					PlayerInfo[] playerPage = playerBussiness.GetPlayerPage(num2, num3, ref num1, num4, num5, ref flag2);
					if (flag2)
					{
						PlayerInfo[] array = playerPage;
						foreach (PlayerInfo playerInfo in array)
						{
							XElement xelement2 = new XElement("Item", new XAttribute("ID", playerInfo.ID), new XAttribute("NickName", (playerInfo.NickName == null) ? "" : playerInfo.NickName), new XAttribute("Grade", playerInfo.Grade), new XAttribute("Colors", (playerInfo.Colors == null) ? "" : playerInfo.Colors), new XAttribute("Skin", (playerInfo.Skin == null) ? "" : playerInfo.Skin), new XAttribute("Sex", playerInfo.Sex), new XAttribute("Style", (playerInfo.Style == null) ? "" : playerInfo.Style), new XAttribute("ConsortiaName", (playerInfo.ConsortiaName == null) ? "" : playerInfo.ConsortiaName), new XAttribute("Hide", playerInfo.Hide), new XAttribute("Offer", playerInfo.Offer), new XAttribute("ReputeOffer", playerInfo.ReputeOffer), new XAttribute("ConsortiaHonor", playerInfo.ConsortiaHonor), new XAttribute("ConsortiaLevel", playerInfo.ConsortiaLevel), new XAttribute("ConsortiaRepute", playerInfo.ConsortiaRepute), new XAttribute("WinCount", playerInfo.Win), new XAttribute("TotalCount", playerInfo.Total), new XAttribute("EscapeCount", playerInfo.Escape), new XAttribute("Repute", playerInfo.Repute), new XAttribute("GP", playerInfo.GP));
							xelement1.Add(xelement2);
						}
						flag1 = true;
						str = "Success!";
					}
				}
			}
			catch (Exception ex)
			{
				log.Error("LoadUsersSort", ex);
			}
			xelement1.Add(new XAttribute("total", num1));
			xelement1.Add(new XAttribute("value", flag1));
			xelement1.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement1.ToString(check: false));
		}
	}
}
