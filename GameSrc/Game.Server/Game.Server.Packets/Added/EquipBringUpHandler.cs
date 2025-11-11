using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.EQUIP_BRING_UP, "客户端日记")]
    public class EquipBringUpHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int count = packet.ReadInt();
            ItemInfo tagItem = client.Player.StoreBag.GetItemAt(0);
            if (tagItem == null)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("EquipBringUpHandler.ItemNotFound"));
                return 0;
            }

            ItemTemplateInfo nextItem = tagItem.Template;
            if (nextItem == null)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("EquipBringUpHandler.DoNotSuport"));
                return 0;
            }

            bool isSuccessful = false;
            string msg = LanguageMgr.GetTranslation("EquipBringUpHandler.Update.Fail");
            string scorssMsg = string.Empty;
            int exp = 0;

            int place = -1;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    eBageType bagType = (eBageType)packet.ReadInt(); //pkg.writeInt(arg.shift());
                    place = packet.ReadInt(); //pkg.writeInt(arg.shift());                        
                    ItemInfo beEatenItem = client.Player.GetItemAt(bagType, place);
                    if (beEatenItem != null && beEatenItem.IsBring() && !beEatenItem.cellLocked)
                    {
                        exp += CalculateExperience(tagItem.Template, beEatenItem);
                        client.Player.EquipBag.RemoveItemAt(place);
                    }

                    //Console.WriteLine("bagType: {0}, place: {1}", bagType, place);
                }

                if (exp != 0)
                {
                    isSuccessful = true;
                    //tagItem.curExp += exp;
                }
                else
                {
                    tagItem = client.Player.StoreBag.GetItemAt(0);
                }

                //Console.WriteLine("exp {0}, count {1}", exp, count);
            }
            else
            {
                packet.ReadInt(); //pkg.writeInt(arg.shift());
                exp = packet.ReadInt(); //pkg.writeInt(arg.shift());
                int needMoney = 35 * exp;
                if (client.Player.MoneyDirect(needMoney, false))
                {
                    isSuccessful = true;
                    //tagItem.curExp += exp;
                }

                //Console.WriteLine("bagType: {0}, buyCount: {1}", bagType, exp);
            }

            if (isSuccessful)
            {
                tagItem.IsBinds = true;
                int totalExp = exp + tagItem.curExp;
                int curExp = tagItem.curExp;
                while (curExp <= totalExp)
                {
                    nextItem = ItemMgr.FindItemTemplate(nextItem.FusionType);
                    //Console.WriteLine("curExp: {0}, tempItem.Property2: {1}:: totalExp: {2} exp: {3}", curExp, nextItem.Property2, totalExp, exp);
                    if (nextItem != null && totalExp >= nextItem.Property2)
                    {
                        msg = LanguageMgr.GetTranslation("EquipBringUpHandler.Update.Success2", tagItem.Template.Name,
                            nextItem.Name);
                        ItemInfo newItem = ItemInfo.CloneFromTemplate(nextItem, tagItem);
                        client.Player.StoreBag.RemoveItemAt(0);
                        client.Player.StoreBag.AddItemTo(newItem, 0);
                        tagItem = newItem;
                        if (nextItem.Property1 >= 5)
                        {
                            scorssMsg = LanguageMgr.GetTranslation("EquipBringUpHandler.congratulation",
                                client.Player.PlayerCharacter.NickName, tagItem.TemplateID);
                        }
                    }
                    else
                    {
                        msg = LanguageMgr.GetTranslation("EquipBringUpHandler.Update.Success1", exp);
                    }

                    curExp += nextItem?.Property2 ?? 0;
                }

                tagItem.curExp = totalExp;
            }

            client.Player.SendMessage(msg);

            GSPacketIn pkg = new GSPacketIn((int)ePackageType.EQUIP_BRING_UP);
            pkg.WriteBoolean(isSuccessful);
            client.Player.SendTCP(pkg);

            client.Player.StoreBag.UpdateItem(tagItem);

            client.Player.StoreBag.SaveNewItemToDatabase();

            if (!string.IsNullOrEmpty(scorssMsg) && tagItem.ItemID > 0)
            {
                GSPacketIn sysNotice = WorldMgr.SendSysNotice(eMessageType.SYS_TIP_NOTICE, scorssMsg, tagItem.ItemID, tagItem.TemplateID, null);
                GameServer.Instance.LoginServer.SendPacket(sysNotice);
            }

            return 0;
        }

        private int CalculateExperience(ItemTemplateInfo tagItem, ItemInfo beEatenItem)
        {
            var tagLevel = tagItem.Property1;
            var tagQuality = tagItem.Property3;

            var eatenLevel = beEatenItem.Template.Property1;
            var eatenOrigExp = beEatenItem.Template.Property2;
            var eatenQuality = beEatenItem.Template.Property3;
            var eatenCurExp = beEatenItem.curExp;

            if (tagQuality < eatenQuality)
            {
                ItemTemplateInfo beEatenTempleteInfo = GetTempleteInfoByLevel(eatenLevel, tagItem);
                eatenOrigExp = beEatenTempleteInfo.Property2;
                eatenQuality = tagQuality;
                eatenCurExp = eatenOrigExp;
            }

            if (eatenCurExp == 0)
            {
                eatenCurExp = eatenOrigExp;
            }

            return eatenCurExp;
        }

        private ItemTemplateInfo GetTempleteInfoByLevel(int eatenLevel, ItemTemplateInfo tagItemInfo)
        {
            int curLevel = tagItemInfo.Property1;
            if (curLevel > eatenLevel)
            {
                while (curLevel > eatenLevel)
                {
                    if (tagItemInfo.Property4 == 0)
                        break;

                    tagItemInfo = ItemMgr.FindItemTemplate(tagItemInfo.Property4);
                    curLevel = tagItemInfo.Property1;
                }
            }
            else if (curLevel < eatenLevel)
            {
                while (curLevel < eatenLevel)
                {
                    if (tagItemInfo.FusionType == 0)
                        break;

                    tagItemInfo = ItemMgr.FindItemTemplate(tagItemInfo.FusionType);
                    curLevel = tagItemInfo.Property1;
                }
            }

            return tagItemInfo;
        }
    }
}