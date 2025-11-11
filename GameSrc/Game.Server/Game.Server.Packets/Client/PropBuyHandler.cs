using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(54, "购买道具")]
	public class PropBuyHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int gold = 0;
			int money = 0;
			int offer = 0;
			int gifttoken = 0;
			int medal = 0;
			int damageScore = 0;
			int petScore = 0;
			int iTemplateID = 0;
			int iCount = 0;
			int hardCurrency = 0;
			int leagueMoney = 0;
			int iD = packet.ReadInt();
			int type = 1;
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			if (PropItemMgr.PropFightBag.ToList().Contains(shopItemInfoById.TemplateID))
			{
				if (shopItemInfoById == null)
				{
					return 0;
				}
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
				ShopMgr.SetItemType(shopItemInfoById, type, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref leagueMoney, ref medal);
				if (goods.CategoryID == 10)
				{
					PlayerInfo playerCharacter = client.Player.PlayerCharacter;
					if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked && (money > 0 || offer > 0 || gifttoken > 0 || medal > 0))
					{
						client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
						return 0;
					}
					if (gold <= playerCharacter.Gold && money <= ((playerCharacter.Money >= 0) ? playerCharacter.Money : 0) && offer <= playerCharacter.Offer && gifttoken <= playerCharacter.GiftToken && medal <= playerCharacter.medal)
					{
						ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 102);
						if (shopItemInfoById.BuyType == 0)
						{
							if (1 == type)
							{
								item.ValidDate = shopItemInfoById.AUnit;
							}
							if (2 == type)
							{
								item.ValidDate = shopItemInfoById.BUnit;
							}
							if (3 == type)
							{
								item.ValidDate = shopItemInfoById.CUnit;
							}
						}
						else
						{
							if (1 == type)
							{
								item.Count = shopItemInfoById.AUnit;
							}
							if (2 == type)
							{
								item.Count = shopItemInfoById.BUnit;
							}
							if (3 == type)
							{
								item.Count = shopItemInfoById.CUnit;
							}
						}
						if (client.Player.FightBag.AddItem(item, 0))
						{
							client.Player.RemoveGold(gold);
							client.Player.RemoveMoney(money, isConsume: false);
							client.Player.RemoveOffer(offer);
							client.Player.RemoveGiftToken(gifttoken);
							client.Player.RemoveMedal(medal);
						}
					}
					else
					{
						client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("PropBuyHandler.NoMoney"));
					}
				}
			}
			return 0;
		}
	}
}
