using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class activitysystemitems : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
                context.Response.Write(activitysystemitems.Bulid(context));
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
                using (ProduceBussiness produceBussiness = new ProduceBussiness())
                {
                    foreach (ActivitySystemItemInfo info in produceBussiness.GetAllActivitySystemItem())
                        result.Add((object)FlashUtils.CreateActivitySystemItems(info));
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                activitysystemitems.log.Error((object)nameof(activitysystemitems), ex);
            }
            result.Add((object)new XAttribute((XName)"value", (object)flag));
            result.Add((object)new XAttribute((XName)"message", (object)str));
            return csFunction.CreateCompressXml(context, result, nameof(activitysystemitems), true);
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