using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;

namespace Tank.Request
{
	public class GetSID : IHttpHandler, IRequiresSessionState
	{
		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			new CspParameters().Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(2048);
			rSACryptoServiceProvider.FromXmlString(ConfigurationManager.AppSettings["privateKey"]);
			RSAParameters rsaParameters = rSACryptoServiceProvider.ExportParameters(includePrivateParameters: false);
			XElement xelement = new XElement("result", new XAttribute("m1", Convert.ToBase64String(rsaParameters.Modulus)), new XAttribute("m2", Convert.ToBase64String(rsaParameters.Exponent)));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString());
		}
	}
}
