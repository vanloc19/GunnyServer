using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(120, "物品倾向转移")]
	public class ItemTrendHandle : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eBageType bagType = (eBageType)packet.ReadInt();
			int place = packet.ReadInt();
			eBageType type2 = (eBageType)packet.ReadInt();
			List<ShopItemInfo> list = new List<ShopItemInfo>();
			int num2 = packet.ReadInt();
			int operation = packet.ReadInt();
			ItemInfo info;
			if (num2 == -1)
			{
				packet.ReadInt();
				packet.ReadInt();
				int num3 = 0;
				int num4 = 0;
				info = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(34101), 1, 102);
				list = ShopMgr.FindShopbyTemplatID(34101);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
					{
						num4 = list[i].AValue1;
						info.ValidDate = list[i].AUnit;
					}
				}
				if (info != null)
				{
					if (num3 <= client.Player.PlayerCharacter.Gold && num4 <= client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock)
					{
						client.Player.RemoveMoney(num4, isConsume: false);
						client.Player.RemoveGold(num3);
					}
					else
					{
						info = null;
					}
				}
			}
			else
			{
				info = client.Player.GetItemAt(type2, num2);
			}
			ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
			StringBuilder builder = new StringBuilder();
			if (info != null && itemAt != null)
			{
				bool result = false;
				ItemTemplateInfo goods = RefineryMgr.RefineryTrend(operation, itemAt, ref result);
				if (result && goods != null)
				{
					ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 115);
					AbstractInventory itemInventory = client.Player.GetItemInventory(goods);
					if (itemInventory.AddItem(item, itemInventory.BeginSlot))
					{
						client.Player.UpdateItem(item);
						client.Player.RemoveItem(itemAt);
						info.Count--;
						client.Player.UpdateItem(info);
						client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemTrendHandle.Success"));
					}
					else
					{
						builder.Append("NoPlace");
						client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(item.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
					}
					return 1;
				}
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemTrendHandle.Fail"));
			}
			return 1;
		}
	}
}
