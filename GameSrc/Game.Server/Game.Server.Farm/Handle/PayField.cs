using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.PAY_FIELD)]//Mở ô Đất
    public class PayField : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            string msg = LanguageMgr.GetTranslation("Mở rộng thành công!");
            List<int> fieldIds = new List<int>();
            int fieldCount = packet.ReadInt();
            for (int i = 0; i < fieldCount; i++)
            {
                int fieldId = packet.ReadInt();
                fieldIds.Add(fieldId);
            }

            int payFieldTime = packet.ReadInt();
            int payMoney = 0;
            var farm = Player.Farm;
            if (farm.payFieldTimeToMonth() == payFieldTime)
            {
                payMoney = fieldCount * farm.payFieldMoneyToMonth();
            }
            else
            {
                payMoney = fieldCount * farm.payFieldMoneyToWeek();
            }

            if (Player.MoneyDirect(payMoney, false))
            {
                farm.PayField(fieldIds, payFieldTime);
                Player.SendMessage(eMessageType.Normal, msg);
            }

            return true;
        }
    }
}