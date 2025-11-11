using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.GAIN_FIELD)]//Thu Hoạch Cây Trồng & Trộm
    public class GainFields : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int userId = packet.ReadInt();
            int fieldId = packet.ReadInt();
            string msg = LanguageMgr.GetTranslation("Thu hoạch thất bại!");
            if (userId == Player.PlayerCharacter.ID && Player.Farm.GainField(fieldId))
            {
                msg = LanguageMgr.GetTranslation("Thu hoạch thành công!");
            }
            else if (userId != Player.PlayerCharacter.ID)
            {
                if (Player.Farm.GainFriendFields(userId, fieldId))
                {
                    msg = LanguageMgr.GetTranslation("Thao tác thành công.");
                }
                else
                {
                    msg = LanguageMgr.GetTranslation("Không thể chộm nữa.");
                }
            }
            Player.SendMessage(msg);
            return true;
        }
    }
}