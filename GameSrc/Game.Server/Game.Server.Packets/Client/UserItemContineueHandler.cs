using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(62, "续费")]
	public class UserItemContineueHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = packet.ReadInt();
			int num2 = 0;
			int num4;
			ItemInfo itemAt;
			while (true)
			{
				if (num2 < num)
				{
					eBageType eBageType = (eBageType)packet.ReadByte();
					int num3 = packet.ReadInt();
					num4 = packet.ReadInt();
					int num5 = packet.ReadByte();
					packet.ReadBoolean();
					if ((eBageType != 0 || num3 < 31) && eBageType != eBageType.PropBag && eBageType != eBageType.Consortia)
					{
						client.Player.SendMessage("Không thể tiếp phí");
					}
					else
					{
						itemAt = client.Player.GetItemAt(eBageType, num3);
						if (itemAt != null && itemAt.ValidDate != 0)
						{
							int gold = 0;
							int money = 0;
							int offer = 0;
							int gifttoken = 0;
							int petScore = 0;
							int boguScore = 0;
							int damageScore = 0;
							int validDate = itemAt.ValidDate;
							int count = itemAt.Count;
							bool flag = itemAt.IsValidItem();
							List<int> list = new List<int>();
							string text = "UserBuyItemHandler.Success";
							ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(num4);
							if (shopItemInfoById == null || shopItemInfoById.TemplateID != itemAt.TemplateID)
							{
								break;
							}
							list = ItemInfo.SetItemType(shopItemInfoById, num5, ref gold, ref money, ref offer, ref gifttoken, ref petScore, ref boguScore, ref damageScore);
							int count2 = client.Player.EquipBag.GetItems().Count;
							bool flag2 = true;
							for (int i = 0; i < list.Count; i += 2)
							{
								if (client.Player.GetItemCount(list[i]) < list[i + 1])
								{
									flag2 = false;
								}
							}
							if (!flag2)
							{
								text = "UserBuyItemHandler.NoBuyItem";
								client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(text));
								return 1;
							}
							if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money && offer <= client.Player.PlayerCharacter.Offer && gifttoken <= client.Player.PlayerCharacter.GiftToken && petScore <= client.Player.PlayerCharacter.petScore && boguScore <= client.Player.PlayerCharacter.Score && damageScore <= client.Player.PlayerCharacter.damageScores)
							{
								client.Player.RemoveMoney(money, isConsume: true);
								client.Player.RemoveGold(gold);
								client.Player.RemoveOffer(offer);
								client.Player.RemoveGiftToken(gifttoken);
								client.Player.RemovePetScore(petScore);
								client.Player.RemoveScore(boguScore);
								client.Player.RemoveDamageScores(damageScore);
								for (int j = 0; j < list.Count; j += 2)
								{
									client.Player.RemoveTemplate(list[j], list[j + 1]);
									stringBuilder.Append(list[j].ToString() + ":");
								}
								if (itemAt.ValidDate != 0)
								{
									if (1 == num5)
									{
										itemAt.ValidDate = shopItemInfoById.AUnit;
									}
									if (2 == num5)
									{
										itemAt.ValidDate = shopItemInfoById.BUnit;
									}
									if (3 == num5)
									{
										itemAt.ValidDate = shopItemInfoById.CUnit;
									}
									if (!flag)
									{
										itemAt.BeginDate = DateTime.Now;
										itemAt.IsUsed = false;
									}
									else if (itemAt.ValidDate != 0)
									{
										itemAt.ValidDate += validDate;
									}
								}
								switch (eBageType)
								{
									case eBageType.EquipBag:
										client.Player.EquipBag.UpdateItem(itemAt);
										break;
									case eBageType.PropBag:
										client.Player.PropBag.UpdateItem(itemAt);
										break;
									default:
										client.Player.StoreBag.UpdateItem(itemAt);
										break;
								}
								client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.Success"));
								client.Player.AddLog("ContinueGood", "TemplateID: " + itemAt.TemplateID + "|VaildDate: " + itemAt.ValidDate + "|OldVaildDate: " + validDate + "|Name: " + itemAt.Template.Name + "|GoodsID: " + num4);
							}
							else
							{
								itemAt.ValidDate = validDate;
								itemAt.Count = count;
								client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.NoMoney"));
							}
						}
						else
						{
							client.Player.SendMessage("Vật phẩm này không thể tiếp phí");
						}
					}
					num2++;
					continue;
				}
				return 0;
			}
			client.Player.AddLog("Cheat", "Cheat Contineue Item TemplateID: " + itemAt.TemplateID + "|" + itemAt.Template.Name + "|GoodsID: " + num4);
			client.Player.SendMessage("Phát hiện gian lận hệ thống. Điều này sẽ được gửi tới BQT chờ giải quyết.");
			return 0;
		}

		public UserItemContineueHandler()
		{


		}
	}
}
