using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(176, "添加任务")]
	public class QuestAddHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int count = packet.ReadInt();
			for (int i = 0; i < count; i++)
			{
				QuestInfo info = QuestMgr.GetSingleQuest(packet.ReadInt());
				client.Player.QuestInventory.AddQuest(info, out var _);
			}
			return 0;
		}
	}
}
