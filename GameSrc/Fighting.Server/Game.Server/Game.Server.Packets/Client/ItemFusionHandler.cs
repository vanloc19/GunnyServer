#region OLD
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Bussiness;
//using Game.Base.Packets;
//using Game.Server.GameObjects;
//using Game.Server.GameUtils;
//using Game.Server.Managers;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//	[PacketHandler(78, "熔化")]
//	public class ItemFusionHandler : IPacketHandler
//	{
//		public int HandlePacket(GameClient client, GSPacketIn packet)
//		{
//			new StringBuilder();
//			int num1 = packet.ReadByte();
//			int MinValid = int.MaxValue;
//			List<ItemInfo> Items = new List<ItemInfo>();
//			List<ItemInfo> AppendItems = new List<ItemInfo>();
//			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
//			{
//				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
//				return 1;
//			}
//			if (MinValid == int.MaxValue)
//			{
//				MinValid = 0;
//				Items.Clear();
//			}
//			PlayerInventory storeBag = client.Player.StoreBag;
//			for (int slot = 1; slot <= 4; slot++)
//			{
//				ItemInfo itemAt = storeBag.GetItemAt(slot);
//				if (itemAt != null)
//				{
//					Items.Add(itemAt);
//				}
//			}
//			if (Items.Count != 4)
//			{
//				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemFusionHandler.ItemNotEnough"));
//				return 0;
//			}
//			bool isBind = false;
//			Dictionary<int, double> previewItemList = FusionMgr.FusionPreview(Items, AppendItems, ref isBind);
//			if (num1 == 0)
//			{
//				if (previewItemList != null)
//				{
//					if (previewItemList.Count != 0)
//					{
//						client.Out.SendFusionPreview(client.Player, previewItemList, isBind, MinValid);
//					}
//					else
//					{
//						client.Player.SendMessage(LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
//					}
//				}
//				else
//				{
//					Console.WriteLine("previewItemList is NULL");
//				}
//			}
//			else
//			{
//				int num2 = 1600;
//				if (client.Player.PlayerCharacter.Gold < num2)
//				{
//					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney"));
//					return 0;
//				}
//				if (previewItemList == null || previewItemList.Count <= 0)
//				{
//					client.Player.SendMessage(LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
//					return 0;
//				}
//				isBind = false;
//				bool result = false;
//				ItemTemplateInfo goods = FusionMgr.Fusion(Items, AppendItems, ref isBind, ref result);
//				if (goods != null)
//				{
//					ItemInfo itemAt2 = storeBag.GetItemAt(0);
//					if (itemAt2 != null)
//					{
//						if (!client.Player.StackItemToAnother(itemAt2) && !client.Player.AddItem(itemAt2))
//						{
//							GamePlayer player2 = client.Player;
//							List<ItemInfo> items2 = new List<ItemInfo>
//							{
//								itemAt2
//							};
//							string content = "Túi đồ đầy";
//							string title = "Hành trang đã đầy vật phẩm gửi ra thư.";
//							int num4 = 13;
//							player2.SendItemsToMail(items2, content, title, (eMailType)num4);
//							client.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("ItemFusionHandler.full"));
//						}
//						storeBag.TakeOutItemAt(0);
//					}
//					client.Player.RemoveGold(num2);
//					for (int index2 = 0; index2 < Items.Count; index2++)
//					{
//						Items[index2].Count--;
//						client.Player.UpdateItem(Items[index2]);
//					}
//					for (int index = 0; index < AppendItems.Count; index++)
//					{
//						AppendItems[index].Count--;
//						client.Player.UpdateItem(AppendItems[index]);
//					}
//					if (result)
//					{
//						if (goods.BagType == eBageType.EquipBag)
//						{
//							MinValid = 7;
//						}
//						ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(goods, 1, 105);
//						if (fromTemplate == null)
//						{
//							return 0;
//						}
//						fromTemplate.IsBinds = isBind;
//						fromTemplate.ValidDate = MinValid;
//						client.Player.OnItemFusion(fromTemplate.Template.FusionType);
//						client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1", fromTemplate.Template.Name));
//						if (fromTemplate.Template.CategoryID == 7 || fromTemplate.Template.CategoryID == 17 || fromTemplate.Template.CategoryID == 19 || fromTemplate.Template.CategoryID == 16)
//						{
//							client.Player.SaveNewItems();
//							GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemFusionHandler.Notice", client.Player.PlayerCharacter.NickName, fromTemplate.TemplateID), fromTemplate.ItemID, fromTemplate.TemplateID, null));
//							client.Player.AddLog("Fusion", "TemplateID: " + fromTemplate.TemplateID + "|Name: " + fromTemplate.Template.Name);
//						}
//						if (!client.Player.StoreBag.AddItemTo(fromTemplate, 0))
//						{
//							client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(fromTemplate.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
//							client.Player.AddLog("Error", "ItemFusionError" + fromTemplate.Template.Name + "|TemplateID:" + fromTemplate.TemplateID);
//							GamePlayer player = client.Player;
//							List<ItemInfo> items = new List<ItemInfo>();
//							items.Add(fromTemplate);
//							string translation1 = LanguageMgr.GetTranslation("ItemFusionHandler.full");
//							string translation2 = LanguageMgr.GetTranslation("ItemFusionHandler.full");
//							int num3 = 8;
//							player.SendItemsToMail(items, translation1, translation2, (eMailType)num3);
//						}
//						client.Out.SendFusionResult(client.Player, result);
//					}
//					else
//					{
//						client.Out.SendFusionResult(client.Player, result);
//						client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemFusionHandler.Failed"));
//					}
//					client.Player.SaveIntoDatabase();
//				}
//				else
//				{
//					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
//				}
//			}
//			return 0;
//		}
//	}
//}
#endregion
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_FUSION, "物品熔炼")]
    public class ItemFusionHandler : IPacketHandler
    {
        /// <summary>
        /// 熔炼步骤
        /// 第一步：检查四个熔炼类型与熔炼物品&是否有二级密码
        /// 第二步：检验数据是否合乎熔炼规则
        /// 第三步：生成预览或生成物品
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        //修改:  XiaoJian
        //时间:  2020-11-7
        //描述:  物品熔炼<测试完成>   
        //状态： 正在使用
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            StringBuilder str = new StringBuilder();
            //第一步：传入操作类型、与四个石头
            int opertionType = packet.ReadByte();
            int MinValid = int.MaxValue; //默认最短有效时间
            List<ItemInfo> items = new List<ItemInfo>();
            List<ItemInfo> appendItems = new List<ItemInfo>();
            PlayerInventory storeBag = client.Player.StoreBag;
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 1;
            }
            if (MinValid == int.MaxValue)
            {
                MinValid = 0;
                items.Clear();
            }
            for (int i = 0; i < 5; i++)
            {
                ItemInfo itemAt = storeBag.GetItemAt(i);
                if (itemAt != null)
                {
                    items.Add(itemAt);
                }
            }
            if (items.Count != 4)
            {
                return 0;
            }

            //结束：预览或熔炼
            if (0 == opertionType) //预览模式
            {
                bool isBind = false;
                Dictionary<int, double> previewItemList = FusionMgr.FusionPreview(items, appendItems, ref isBind);

                if (previewItemList != null)
                {
                    if (previewItemList.Count != 0)
                    {
                        client.Out.SendFusionPreview(client.Player, previewItemList, isBind, MinValid);
                    }
                }
            }
            else //生成熔炼物品
            {
                int mustGold = 400;
                if (client.Player.PlayerCharacter.Gold < mustGold)
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney"));
                    return 0;
                }

                bool isBind = false;
                bool result = false;
                bool isTips = false;
                ItemTemplateInfo rewardItem = FusionMgr.Fusion(items, appendItems, ref isBind, ref result, ref isTips);
                if (rewardItem != null)
                {
                    ItemInfo info = storeBag.GetItemAt(0);
                    if (info != null)
                    {
                        if (!client.Player.StackItemToAnother(info) && !client.Player.AddItem(info))
                        {
                            client.Player.SendItemsToMail(new List<ItemInfo>
                            {
                                info
                            }, "Hành trang của bạn đã đầy.Vật phẩm được trả về thư!", "Hành trang đã đầy.", eMailType.StoreCanel);
                        }
                        storeBag.TakeOutItemAt(0);
                    }
                    client.Player.RemoveGold(mustGold);
                    for (int i = 0; i < items.Count; i++)
                    {
                        items[i].Count--;
                        client.Player.UpdateItem(items[i]);

                    }
                    for (int i = 0; i < appendItems.Count; i++)
                    {
                        appendItems[i].Count--;
                        client.Player.UpdateItem(appendItems[i]);

                    }
                    if (result)
                    {

                        if (rewardItem.BagType == eBageType.EquipBag)
                        {
                            MinValid = 7;
                        }
                        ItemInfo item = ItemInfo.CreateFromTemplate(rewardItem, 1, (int)ItemAddType.Fusion);
                        if (item == null)
                        {
                            return 0;
                        }

                        item.IsBinds = isBind;
                        item.ValidDate = MinValid;
                        client.Player.OnItemFusion(item.Template.FusionType);  //熔炼成功
                        client.Out.SendFusionResult(client.Player, result);
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1") + item.Template.Name);
                        client.Player.SaveNewItems();

                        if (isTips)
                        {
                            client.Player.SaveNewItems();
                            GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemFusionHandler.Notice", client.Player.PlayerCharacter.NickName, item.TemplateID), item.ItemID, item.TemplateID, null));
                            client.Player.AddLog("Fusion", "TemplateID: " + item.TemplateID + "|Name: " + item.Template.Name);
                        }
                        if (!client.Player.StoreBag.AddItemTo(item, 0))
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(item.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
                            client.Player.SendItemsToMail(new List<ItemInfo>
                            {
                                item
                            }, LanguageMgr.GetTranslation("GameServer.Fustion.msg2", item.Template.Name), LanguageMgr.GetTranslation("GameServer.Fustion.Msg3"), eMailType.BuyItem);
                        }
                    }
                    else
                    {
                        client.Out.SendFusionResult(client.Player, result);
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed"));
                    }
                    client.Player.SaveIntoDatabase();//保存到数据库
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
                }
            }
            return 0;
        }
    }
}
