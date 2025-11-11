using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.GodCardRaise.Handle
{
    [GodCardRaiseHandleAttbute((byte)GodCardRaisePackageType.AWARD_INFO)]
    public class GodCardAwardInfo : IGodCardRaiseCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
            pkg.WriteByte((byte)GodCardRaisePackageType.AWARD_INFO);
            pkg.WriteInt(0);
            player.Out.SendTCP(pkg);
            return 1;
        }
    }
}