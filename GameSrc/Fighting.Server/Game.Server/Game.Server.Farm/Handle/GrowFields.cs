using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.GROW_FIELD)]
    public class GrowFields : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int emptyValue = packet.ReadByte();
            int templateID = packet.ReadInt();
            int fieldId = packet.ReadInt();
            if (Player.Farm.GrowField(fieldId, templateID))
            {
                Player.FarmBag.RemoveTemplate(templateID, 1);
                Player.OnSeedFoodPetEvent();
            }
            return true;
        }
    }
}