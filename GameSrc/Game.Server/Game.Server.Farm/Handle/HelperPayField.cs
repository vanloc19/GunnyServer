using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.HELPER_PAY_FIELD)]
    public class HelperPayField : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            string msg = LanguageMgr.GetTranslation("Gia hạn trợ thủ thành công!");
            int payAutoTime = packet.ReadInt();
            int payMoney = 0;
            var farm = Player.Farm;

            if (farm.payAutoTimeToMonth() == payAutoTime)
            {
                payMoney = 300;
            }
            else
            {
                payMoney = 100;
            }

            if (Player.MoneyDirect(payMoney, IsAntiMult: false))
            {
                UserFarmInfo farmPlayer = new UserFarmInfo();
                farmPlayer.AutoValidDate = payAutoTime;
                Player.SendMessage(eMessageType.Normal, msg);
                Player.Out.SendHelperSwitchField(Player.PlayerCharacter, farmPlayer);
            }
            return true;
        }
    }
}