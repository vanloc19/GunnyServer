using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.PYRAMID_STARTORSTOP)]
    public class PyramidStartOrStop : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn((byte)145, Player.PlayerCharacter.ID);
            PyramidInfo pyramid = Player.Actives.Pyramid;
            if (pyramid == null)
            {
                Player.Actives.LoadPyramid();
                pyramid = Player.Actives.Pyramid;
            }

            bool startorstop = packet.ReadBoolean();
            pyramid.isPyramidStart = startorstop;
            if(!startorstop)
            {
                pyramid.currentCountNow = 0;
            }
            pkg.WriteByte((byte)ActiveSystemPackageType.PYRAMID_STARTORSTOP);
            pkg.WriteBoolean(startorstop); //this.model.isPyramidStart = param1.readBoolean();
            if (!startorstop)
            {
                pyramid.turnPoint += (pyramid.turnPoint * pyramid.pointRatio) / 100;
                pyramid.totalPoint += pyramid.turnPoint;
                pyramid.turnPoint = 0;
                pyramid.pointRatio = 0;
                pyramid.currentLayer = 1;
                pyramid.currentReviveCount = 0;
                pyramid.currentCountNow = 0;
                pyramid.LayerItems = "";
                pkg.WriteInt(pyramid.totalPoint); //model.totalPoint = param1.readInt();
                pkg.WriteInt(pyramid.turnPoint); //model.turnPoint = param1.readInt();
                pkg.WriteInt(pyramid.pointRatio); //model.pointRatio = param1.readInt();
                pkg.WriteInt(pyramid.currentLayer); //model.currentLayer = param1.readInt();
            }

            Player.SendTCP(pkg);
            return true;
        }
    }
}