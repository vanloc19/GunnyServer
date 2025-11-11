using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CHRISTMAS_MAKING_SNOWMAN_ENTER)]
    public class ChristmasMakingSnowmanEnter : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            UserChristmasInfo info = Player.Actives.Christmas;
            pkg.WriteByte((byte)ActiveSystemPackageType.CHRISTMAS_MAKING_SNOWMAN_ENTER);
            pkg.WriteInt(info.count); //_model.count = param1.readInt();
            pkg.WriteInt(info.exp); //_model.exp = param1.readInt();
            pkg.WriteInt(info.awardState); //_model.awardState = param1.readInt();
            pkg.WriteInt(info.packsNumber); //_model.packsNumber = param1.readInt();
            Player.Out.SendTCP(pkg);
            return true;
        }
    }
}