using System;
using System.Configuration;
using System.Linq;
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
	public class NickNameCheck : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static FileSystem fileIllegal = new FileSystem(HttpContext.Current.Server.MapPath(IllegalCharacters), HttpContext.Current.Server.MapPath(IllegalDirectory), "*.txt");

		private static string CharacterAllow = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789áàạảãâấầậẩẫăắằặẳẵÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴéèẹẻẽêếềệểễÉÈẸẺẼÊẾỀỆỂỄóòọỏõôốồộổỗơớờợởỡÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠúùụủũưứừựửữÚÙỤỦŨƯỨỪỰỬỮíìịỉĩÍÌỊỈĨđĐýỳỵỷỹÝỲỴỶỸ.-_";

		public static string IllegalCharacters => ConfigurationManager.AppSettings["IllegalCharacters"];

		public static string IllegalDirectory => ConfigurationManager.AppSettings["IllegalDirectory"];

		public bool IsReusable => false;

		private bool CheckCharacterAllow(string text)
		{
			bool flag = true;
			char[] array = text.ToCharArray();
			foreach (char ch in array)
			{
				if (!CharacterAllow.ToCharArray().Contains(ch))
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public void ProcessRequest(HttpContext context)
		{
			LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
			bool flag = false;
			string translation = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Exist");
			XElement xelement = new XElement("Result");
			try
			{
				string str = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["NickName"]));
				if (Encoding.Default.GetByteCount(str) <= 14)
				{
					if (!fileIllegal.checkIllegalChar(str))
					{
						if (!string.IsNullOrEmpty(str))
						{
							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								if (playerBussiness.GetUserSingleByNickName(str) == null)
								{
									flag = true;
									translation = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Right");
								}
							}
						}
					}
					else
					{
						translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Illegalcharacters");
					}
				}
				else
				{
					translation = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Long");
				}
			}
			catch (Exception ex)
			{
				log.Error("NickNameCheck", ex);
				flag = false;
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", translation));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
