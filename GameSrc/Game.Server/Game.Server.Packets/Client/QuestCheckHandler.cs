using Game.Base.Packets;
using Game.Server.Quests;

namespace Game.Server.Packets.Client
{
	[PacketHandler(181, "客服端任务检查")]
	public class QuestCheckHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			BaseQuest quest = client.Player.QuestInventory.FindQuest(id);
			if (quest != null)
			{
				ClientModifyCondition conditionById = quest.GetConditionById(num2) as ClientModifyCondition;
				if (conditionById != null)
				{
					conditionById.Value = num3;
				}
			}
			return 0;
		}
	}
}
