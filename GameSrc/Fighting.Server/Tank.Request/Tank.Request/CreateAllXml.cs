using System.Text;
using System.Web;
using System.Web.Services;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class CreateAllXml : IHttpHandler
	{
		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			var userAdress = context.Request.UserHostAddress;
			//var userAdress1 = context.Request.Headers.Get("X-Real-IP").ToString();
			if (csFunction.ValidAdminIP(userAdress))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(ActiveList.Bulid(context));
				stringBuilder.Append(BallList.Bulid(context));
				stringBuilder.Append(bombconfig.Bulid(context));
				stringBuilder.Append(LoadMapsItems.Bulid(context));
				stringBuilder.Append(LoadPVEItems.Build(context));
				stringBuilder.Append(QuestList.Bulid(context));
				stringBuilder.Append(TemplateAllList.Bulid(context));
				stringBuilder.Append(ShopItemList.Bulid(context));
				stringBuilder.Append(ShopGoodsShowList.Bulid(context));
				stringBuilder.Append(LoadItemsCategory.Bulid(context));
				stringBuilder.Append(MapServerList.Bulid(context));
				stringBuilder.Append(ConsortiaLevelList.Bulid(context));
				stringBuilder.Append(DailyAwardList.Bulid(context));
				stringBuilder.Append(NPCInfoList.Bulid(context));
				stringBuilder.Append(LevelList.Bulid(context));
				stringBuilder.Append(eventrewarditemlist.Bulid(context));
				stringBuilder.Append(godcardlist.Bulid(context));
				stringBuilder.Append(LoginAwardItemTemplate.Bulid(context));
				stringBuilder.Append(MiniGameShopTemplate.Bulid(context));
				stringBuilder.Append(ThreeCleanPointAward.Bulid(context));
				stringBuilder.Append(NewTitleInfo.Bulid(context));
				stringBuilder.Append(GmActivityInfo.Bulid(context));
				stringBuilder.Append(clothgrouptemplateinfo.Bulid(context));
				stringBuilder.Append(clothpropertytemplateinfo.Bulid(context));
				stringBuilder.Append(activitysystemitems.Bulid(context));
				stringBuilder.Append(toteminfo.Bulid(context));
				stringBuilder.Append(DailyLeagueAwardList.Build(context));
				stringBuilder.Append(DailyLeagueLevelList.Build(context));
				stringBuilder.Append(fightspirittemplatelist.Bulid(context));
				context.Response.ContentType = "text/plain";
				context.Response.Write(stringBuilder.ToString());
			}
			else
			{
				context.Response.Write("IP is not valid!");
			}
		}
	}
}
