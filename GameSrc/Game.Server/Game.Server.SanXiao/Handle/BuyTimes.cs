using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.BUY_TIMES)]
    public class BuyTimes : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int count = packet.ReadInt();
            bool isBind = packet.ReadBoolean();

            if (count > 0 && count <= 99)
            {
                int moneyPay = GameProperties.ThreeCleanBuyCost * count;

                if (moneyPay <= player.PlayerCharacter.Money)
                {
                    player.RemoveMoney(moneyPay, isConsume: false);
                    player.Actives.Info.SXStepRemain += count;
                    player.Actives.SendSXBuyTime();
                }
                else
                {
                    player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
                }
            }

            return 1;
        }
    }
}