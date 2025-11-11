using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class KeyGenerator : IHttpHandler
	{
		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			new CspParameters().Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(2048);
			RSAParameters rsaParameters = cryptoServiceProvider.ExportParameters(includePrivateParameters: true);
			StringBuilder stringBuilder1 = new StringBuilder();
			for (int index2 = 0; index2 < rsaParameters.Modulus.Length; index2++)
			{
				stringBuilder1.Append(rsaParameters.Modulus[index2].ToString("X2"));
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int index = 0; index < rsaParameters.Exponent.Length; index++)
			{
				stringBuilder2.Append(rsaParameters.Exponent[index].ToString("X2"));
			}
			XElement xelement1 = new XElement("list");
			XElement xelement2 = new XElement("private", new XAttribute("key", cryptoServiceProvider.ToXmlString(includePrivateParameters: true)));
			XElement xelement3 = new XElement("public", new XAttribute("model", stringBuilder1.ToString()), new XAttribute("exponent", stringBuilder2.ToString()));
			xelement1.Add(xelement2);
			xelement1.Add(xelement3);
			context.Response.Write(xelement1.ToString());
		}
	}
}
