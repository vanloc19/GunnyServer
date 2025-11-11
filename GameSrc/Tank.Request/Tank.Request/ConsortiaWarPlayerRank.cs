using Bussiness;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class ConsortiaWarPlayerRank : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string s = context.Request["ConsortiaID"];
            string UserID = context.Request["UserID"];
            bool flag = false;
            string str = "fail!";
            XElement node = new XElement((XName)"Result");
            try
            {
                using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                {
                    ConsortiaWarPlayerRankInfo[] array = ((IEnumerable<ConsortiaWarPlayerRankInfo>)consortiaBussiness.GetConsortiaWarPlayerRank(int.Parse(s))).OrderByDescending<ConsortiaWarPlayerRankInfo, int>((Func<ConsortiaWarPlayerRankInfo, int>)(a => a.Score)).ToArray<ConsortiaWarPlayerRankInfo>();
                    int num = 1;
                    foreach (ConsortiaWarPlayerRankInfo warPlayerRankInfo in array)
                    {
                        XElement xelement = new XElement((XName)"Item", new object[5]
                        {
              (object) new XAttribute((XName) "Rank", (object) num),
              (object) new XAttribute((XName) "ConsortiaID", (object) warPlayerRankInfo.ConsortiaID),
              (object) new XAttribute((XName) "Name", (object) warPlayerRankInfo.NickName),
              (object) new XAttribute((XName) "Score", (object) warPlayerRankInfo.Score),
              (object) new XAttribute((XName) "UserID", (object) warPlayerRankInfo.UserID)
                        });
                        node.Add((object)xelement);
                        ++num;
                    }
                    if (((IEnumerable<ConsortiaWarPlayerRankInfo>)array).SingleOrDefault<ConsortiaWarPlayerRankInfo>((Func<ConsortiaWarPlayerRankInfo, bool>)(a => a.UserID == int.Parse(UserID))) == null)
                    {
                        XElement xelement = new XElement((XName)"Item", new object[4]
                        {
              (object) new XAttribute((XName) "Rank", (object) -1),
              (object) new XAttribute((XName) "ConsortiaID", (object) 0),
              (object) new XAttribute((XName) "Score", (object) 0),
              (object) new XAttribute((XName) "UserID", (object) UserID)
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