using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle
{
    [GodCardRaiseHandleAttbute((byte)GodCardRaisePackageType.OPERATE_CARD)]
    public class GodCardOperateCard : IGodCardRaiseCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int type = packet.ReadInt();
            int cardId = packet.ReadInt();
            int count = packet.ReadInt();
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
            pkg.WriteByte((byte)GodCardRaisePackageType.OPERATE_CARD);
            GodCardInfo item = GodCardMgr.FindGodCard(cardId);
            if (item != null)
            {
                int currCount = player.Actives.FindCardCount(item.ID);
                pkg.WriteInt(item.ID);
                switch (type)
                {
                    case 1:
                        {
                            if (currCount >= count)
                            {
                                pkg.WriteInt(player.Actives.RemoveListCard(item.ID, count));
                                player.Actives.Info.cardChipCount += item.Decompose * count;
                                player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Success", item.Decompose * count));
                            }
                            else
                            {
                                pkg.WriteInt(currCount);
                            }

                            break;
                        }
                    case 2:
                        {
                            int needChip = item.Composition * count;
                            if (player.Actives.Info.cardChipCount >= needChip)
                            {
                                player.Actives.Info.cardChipCount -= needChip;
                                player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Success2", item.Name, count));
                                pkg.WriteInt(player.Actives.SaveListCard(item.ID, count));
                            }
                            else
                            {
                                pkg.WriteInt(currCount);
                            }

                            break;
                        }
                }

                pkg.WriteInt(player.Actives.Info.cardChipCount);
                player.Out.SendTCP(pkg);
            }
            else
            {
                player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Fail"));
            }

            return 1;
        }
    }
}