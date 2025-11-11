using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(138, "物品强化")]
	public class ItemAdvanceHandler : IPacketHandler
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			//client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
			//return 0;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			packet.ReadBoolean();
			packet.ReadBoolean();
			GSPacketIn gSPacketIn = new GSPacketIn(138, client.Player.PlayerCharacter.ID);
			ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
			ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(1);
			int strengthenLevel = itemInfo.StrengthenLevel;
			int num;
			int result;
			if (itemAt == null || itemInfo == null || itemAt.Count <= 0)
			{
				client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("Đặt đá tăng cấp và trang bị cần tăng cấp vào!", new object[0]));
				num = 0;
			}
			else if (strengthenLevel >= 15)
			{
				client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("Level đã đạt cấp độ cao nhất, không thể thăng cấp!", new object[0]));
				num = 0;
			}
			else
			{
				int count = 1;
				string text = "";
				if (itemInfo != null && itemInfo.Template.CanStrengthen && itemInfo.Template.CategoryID < 18 && itemInfo.Count == 1)
				{
					flag = (flag || itemInfo.IsBinds);
					stringBuilder.Append(string.Concat(new object[]
					{
						itemInfo.ItemID,
						":",
						itemInfo.TemplateID,
						","
					}));
					if (itemAt.TemplateID < 11150 && itemAt.TemplateID > 11154)
					{
						client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("Đặt đá tăng cấp vào!", new object[0]));
						num = 0;
						result = num;
						return result;
					}
					flag = (flag || itemAt.IsBinds);
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						",",
						itemAt.ItemID.ToString(),
						":",
						itemAt.Template.Name
					});
					int num2 = (itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2;
					stringBuilder.Append("true");
					bool flag2 = false;
					int num3 = ItemAdvanceHandler.random.Next(50000);
					//double num4 = (double)(itemInfo.StrengthenExp / strengthenLevel);
					bool ActiveUpgrade = false;
					if (itemInfo.StrengthenExp >= random.Next(4000,7500) && itemInfo.StrengthenLevel == 12)
						ActiveUpgrade = true;
					if (itemInfo.StrengthenExp >= random.Next(12500,17500) && itemInfo.StrengthenLevel == 13)
						ActiveUpgrade = true;
					if (itemInfo.StrengthenExp >= random.Next(25000,35000) && itemInfo.StrengthenLevel == 14)
						ActiveUpgrade = true;
					if (ActiveUpgrade)
					{
						itemInfo.IsBinds = flag;
						itemInfo.StrengthenLevel++;
						itemInfo.StrengthenExp = 0;
						gSPacketIn.WriteByte(0);
						gSPacketIn.WriteInt(num2);
						flag2 = true;
						StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(itemInfo.StrengthenLevel, itemInfo.TemplateID);
						if (strengthenGoodsInfo != null && itemInfo.Template.CategoryID == 7 && strengthenGoodsInfo.GainEquip > itemInfo.TemplateID)
						{
							ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(strengthenGoodsInfo.GainEquip);
							if (itemTemplateInfo != null)
							{
								ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
								client.Player.StoreBag.RemoveItemAt(1);
								client.Player.StoreBag.AddItemTo(itemInfo2, 1);
								itemInfo = itemInfo2;
							}
						}
					}
					else
					{
						itemInfo.StrengthenExp += num2;
						gSPacketIn.WriteByte(1);
						gSPacketIn.WriteInt(num2);
					}
					client.Player.StoreBag.RemoveCountFromStack(itemAt, count);
					client.Player.StoreBag.UpdateItem(itemInfo);
					client.Out.SendTCP(gSPacketIn);
					if (flag2 && itemInfo.ItemID > 0)
					{
						string msg = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation2", client.Player.PlayerCharacter.NickName, itemInfo.TemplateID, itemInfo.StrengthenLevel - 12);
						GSPacketIn sysNotice = WorldMgr.SendSysNotice(eMessageType.SYS_TIP_NOTICE, msg, itemInfo.ItemID, itemInfo.TemplateID, null);
						GameServer.Instance.LoginServer.SendPacket(sysNotice);
					}
					stringBuilder.Append(itemInfo.StrengthenLevel);
				}
				else
				{
					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1", new object[0]) + itemAt.Template.Name + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2", new object[0]));
				}
				if (itemInfo.Place < 31)
				{
					client.Player.EquipBag.UpdatePlayerProperties();
				}
				num = 0;
			}
			result = num;
			return result;
		}
	}
}
