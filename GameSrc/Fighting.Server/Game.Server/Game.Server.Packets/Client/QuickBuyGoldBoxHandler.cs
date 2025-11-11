using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(126, "场景用户离开")]
	public class QuickBuyGoldBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadBoolean();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 1;
			}
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(1123301);
			ItemTemplateInfo info2 = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			int num3 = num * shopItemInfoById.AValue1;
			if (client.Player.MoneyDirect(num3, IsAntiMult: false))
			{
				int point = 0;
				int gold = 0;
				int giftToken = 0;
				int medal = 0;
				int exp = 0;
				List<ItemInfo> itemInfos = new List<ItemInfo>();
				ItemBoxMgr.CreateItemBox(info2.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp);
				int num2 = num * gold;
				client.Player.AddGold(num2);
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bạn nhận được " + num2 + " vàng từ rương vàng."));
			}
			return 0;
		}
	}
}
