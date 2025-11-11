using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using System.Collections.Generic;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.ROOMLIST_UPDATE)]
    public class RoomListUpdate : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int hallType = packet.ReadInt();
            int updateType = packet.ReadInt();
            int pveMapRoom = 10000;
            int pveHardLevel = 1011;
            if (hallType == 2 && updateType == -2)
            {
                pveMapRoom = packet.ReadInt();
                pveHardLevel = packet.ReadInt();
            }

            List<BaseRoom> rooms = new List<BaseRoom>();
            switch (hallType)
            {
                case 1:
                    rooms.AddRange(RoomMgr.GetAllMatchRooms());
                    break;
                case 2:
                    rooms.AddRange(RoomMgr.GetAllPveRooms());
                    break;
            }

            Player.Out.SendUpdateRoomList(rooms);
            return true;
        }
    }
}