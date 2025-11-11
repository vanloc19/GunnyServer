using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.GET_SIGNAWARD, "场景用户离开")]
	public class SignAwardHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
			return 0;
			int dailyLog = packet.ReadInt();
			string message = "Nhận thưởng quà điểm danh thành công!";
			if (AwardMgr.AddSignAwards(client.Player, dailyLog))
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, message);
			}
			return 0;
		}
	}
}
