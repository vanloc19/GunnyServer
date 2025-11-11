using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(77, "物品过期")]
	public class ItemOverdueHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentRoom == null || !client.Player.CurrentRoom.IsPlaying)
			{
				int num = packet.ReadByte();
				int slot = packet.ReadInt();
				PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
				ItemInfo itemAt = inventory.GetItemAt(slot);
				if (itemAt != null && !itemAt.IsValidItem())
				{
					if (num == 0 && slot < 30)
					{
						int toSlot = inventory.FindFirstEmptySlot();
						if (toSlot == -1 || !inventory.MoveItem(itemAt.Place, toSlot, itemAt.Count))
						{
							client.Player.SendItemToMail(itemAt, LanguageMgr.GetTranslation("ItemOverdueHandler.Content"), LanguageMgr.GetTranslation("ItemOverdueHandler.Title"), eMailType.ItemOverdue);
							client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
						}
					}
					else
					{
						inventory.UpdateItem(itemAt);
					}
				}
			}
			return 0;
		}
	}
}
