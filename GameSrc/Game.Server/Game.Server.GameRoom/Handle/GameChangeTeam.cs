using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_TEAM)]
    public class GameChangeTeam : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.CurrentRoom != null && Player.CurrentRoom.RoomType != eRoomType.Match)
            {
                RoomMgr.SwitchTeam(Player);
            }
            return true;
        }
    }
}