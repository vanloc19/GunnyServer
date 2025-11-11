using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CATCHBEAST_VIEWINFO)]
    public class CatchbeastViewInfo : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.Actives.YearMonterValidate())
            {
                GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
                pkg.WriteByte(33);
                pkg.WriteInt(Player.Actives.Info.ChallengeNum); //_info.ChallengeNum = _loc_2.readInt();
                pkg.WriteInt(Player.Actives.Info.BuyBuffNum); //_info.BuyBuffNum = _loc_2.readInt();
                pkg.WriteInt(GameProperties.YearMonsterBuffMoney); //_info.BuffPrice = _loc_2.readInt();
                pkg.WriteInt(Player.Actives.Info.DamageNum); //_info.DamageNum = _loc_2.readInt();
                string[] YearMonsterBoxInfo = GameProperties.YearMonsterBoxInfo.Split('|');
                string[] BoxState = Player.Actives.Info.BoxState.Split('-');
                pkg.WriteInt(YearMonsterBoxInfo.Length);
                for (int i = 0; i < YearMonsterBoxInfo.Length; i++)
                {
                    string[] award = YearMonsterBoxInfo[i].Split(',');
                    pkg.WriteInt(int.Parse(award[0]));
                    pkg.WriteInt(int.Parse(award[1]) * 10000);
                    pkg.WriteInt(int.Parse(BoxState[i]));
                }
                Player.Out.SendTCP(pkg);
            }

            return true;
        }
    }
}