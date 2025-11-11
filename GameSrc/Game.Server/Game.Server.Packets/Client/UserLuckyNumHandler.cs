using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.USER_LUCKYNUM, "场景用户离开")]
	public class UserLuckyNumHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadBoolean();
			packet.ReadInt();
			return 1;
		}
	}
}
