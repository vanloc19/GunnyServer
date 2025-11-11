using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DDTQiYuan.Handle
{
    [QiYuanHandleAttbute((byte)QiYuanPackageType.PACK_TYPE_OPEN_BOGU_BOX)]
    public class OpenBoguBox : IQiYuanCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            bool isBind = packet.ReadBoolean();

            if (packet.DataLeft <= 0)
            {
                // send request to center
                packet.ClientID = player.PlayerId;
                packet.WriteHeader();
                GameServer.Instance.LoginServer.SendPacket(packet);
            }
            else
            {
                bool canBuy = false;
                int countItems = player.PropBag.GetItemCount(12544);
                if (countItems > 0)
                {
                    canBuy = player.PropBag.RemoveTemplate(12544, 1);
                }
                else
                {
                    int moneyNeedPay = GameProperties.DdtLuckOpenBoxMoney;
                    canBuy = player.RemoveMoney(moneyNeedPay, isConsume: false) > 0 ? true : false;
                }

                if (canBuy)
                {
                    QiYuanAwardInfo[] tempList = QiYuanAwardMgr.GetRandom(eQiYuanAwardType.TREASURE_BOX);
                    if (tempList != null && tempList.Length > 0)
                    {
                        ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(tempList[0].TemplateID),
                            tempList[0].Count, 103);
                        item.IsBinds = tempList[0].IsBind;
                        item.ValidDate = tempList[0].VaildDate;

                        player.AddTemplate(item);

                        GSPacketIn pkg = new GSPacketIn((int)ePackageType.SAN_XIAO);
                        pkg.WriteByte((byte)QiYuanPackageType.PACK_TYPE_OPEN_BOGU_BOX);

                        pkg.WriteInt(1);

                        pkg.WriteInt(item.TemplateID);//inventoryItemInfo.TemplateID = pkg.readInt();
                        pkg.WriteInt(item.Count);//inventoryItemInfo.Count = pkg.readInt();
                        pkg.WriteBoolean(item.IsBinds);//inventoryItemInfo.IsBinds = pkg.readBoolean();
                        pkg.WriteInt(item.ValidDate);//inventoryItemInfo.ValidDate = pkg.readInt();
                        pkg.WriteInt(item.StrengthenLevel);//inventoryItemInfo.StrengthenLevel = pkg.readInt();
                        pkg.WriteInt(item.AttackCompose);//inventoryItemInfo.AttackCompose = pkg.readInt();
                        pkg.WriteInt(item.DefendCompose);//inventoryItemInfo.DefendCompose = pkg.readInt();
                        pkg.WriteInt(item.AgilityCompose);//inventoryItemInfo.AgilityCompose = pkg.readInt();
                        pkg.WriteInt(item.LuckCompose);//inventoryItemInfo.LuckCompose = pkg.readInt();
                        /*pkg.WriteInt(item.MagicAttack);
                        pkg.WriteInt(item.MagicDefence);*/

                        player.SendTCP(pkg);
                    }
                }
                else
                {
                    player.SendMessage(LanguageMgr.GetTranslation("QiYuan.CenterServer.OffterTimesNoMoney"));
                }
            }

            return 1;
        }
    }
}