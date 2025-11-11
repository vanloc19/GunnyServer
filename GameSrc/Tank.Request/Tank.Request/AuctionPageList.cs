using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class AuctionPageList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag1 = false;
			string str1 = "Fail!";
			int num1 = 0;
			XElement xelement = new XElement("Result");
			try
			{
				int num2 = int.Parse(context.Request["page"]);
				string str2 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
				int num3 = int.Parse(context.Request["type"]);
				int num4 = int.Parse(context.Request["pay"]);
				int num5 = int.Parse(context.Request["userID"]);
				int num6 = int.Parse(context.Request["buyID"]);
				int num7 = int.Parse(context.Request["order"]);
				bool flag2 = bool.Parse(context.Request["sort"]);
				string str3 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["Auctions"]));
				string str4 = (string.IsNullOrEmpty(str3) ? "0" : str3);
				int num8 = 50;
				using (PlayerBussiness playerBussiness1 = new PlayerBussiness())
				{
					AuctionInfo[] auctionPage = playerBussiness1.GetAuctionPage(num2, str2, num3, num4, ref num1, num5, num6, num7, flag2, num8, str4);
					foreach (AuctionInfo auctionInfo1 in auctionPage)
					{
						XElement auctionInfo2 = FlashUtils.CreateAuctionInfo(auctionInfo1);
						using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
						{
							ItemInfo userItemSingle = playerBussiness2.GetUserItemSingle(auctionInfo1.ItemID);
							if (userItemSingle != null)
							{
								auctionInfo2.Add(FlashUtils.CreateGoodsInfo(userItemSingle));
							}
							xelement.Add(auctionInfo2);
						}
					}
					flag1 = true;
					str1 = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error("AuctionPageList", ex);
			}
			xelement.Add(new XAttribute("total", num1));
			xelement.Add(new XAttribute("value", flag1));
			xelement.Add(new XAttribute("message", str1));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
