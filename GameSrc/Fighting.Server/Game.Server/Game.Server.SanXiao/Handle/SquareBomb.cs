using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.PROP_SQUARE_BOMB)]
    public class SquareBomb : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            if (player.Actives.Info.SXStepRemain <= 0)
            {
                player.SendMessage(LanguageMgr.GetTranslation("SanXiao.NoEnouchStepRemain.Msg"));
                return 1;
            }

            if (player.PlayerCharacter.Money >= GameProperties.ThreeClearRectCost)
            {
                player.RemoveMoney(GameProperties.ThreeClearRectCost, isConsume: false);
                player.Actives.Info.SXScore += GameProperties.ThreeClearRectPoint;
            }
            else
            {
                player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
            }

            return 1;
        }
    }
}