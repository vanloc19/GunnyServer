//using Game.Base.Packets;
//using Game.Server.GameObjects;
//using Game.Server.GameUtils;
//using Game.Server.Managers;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//	[PacketHandler(127, "物品比较")]
//	public class ItemReclaimHandler : IPacketHandler
//	{
//		public int HandlePacket(GameClient client, GSPacketIn packet)
//		{
//			eBageType bageType = (eBageType)packet.ReadByte();
//			int slot = packet.ReadInt();
//			int count = packet.ReadInt();
//			PlayerInventory inventory = client.Player.GetInventory(bageType);
//			if (inventory != null && inventory.GetItemAt(slot) != null)
//			{
//				if (inventory.GetItemAt(slot).Count <= count)
//				{
//					count = inventory.GetItemAt(slot).Count;
//				}
//				ItemTemplateInfo template = inventory.GetItemAt(slot).Template;
//				int num3 = count * template.ReclaimValue;
//				if (template.ReclaimType == 3 && template.is)
//                {
//					client.Player.AddMoney(num3);
//					client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Bạn nhận được {0} xu.", num3));
//					GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
//					for (int i = 0; i < allPlayers.Length; i++)
//					{
//						allPlayers[i].Out.SendMessage(eMessageType.ChatNormal, string.Format("Chúc mừng người chơi [{0}] vừa bán vật phẩm {1} nhận được {2} xu.", client.Player.PlayerCharacter.NickName, template.Name, num3));
//					}
//				}
//				if (template.ReclaimType == 2)
//				{
//					client.Player.AddGiftToken(num3);
//					client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Bạn nhận được {0} lễ kim.", num3));
//				}
//				if (template.ReclaimType == 1)
//				{
//					client.Player.AddGold(num3);
//					client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Bạn nhận được {0} vàng.", num3));
//				}
//				if (template.TemplateID == 11408)
//				{
//					client.Player.RemoveMedal(count);
//				}
//				inventory.RemoveItemAt(slot);
//				return 0;
//			}
//			client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Bán vật phẩm không thành công."));
//			return 1;
//		}
//	}
//}
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;                                                                                                                                              
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.REClAIM_GOODS, "物品回收")]
    public class ItemReclaimHandler : IPacketHandler
    {
        //修改:  XiaoJian
        //时间:  2020-11-7
        //描述:  物品回收<测试完成>   
        //状态： 正在使用

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bagType = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            int count = packet.ReadInt();
            PlayerInventory bag = client.Player.GetInventory(bagType);
            ItemInfo item = bag.GetItemAt(place);
            if (item != null)
            {
                if (bag.GetItemAt(place).Count <= count)
                {
                    count = bag.GetItemAt(place).Count;
                }

                int price = count * item.Template.ReclaimValue;
                if (item.Template.ReclaimType == 2)
                {
                    client.Player.AddGiftToken(price);
                    client.Out.SendMessage(eMessageType.Normal, string.Format("Bán thành công nhận được {0} lễ kim.", price));
                }
                if (item.Template.ReclaimType == 1)
                {
                    client.Player.AddGold(price);
                    client.Out.SendMessage(eMessageType.Normal, string.Format("Bán thành công nhận được {0} vàng.", price));
                }
                if (item.Template.ReclaimType == 3)
                {
                    if (item.IsBinds)
                    {
                        client.Player.AddGold(price);
                        client.Player.SendMessage(eMessageType.Normal, string.Format($"Bán thành công vật phẩm khoá bạn nhận được {price} vàng."));
                    }
                    else
                    {
                        client.Player.AddMoney(price);
                        client.Player.SendMessage(eMessageType.Normal, string.Format($"Bán thành công vật phẩm không khoá bạn nhận được {price} xu."));
                        // GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                        // for (int i = 0; i < allPlayers.Length; i++)
                        // {
                        //     allPlayers[i].Out.SendMessage(eMessageType.ChatNormal, string.Format("《Bán Vật Phẩm》-  Người chơi [{0}] vừa bán vật phẩm {1} x{3} không khoá nhận được {2} xu.", client.Player.PlayerCharacter.NickName, item.Template.Name, price, item.Count));
                        // }
                    }
                }

                bag.RemoveItemAt(place);
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.NoSuccess"));
                return 1;
            }
            return 0;
        }
    }
}
