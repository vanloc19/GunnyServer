using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.GET_PAKCS_TO_PLAYER)]
    public class ChristmasGetPakcsToPlayer : IActiveSystemCommandHadler
    {
        public static Random random = new Random();

        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            UserChristmasInfo info = Player.Actives.Christmas;
            string title = "Event Noel";
            byte type = packet.ReadByte();
            int[] templateID = { 201146, 201147 };
            int index = random.Next(templateID.Length);
            if (DateTime.Compare(Player.LastOpenChristmasPackage.AddSeconds(1.0), DateTime.Now) > 0)
            {
                return false;
            }

            if (type == 1 && info.dayPacks < 2)
            {
                info.dayPacks++;
                Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg4"));
                Player.SendItemToMail(templateID[index], "", title);
            }
            else
            {
                if (info.count < 3)
                {
                    Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg5"));
                }
                else
                {
                    pkg.WriteByte((byte)ActiveSystemPackageType.GET_PAKCS_TO_PLAYER);
                    pkg.WriteBoolean(true); //_loc_3:* = _loc_2.readBoolean();
                    pkg.WriteInt(info.dayPacks); //_loc_4:* = _loc_2.readInt();
                    pkg.WriteInt(0); // _loc_5:* = _loc_2.readInt();
                    pkg.WriteInt(0); // _loc_6:* = _loc_2.readInt();
                    Player.Out.SendTCP(pkg);
                }
            }

            Player.LastOpenChristmasPackage = DateTime.Now;
            return true;
        }
    }
}