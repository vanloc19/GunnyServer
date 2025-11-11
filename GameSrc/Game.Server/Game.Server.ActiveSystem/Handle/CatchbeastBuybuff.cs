using Bussiness;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CATCHBEAST_BUYBUFF)]
    public class CatchbeastBuybuff : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);

            bool isBand = packet.ReadBoolean();
            if(Player.PlayerCharacter.Grade > 10)
            {
                Player.SendMessage("Tạm khoá chức năng.");
                return false;
            }
            if (Player.MoneyDirect(GameProperties.YearMonsterBuffMoney, IsAntiMult: false))
            {
                if (Player.Actives.Info.BuyBuffNum > 0)
                {
                    Player.Actives.Info.BuyBuffNum--;
                }

                pkg.WriteByte(35);
                pkg.WriteInt(Player.Actives.Info.BuyBuffNum);
                Player.Out.SendTCP(pkg);
                Player.SendMessage(LanguageMgr.GetTranslation("Mua Buff Thành Công."));

                AbstractBuffer buffer = BufferList.CreatePayBuffer((int)BuffType.WorldBossHP, 30000, 1);
                if (buffer != null)
                {
                    buffer.Start(Player);
                }

                buffer = BufferList.CreatePayBuffer((int)BuffType.WorldBossAddDamage, 30000, 1);
                if (buffer != null)
                {
                    buffer.Start(Player);
                }
            }

            return true;
        }
    }
}