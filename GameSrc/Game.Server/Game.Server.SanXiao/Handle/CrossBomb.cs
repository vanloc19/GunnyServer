using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.PROP_CROSS_BOMB)]
    public class CrossBomb : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            if (player.Actives.Info.SXStepRemain <= 0)
            {
                player.SendMessage(LanguageMgr.GetTranslation("SanXiao.NoEnouchStepRemain.Msg"));
                return 1;
            }

            if (player.PlayerCharacter.Money >= GameProperties.ThreeClearCrossCost)
            {
                player.RemoveMoney(GameProperties.ThreeClearCrossCost, isConsume: false);
                player.Actives.Info.SXScore += GameProperties.ThreeClearCrossPoint;
            }
            else
            {
                player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
            }

            return 1;
        }
    }
}