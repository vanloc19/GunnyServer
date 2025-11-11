// Decompiled with JetBrains decompiler
// Type: Tank.Request.clothgrouptemplateinfo
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
    public class clothgrouptemplateinfo : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
                context.Response.Write(clothgrouptemplateinfo.Bulid(context));
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
                    foreach (ClothGroupTemplateInfo groupTemplateInfo in produceBussiness.GetAllClothGroupTemplateInfos())
                        result.Add((object)FlashUtils.CreateClothGroupTemplateItems(groupTemplateInfo));
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                clothgrouptemplateinfo.log.Error((object)nameof(clothgrouptemplateinfo), ex);
            }
            result.Add((object)new XAttribute((XName)"value", (object)flag));
            result.Add((object)new XAttribute((XName)"message", (object)str));
            return csFunction.CreateCompressXml(context, result, nameof(clothgrouptemplateinfo), true);
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
