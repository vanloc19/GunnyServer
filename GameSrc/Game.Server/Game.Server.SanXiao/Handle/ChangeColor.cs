using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.PROP_CHANGE_COLOR)]
    public class ChangeColor : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            if (player.Actives.Info.SXStepRemain <= 0)
            {
                player.SendMessage(LanguageMgr.GetTranslation("SanXiao.NoEnouchStepRemain.Msg"));
                return 1;
            }

            if (player.PlayerCharacter.Money >= GameProperties.ThreeClearColourChangeCost)
            {
                player.RemoveMoney(GameProperties.ThreeClearColourChangeCost, isConsume: false);
                player.Actives.Info.SXScore += GameProperties.ThreeClearColourChangePoint;
            }
            else
            {
                player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
            }

            return 1;
        }
    }
}