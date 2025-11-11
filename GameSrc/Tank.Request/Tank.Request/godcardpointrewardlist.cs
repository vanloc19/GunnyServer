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
    public class godcardpointrewardlist : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement xelement = new XElement((XName)"Result");
            try
            {
                using (ProduceBussiness produceBussiness = new ProduceBussiness())
                {
                    foreach (GodCardPointRewardInfo info in produceBussiness.GetAllGodCardPointReward())
                        xelement.Add((object)FlashUtils.CreateGodCardPointReward(info));
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                godcardpointrewardlist.log.Error((object)"godcardpointrewardlist.ashx", ex);
            }
            xelement.Add((object)new XAttribute((XName)"value", (object)flag));
            xelement.Add((object)new XAttribute((XName)"message", (object)str));
            context.Response.ContentType = "text/plain";
            context.Response.BinaryWrite(StaticFunction.Compress(xelement.ToString()));
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
