using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(85, "场景用户离开")]
	public class MateTimeHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			PlayerInfo playerCharacter;
			if (playerById != null)
			{
				playerCharacter = playerById.PlayerCharacter;
			}
			else
			{
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					playerCharacter = bussiness.GetUserSingleByUserID(playerId);
				}
			}
			GSPacketIn pkg = new GSPacketIn(85, client.Player.PlayerCharacter.ID);
			if (playerCharacter == null)
			{
				pkg.WriteDateTime(DateTime.Now);
			}
			else
			{
				pkg.WriteDateTime(playerCharacter.LastDate);
			}
			client.SendTCP(pkg);
			return 0;
		}
	}
}
