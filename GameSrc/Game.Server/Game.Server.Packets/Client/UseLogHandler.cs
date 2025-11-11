using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.USE_LOG, "场景用户离开")]
	public class UseLogHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadInt();
			return 0;
		}
	}
}
