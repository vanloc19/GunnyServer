using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ShopItemList : IHttpHandler
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
			int length = 0;
			try
			{
				using (ProduceBussiness produceBussiness = new ProduceBussiness())
				{
					XElement xelement = new XElement("Store");
					ShopItemInfo[] aLllShop = produceBussiness.GetALllShop();
					foreach (ShopItemInfo shopItemInfo in aLllShop)
					{
						xelement.Add(FlashUtils.CreateShopInfo(shopItemInfo));
					}

					length = aLllShop.Length;
					result.Add(xelement);
					flag = true;
					str = "Successcvbcvbcb!";
				}
			}
			catch
			{
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "ShopItemList", isCompress: true, length);
		}
	}
}
