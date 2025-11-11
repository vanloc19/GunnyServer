using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class consortianamecheck : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
			bool flag = false;
			string str = "O nome já foi usado.";
			XElement xelement = new XElement("Result");
			try
			{
				string s = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["NickName"]));
				if (Encoding.Default.GetByteCount(s) <= 20)
				{
					if (!string.IsNullOrEmpty(s))
					{
						using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
						{
							if (consortiaBussiness.GetConsortiaSingleByName(s) == null)
							{
								flag = true;
								str = "Sucesso! O nome pode ser utilizado.";
							}
						}
					}
				}
				else
				{
					str = "O nome da associação é muito longo";
				}
			}
			catch (Exception ex)
			{
				log.Error("NickNameCheck", ex);
				flag = false;
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
