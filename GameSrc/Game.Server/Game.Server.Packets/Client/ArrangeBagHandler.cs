using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Packets;
using Game.Server;
using Game.Server.GameUtils;
using Game.Server.Packets.Client;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CHANGE_PLACE_GOODS_ALL, "背包整理")]
    public class ArrangeBagHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool isSegistration = packet.ReadBoolean();
            int count = packet.ReadInt();
            PlayerInventory bag = client.Player.GetInventory((eBageType)packet.ReadInt());
            Dictionary<int, int> switches = new Dictionary<int, int>();
            ItemInfo itemEquipBag = client.Player.EquipBag.GetItemAt(126);
            ItemInfo itemPropBag = client.Player.PropBag.GetItemAt(95);
            if(itemEquipBag != null)// || itemPropBag != null)
            {
                client.Out.SendAcademySystemNotice($"Trang Bị | Vui lòng di chuyển {itemEquipBag.Template.Name} sang ô khác để sắp xếp túi đồ.", true);
                return 0;
            }
            else if(itemPropBag != null)
            {
                client.Out.SendAcademySystemNotice($"Đạo Cụ | Vui lòng di chuyển {itemPropBag.Template.Name} sang ô khác để sắp xếp túi đồ.", true);
                return 0;
            }

            for (int i = 0; i < count; i++)
            {
                int old_place = packet.ReadInt();
                int new_place = packet.ReadInt();
                if (!switches.ContainsKey(old_place))
                {
                    switches.Add(old_place, new_place);
                }
                else
                {
                    Console.Write(("client:{0} error client data,index already exist in the dics.", client.Player.PlayerId));
                }
            }
            int result2;
            if (switches.Count != bag.GetItems(bag.BeginSlot, bag.Capalility - 1).Count)
            {
                result2 = 0;
            }
            else
            {
                Dictionary<int, ItemInfo> rawitems = bag.GetRawSpaces();
                bag.BeginChanges();
                bool result = false;
                try
                {
                    bag.Clear(bag.BeginSlot, bag.Capalility - 1);
                    foreach (KeyValuePair<int, int> sp in switches)
                    {
                        if (sp.Key < bag.BeginSlot || sp.Value < bag.BeginSlot)
                        {
                            throw new Exception(string.Format("can't operate that place: old {0}  new  {1}", sp.Key, sp.Value));
                        }
                        ItemInfo it = rawitems[sp.Key];
                        if (!bag.AddItemTo(it, sp.Value))
                        {
                            throw new Exception(string.Format("move item error: old place:{0} new place:{1}", sp.Key, sp.Value));
                        }
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.Write(("Arrange bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, ex.Message));
                }
                finally
                {
                    if (!result)
                    {
                        bag.Clear(0, bag.Capalility - 1);
                        foreach (KeyValuePair<int, ItemInfo> sp2 in rawitems)
                        {
                            bag.AddItemTo(sp2.Value, sp2.Key);
                        }
                    }
                    if (isSegistration)
                    {
                        try
                        {
                            List<ItemInfo> items = bag.GetItems();
                            List<int> list = new List<int>();
                            for (int i = 0; i < items.Count; i++)
                            {
                                if (!list.Contains(i))
                                {
                                    for (int j = items.Count - 1; j > i; j--)
                                    {
                                        if (!list.Contains(j) && items[i].TemplateID == items[j].TemplateID && items[i].CanStackedTo(items[j]))
                                        {
                                            bag.MoveItem(items[j].Place, items[i].Place, items[j].Count);
                                            list.Add(j);
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            List<ItemInfo> items = bag.GetItems();
                            if (bag.FindFirstEmptySlot() != -1)
                            {
                                int i = 1;
                                while (bag.FindFirstEmptySlot() < items[items.Count - i].Place)
                                {
                                    bag.MoveItem(items[items.Count - i].Place, bag.FindFirstEmptySlot(), items[items.Count - i].Count);
                                    i++;
                                }
                            }
                        }
                    }
                    bag.CommitChanges();
                }
                result2 = 0;
            }
            return result2;
        }
    }
}
