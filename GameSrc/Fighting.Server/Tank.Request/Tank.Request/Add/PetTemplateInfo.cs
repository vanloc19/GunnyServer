using Bussiness;
using Road.Flash;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class PetTemplateInfo : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                context.Response.Write(PetTemplateInfo.Bulid(context));
            }
            else
            {
                context.Response.Write("IP is nos valid!");
            }
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
                    SqlDataProvider.Data.PetTemplateInfo[] allPetTemplateInfo = pb.GetAllPetTemplateInfo();
                    foreach(SqlDataProvider.Data.PetTemplateInfo petTemplateInfo in allPetTemplateInfo)
                    {
                        result.Add(FlashUtils.CreatePetTemplate(petTemplateInfo));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "pettemplateinfo", isCompress: false);
        }
    }
}