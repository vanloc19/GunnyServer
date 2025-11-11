using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.PYRAMID_DIEEVENT)]
    public class PyramidDieevent : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            pkg.WriteByte((byte)ActiveSystemPackageType.PYRAMID_DIEEVENT);
            PyramidInfo pyramid = Player.Actives.Pyramid;
            if (pyramid == null)
            {
                Player.Actives.LoadPyramid();
                pyramid = Player.Actives.Pyramid;
            }

            bool isRevert = packet.ReadBoolean();
            if (isRevert)
            {
                int turnCardPrice = Player.Actives.PyramidConfig.revivePrice[pyramid.currentReviveCount];
                if (!Player.MoneyDirect(turnCardPrice, false))
                {
                    return false;
                }

                pyramid.currentReviveCount++;
                pkg.WriteBoolean(pyramid.isPyramidStart); //this.model.isPyramidStart = param1.readBoolean();
                pkg.WriteInt(pyramid.currentLayer); //model.currentLayer = param1.readInt();
                pkg.WriteInt(pyramid.totalPoint); //model.totalPoint = param1.readInt();
                pkg.WriteInt(pyramid.turnPoint); //model.turnPoint = param1.readInt();
                pkg.WriteInt(pyramid.pointRatio); //model.pointRatio = param1.readInt();
                pkg.WriteInt(pyramid.currentReviveCount); //model.currentReviveCount = param1.readInt();
                Player.SendTCP(pkg);
            }
            else
            {
                pyramid.isPyramidStart = false;
                pyramid.currentLayer = 1;
                pyramid.currentCountNow = 0;
                pyramid.currentReviveCount = 0;
                pyramid.turnPoint += (pyramid.turnPoint * pyramid.pointRatio) / 100;
                pyramid.totalPoint += pyramid.turnPoint;
                pyramid.turnPoint = 0;
                pyramid.pointRatio = 0;
                pyramid.LayerItems = "";
                pkg.WriteBoolean(pyramid.isPyramidStart); //this.model.isPyramidStart = param1.readBoolean();
                pkg.WriteInt(pyramid.currentLayer); //model.currentLayer = param1.readInt();
                pkg.WriteInt(pyramid.totalPoint); //model.totalPoint = param1.readInt();
                pkg.WriteInt(pyramid.turnPoint); //model.turnPoint = param1.readInt();
                pkg.WriteInt(pyramid.pointRatio); //model.pointRatio = param1.readInt();
                pkg.WriteInt(pyramid.currentReviveCount); //model.currentReviveCount = param1.readInt();
                Player.SendTCP(pkg);
            }

            return true;
        }
    }
}