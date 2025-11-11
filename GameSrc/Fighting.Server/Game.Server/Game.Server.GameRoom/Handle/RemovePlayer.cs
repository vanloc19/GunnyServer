using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using System.Linq;
namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_ROOM_REMOVEPLAYER)]
    public class RemovePlayer : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.CurrentRoom != null)
            {
                var listplayer = Player.CurrentRoom.GetPlayers().Where(p => !p.IsQuanChien && p.PlayerId != Player.PlayerId).ToList();
                if (listplayer.Count == 0)
                {
                    listplayer = Player.CurrentRoom.GetPlayers().Where(p => p.IsQuanChien && p.PlayerId != Player.PlayerId).ToList();
                    foreach (var p in listplayer)
                    {
                        RoomMgr.ExitRoom(p.CurrentRoom, p);
                    }
                }
                RoomMgr.ExitRoom(Player.CurrentRoom, Player);
            }
            return true;
        }
    }
}