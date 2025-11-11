using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.KILLCROP_FIELD)]
    public class KillCropField : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int fieldId = packet.ReadInt();
            Player.Farm.killCropField(fieldId);
            return true;
        }
    }
}