using System;
using System.Configuration;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.WISHBEADEQUIP, "场景用户离开")]
    public class WishBeadEquipHandler : IPacketHandler
    {
        public static Random random = new Random();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
            //return 0;
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["WishBeadOpenHandle"]))
            {
                return 0;
            }
            else
            {
                if (client.Player.RunningGold)
                {
                    client.Player.RunningGold = false;
                    return 0;
                }
                int Place = packet.ReadInt(); //param1.itemInfo.Place          
                int BagType = packet.ReadInt(); //param1.itemInfo.BagType
                int templateID = packet.ReadInt(); //param1.info.TemplateID

                int PlaceBead = packet.ReadInt(); //_loc_3.Place           
                int BagTypeBead = packet.ReadInt(); //_loc_3.BagType
                int BeadId = packet.ReadInt(); //_loc_3.TemplateID
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.WISHBEADEQUIP, client.Player.PlayerCharacter.ID);
                PlayerInventory itemBag = client.Player.GetInventory((eBageType)BagType);
                PlayerInventory beadBag = client.Player.GetInventory((eBageType)BagTypeBead);
                ItemInfo item = itemBag.GetItemAt(Place);
                ItemInfo bead = beadBag.GetItemAt(PlaceBead);

                if (bead == null || item == null)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("WishBeadEquipHandler.Msg1"));
                    pkg.WriteInt(5);
                    client.Out.SendTCP(pkg);
                    return 0;
                }

                if (bead.Count < 1 || bead.TemplateID != BeadId)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("WishBeadEquipHandler.Msg2"));
                    pkg.WriteInt(5);
                    client.Out.SendTCP(pkg);
                    return 0;
                }

                if (!CanWishBeat(bead.TemplateID, item.Template.CategoryID))
                {
                    pkg.WriteInt(5);
                    client.Out.SendTCP(pkg);
                    return 0;
                }

                var probability = Convert.ToInt32(ConfigurationManager.AppSettings["WishBeadRate"]);
                GoldEquipTemplateInfo goldEquip = GoldEquipMgr.FindGoldEquipByTemplate(templateID);
                item.IsBinds = true;
                if (goldEquip == null && item.Template.CategoryID == 7)
                {
                    pkg.WriteInt(5);
                }
                else if (item.StrengthenLevel > GameProperties.WishBeadLimitLv && GameProperties.IsWishBeadLimit)
                {
                    pkg.WriteInt(5);
                }
                else if (!item.IsValidGoldItem())
                {
                    var rate = random.Next(100000);
                    if (probability >= rate)
                    {
                        item.goldBeginTime = DateTime.Now;
                        item.goldValidDate = 30;
                        if (item.Template.CategoryID == 7)
                        {
                            ItemTemplateInfo template = ItemMgr.FindItemTemplate(goldEquip.NewTemplateId);
                            if (template != null)
                                item.GoldEquip = template;
                        }

                        GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemWishBeadEquipHandler.Success", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, item.TemplateID), item.ItemID, item.TemplateID, null));

                        client.Player.UpdateItem(item);
                        pkg.WriteInt(0);
                    }
                    else
                    {
                        pkg.WriteInt(1);
                    }

                    beadBag.RemoveCountFromStack(bead, 1);
                }
                else
                {
                    pkg.WriteInt(6);
                }

                client.Out.SendTCP(pkg);
                return 0;
            }
        }

        private bool CanWishBeat(int beatID, int CategoryID)
        {
            if (beatID == 11560 && CategoryID == 7)
            {
                return true;
            }
            else if (beatID == 11561 && CategoryID == 5)
            {
                return true;
            }
            else if (beatID == 11562 && CategoryID == 1)
            {
                return true;
            }

            return false;
        }
    }
}
