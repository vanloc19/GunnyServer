using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler(169, "礼堂数据")]
	public class HotSpringRoomPlayerRemoveHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentHotSpringRoom != null)
			{
				client.Player.CurrentHotSpringRoom.RemovePlayer(client.Player);
				GSPacketIn pkg = new GSPacketIn(169);
				pkg.WriteString(string.Format("Bạn đã thoát khỏi suối nước nóng!"));
				client.SendTCP(pkg);
			}
			return 0;
		}
	}
}
