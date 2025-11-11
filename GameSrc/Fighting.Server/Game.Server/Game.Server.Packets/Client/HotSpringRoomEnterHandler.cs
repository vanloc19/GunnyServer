using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.HotSpringRooms;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler(202, "礼堂数据")]
	public class HotSpringRoomEnterHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			packet.ReadString();
			if (client.Player.CurrentHotSpringRoom == null)
			{
				HotSpringRoom hotSpringRoombyID = HotSpringMgr.GetHotSpringRoombyID(id);
				if (hotSpringRoombyID != null)
				{
					if (hotSpringRoombyID.Info.roomID <= 4)
					{
						int Gold = 10000;
						if (client.Player.PlayerCharacter.Gold >= Gold)
						{
							if (hotSpringRoombyID.AddPlayer(client.Player) && client.Player.RemoveGold(Gold) > 0)
							{
								client.Out.SendEnterHotSpringRoom(client.Player);
							}
						}
						else
						{
							client.Player.SendMessage("Không đủ Vàng để vào Suối Nước Nóng.");
						}
					}
					if (hotSpringRoombyID.Info.roomID >= 5)
					{
						int Money = 2500;
						if (client.Player.PlayerCharacter.Money >= Money)
						{
							if (hotSpringRoombyID.AddPlayer(client.Player) && client.Player.RemoveMoney(Money, isConsume: true) > 0)
							{
								client.Out.SendEnterHotSpringRoom(client.Player);
							}
						}
						else
						{
							client.Player.SendMessage("Không đủ Xu để vào Suối Nước Nóng.");
						}
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed4"));
				}
			}
			return 0;
		}
	}
}
