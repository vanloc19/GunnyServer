using System.Web;
using System.Xml.Linq;
using Bussiness;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	public class LoadUserBox : IHttpHandler
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
					UserBoxInfo[] allUserBox = produceBussiness.GetAllUserBox();
					foreach (UserBoxInfo userBoxInfo in allUserBox)
					{
						result.Add(FlashUtils.CreateUserBoxInfo(userBoxInfo));
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
			return csFunction.CreateCompressXml(context, result, "LoadUserBox", isCompress: true);
		}
	}
}
