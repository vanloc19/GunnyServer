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
    public class MiniGameShopTemplate : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void ProcessRequest(HttpContext context)
        {
            if(csFunction.ValidAdminIP(context.Request.UserHostAddress))
                context.Response.Write(MiniGameShopTemplate.Bulid(context));
            else
                context.Response.Write("IP is not valid!");
        }

        public static string Bulid(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement((XName)"Result");
            try
            {
                using (ProduceBussiness pb = new ProduceBussiness())
                {
                    foreach (MinigameShopTemplateInfo info in pb.GetAllMinigameShop())
                        result.Add((object)FlashUtils.CreateMiniGameShop(info));
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                log.Error((object)nameof(MiniGameShopTemplate), ex);
            }
            result.Add((object)new XAttribute((XName)"value", (object)flag));
            result.Add((object)new XAttribute((XName)"message", (object)str));
            return csFunction.CreateCompressXml(context, result, nameof(MiniGameShopTemplate), true);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}