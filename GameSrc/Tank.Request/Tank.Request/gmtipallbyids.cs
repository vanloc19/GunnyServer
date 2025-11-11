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
	public class gmtipallbyids : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str1 = "Fail!";
			XElement xelement = new XElement("Result");
			try
			{
				string str2 = context.Request["ids"];
				string[] strArray = null;
				if (!string.IsNullOrEmpty(str2))
				{
					strArray = str2.Split(',');
				}
				if (strArray == null)
				{
					return;
				}
				using (ProduceBussiness produceBussiness = new ProduceBussiness())
				{
					EdictumInfo[] allEdictum = produceBussiness.GetAllEdictum();
					foreach (EdictumInfo edictumInfo in allEdictum)
					{
						edictumInfo.ID = int.Parse(strArray[0]);
						DateTime date3 = edictumInfo.EndDate.Date;
						DateTime date2 = DateTime.Now.Date;
						if (date3 > date2)
						{
							xelement.Add(FlashUtils.CreateEdictum(edictumInfo));
						}
					}
					flag = true;
					str1 = "Success!";
				}
			}
			catch (Exception ex)
			{
				str1 = ex.ToString();
			}
			finally
			{
				xelement.Add(new XAttribute("value", flag));
				xelement.Add(new XAttribute("message", str1));
				context.Response.ContentType = "text/plain";
				context.Response.Write(xelement.ToString(check: false));
			}
		}
	}
}
