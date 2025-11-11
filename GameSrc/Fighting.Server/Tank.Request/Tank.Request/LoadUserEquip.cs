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
	public class LoadUserEquip : IHttpHandler
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
					PlayerInfo userSingleByUserId = playerBussiness.GetUserSingleByUserID(num);
					xelement.Add(new XAttribute("Agility", userSingleByUserId.Agility), new XAttribute("Attack", userSingleByUserId.Attack), new XAttribute("Colors", userSingleByUserId.Colors), new XAttribute("Skin", userSingleByUserId.Skin), new XAttribute("Defence", userSingleByUserId.Defence), new XAttribute("GP", userSingleByUserId.GP), new XAttribute("Grade", userSingleByUserId.Grade), new XAttribute("Luck", userSingleByUserId.Luck), new XAttribute("Hide", userSingleByUserId.Hide), new XAttribute("Repute", userSingleByUserId.Repute), new XAttribute("Offer", userSingleByUserId.Offer), new XAttribute("NickName", userSingleByUserId.NickName), new XAttribute("ConsortiaName", userSingleByUserId.ConsortiaName), new XAttribute("ConsortiaID", userSingleByUserId.ConsortiaID), new XAttribute("ReputeOffer", userSingleByUserId.ReputeOffer), new XAttribute("ConsortiaHonor", userSingleByUserId.ConsortiaHonor), new XAttribute("ConsortiaLevel", userSingleByUserId.ConsortiaLevel), new XAttribute("ConsortiaRepute", userSingleByUserId.ConsortiaRepute), new XAttribute("WinCount", userSingleByUserId.Win), new XAttribute("TotalCount", userSingleByUserId.Total), new XAttribute("EscapeCount", userSingleByUserId.Escape), new XAttribute("Sex", userSingleByUserId.Sex), new XAttribute("Style", userSingleByUserId.Style), new XAttribute("FightPower", userSingleByUserId.FightPower));
					ItemInfo[] array = playerBussiness.GetUserEuqip(num).ToArray();
					foreach (ItemInfo itemInfo in array)
					{
						xelement.Add(FlashUtils.CreateGoodsInfo(itemInfo));
					}
				}
				flag = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("LoadUserEquip", ex);
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
