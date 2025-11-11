using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(87, "客户端日记")]
    public class ChickenBoxHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int cmd = packet.ReadInt();
            GSPacketIn pkg = new GSPacketIn(87);
            ActiveSystemInfo ChickenBox = client.Player.Actives.Info;
            switch (cmd)
            {
                case 13:
                    {
                        int position = packet.ReadInt();
                        if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                            return 1;
                        }

                        int OpenCounts = ChickenBox.canOpenCounts;
                        if (OpenCounts > 0)
                        {
                            NewChickenBoxItemInfo item = client.Player.Actives.GetAward(position);
                            if (item != null)
                            {
                                item.IsBinds = true;
                                item.IsSelected = true;
                                if (OpenCounts > client.Player.Actives.openCardPrice.Length)
                                    OpenCounts = client.Player.Actives.openCardPrice.Length;

                                int needMoney = client.Player.Actives.openCardPrice[OpenCounts - 1];
                                if (client.Player.MoneyDirect(needMoney, IsAntiMult: false))
                                {
                                    pkg.WriteInt((int)NewChickenBoxPackageType.TACKOVERCARD);
                                    pkg.WriteInt(item.TemplateID);
                                    pkg.WriteInt(item.StrengthenLevel);
                                    pkg.WriteInt(item.Count);
                                    pkg.WriteInt(item.ValidDate);
                                    pkg.WriteInt(item.AttackCompose);
                                    pkg.WriteInt(item.DefendCompose);
                                    pkg.WriteInt(item.AgilityCompose);
                                    pkg.WriteInt(item.LuckCompose);
                                    pkg.WriteInt(item.Position);
                                    pkg.WriteBoolean(item.IsSelected);
                                    pkg.WriteBoolean(item.IsSeeded);
                                    pkg.WriteBoolean(item.IsBinds);
                                    pkg.WriteInt(client.Player.Actives.freeOpenCardCount);
                                    client.Out.SendTCP(pkg);
                                    client.Player.Actives.UpdateChickenBoxAward(item);
                                    ItemInfo itemAward = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), 1, 105);
                                    itemAward.IsBinds = item.IsBinds;
                                    itemAward.ValidDate = item.ValidDate;
                                    client.Player.AddTemplate(itemAward, "Rương gà");
                                    client.Player.SendMessage(string.Format("Bạn nhận được {0}x{1}", itemAward.Template.Name, item.Count));
                                    ChickenBox.canOpenCounts--;
                                    if (ChickenBox.canOpenCounts == 0)
                                    {
                                        GSPacketIn spkg = new GSPacketIn((byte)ePackageType.NEWCHICKENBOX_SYS);
                                        spkg.WriteInt((byte)NewChickenBoxPackageType.OVERSHOWITEMS);
                                        client.Player.SendTCP(spkg);
                                    }
                                }
                            }
                            else
                            {
                                client.Player.SendMessage("Dữ liệu server lỗi.");
                            }
                        }
                        else
                        {
                            client.Player.SendMessage("Số lần lật thẻ vòng này đã hết.");
                        }

                        break;
                    }
                case 11:
                    {
                        int position = packet.ReadInt();
                        int EagleEyeCounts = ChickenBox.canEagleEyeCounts;
                        if (EagleEyeCounts > 0)
                        {
                            NewChickenBoxItemInfo item = client.Player.Actives.ViewAward(position);
                            if (item != null)
                            {
                                if (EagleEyeCounts > client.Player.Actives.eagleEyePrice.Length)
                                    EagleEyeCounts = client.Player.Actives.eagleEyePrice.Length;

                                int needMoney = client.Player.Actives.eagleEyePrice[EagleEyeCounts - 1];
                                if (client.Player.MoneyDirect(needMoney, IsAntiMult: false))
                                {
                                    item.IsSeeded = true;
                                    pkg.WriteInt((int)NewChickenBoxPackageType.EAGLEEYE);
                                    pkg.WriteInt(item.TemplateID);
                                    pkg.WriteInt(item.StrengthenLevel);
                                    pkg.WriteInt(item.Count);
                                    pkg.WriteInt(item.ValidDate);
                                    pkg.WriteInt(item.AttackCompose);
                                    pkg.WriteInt(item.DefendCompose);
                                    pkg.WriteInt(item.AgilityCompose);
                                    pkg.WriteInt(item.LuckCompose);
                                    pkg.WriteInt(item.Position);
                                    pkg.WriteBoolean(item.IsSelected);
                                    pkg.WriteBoolean(item.IsSeeded);
                                    pkg.WriteBoolean(item.IsBinds);
                                    pkg.WriteInt(client.Player.Actives.freeEyeCount);
                                    client.Player.SendTCP(pkg);
                                    client.Player.Actives.UpdateChickenBoxAward(item);
                                    ChickenBox.canEagleEyeCounts--;
                                }
                            }
                            else
                            {
                                client.Player.SendMessage("Dữ liệu server lỗi.");
                            }
                        }
                        else
                        {
                            client.Player.SendMessage("Số lần xuyên thấu vòng này đã hết.");
                        }

                        break;
                    }
                case 14:
                    {
                        int needMoney = client.Player.Actives.flushPrice;
                        if (client.Player.Actives.IsFreeFlushTime())
                        {
                            if (client.Player.MoneyDirect(needMoney, IsAntiMult: false))
                            {
                                client.Player.Actives.PayFlushView();
                                client.Player.Actives.SendChickenBoxItemList();
                                client.Player.SendMessage("Tiêu hao Xu, tạo mới thành công.");
                            }
                        }
                        else
                        {
                            client.Player.Actives.PayFlushView();
                            client.Player.Actives.SendChickenBoxItemList();
                            client.Player.SendMessage("Tạo mới miễn phí thành công.");
                        }

                        break;
                    }
                case 12:
                    {
                        client.Player.Actives.SendChickenBoxItemList();
                        client.Player.Actives.PayFlushView();
                        break;
                    }
                case 15:
                    {
                        ChickenBox.isShowAll = false;
                        client.Player.Actives.RandomPosition();
                        pkg.WriteInt((int)NewChickenBoxPackageType.CANCLICKCARD);
                        pkg.WriteBoolean(true);
                        client.Player.SendTCP(pkg);
                        break;
                    }
                case 10:
                    {
                        client.Player.Actives.EnterChickenBox();
                        client.Player.Actives.SendChickenBoxItemList();
                        break;
                    }
                default:
                    Console.WriteLine("NewChickenBoxPackageType." + (NewChickenBoxPackageType)cmd);
                    break;
            }
            return 0;
        }
    }
}