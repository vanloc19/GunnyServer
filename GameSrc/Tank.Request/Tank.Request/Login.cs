using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Xml.Linq;
using Bussiness;
using Bussiness.Interface;
using Game.Base;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class Login : IHttpHandler, IRequiresSessionState
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag1 = false;
			string translation = LanguageMgr.GetTranslation("Tank.Request.Login.Fail1");
			bool flag2 = false;
			XElement xelement1 = new XElement("Result");
			string s = context.Request["p"];
			try
			{
				BaseInterface baseInterface = BaseInterface.CreateInterface();
				string str1 = ((context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]));
				string userHostAddress = context.Request.UserHostAddress;
				//string userHostAddress1 = context.Request.Headers.Get("X-Real-IP").ToString();
				if (string.IsNullOrEmpty(s))
				{
					return;
				}
				byte[] bytes = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, s);
				string[] strArray = Encoding.UTF8.GetString(bytes, 7, bytes.Length - 7).Split(',');
				if (strArray.Length != 4)
				{
					return;
				}
				string name = strArray[0];
				string pass1 = strArray[1];
				string pass2 = strArray[2];
				string str2 = strArray[3];
				if (PlayerManager.Login(name, pass1))
				{
					int num = 0;
					bool flag3 = false;
					bool byUserIsFirst = PlayerManager.GetByUserIsFirst(name);
					PlayerInfo login = baseInterface.CreateLogin(name, pass2, int.Parse(ConfigurationManager.AppSettings["ServerID"]), ref translation, ref num, userHostAddress, ref flag2, byUserIsFirst, ref flag3, str1, str2);
					if (flag3)
					{
						StaticsMgr.RegCountAdd();
					}
					if (login != null && !flag2)
					{
						if (num == 0)
						{
							PlayerManager.Update(name, pass2);
						}
						else
						{
							PlayerManager.Remove(name);
						}
						string str3 = (string.IsNullOrEmpty(login.Style) ? ",,,,,,,," : login.Style);
						login.Colors = (string.IsNullOrEmpty(login.Colors) ? ",,,,,,,," : login.Colors);
						XElement xelement2 = new XElement("Item", new XAttribute("ID", login.ID), new XAttribute("IsFirst", num), new XAttribute("NickName", login.NickName), new XAttribute("Date", ""), new XAttribute("IsConsortia", 0), new XAttribute("ConsortiaID", login.ConsortiaID), new XAttribute("Sex", login.Sex), new XAttribute("WinCount", login.Win), new XAttribute("TotalCount", login.Total), new XAttribute("EscapeCount", login.Escape), new XAttribute("DutyName", (login.DutyName == null) ? "" : login.DutyName), new XAttribute("GP", login.GP), new XAttribute("Honor", ""), new XAttribute("Style", str3), new XAttribute("Gold", login.Gold), new XAttribute("Colors", (login.Colors == null) ? "" : login.Colors), new XAttribute("Attack", login.Attack), new XAttribute("Defence", login.Defence), new XAttribute("Agility", login.Agility), new XAttribute("Luck", login.Luck), new XAttribute("Grade", login.Grade), new XAttribute("Hide", login.Hide), new XAttribute("Repute", login.Repute), new XAttribute("ConsortiaName", (login.ConsortiaName == null) ? "" : login.ConsortiaName), new XAttribute("Offer", login.Offer), new XAttribute("Skin", (login.Skin == null) ? "" : login.Skin), new XAttribute("ReputeOffer", login.ReputeOffer), new XAttribute("ConsortiaHonor", login.ConsortiaHonor), new XAttribute("ConsortiaLevel", login.ConsortiaLevel), new XAttribute("ConsortiaRepute", login.ConsortiaRepute), new XAttribute("Money", login.Money + login.MoneyLock), new XAttribute("AntiAddiction", login.AntiAddiction), new XAttribute("IsMarried", login.IsMarried), new XAttribute("SpouseID", login.SpouseID), new XAttribute("SpouseName", (login.SpouseName == null) ? "" : login.SpouseName), new XAttribute("MarryInfoID", login.MarryInfoID), new XAttribute("IsCreatedMarryRoom", login.IsCreatedMarryRoom), new XAttribute("IsGotRing", login.IsGotRing), new XAttribute("LoginName", (login.UserName == null) ? "" : login.UserName), new XAttribute("Nimbus", login.Nimbus), new XAttribute("FightPower", login.FightPower), new XAttribute("AnswerSite", login.AnswerSite), new XAttribute("WeaklessGuildProgressStr", (login.WeaklessGuildProgressStr == null) ? "" : login.WeaklessGuildProgressStr), new XAttribute("IsOldPlayer", false));
						xelement1.Add(xelement2);
						flag1 = true;
						translation = LanguageMgr.GetTranslation("Tank.Request.Login.Success");
					}
					else
					{
						PlayerManager.Remove(name);
					}
				}
				else
				{
					log.Error("name:" + name + "-pwd:" + pass1);
					translation = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Try");
				}
			}
			catch (Exception ex)
			{
				byte[] numArray = Convert.FromBase64String(s);
				log.Error("User Login error: (--" + StaticFunction.RsaCryptor.KeySize + "--)" + ex.ToString());
				log.Error("--dataarray: " + Marshal.ToHexDump("fuckingbitch " + numArray.Length, numArray));
				flag1 = false;
				translation = LanguageMgr.GetTranslation("Tank.Request.Login.Fail2");
			}
			finally
			{
				xelement1.Add(new XAttribute("value", flag1));
				xelement1.Add(new XAttribute("message", translation));
				context.Response.ContentType = "text/plain";
				context.Response.Write(xelement1.ToString(check: false));
			}
		}
	}
}
