using Bussiness;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class ConsortiaWarConsortiaRank : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "fail!";
            XElement node = new XElement((XName)"Result");
            try
            {
                using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                {
                    foreach (ConsortiaWarRankInfo consortiaWarRankInfo in ((IEnumerable<ConsortiaWarRankInfo>)consortiaBussiness.GetConsortiaWarRank()).OrderByDescending<ConsortiaWarRankInfo, int>((Func<ConsortiaWarRankInfo, int>)(a => a.Score)).ToArray<ConsortiaWarRankInfo>())
                    {
                        XElement xelement = new XElement((XName)"Item", new object[4]
                        {
              (object) new XAttribute((XName) "Rank", (object) consortiaWarRankInfo.Rank),
              (object) new XAttribute((XName) "ConsortiaID", (object) consortiaWarRankInfo.ConsortiaID),
              (object) new XAttribute((XName) "Name", (object) consortiaWarRankInfo.ConsortiaName),
              (object) new XAttribute((XName) "Score", (object) consortiaWarRankInfo.Score)
                        });
                        node.Add((object)xelement);
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch
            {
            }
            node.Add((object)new XAttribute((XName)"value", (object)flag));
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