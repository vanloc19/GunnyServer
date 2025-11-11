using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.HELPER_SWITCH_FIELD)]
    public class HelperSwitchField : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            string msg = LanguageMgr.GetTranslation("Kích hoạt trợ thủ thất bại!");
            bool isHelper = packet.ReadBoolean();
            int seedID = packet.ReadInt();
            int seedTime = packet.ReadInt();
            int haveCount = packet.ReadInt();
            int getCount = packet.ReadInt();
            int moneyType = packet.ReadInt();
            int needMoney = packet.ReadInt();

            bool canOpen = false;
            if (isHelper)
            {
                if (Player.MoneyDirect(needMoney,false) && moneyType == -1)
                {
                    canOpen = true;
                }
                else if (Player.PlayerCharacter.GiftToken >= needMoney && moneyType == -2)
                {
                    Player.RemoveGiftToken(needMoney);
                    canOpen = true;
                }
                else
                {
                    if (moneyType == -1)
                        msg = LanguageMgr.GetTranslation("Xu không đủ!");
                    else
                        msg = LanguageMgr.GetTranslation("Xu khóa không đủ!");
                }
            }
            else
            {
                msg = LanguageMgr.GetTranslation("Hủy trợ thủ thành công!");
                Player.Farm.CropHelperSwitchField(true);
            }

            if (canOpen)
            {
                msg = LanguageMgr.GetTranslation("Kích hoạt trợ thủ thành công!");
                Player.Farm.HelperSwitchField(isHelper, seedID, seedTime, haveCount, getCount);
                Player.FarmBag.RemoveTemplate(seedID, haveCount);
            }

            Player.SendMessage(eMessageType.Normal, msg);
            return true;
        }
    }
}