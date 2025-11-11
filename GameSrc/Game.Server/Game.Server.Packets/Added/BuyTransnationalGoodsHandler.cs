using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
	[PacketHandler(156, "客户端日记")]
	public class BuyTransnationalGoodsHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int iD = packet.ReadInt();
			PyramidInfo pyramid = client.Player.Actives.Pyramid;
			int NewType = 1;
			int damageScore = 0;
			int petScore = 0;
			int iTemplateID = 0;
			int iCount = 0;
			int gold = 0;
			int money = 0;
			int offer = 0;
			int gifttoken = 0;
			int medal = 0;
			int hardCurrency = 0;
			int leagueMoney = 0;
			eMessageType type = eMessageType.GM_NOTICE;
			string translateId = "UserBuyItemHandler.Success";
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			bool flag = false;
			if (shopItemInfoById != null && ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 98)
			{
				flag = true;
			}
			int result;
			if (!flag)
			{
				client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
				result = 1;
			}
			else
			{
				Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 102);
				if (shopItemInfoById.BuyType == 0)
				{
					itemInfo.ValidDate = shopItemInfoById.AUnit;
				}
				else
				{
					itemInfo.Count = shopItemInfoById.AUnit;
				}
				itemInfo.IsBinds = true;
				if (!dictionary.ContainsKey(itemInfo.TemplateID))
				{
					dictionary.Add(itemInfo.TemplateID, itemInfo);
				}
				else
				{
					ItemInfo itemInfo2 = dictionary[itemInfo.TemplateID];
					itemInfo2.Count += itemInfo.Count;
				}
				ShopMgr.SetItemType(ShopMgr.GetShopItemInfoById(iD), NewType, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref leagueMoney, ref medal);
				if (dictionary.Values.Count == 0)
				{
					result = 1;
				}
				else if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
				{
					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
					result = 1;
				}
				else if (pyramid.totalPoint < damageScore)
				{
					client.Player.SendMessage("Tích lũy không đủ.");
					result = 0;
				}
				else
				{
					pyramid.totalPoint -= damageScore;
					string text = "";
					foreach (ItemInfo current in dictionary.Values)
					{
						text += ((text == "") ? current.TemplateID.ToString() : ("," + current.TemplateID.ToString()));
						if (itemInfo.Template.MaxCount == 1)
						{
							for (int i = 0; i < itemInfo.Count; i++)
							{
								ItemInfo itemInfo3 = ItemInfo.CloneFromTemplate(itemInfo.Template, itemInfo);
								itemInfo3.Count = 1;
								client.Player.AddTemplate(itemInfo3);
							}
						}
						else
						{
							int num15 = 0;
							for (int j = 0; j < itemInfo.Count; j++)
							{
								if (num15 == itemInfo.Template.MaxCount)
								{
									ItemInfo itemInfo4 = ItemInfo.CloneFromTemplate(itemInfo.Template, itemInfo);
									itemInfo4.Count = num15;
									client.Player.AddTemplate(itemInfo4);
									num15 = 0;
								}
								num15++;
							}
							if (num15 > 0)
							{
								ItemInfo itemInfo5 = ItemInfo.CloneFromTemplate(itemInfo.Template, itemInfo);
								itemInfo5.Count = num15;
								client.Player.AddTemplate(itemInfo5);
							}
						}
					}
					client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId, new object[0]));
					GSPacketIn gSPacketIn = new GSPacketIn(145, client.Player.PlayerCharacter.ID);
					gSPacketIn.WriteByte(2);
					gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
					gSPacketIn.WriteInt(pyramid.totalPoint);
					gSPacketIn.WriteInt(pyramid.turnPoint);
					gSPacketIn.WriteInt(pyramid.pointRatio);
					gSPacketIn.WriteInt(pyramid.currentLayer);
					client.Player.SendTCP(gSPacketIn);
					result = 0;
				}
			}
			return result;
		}
	}
}
