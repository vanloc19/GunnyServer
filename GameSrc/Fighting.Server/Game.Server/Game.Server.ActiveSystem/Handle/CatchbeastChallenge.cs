using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CATCHBEAST_CHALLENGE)]
    public class CatchbeastChallenge : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);

            if (Player.MainWeapon == null)
            {
                Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                return false;
            }

            if (Player.Actives.Info.ChallengeNum > 0)
            {
                Player.Actives.Info.ChallengeNum--;
                RoomMgr.CreateCatchBeastRoom(Player);
            }

            return true;
        }
    }
}