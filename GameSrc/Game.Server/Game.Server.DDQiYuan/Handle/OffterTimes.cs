using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DDTQiYuan.Handle
{
    [QiYuanHandleAttbute((byte)QiYuanPackageType.PACK_TYPE_OFFER)]
    public class OffterTimes : IQiYuanCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int times = packet.ReadInt();
            bool isBind = packet.ReadBoolean();

            bool canBuy = false;

            if (times <= 0)
                times = 1;
            else
                times = 10;

            // check item count
            int countItems = player.PropBag.GetItemCount(12543);
            if (countItems >= times)
            {
                canBuy = player.PropBag.RemoveTemplate(12543, times);
            }
            else
            {
                // check money
                string[] moneyNeedPayArr = GameProperties.DdtLuckWorshipMoney.Split('|');
                int moenyPay = 0;
                if (times == 1)
                    moenyPay = int.Parse(moneyNeedPayArr[0]);
                else if (times == 10)
                    moenyPay = int.Parse(moneyNeedPayArr[1]);

                if (moenyPay > 0 && player.PlayerCharacter.Money >= moenyPay)
                {
                    player.RemoveMoney(moenyPay, isConsume: false);
                    canBuy = true;
                }
            }

            if (canBuy)
            {
                List<ItemInfo> itemsAwards = new List<ItemInfo>();
                // send award
                for (int i = 0; i < times; i++)
                {
                    QiYuanAwardInfo[] tempList = QiYuanAwardMgr.GetRandom(eQiYuanAwardType.REWARD_BOX);
                    if (tempList != null && tempList.Length > 0)
                    {
                        ItemInfo temp = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(tempList[0].TemplateID),
                            tempList[0].Count, 103);
                        temp.IsBinds = tempList[0].IsBind;
                        temp.ValidDate = tempList[0].VaildDate;

                        itemsAwards.Add(temp);
                    }
                }

                // send to center
                packet.ClientID = player.PlayerId;
                packet.WriteHeader();

                GSPacketIn pkg = new GSPacketIn((int)ePackageType.SAN_XIAO);
                pkg.WriteByte((byte)QiYuanPackageType.PACK_TYPE_OFFER);

                pkg.WriteInt(itemsAwards.Count);
                packet.WriteInt(itemsAwards.Count);

                foreach (ItemInfo item in itemsAwards)
                {
                    player.AddTemplate(item);

                    pkg.WriteInt(item.TemplateID);
                    pkg.WriteInt(item.Count);
                    pkg.WriteBoolean(item.IsBinds);
                    pkg.WriteInt(item.ValidDate);
                    pkg.WriteInt(item.StrengthenLevel);
                    pkg.WriteInt(item.AttackCompose);
                    pkg.WriteInt(item.DefendCompose);
                    pkg.WriteInt(item.AgilityCompose);
                    pkg.WriteInt(item.LuckCompose);
                    /*pkg.WriteInt(item.MagicAttack);
                    pkg.WriteInt(item.MagicDefence);*/

                    packet.WriteInt(item.TemplateID);
                    packet.WriteInt(item.Count);
                }

                player.SendTCP(pkg);

                GameServer.Instance.LoginServer.SendPacket(packet);
            }
            else
            {
                player.SendMessage(LanguageMgr.GetTranslation("QiYuan.CenterServer.OffterTimesNoMoney"));
            }

            return 1;
        }
    }
}