using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(108, "选取")]
	public class GameTakeTempItemsHandler : IPacketHandler
	{
		private bool GetItem(GamePlayer player, ItemInfo item, ref string message)
		{
			if (item != null)
			{
				if (item.Template.BagType == eBageType.Card)
				{
					if (player.CardBag.AddCard(item.Template.TemplateID, item.Count))
					{
						player.TempBag.RemoveItem(item);
					}
					return true;
				}
				PlayerInventory itemInventory = player.GetItemInventory(item.Template);
				if (itemInventory.AddItem(item))
				{
					player.TempBag.RemoveItem(item);
					item.IsExist = true;
					return true;
				}
				itemInventory.UpdateChangedPlaces();
				message = LanguageMgr.GetTranslation("GameTakeTempItemsHandler.Msg");
				List<ItemInfo> items = player.TempBag.GetItems();
				if (player.SendItemsToMail(items, "Túi của bạn đầy vật phẩm được gửi ra thư", "Túi đầy", eMailType.ItemOverdue))
				{
					foreach (ItemInfo _item in items)
					{
						player.TempBag.RemoveItem(_item);
					}
				}
			}
			return false;
		}

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			string message = string.Empty;
			int slot = packet.ReadInt();
			if (slot != -1)
			{
				ItemInfo itemAt = client.Player.TempBag.GetItemAt(slot);
				GetItem(client.Player, itemAt, ref message);
			}
			else
			{
				foreach (ItemInfo info2 in client.Player.TempBag.GetItems())
				{
					if (!GetItem(client.Player, info2, ref message))
					{
						break;
					}
				}
			}
			if (!string.IsNullOrEmpty(message))
			{
				client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, message);
			}
			return 0;
		}
	}
}
