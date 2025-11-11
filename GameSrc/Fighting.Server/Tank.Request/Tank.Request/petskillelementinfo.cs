using System;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	public class petskillelementinfo : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write(Bulid(context));
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
					PetSkillElementInfo[] allPetSkillElementInfo = produceBussiness.GetAllPetSkillElementInfo();
					foreach (PetSkillElementInfo skillElementInfo in allPetSkillElementInfo)
					{
						result.Add(FlashUtils.CreatePetSkillElement(skillElementInfo));
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("Load petskillelementinfo is fail!", ex);
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "petskillelementinfo", isCompress: false);
		}
	}
}
