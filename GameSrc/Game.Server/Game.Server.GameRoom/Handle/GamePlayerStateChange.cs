using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_PLAYER_STATE_CHANGE)]
    public class GamePlayerStateChange : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.MainWeapon == null)
            {
                Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                return false;
            }

            if (Player.CurrentRoom != null)
            {
                RoomMgr.UpdatePlayerState(Player, packet.ReadByte());
            }

            return true;
        }
    }
}