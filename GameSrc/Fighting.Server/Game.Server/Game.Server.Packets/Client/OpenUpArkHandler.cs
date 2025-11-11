using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
    [PacketHandler(63, "打开物品")]
    public class OpenUpArkHandler : IPacketHandler
    {
        public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int bageType = (int)packet.ReadByte();
            int slot = packet.ReadInt();
            int num = packet.ReadInt();
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();
            PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
            ItemInfo itemAt = inventory.GetItemAt(slot);
            string str = "";
            int num3;
            int result;
            if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 11 && itemAt.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel)
            {
                if (num < 1 || num > itemAt.Count)
                {
                    num = itemAt.Count;
                }
                int num2 = 0;
                string arg = "";
                StringBuilder stringBuilder = new StringBuilder();
                StringBuilder stringBuilder2 = new StringBuilder();
                if (!inventory.RemoveCountFromStack(itemAt, num))
                {
                    num3 = 0;
                    result = num3;
                    return result;
                }
                Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
                List<ItemInfo> list = new List<ItemInfo>();
                stringBuilder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start", new object[0]));
                for (int i = 0; i < num; i++)
                {
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    int num8 = 0;
                    int num10 = 0;
                    int num11 = 0;
                    int num12 = 0;
                    int num13 = 0;
                    Random rand = new Random();
                    List<ItemInfo> list2 = new List<ItemInfo>();
                    ItemBoxMgr.CreateItemBox(itemAt.TemplateID, list2, ref num5, ref num4, ref num6, ref num7, ref num8, ref num10, ref num11, ref num12, ref num13);
                    #region Rương Xu
                    if (itemAt.TemplateID == 52000621)//Rương Trung Cấp
                    {
                        int moneyRand = rand.Next(500, 1000);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} xu từ Rương Xu PVE(Trung Cấp)", moneyRand));
                        client.Player.AddMoney(moneyRand);
                    }
                    if (itemAt.TemplateID == 52000622)//Rương Cao Cấp
                    {
                        int moneyRand = rand.Next(5000, 10000);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} xu từ Rương Xu PVE(Cao Cấp)", moneyRand));
                        client.Player.AddMoney(moneyRand);
                    }
                    if(itemAt.TemplateID == 11917)
                    {
                        int myHonor = 200;
                        client.Player.AddHonor(myHonor);
                        client.Player.SendMessage($"Bạn nhận được {myHonor} tinh hoa vinh dự");
                    }
                    #endregion
                    #region Rương Random
                    if (itemAt.TemplateID == 52000633)
                    {
                        int totalCount = rand.Next(30, 50);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(GameProperties.AwardItemBox);
                        ItemInfo item = ItemInfo.CreateFromTemplate(goods, totalCount, 106);
                        item.IsBinds = true;
                        client.Player.PropBag.AddItem(item);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} {1}.", totalCount, item.Template.Name));
                    }
                    if (itemAt.TemplateID == 52000634)
                    {
                        int totalCount = rand.Next(50, 100);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(GameProperties.AwardItemBox);
                        ItemInfo item = ItemInfo.CreateFromTemplate(goods, totalCount, 106);
                        item.IsBinds = true;
                        client.Player.PropBag.AddItem(item);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} {1}.", totalCount, item.Template.Name));
                    }
                    if (itemAt.TemplateID == 52000635)
                    {
                        int totalCount = rand.Next(100, 200);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(GameProperties.AwardItemBox);
                        ItemInfo item = ItemInfo.CreateFromTemplate(goods, totalCount, 106);
                        item.IsBinds = true;
                        client.Player.PropBag.AddItem(item);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} {1}.", totalCount, item.Template.Name));
                    }
                    if (itemAt.TemplateID == 52000636)
                    {
                        int totalCount = rand.Next(200, 400);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(GameProperties.AwardItemBoxI);
                        ItemInfo item = ItemInfo.CreateFromTemplate(goods, totalCount, 106);
                        item.IsBinds = true;
                        client.Player.PropBag.AddItem(item);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} {1}.", totalCount, item.Template.Name));
                    }
                    if (itemAt.TemplateID == 52000637)
                    {
                        int totalCount = rand.Next(350, 500);
                        ItemTemplateInfo goods = ItemMgr.FindItemTemplate(GameProperties.AwardItemBoxI);
                        ItemInfo item = ItemInfo.CreateFromTemplate(goods, totalCount, 106);
                        item.IsBinds = true;
                        client.Player.PropBag.AddItem(item);
                        client.Player.SendMessage(string.Format("Bạn nhận được {0} {1}.", totalCount, item.Template.Name));
                    }
                    #endregion

                    if (num4 != 0)
                    {
                        num2 += num4;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]);
                        client.Player.AddMoney(num4);
                    }
                    if (num5 != 0)
                    {
                        num2 += num5;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]);
                        client.Player.AddGold(num5);
                    }
                    if (num6 != 0)
                    {
                        num2 += num6;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]);
                        client.Player.AddGiftToken(num6);
                    }
                    if (num7 != 0)
                    {
                        num2 += num7;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]);
                        client.Player.AddMedal(num7);
                    }
                    if (num8 != 0)
                    {
                        num2 += num8;
                        if (client.Player.Level == LevelMgr.MaxLevel)
                        {
                            int num16 = num2 / 500;
                            if (num16 > 0)
                            {
                                client.Player.AddOffer(num16);
                                arg = string.Format("Max level khinh nghiệm quy đổi thành {0} công trạng", num16);
                            }
                        }
                        else
                        {
                            arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Exp", new object[0]);
                            client.Player.AddGP(num8, false, false);
                        }
                    }
                    if (num10 != 0)
                    {
                        num2 += num10;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.hardCurrency", new object[0]);
                        client.Player.AddHardCurrency(num10);
                    }
                    if (num11 != 0)
                    {
                        num2 += num11;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.leagueMoney", new object[0]);
                        client.Player.AddLeagueMoney(num11);
                    }
                    if (num12 != 0)
                    {
                        num2 += num12;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.useableScore", new object[0]);
                    }
                    if (num13 != 0)
                    {
                        num2 += num13;
                        arg = LanguageMgr.GetTranslation("OpenUpArkHandler.prestge", new object[0]);
                    }
                    foreach (ItemInfo current in list2)
                    {
                        if (!dictionary.ContainsKey(current.TemplateID))
                        {
                            dictionary.Add(current.TemplateID, current);
                        }
                        else
                        {
                            ItemInfo itemInfo = dictionary[current.TemplateID];
                            itemInfo.Count += current.Count;
                        }
                    }
                }
                string name = itemAt.Template.Name;
                if (num2 > 0)
                {
                    stringBuilder2.Append(num2 + arg);
                }
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    string[] array = stringBuilder.ToString().Split(new char[]
                    {
                        ','
                    });
                    for (int j = 0; j < array.Length; j++)
                    {
                        int num17 = 1;
                        for (int k = j + 1; k < array.Length; k++)
                        {
                            if (array[j].Contains(array[k]) && array[k].Length == array[j].Length)
                            {
                                num17++;
                                array[k] = k.ToString();
                            }
                        }
                        if (num17 > 1)
                        {
                            array[j] = array[j].Remove(array[j].Length - 1, 1);
                            string[] array2;
                            IntPtr intPtr;
                            (array2 = array)[(int)(intPtr = (IntPtr)j)] = array2[(int)intPtr] + num17.ToString();
                        }
                        if (array[j] != j.ToString())
                        {
                            string[] array2;
                            IntPtr intPtr;
                            (array2 = array)[(int)(intPtr = (IntPtr)j)] = array2[(int)intPtr] + ",";
                            stringBuilder2.Append(array[j]);
                        }
                    }
                }
                stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
                stringBuilder2.Append(".");
                if (num2 > 0)
                {
                    client.Out.SendMessage(eMessageType.GM_NOTICE, str + stringBuilder2.ToString());
                }
                if (dictionary.Count > 0)
                {
                    string translation = "";
                    GSPacketIn gSPacketIn = new GSPacketIn(63, client.Player.PlayerCharacter.ID);
                    gSPacketIn.WriteString(name);
                    gSPacketIn.WriteInt(dictionary.Count);//((byte)dictionary.Count);
                    foreach (ItemInfo current2 in dictionary.Values)
                    {
                        gSPacketIn.WriteInt(current2.TemplateID);
                        gSPacketIn.WriteInt(current2.Count);
                        gSPacketIn.WriteBoolean(current2.IsBinds);
                        gSPacketIn.WriteInt(current2.ValidDate);
                        gSPacketIn.WriteInt(current2.StrengthenLevel);
                        gSPacketIn.WriteInt(current2.AttackCompose);
                        gSPacketIn.WriteInt(current2.DefendCompose);
                        gSPacketIn.WriteInt(current2.AgilityCompose);
                        gSPacketIn.WriteInt(current2.LuckCompose);
                        stringBuilder.Append(current2.Template.Name + "x" + current2.Count.ToString() + ",");
                        List<ItemInfo> list3 = ItemMgr.SpiltGoodsMaxCount(current2);
                        foreach (ItemInfo current3 in list3)
                        {
                            if (!client.Player.StoreBag.AddItem(current3))
                            {
                                list.Add(current3);
                            }
                            if (current2.IsTips)
                            {
                                translation = LanguageMgr.GetTranslation("GameServer.OpenUpArkNoticeServer", client.Player.PlayerCharacter.NickName, name, current3.TemplateID, current3.Count);
                                GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, current3.ItemID, current3.TemplateID, null);
                                GameServer.Instance.LoginServer.SendPacket(packet2);
                            }
                        }
                    }
                    #region Explorer
                    gSPacketIn.WriteInt(0);
                    gSPacketIn.WriteInt(itemAt.Template.CategoryID);
                    gSPacketIn.WriteInt(0);
                    #endregion
                    client.Player.SendTCP(gSPacketIn);
                    if (list.Count > 0)
                    {
                        client.Player.SendItemsToMail(list, "Vật phẩm gửi về thư do mở quà trong khi túi đầy.", "Mở túi quà đầy gửi về thư", eMailType.BuyItem);
                    }
                }
            }
            else
            {
                if(itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 72)
                {
                    if (!inventory.RemoveCountFromStack(itemAt, num));
                    {
                        int value = 1;
                        int type = 0;
                        switch(itemAt.TemplateID)
                        {
                            case 1120657:
                                value = 3200;
                                type = 1;
                                break;
                            case 1120656:
                                value = (17500);
                                type = 1;
                                break;
                            case 1120655:
                                value = (96000);
                                type = 1;
                                break;
                            case 1120637://Sổ Rách Nát With Type = 1;
                                type = 2;
                                getJamps(client, num, 9, -1, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120638://Sổ Nguyên Vẹn With Type = 2;
                                type = 2;
                                getJamps(client, num, 16, -1, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120639://Sổ Rực Rỡ With Type = 3;
                                type = 2;
                                getJamps(client, num, 25, -1, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120640:
                                type = 2;
                                getJamps(client, num, 9, 1001, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120641:
                                type = 2;
                                getJamps(client, num, 9, 1002, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120642:
                                type = 2;
                                getJamps(client, num, 9, 1003, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120643:
                                type = 2;
                                getJamps(client, num, 9, 1004, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120644:
                                type = 2;
                                getJamps(client, num, 9, 1005, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120645:
                                type = 2;
                                getJamps(client, num, 16, 1001, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120646:
                                type = 2;
                                getJamps(client, num, 16, 1002, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120647:
                                type = 2;
                                getJamps(client, num, 16, 1003, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120648:
                                type = 2;
                                getJamps(client, num, 16, 1004, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120649:
                                type = 2;
                                getJamps(client, num, 16, 1005, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120650:
                                type = 2;
                                getJamps(client, num, 25, 1001, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120651:
                                type = 2;
                                getJamps(client, num, 25, 1002, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120652:
                                type = 2;
                                getJamps(client, num, 25, 1003, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120653:
                                type = 2;
                                getJamps(client, num, 25, 1004, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                            case 1120654:
                                type = 2;
                                getJamps(client, num, 25, 1005, ref value);
                                client.Player.EquipBag.UpdatePlayerProperties();
                                break;
                        }

                        if (type == 2)
                        {
                            num = 1;
                        }

                        int jampsCurrency = value * num;
                        client.Player.AddJampsCurrency(jampsCurrency);
                        switch (type)
                        {
                            case 1:
                            case 2:
                                client.Player.SendMessage($"Bạn nhận được {jampsCurrency} điểm thám hiểm.");
                                break;
                        }
                    }
                }
            }
            num3 = 1;
            result = num3;
            return result;
        }

        private void getJamps(GameClient client, int num, int max, int chapter, ref int value)
        {
            List<int> intList = new List<int>();
            for (int index = 0; index < num; ++index)
            {
                string msg = "";
                JampsChapterItemList jampsChapterItemList = chapter == -1 ? JampsManualMgr.getRandomChapter() : JampsManualMgr.getChapter(chapter);
                JampsPageItemList randomPageFromChapter = JampsManualMgr.getRandomPageFromChapter(jampsChapterItemList.ID, max);
                PagesInfo p = new PagesInfo();
                p.activate = false;
                p.pageID = randomPageFromChapter.ID;
                if (JampsManualMgr.randNumber(1, 100) <= 60)
                    value += 40;
                else if (JampsManualMgr.randNumber(1, 100) <= 30 || client.Player.PlayerCharacter.explorerManualInfo.activesPage.Count == 0)
                {
                    if (client.Player.PlayerCharacter.explorerManualInfo.addPage(p))
                    {
                        msg = msg + "chúc mừng bạn nhận được sổ thám hiểm:" + randomPageFromChapter.Name;
                        if (!intList.Contains(jampsChapterItemList.ID))
                            intList.Add(jampsChapterItemList.ID);
                    }
                    else
                        value += 60;
                }
                else if (JampsManualMgr.randNumber(1, 100) <= 15)
                {
                    JampsDebrisItemList jampsDebrisItemList = chapter == -1 ? JampsManualMgr.getRandomDebrisFromPages(client.Player.PlayerCharacter.explorerManualInfo.activesPage) : JampsManualMgr.getRandomDebrisFromPages(client.Player.PlayerCharacter.explorerManualInfo.activesPage, chapter);
                    DebrisInfo debris = new DebrisInfo();
                    debris.date = DateTime.Now;
                    debris.ID = jampsDebrisItemList.ID;
                    debris.pageID = jampsDebrisItemList.PageID;
                    debris.chapterID = JampsManualMgr.getChapterIDFromDebrisID(jampsDebrisItemList.ID);
                    if (debris.chapterID == -1)
                        msg = msg + "không đạt" + jampsDebrisItemList.Describe;
                    else if (client.Player.PlayerCharacter.explorerManualInfo.addDebris(debris))
                    {
                        msg = msg + "chúc mừng bạn nhận được sổ thám hiểm:" + jampsDebrisItemList.Describe;
                        if (!intList.Contains(debris.chapterID))
                            intList.Add(debris.chapterID);
                    }
                }
                else
                    value += 25;
                if (msg != "")
                    client.Player.SendMessage(msg);
            }
            GSPacketIn pkg = new GSPacketIn((short)63);
            pkg.WriteString("Sổ tay của Người khám phá");
            pkg.WriteInt(0);
            pkg.WriteInt(num);
            pkg.WriteInt(72);
            pkg.WriteInt(intList.Count);
            foreach (int val in intList)
                pkg.WriteInt(val);
            client.SendTCP(pkg);
        }
    }
}