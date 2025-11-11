using System.Web;
using System.Xml.Linq;
using Bussiness;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	public class LoadBoxTemp : IHttpHandler
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
					ItemBoxInfo[] itemBoxInfos = produceBussiness.GetItemBoxInfos();
					foreach (ItemBoxInfo itemBoxInfo in itemBoxInfos)
					{
						result.Add(FlashUtils.CreateItemBoxInfo(itemBoxInfo));
					}
					flag = true;
					str = "Success!";
				}
			}
			catch
			{
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "LoadBoxTemp", isCompress: true);
		}
	}
}
