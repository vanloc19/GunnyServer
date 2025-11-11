using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_ROOM_LOGIN)]
    public class Login : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            bool isInvite = packet.ReadBoolean();
            int type = packet.ReadInt();
            int isRoundId = packet.ReadInt();
            int roomId = -1;
            string pwd = "";

            if (isRoundId == -1)
            {
                roomId = packet.ReadInt();
                pwd = packet.ReadString();
            }

            RoomMgr.EnterRoom(Player, roomId, pwd, type, isInvite);
            return true;
        }
    }
}