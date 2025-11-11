using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(121, "物品镶嵌")]
	public class ItemInlayHandle : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = packet.Clone();
			gSPacketIn.ClearContext();
			int bagType = packet.ReadInt();
			int place = packet.ReadInt();
			int num = packet.ReadInt();
			int bagType2 = packet.ReadInt();
			int place2 = packet.ReadInt();
			ItemInfo itemAt = client.Player.GetItemAt((eBageType)bagType, place);
			ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)bagType2, place2);
			string text = "";
			int inlayGoldPrice = GameProperties.InlayGoldPrice;
			if (itemAt != null && itemAt2 != null && itemAt2.Template.Property1 == 31)
			{
				if (client.Player.PlayerCharacter.Gold > inlayGoldPrice)
				{
					string[] array = itemAt.Template.Hole.Split('|');
					if (num > 0 && num < 7)
					{
						client.Player.RemoveGold(inlayGoldPrice);
						bool flag = false;
						switch (num)
						{
							case 1:
								if (Convert.ToInt32(array[0].Split(',')[1]) == itemAt2.Template.Property2)
								{
									itemAt.Hole1 = itemAt2.TemplateID;
									text = text + "," + itemAt2.ItemID + "," + itemAt2.Template.Name;
									flag = true;
								}
								break;
							case 2:
								if (Convert.ToInt32(array[1].Split(',')[1]) == itemAt2.Template.Property2)
								{
									itemAt.Hole2 = itemAt2.TemplateID;
									text = text + "," + itemAt2.ItemID + "," + itemAt2.Template.Name;
									flag = true;
								}
								break;
							case 3:
								if (Convert.ToInt32(array[2].Split(',')[1]) == itemAt2.Template.Property2)
								{
									itemAt.Hole3 = itemAt2.TemplateID;
									text = text + "," + itemAt2.ItemID + "," + itemAt2.Template.Name;
									flag = true;
								}
								break;
							case 4:
								if (Convert.ToInt32(array[3].Split(',')[1]) == itemAt2.Template.Property2)
								{
									itemAt.Hole4 = itemAt2.TemplateID;
									text = text + "," + itemAt2.ItemID + "," + itemAt2.Template.Name;
									flag = true;
								}
								break;
							case 5:
								if (Convert.ToInt32(array[4].Split(',')[1]) != itemAt2.Template.Property2)
								{
									break;
								}
								if (itemAt.Hole5 != 0)
								{
									ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(itemAt.Hole5), 1, 102);
									itemInfo2.IsBinds = true;
									itemInfo2.ValidDate = 0;
									if (!client.Player.AddItem(itemInfo2))
									{
										client.Player.SendItemsToMail(new List<ItemInfo>
								{
									itemInfo2
								}, "Tháo châu báu túi đầy.", "Tháo châu báu túi đầy.", eMailType.BuyItem);
									}
								}
								itemAt.Hole5 = itemAt2.TemplateID;
								text = text + "," + itemAt2.ItemID + "," + itemAt2.Template.Name;
								flag = true;
								break;
							case 6:
								if (Convert.ToInt32(array[5].Split(',')[1]) != itemAt2.Template.Property2)
								{
									break;
								}
								if (itemAt.Hole6 != 0)
								{
									ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(itemAt.Hole6), 1, 102);
									itemInfo.IsBinds = true;
									itemInfo.ValidDate = 0;
									if (!client.Player.AddItem(itemInfo))
									{
										client.Player.SendItemsToMail(new List<ItemInfo>
								{
									itemInfo
								}, "Tháo châu báu túi đầy.", "Tháo châu báu túi đầy.", eMailType.BuyItem);
									}
								}
								itemAt.Hole6 = itemAt2.TemplateID;
								text = text + "," + itemAt2.ItemID + "," + itemAt2.Template.Name;
								flag = true;
								break;
						}
						if (flag)
						{
							gSPacketIn.WriteInt(0);
							itemAt2.Count--;
							itemAt.IsBinds = true;
							client.Player.UpdateItem(itemAt2);
							client.Player.UpdateItem(itemAt);
							GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemInlay.Success", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, itemAt2.Template.Name, itemAt.TemplateID), itemAt.ItemID, itemAt.TemplateID, null));
						}
						else
						{
							client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.InlayItem.Msg1"));
						}
					}
					else
					{
						gSPacketIn.WriteByte(1);
						client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemInlayHandle.NoPlace"));
					}
					client.Player.SendTCP(gSPacketIn);
					client.Player.SaveIntoDatabase();
				}
				else
				{
					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
				}
				return 0;
			}
			return 0;
		}
	}
}