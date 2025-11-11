using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class godcardlistgroup : IHttpHandler
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
                    GodCardGroupInfo[] allGodCardGroup = produceBussiness.GetAllGodCardGroup();
                    GodCardGroupDetailInfo[] godCardGroupDetail = produceBussiness.GetAllGodCardGroupDetail();
                    foreach (GodCardGroupInfo godCardGroupInfo in allGodCardGroup)
                    {
                        GodCardGroupInfo info = godCardGroupInfo;
                        XElement godCardGroup = FlashUtils.CreateGodCardGroup(info);
                        foreach (GodCardGroupDetailInfo info1 in (IEnumerable)((IEnumerable<GodCardGroupDetailInfo>)godCardGroupDetail).Where<GodCardGroupDetailInfo>((Func<GodCardGroupDetailInfo, bool>)(s => s.GroupID == info.GroupID)))
                            godCardGroup.Add((object)FlashUtils.CreateGodCardGroupDetail(info1));
                        xelement.Add((object)godCardGroup);
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                godcardlistgroup.log.Error((object)"godcardpointrewardlist.ashx", ex);
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
