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
    public class NewTitleInfo : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
			{
				context.Response.Write(Bulid(context));
			}
			else
			{
				context.Response.Write("IP is not valid!" + context.Request.UserHostAddress);
			}
		}

		public static string Bulid(HttpContext context)
        {
			bool flag = false;
			string str = "Fail!";
			XElement result = new XElement("Result");
			try
            {
				using (ProduceBussiness pb = new ProduceBussiness())
                {
					SqlDataProvider.Data.NewTitleInfo[] allTitle = pb.GetAllNewTitle();
					foreach (SqlDataProvider.Data.NewTitleInfo info in allTitle)
                    {
						result.Add(FlashUtils.CreateNewTitleInfo(info));
                    }
					flag = true;
					str = "Success!";
				}
            }
			catch (Exception ex)
			{
				log.Error("Load NewTitle is fail!", ex);
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "NewTitleInfo", isCompress: false);
		}
	}
}