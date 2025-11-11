using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Xml.Linq;
using log4net;
using zlib;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class CheckRegistration : IHttpHandler, IRequiresSessionState
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = true;
			string str = "Registered!";
			XElement xelement = new XElement("Result");
			int num = 1;
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			xelement.Add(new XAttribute("status", num));
			context.Response.ContentType = "text/plain";
			context.Response.BinaryWrite(StaticFunction.Compress(xelement.ToString()));
		}

		public static byte[] Compress(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			ZOutputStream zOutputStream = new ZOutputStream(memoryStream, 3);
			zOutputStream.Write(data, 0, data.Length);
			zOutputStream.Flush();
			zOutputStream.Close();
			return memoryStream.ToArray();
		}
	}
}
