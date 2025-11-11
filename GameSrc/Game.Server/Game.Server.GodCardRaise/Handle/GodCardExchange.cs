using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle
{
    [GodCardRaiseHandleAttbute((byte)GodCardRaisePackageType.EXCHANGE)]
    public class GodCardExchange : IGodCardRaiseCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int groupId = packet.ReadInt();
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
            pkg.WriteByte((byte)GodCardRaisePackageType.EXCHANGE);
            pkg.WriteInt(groupId);
            GodCardGroupInfo info = GodCardGroupMgr.FindGodCardGroup(groupId);
            if (info != null)
            {
                if (info.ExchangeTimes - player.Actives.FindCardExchangesCount(groupId) <= 0)
                {
                    player.SendMessage(LanguageMgr.GetTranslation("GodCardExchange.End"));
                    return 0;
                }

                pkg.WriteInt(player.Actives.SaveListCardExchanges(groupId, 1));
                GodCardGroupDetailInfo[] listItems = GodCardGroupMgr.FindGodCardGroupDetail(info.GroupID).ToArray();
                pkg.WriteInt(listItems.Length);
                if (player.Actives.CheckCardExchangesCount(listItems))
                {
                    var itemnew = ItemMgr.FindItemTemplate(info.GiftID);
                    if (itemnew != null)
                    {
                        if (itemnew != null)
                        {
                            if (!player.FindEmptySlot(itemnew.BagType))
                            {
                                player.SendMessage(LanguageMgr.GetTranslation("GodCardExchange.BagFull"));
                                return 0;
                            }

                            ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemnew, 1, 102);
                            itemInfo.IsBinds = true;
                            player.SendItemsToMail(itemInfo, $"Quà Bói thẻ từ {info.GroupName}", "Event Bói Thẻ", eMailType.BuyItem);
                            player.SendMessage(LanguageMgr.GetTranslation("GodCardExchange.Success", itemnew.Name, itemInfo.Count));
                        }
                    }

                    foreach (var card in listItems)
                    {
                        pkg.WriteInt(card.CardID);
                        pkg.WriteInt(player.Actives.RemoveListCard(card.CardID, card.Number));
                    }
                }
                else
                {
                    foreach (var card in listItems)
                    {
                        pkg.WriteInt(card.CardID);
                        pkg.WriteInt(player.Actives.FindCardCount(card.CardID));
                    }
                }

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