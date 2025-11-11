//using Bussiness;
//using Game.Base.Packets;
//using Game.Logic;
//using Game.Server.Rooms;

//namespace Game.Server.Packets.Client
//{
//	[PacketHandler(82, "游戏开始")]
//	public class GameUserStartHandler : IPacketHandler
//	{
//		public int HandlePacket(GameClient client, GSPacketIn packet)
//		{
//			bool num = packet.ReadBoolean();
//			BaseRoom currentRoom = client.Player.CurrentRoom;
//			if (num && currentRoom != null)
//			{
//				if (currentRoom.RoomType == eRoomType.FightLab && !client.Player.IsFightLabPermission(currentRoom.MapId, currentRoom.HardLevel))
//				{
//					client.Player.SendMessage(LanguageMgr.GetTranslation("GameUserStartHandler.level"));
//					return 0;
//				}
//				RoomMgr.StartGameMission(currentRoom);
//			}
//			return 0;
//		}
//	}
//}
using Game.Server.Rooms;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler(82, "游戏开始")]
    public class GameUserStartHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool isReady = packet.ReadBoolean();
            BaseRoom room = client.Player.CurrentRoom;
            if (isReady == true && room != null)
            {
                RoomMgr.StartGameMission(room);
            }

            return 0;
        }
    }
}