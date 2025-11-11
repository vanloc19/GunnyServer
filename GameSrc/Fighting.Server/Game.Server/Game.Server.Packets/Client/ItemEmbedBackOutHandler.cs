using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(125, "物品比较")]
	public class ItemEmbedBackOutHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			int num1 = packet.ReadInt();
			int templateId = packet.ReadInt();
			int num2 = 200;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			if (client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock < num2)
			{
				client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("ItemComposeHandler.NoMoney"));
				return 0;
			}
			if (client.Player.PropBag.CountTotalEmptySlot() <= 0)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
				return 0;
			}
			ItemInfo itemAt = client.Player.GetInventory(eBageType.Store).GetItemAt(0);
			ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId);
			if (itemTemplate == null)
			{
				return 11;
			}
			bool flag = false;
			switch (num1)
			{
			case 1:
				if (itemAt.Hole1 > 0 && itemTemplate.TemplateID == itemAt.Hole1)
				{
					itemAt.Hole1 = 0;
					flag = true;
				}
				break;
			case 2:
				if (itemAt.Hole2 > 0 && itemTemplate.TemplateID == itemAt.Hole2)
				{
					itemAt.Hole2 = 0;
					flag = true;
				}
				break;
			case 3:
				if (itemAt.Hole3 > 0 && itemTemplate.TemplateID == itemAt.Hole3)
				{
					itemAt.Hole3 = 0;
					flag = true;
				}
				break;
			case 4:
				if (itemAt.Hole4 > 0 && itemTemplate.TemplateID == itemAt.Hole4)
				{
					itemAt.Hole4 = 0;
					flag = true;
				}
				break;
			case 5:
				if (itemAt.Hole5 > 0 && itemTemplate.TemplateID == itemAt.Hole5)
				{
					itemAt.Hole5 = 0;
					flag = true;
				}
				break;
			case 6:
				if (itemAt.Hole6 > 0 && itemTemplate.TemplateID == itemAt.Hole6)
				{
					itemAt.Hole6 = 0;
					flag = true;
				}
				break;
			default:
				return 1;
			}
			if (flag)
			{
				pkg.WriteInt(0);
				client.Player.BeginChanges();
				ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(itemTemplate, 1, 102);
				fromTemplate.IsBinds = true;
				fromTemplate.ValidDate = 0;
				if (!client.Player.AddItem(fromTemplate))
				{
					GamePlayer player = client.Player;
					List<ItemInfo> items = new List<ItemInfo>
					{
						fromTemplate
					};
					string content = "Hành trang đã đầy vật phẩm gửi ra thư.";
					string title = "Túi đồ đầy";
					int num3 = 8;
					player.SendItemsToMail(items, content, title, (eMailType)num3);
				}
				client.Player.ClearStoreBag();
				client.Player.UpdateItem(itemAt);
				client.Player.RemoveMoney(num2, isConsume: true);
				client.Player.CommitChanges();
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("OK"));
			}
			else
			{
				pkg.WriteInt(1);
			}
			client.Player.SendTCP(pkg);
			client.Player.SaveIntoDatabase();
			return 0;
		}
	}
}
