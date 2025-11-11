using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CHRISTMAS_PACKS)]
    public class ChristmasPacks : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            UserChristmasInfo info = Player.Actives.Christmas;
            string title = "Event Noel";
            int templateID = packet.ReadInt();
            if (DateTime.Compare(Player.LastOpenChristmasPackage.AddSeconds(1.0), DateTime.Now) > 0)
            {
                return false;
            }

            if (info.packsNumber >= GameProperties.ChristmasGiftsMaxNum - 1)
            {
                Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg6"));
                return false;
            }

            string[] lists = GameProperties.ChristmasGifts.Split('|');
            string GetGift = "";
            //int packsLen = lists.Length;
            int packCount = 0;
            foreach (string gift in lists)
            {
                if (gift.Split(',')[0] == templateID.ToString())
                {
                    GetGift = gift;
                    break;
                }

                packCount++;
            }

            if (GetGift != "")
            {
                int needSnowerCount = int.Parse(GetGift.Split(',')[1]);
                if ((info.awardState >> packCount & 1) != 0)
                {
                    return false;
                }

                if (packCount >= lists.Length - 1)
                {
                    string PrevGift = lists[packCount - 1];
                    needSnowerCount += int.Parse(PrevGift.Split(',')[1]);
                }

                if (needSnowerCount <= info.count)
                {
                    info.packsNumber++;
                    info.awardState = info.awardState | 1 << packCount;
                    Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg4"));
                    Player.SendItemToMail(templateID, "", title);
                }
                else
                {
                    Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg7"));
                    return false;
                }
            }
            else
            {
                Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg8"));
                return false;
            }

            pkg.WriteByte((byte)ActiveSystemPackageType.CHRISTMAS_PACKS);
            pkg.WriteInt(info.awardState); //awardState = _loc_2.readInt();
            pkg.WriteInt(info.packsNumber); //packsNumber = _loc_2.readInt();
            pkg.WriteInt(templateID); //itemID = _loc_2.readInt();
            Player.Out.SendTCP(pkg);
            Player.LastOpenChristmasPackage = DateTime.Now;
            return true;
        }
    }
}