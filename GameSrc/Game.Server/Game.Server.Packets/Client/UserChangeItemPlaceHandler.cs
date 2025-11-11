using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CHANGE_PLACE_GOODS, "改变物品位置")]
    public class UserChangeItemPlaceHandler : IPacketHandler
    {
        //修改:  XiaoJian
        //时间:  2020-11-7
        //描述:  改变物品位置<测试完成>   
        //状态： 正常使用

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bagType = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            eBageType tobagType = (eBageType)packet.ReadByte();
            int result;
            if (tobagType == eBageType.TempBag)
            {
                GameServer.log.Error("User want to put item into temp bag!");
                result = 0;
            }
            else
            {
                int toplace = packet.ReadInt();
                int count = packet.ReadInt();
                bool allMove = packet.ReadBoolean();
                PlayerInventory bag = client.Player.GetInventory(bagType);
                PlayerInventory tobag = client.Player.GetInventory(tobagType);
                if (tobagType == eBageType.TempBag)
                {
                    result = 0;
                }
                else
                {
                    bag.BeginChanges();
                    tobag.BeginChanges();
                    try
                    {
                        if (place != -1)
                        {
                            ItemInfo item = bag.GetItemAt(place);
                            ItemInfo item2 = bag.GetItemAt(toplace);
                            List<ItemInfo> mailList = new List<ItemInfo>();
                            if (tobagType == eBageType.Consortia)
                            {
                                ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                                if (info != null)
                                {
                                    tobag.Capalility = info.StoreLevel * 10;
                                }
                            }
                            if(toplace != -1 && bagType == tobagType && bagType == eBageType.EquipBag && toplace >= 31 && place < 31 && item != null && item2 != null && !item2.IsValidItem())
                            {
                                client.Player.SendMessage("Hết hạn không thể trang bị");
                                return 0;
                            }
                            if (toplace != -1 && bagType == tobagType && bagType == eBageType.EquipBag && toplace < 31 && place >= 31 && item != null && !item.IsValidItem())
                            {
                                client.Player.SendMessage("Hết hạn không thể trang bị");
                                return 0;
                            }
                            if (toplace == -1)
                            {
                                bool isFull = false;
                                if (bagType == eBageType.CaddyBag)
                                {
                                    toplace = tobag.FindFirstEmptySlot(tobag.BeginSlot);
                                    if (tobag.AddItemTo(item, toplace))
                                    {
                                        bag.TakeOutItem(item);
                                    }
                                    else
                                    {
                                        mailList.Add(item);
                                        bag.TakeOutItem(item);
                                        isFull = true;
                                    }
                                }
                                else if (bagType == tobagType && tobagType == eBageType.EquipBag)
                                {
                                    toplace = tobag.FindFirstEmptySlot(tobag.BeginSlot, 127);
                                    if (toplace > 0)
                                    {
                                        bag.MoveItem(place, toplace, count);
                                    }
                                    else
                                    {
                                        isFull = true;
                                    }
                                }
                                #region unknow
                                /*else if (bagType == eBageType.BankBag)
                                {
                                    toplace = tobag.FindFirstEmptySlot();// (tobag.BeginSlot, 127);
                                    if (toplace > 0)
                                    {
                                        bag.MoveItem(place, toplace, count);
                                    }
                                    else
                                    {
                                        isFull = true;
                                    }
                                }*/
                                #endregion
                                else if (bagType == eBageType.BankBag || bagType == eBageType.Consortia)
                                {
                                    toplace = tobag.FindFirstEmptySlot();
                                    if (toplace != -1)
                                    {
                                        MoveFromBank(client.Player, place, toplace, bag, tobag, item);
                                    }
                                }
                                else if (tobagType == eBageType.BankBag || tobagType == eBageType.Consortia)
                                {
                                    toplace = tobag.FindFirstEmptySlot();
                                    if (toplace != -1)
                                    {
                                        MoveToBank(place, toplace, bag, tobag, item);
                                    }
                                }
                                else if (tobag.StackItemToAnother(item) || tobag.AddItem(item))
                                {
                                    bag.TakeOutItem(item);
                                }
                                else
                                {
                                    isFull = true;
                                }
                                if (isFull)
                                {
                                    client.Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler..full"));
                                    if (mailList.Count > 0 && client.Player.SendItemsToMail(mailList, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.mail"), LanguageMgr.GetTranslation("UserGetTimeBoxHandler.title"), eMailType.OpenUpArk))
                                    {
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                    }
                                }
                            }
                            else if (bagType == tobagType)
                            {
                                bag.MoveItem(place, toplace, count);
                            }
                            else if (bagType == eBageType.Store)
                            {
                                MoveFromHide(client.Player, bag, item, toplace, tobag, count);
                            }
                            else if (tobagType == eBageType.Store)
                            {
                                MoveToHide(client.Player, bag, item, toplace, tobag, count);
                            }
                            else if (bagType == eBageType.BankBag || bagType == eBageType.Consortia)
                            {
                                MoveFromBank(client.Player, place, toplace, bag, tobag, item);
                            }
                            else if (tobagType == eBageType.BankBag || tobagType == eBageType.Consortia)
                            {
                                MoveToBank(place, toplace, bag, tobag, item);
                            }
                            else if (tobag.AddItemTo(item, toplace))
                            {
                                bag.TakeOutItem(item);
                            }
                        }
                        else if (toplace != -1)
                        {
                            bag.RemoveItemAt(toplace);
                        }
                    }
                    finally
                    {
                        bag.CommitChanges();
                        tobag.CommitChanges();
                    }
                    result = 0;
                }
            }
            return result;
        }

        private static void MoveFromBank(GamePlayer player, int place, int toplace, PlayerInventory bag, PlayerInventory tobag, ItemInfo item)
        {
            if (item != null)
            {
                PlayerInventory tb = player.GetItemInventory(item.Template);
                if (tb == tobag)
                {
                    ItemInfo toitem = tb.GetItemAt(toplace);
                    if (toitem == null)
                    {
                        if (tb.AddItemTo(item, toplace))
                        {
                            bag.TakeOutItem(item);
                        }
                    }
                    else if (item.CanStackedTo(toitem) && item.Count + toitem.Count <= item.Template.MaxCount)
                    {
                        if (tb.AddCountToStack(toitem, item.Count))
                        {
                            bag.RemoveItem(item);
                        }
                    }
                    else
                    {
                        tb.TakeOutItem(toitem);
                        bag.TakeOutItem(item);
                        tb.AddItemTo(item, toplace);
                        bag.AddItemTo(toitem, place);
                    }
                }
                else if (tb.AddItem(item))
                {
                    bag.TakeOutItem(item);
                }
            }
        }

        private static void MoveToBank(int place, int toplace, PlayerInventory bag, PlayerInventory BankBag, ItemInfo item)
        {
            if (bag != null && item != null && bag != null)
            {
                ItemInfo toitem = BankBag.GetItemAt(toplace);
                if (toitem != null)
                {
                    if (item.CanStackedTo(toitem) && item.Count + toitem.Count <= item.Template.MaxCount)
                    {
                        if (BankBag.AddCountToStack(toitem, item.Count))
                        {
                            bag.RemoveItem(item);
                        }
                    }
                    else if (toitem.Template.BagType == (eBageType)bag.BagType)
                    {
                        if (bag is PlayerEquipInventory equipInventory && place < equipInventory.BeginSlot
                            && !equipInventory.CanEquipSlotContains(place, toitem.Template))
                        {
                            place = bag.FindFirstEmptySlot();
                            if (place == -1)
                            {
                                return;
                            }
                        }

                        bag.TakeOutItem(item);
                        BankBag.TakeOutItem(toitem);
                        bag.AddItemTo(toitem, place);
                        BankBag.AddItemTo(item, toplace);
                    }
                }
                else if (BankBag.AddItemTo(item, toplace))
                {
                    bag.TakeOutItem(item);
                }
            }
        }

        public void MoveToHide(GamePlayer player, PlayerInventory bag, ItemInfo item, int toSlot, PlayerInventory StoreBag, int count)//MoveToStore
        {
            if (player != null && bag != null && item != null && StoreBag != null)
            {
                int oldplace = item.Place;
                ItemInfo toItem = StoreBag.GetItemAt(toSlot);
                if (toItem != null)
                {
                    if (toItem.CanStackedTo(item))
                    {
                        return;
                    }
                    if (item.Count == 1 && item.BagType == toItem.BagType)
                    {
                        bag.TakeOutItem(item);
                        StoreBag.TakeOutItem(toItem);
                        bag.AddItemTo(toItem, oldplace);
                        StoreBag.AddItemTo(item, toSlot);
                        return;
                    }
                    string key = string.Format("temp_place_{0}", toItem.ItemID);
                    PlayerInventory tb = player.GetItemInventory(toItem.Template);
                    if (player.TempProperties.ContainsKey(key) && tb.BagType == 0)
                    {
                        int tempSlot = (int)player.TempProperties[key];
                        player.TempProperties.Remove(key);
                        if (tb.AddItemTo(toItem, tempSlot))
                        {
                            StoreBag.TakeOutItem(toItem);
                        }
                    }
                    else if (tb.StackItemToAnother(toItem))
                    {
                        StoreBag.RemoveItem(toItem);
                    }
                    else if (tb.AddItem(toItem))
                    {
                        StoreBag.TakeOutItem(toItem);
                    }
                    else
                    {
                        player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
                    }
                }
                if (StoreBag.IsEmpty(toSlot))
                {
                    if (item.Count == 1)
                    {
                        if (StoreBag.AddItemTo(item, toSlot))
                        {
                            bag.TakeOutItem(item);
                            if (item.Template.BagType == eBageType.EquipBag)
                            {
                                string key = string.Format("temp_place_{0}", item.ItemID);
                                if (player.TempProperties.ContainsKey(key))
                                {
                                    player.TempProperties[key] = oldplace;
                                }
                                else
                                {
                                    player.TempProperties.Add(key, oldplace);
                                }
                            }
                        }
                    }
                    else
                    {
                        ItemInfo newItem = item.Clone();
                        newItem.Count = count;
                        if (bag.RemoveCountFromStack(item, count))
                        {
                            if (!StoreBag.AddItemTo(newItem, toSlot))
                            {
                                bag.AddCountToStack(item, count);
                            }
                        }
                    }
                }
            }
        }

        public void MoveFromHide(GamePlayer player, PlayerInventory StoreBag, ItemInfo item, int toSlot, PlayerInventory bag, int count)//MoveFromStore
        {
            if (player == null || item == null || StoreBag == null || bag == null || item.Template.BagType != (eBageType)bag.BagType || item.Template.BagType != (eBageType)bag.BagType)
            {
                return;
            }
            if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
            {
                if (bag.StackItemToAnother(item))
                {
                    StoreBag.RemoveItem(item, eItemRemoveType.Stack);
                    return;
                }
                string key = $"temp_place_{item.ItemID}";
                if (player.TempProperties.ContainsKey(key))
                {
                    toSlot = (int)StoreBag.Player.TempProperties[key];
                    StoreBag.Player.TempProperties.Remove(key);
                }
                else
                {
                    toSlot = bag.FindFirstEmptySlot();
                }
            }
            if (!bag.StackItemToAnother(item) && !bag.AddItemTo(item, toSlot))
            {
                toSlot = bag.FindFirstEmptySlot();
                if (bag.AddItemTo(item, toSlot))
                {
                    StoreBag.TakeOutItem(item);
                    return;
                }
                StoreBag.TakeOutItem(item);
                player.SendItemToMail(item, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
                player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            else
            {
                StoreBag.TakeOutItem(item);
            }
            /*if (player != null && item != null && StoreBag != null && bag != null)
            {
                if (item.Template.BagType == (eBageType)bag.BagType)
                {
                    if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
                    {
                        if (bag.StackItemToAnother(item))
                        {
                            StoreBag.RemoveItem(item, eItemRemoveType.Stack);
                            return;
                        }
                        string key = string.Format("temp_place_{0}", item.ItemID);
                        if (player.TempProperties.ContainsKey(key))
                        {
                            toSlot = (int)StoreBag.Player.TempProperties[key];
                            StoreBag.Player.TempProperties.Remove(key);
                        }
                        else
                        {
                            toSlot = bag.FindFirstEmptySlot();
                        }
                    }
                    if (bag.AddItemTo(item, toSlot) && bag.StackItemToAnother(item))
                    {
                        StoreBag.TakeOutItem(item);
                    }
                    else
                    {
                        StoreBag.SaveToDatabase();
                        player.SendItemToMail(item, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
                        player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
            }*/
        }
    }
}
