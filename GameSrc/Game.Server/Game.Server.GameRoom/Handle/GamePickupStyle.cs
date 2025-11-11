using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_PICKUP_STYLE)]
    public class GamePickupStyle : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.CurrentRoom != null)
            {
                int GameStyle = packet.ReadInt();
                Player.CurrentRoom.GameStyle = GameStyle;
                switch (GameStyle)
                {
                    case 0:
                        Player.CurrentRoom.GameType = eGameType.Free;
                        break;
                    case 1:
                        Player.CurrentRoom.GameType = eGameType.Guild;
                        break;
                }

                GSPacketIn pkg = Player.Out.SendRoomType(Player, Player.CurrentRoom);
                Player.CurrentRoom.SendToAll(pkg, Player);
            }
            return true;
        }
    }
}