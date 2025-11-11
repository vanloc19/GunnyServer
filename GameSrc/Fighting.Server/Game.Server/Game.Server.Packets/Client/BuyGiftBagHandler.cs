using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Configuration;

namespace Game.Server.Packets.Client
{
	[PacketHandler(46, "物品强化")]
	public class BuyGiftBagHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (Convert.ToBoolean(ConfigurationManager.AppSettings["BuyGiftBag"]))
			{
				int GiftPrice = 988;
				if (client.Player.PlayerCharacter.Money < GiftPrice)
				{
					client.Player.SendMessage(string.Format("Thao tác thất bại."));
					return 0;
				}
				if (client.Player.PropBag.CountTotalEmptySlot() <= 5)
				{
					client.Player.SendMessage("Túi đạo cụ của bạn không đủ 5 chỗ trống!");
				}
				else
				{
					if (client.Player.RemoveMoney(GiftPrice, isConsume: true) > 0)
					{
						client.Player.ClearStoreBagWithOutPlace(5);
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11023), 1, 101);
						itemInfo.Count = 1;
						itemInfo.ValidDate = 0;
						itemInfo.IsBinds = true;
						client.Player.StoreBag.AddItemTo(itemInfo, 0);
						itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11023), 1, 101);
						itemInfo.Count = 1;
						itemInfo.ValidDate = 0;
						itemInfo.IsBinds = true;
						client.Player.StoreBag.AddItemTo(itemInfo, 1);
						itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11023), 1, 101);
						itemInfo.Count = 1;
						itemInfo.ValidDate = 0;
						itemInfo.IsBinds = true;
						client.Player.StoreBag.AddItemTo(itemInfo, 2);
						itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11020), 1, 101);
						itemInfo.Count = 1;
						itemInfo.ValidDate = 0;
						itemInfo.IsBinds = true;
						client.Player.StoreBag.AddItemTo(itemInfo, 3);
						itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11018), 1, 101);
						itemInfo.Count = 1;
						itemInfo.ValidDate = 0;
						itemInfo.IsBinds = true;
						client.Player.StoreBag.AddItemTo(itemInfo, 4);
						client.Player.SendMessage(string.Format("Mua thành công túi quà cường hóa!"));
					}
				}
			}
			return 0;
		}
	}
}
