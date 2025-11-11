using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(251, "当前场景状态")]
	internal class MarryStateHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			switch (packet.ReadInt())
			{
				case 0:
					{
						if (!client.Player.IsInMarryRoom)
						{
							break;
						}
						if (client.Player.MarryMap != 1)
						{
							if (client.Player.MarryMap == 2)
							{
								client.Player.X = 800;
								client.Player.Y = 763;
							}
						}
						else
						{
							client.Player.X = 646;
							client.Player.Y = 1241;
						}
						GamePlayer[] allPlayers = client.Player.CurrentMarryRoom.GetAllPlayers();
						foreach (GamePlayer player in allPlayers)
						{
							if (player != client.Player && player.MarryMap == client.Player.MarryMap)
							{
								player.Out.SendPlayerEnterMarryRoom(client.Player);
								client.Player.Out.SendPlayerEnterMarryRoom(player);
							}
						}
						break;
					}
				case 1:
					RoomMgr.EnterWaitingRoom(client.Player);
					break;
			}
			return 0;
		}
	}
}
