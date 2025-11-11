using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CHRISTMAS_BUY_TIMER)]
    public class ChristmasBuyTimer : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            int needMoney = GameProperties.ChristmasBuyTimeMoney;
            if (Player.MoneyDirect(needMoney, false))
            {
                int min = GameProperties.ChristmasBuyMinute;
                Player.Actives.AddTime(min);
                Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg2"));
            }

            return true;
        }
    }
}