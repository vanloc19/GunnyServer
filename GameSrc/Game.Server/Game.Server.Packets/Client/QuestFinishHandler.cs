#region OLD
//using Game.Base.Packets;
//using Game.Server.Quests;

//namespace Game.Server.Packets.Client
//{
//	[PacketHandler(179, "任务完成")]
//	public class QuestFinishHandler : IPacketHandler
//	{
//		public int HandlePacket(GameClient client, GSPacketIn packet)
//		{
//			int id = packet.ReadInt();
//			int selectedItem = packet.ReadInt();
//			BaseQuest baseQuest = client.Player.QuestInventory.FindQuest(id);
//			bool flag = false;
//			if (baseQuest != null)
//			{
//				flag = client.Player.QuestInventory.Finish(baseQuest, selectedItem);
//			}
//			if (flag)
//			{
//				GSPacketIn @in = new GSPacketIn(179, client.Player.PlayerCharacter.ID);
//				@in.WriteInt(id);
//				client.Out.SendTCP(@in);
//			}
//			return 1;
//		}
//	}
//}
#endregion
using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Quests;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.QUEST_FINISH, "任务完成")]
    public class QuestFinishHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            int rewardItemID = packet.ReadInt();

            if (DateTime.Compare(client.Player.LastDrillUpTime.AddSeconds(1), DateTime.Now) > 0)
            {
                client.Out.SendMessage(eMessageType.GM_NOTICE, "Nhận từ từ thôi nha !");
                return 0;
            }

            client.Player.LastDrillUpTime = DateTime.Now;

            BaseQuest baseQuest = client.Player.QuestInventory.FindQuest(id);
            bool result = false;
            if (baseQuest != null)
            {
                result = client.Player.QuestInventory.Finish(baseQuest, rewardItemID);
            }

            if (result)
            {
                GSPacketIn pkg = new GSPacketIn(179, client.Player.PlayerCharacter.ID);
                pkg.WriteInt(id);
                client.Out.SendTCP(pkg);
            }
            return 1;
        }
    }
}