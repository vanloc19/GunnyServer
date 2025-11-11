using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.FIGHT_SPIRIT_LEVELUP)]
    public class ChristmasFightSpiritLevelUp : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            UserChristmasInfo info = Player.Actives.Christmas;
            int snowTemplateID = 201144;
            int count = packet.ReadInt();
            bool isDouble = packet.ReadBoolean();
            int snowCount = Player.GetItemCount(snowTemplateID);
            if (count > snowCount)
            {
                Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg3"));
                return false;
            }

            if (count > 5)
            {
                count = 5;
            }

            bool isUp = false;
            int num = count;
            int snowNum = 0;
            int needExp = 10;
            if (isDouble)
            {
                if (Player.MoneyDirect(GameProperties.ChristmasBuildSnowmanDoubleMoney, false))
                {
                    num = count * 2;
                }
            }

            info.exp += num;
            if (info.exp >= needExp)
            {
                info.exp -= needExp;
                isUp = true;
                info.count++;
                snowNum = 1;
            }

            Player.RemoveTemplate(snowTemplateID, count);
            pkg.WriteByte((byte)ActiveSystemPackageType.FIGHT_SPIRIT_LEVELUP);
            pkg.WriteBoolean(isUp); //_loc_2.isUp = param1.readBoolean();
            pkg.WriteInt(info.count); //_model.count = param1.readInt();
            pkg.WriteInt(info.exp); //_model.exp = param1.readInt();
            pkg.WriteInt(num); //_loc_2.num = param1.readInt();
            pkg.WriteInt(snowNum); //_loc_2.snowNum = param1.readInt();                   
            Player.Out.SendTCP(pkg);

            return true;
        }
    }
}