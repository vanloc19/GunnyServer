using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using Bussiness;
using Bussiness.Interface;
using log4net;

namespace Tank.Request
{
    public class ShopItemAllList : IHttpHandler
    {
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
        {
            context.Response.Write(Bulid(context));
        }
        else
        {
            context.Response.Write("IP is not valid!");
        }
    }

    public static string Bulid(HttpContext context)
    {
        bool flag = false;
        string str = "Fail!";
        XElement result = new XElement("Result");
        try
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                // XElement xelement = new XElement("Store");
                // ShopItemInfo[] aLllShop = produceBussiness.GetALllShop();
                // foreach (ShopItemInfo shopItemInfo in aLllShop)
                // {
                // 	xelement.Add(FlashUtils.CreateShopInfo(shopItemInfo));
                // }
                // result.Add(xelement);
                // flag = true;
                // str = "Successcvbcvbcb! " + aLllShop.Length;
            }
        }
        catch
        {
        }
        result.Add(new XAttribute("value", flag));
        result.Add(new XAttribute("message", str));
        return csFunction.CreateCompressXml(context, result, "ShopItemList", isCompress: true);
    }
    }
}