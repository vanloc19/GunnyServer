using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Tank.Request.Illegalcharacters;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class VisualizeRegister : IHttpHandler
	{
		private static FileSystem fileIllegal = new FileSystem(HttpContext.Current.Server.MapPath(IllegalCharacters), HttpContext.Current.Server.MapPath(IllegalDirectory), "*.txt");

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static string IllegalCharacters => ConfigurationManager.AppSettings["IllegalCharacters"];

		public static string IllegalDirectory => ConfigurationManager.AppSettings["IllegalDirectory"];

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Fail1");
			XElement node = new XElement("Result");
			try
			{
				NameValueCollection nameValueCollection = context.Request.Params;
				string userName = nameValueCollection["Name"];
				string passWord = nameValueCollection["Pass"];
				string str1 = nameValueCollection["NickName"].Trim().Replace(",", "");
				_ = nameValueCollection["Arm"];
				_ = nameValueCollection["Hair"];
				_ = nameValueCollection["Face"];
				_ = nameValueCollection["Cloth"];
				_ = nameValueCollection["Cloth"];
				_ = nameValueCollection["ArmID"];
				_ = nameValueCollection["HairID"];
				_ = nameValueCollection["FaceID"];
				_ = nameValueCollection["ClothID"];
				_ = nameValueCollection["ClothID"];
				int sex = -1;
				if (bool.Parse(ConfigurationManager.AppSettings["MustSex"]))
				{
					sex = (bool.Parse(nameValueCollection["Sex"]) ? 1 : 0);
				}
				if (Encoding.Default.GetByteCount(str1) <= 14)
				{
					if (!fileIllegal.checkIllegalChar(str1))
					{
						if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(passWord) && !string.IsNullOrEmpty(str1))
						{
							string[] strArray1 = ((sex == 1) ? ConfigurationManager.AppSettings["BoyVisualizeItem"].Split(';') : ConfigurationManager.AppSettings["GrilVisualizeItem"].Split(';'));
							string[] strArray2 = strArray1;
							char[] chArray1 = new char[1]
							{
								','
							};
							string str2 = strArray2[0].Split(chArray1)[0];
							char[] chArray2 = new char[1]
							{
								','
							};
							string str3 = strArray2[0].Split(chArray2)[1];
							char[] chArray3 = new char[1]
							{
								','
							};
							string str4 = strArray2[0].Split(chArray3)[2];
							char[] chArray4 = new char[1]
							{
								','
							};
							string str5 = strArray2[0].Split(chArray4)[3];
							char[] chArray5 = new char[1]
							{
								','
							};
							string str6 = strArray2[0].Split(chArray5)[4];
							string armColor = "";
							string hairColor = "";
							string faceColor = "";
							string clothColor = "";
							string hatColor = "";
							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								string str7 = str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6;
								if (playerBussiness.RegisterPlayer(userName, passWord, str1, str7, str7, armColor, hairColor, faceColor, clothColor, hatColor, sex, ref translation, int.Parse(ConfigurationManager.AppSettings["ValidDate"])))
								{
									flag = true;
									translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Success");
								}
							}
						}
						else
						{
							translation = LanguageMgr.GetTranslation("!string.IsNullOrEmpty(name) && !");
						}
					}
					else
					{
						translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Illegalcharacters");
					}
				}
				else
				{
					translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Long");
				}
			}
			catch (Exception ex)
			{
				log.Error("VisualizeRegister", ex);
			}
			node.Add(new XAttribute("value", flag));
			node.Add(new XAttribute("message", translation));
			context.Response.ContentType = "text/plain";
			context.Response.Write(node.ToString(check: false));
		}
	}
}
