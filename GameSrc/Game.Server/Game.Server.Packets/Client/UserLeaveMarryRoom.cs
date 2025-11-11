using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.PLAYER_EXIT_MARRY_ROOM, "玩家退出礼堂")]
	public class UserLeaveMarryRoom : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.IsInMarryRoom)
			{
				client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
			}
			return 0;
		}
	}
}
