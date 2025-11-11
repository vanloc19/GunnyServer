using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DailyLeagueLevelList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                context.Response.Write(Build(context));//(context.Server.MapPath("~")));
            }
            else
            {
                context.Response.Write("IP is not valid!");
            }
        }

        public static string Build(HttpContext context)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    foreach (DailyLeagueLevelInfo info in db.GetAllDailyLeagueLevelInfo())
                    {
                        result.Add(FlashUtils.CreateDailyLeagueLevelInfo(info));
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception e)
            {
                log.Error("DailyLeagueLevel", e);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return csFunction.CreateCompressXml(context, result, "DailyLeagueLevel", true);
        }
    }
}
