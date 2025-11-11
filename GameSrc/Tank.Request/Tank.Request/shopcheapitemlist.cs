using Bussiness;
using Road.Flash;
using SqlDataProvider.Data;
using System;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class shopcheapitemlist : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement node = new XElement((XName)"Result");
            try
            {
                using (ProduceBussiness produceBussiness = new ProduceBussiness())
                {
                    foreach (ShopItemInfo info in produceBussiness.GetALllShop())
                    {
                        if (info.IsCheap && info.EndDate > DateTime.Now && info.Label == 4)
                            node.Add((object)FlashUtils.CreateShopCheapItems(info));
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