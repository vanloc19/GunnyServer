// Decompiled with JetBrains decompiler
// Type: Tank.Request.subactivelist
// Assembly: Tank.Request, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9A63E34D-BA53-472F-A1E3-A6542CA79FB7
// Assembly location: C:\Users\Pham Van Hungg\Desktop\Bin\Tank.Request.dll

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
    public class subactivelist : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "fail!";
            XElement node = new XElement((XName)"Result");
            try
            {
                using (ProduceBussiness activeBussiness = new ProduceBussiness())
                {
                    foreach (SubActiveInfo info1 in activeBussiness.GetAllSubActive())
                    {
                        node.Add((object)FlashUtils.CreateActiveInfo(info1));
                        foreach (SubActiveConditionInfo info2 in activeBussiness.GetAllSubActiveCondition(info1.ActiveID))
                            node.Add((object)FlashUtils.CreateActiveConditionInfo(info2));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                subactivelist.log.Error((object)nameof(subactivelist), ex);
            }
            node.Add((object)new XAttribute((XName)"value", (object)flag));
            node.Add((object)new XAttribute((XName)"nowTime", (object)DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
            node.Add((object)new XAttribute((XName)"message", (object)str));
            context.Response.ContentType = "text/plain";
            context.Response.Write(node.ToString(false));
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
