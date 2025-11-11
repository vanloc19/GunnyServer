using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.BUY_GOODS, "购买物品")]
	public class UserBuyItemHandler : IPacketHandler
	{
		public static int countConnect;

		private static readonly ILog log;

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (countConnect >= 3000)
			{
				client.Disconnect();
				return 0;
			}
			int gold = 0;
			int money1 = 0;
			int offer = 0;
			int gifttoken = 0;
			int petScore = 0;
			int boguScore = 0;
			int damageScore = 0;
			StringBuilder stringBuilder1 = new StringBuilder();
			eMessageType type1 = eMessageType.GM_NOTICE;
			string translateId1 = "UserBuyItemHandler.Success";
			GSPacketIn pkg = new GSPacketIn(44, client.Player.PlayerCharacter.ID);
			List<ItemInfo> itemInfoList = new List<ItemInfo>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			List<bool> boolList = new List<bool>();
			List<int> intList1 = new List<int>();
			StringBuilder stringBuilder2 = new StringBuilder();
			bool isBinds = true;
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			int num1 = packet.ReadInt();
			if (num1 > 0 && num1 <= 99)
			{
				List<string> stringList = new List<string>();
				for (int index2 = 0; index2 < num1; index2++)
				{
					int ID = packet.ReadInt();
					int type2 = packet.ReadInt();
					string str1 = packet.ReadString();
					bool flag = packet.ReadBoolean();
					string str4 = packet.ReadString();
					int num2 = packet.ReadInt();
					stringList.Add(ID.ToString());
					ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(ID);
					//Console.WriteLine($"{shopItemInfoById.EndDate} & {DateTime.Now}");
					if(DateTime.Now >= shopItemInfoById.EndDate)
                    {
						client.Player.SendMessage("Vật phẩm quá hạn không thể mua!");
						return 1;
                    }						
					if (shopItemInfoById == null || !ShopMgr.IsOnShop(shopItemInfoById.ID))
					{
						continue;
					}
					if (shopItemInfoById.ShopID != 2 && ShopMgr.CanBuy(shopItemInfoById.ShopID, consortiaInfo?.ShopLevel ?? 1, ref isBinds, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches))
					{
						if (shopItemInfoById.ShopID == 20)
						{
							if (client.Player.PlayerCharacter.ShopFinallyGottenTime.Date == DateTime.Now.Date || !WorldMgr.UpdateShopFreeCount(shopItemInfoById.ID, shopItemInfoById.LimitCount))
							{
								client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission2"));
								return 1;
							}
							List<ShopFreeCountInfo> allShopFreeCount = WorldMgr.GetAllShopFreeCount();
							client.Out.SendShopGoodsCountUpdate(allShopFreeCount);
							client.Player.PlayerCharacter.ShopFinallyGottenTime = DateTime.Now.Date;
							dictionary.Add(-9999, 1);
							//string translation = LanguageMgr.GetTranslation("GameServer.FreeItem.Notice.Msg", client.Player.PlayerCharacter.NickName, shopItemInfoById.TemplateID);
							//GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, 0, shopItemInfoById.TemplateID, null);
							//GameServer.Instance.LoginServer.SendPacket(packet2);
						}
						ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 102);
						if (shopItemInfoById.BuyType == 0)
						{
							if (1 == type2)
							{
								fromTemplate.ValidDate = shopItemInfoById.AUnit;
							}
							if (2 == type2)
							{
								fromTemplate.ValidDate = shopItemInfoById.BUnit;
							}
							if (3 == type2)
							{
								fromTemplate.ValidDate = shopItemInfoById.CUnit;
							}
						}
						else
						{
							if (1 == type2)
							{
								fromTemplate.Count = shopItemInfoById.AUnit;
							}
							if (2 == type2)
							{
								fromTemplate.Count = shopItemInfoById.BUnit;
							}
							if (3 == type2)
							{
								fromTemplate.Count = shopItemInfoById.CUnit;
							}
						}
						if (fromTemplate == null && shopItemInfoById == null)
						{
							continue;
						}
						fromTemplate.Color = ((str1 == null) ? "" : str1);
						fromTemplate.Skin = ((str4 == null) ? "" : str4);
						fromTemplate.IsBinds = isBinds || Convert.ToBoolean(shopItemInfoById.IsBind);
						fromTemplate.IsBinds = true;
						stringBuilder2.Append(type2);
						stringBuilder2.Append(",");
						itemInfoList.Add(fromTemplate);
						boolList.Add(flag);
						intList1.Add(num2);
						List<int> intList2 = ItemInfo.SetItemType(shopItemInfoById, type2, ref gold, ref money1, ref offer, ref gifttoken, ref petScore, ref boguScore, ref damageScore);
						for (int index3 = 0; index3 < intList2.Count; index3 += 2)
						{
							if (dictionary.ContainsKey(intList2[index3]))
							{
								dictionary[intList2[index3]] += intList2[index3 + 1];
							}
							else
							{
								dictionary.Add(intList2[index3], intList2[index3 + 1]);
							}
						}
						continue;
					}
					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
					return 1;
				}
				if (itemInfoList.Count == 0)
				{
					return 1;
				}
				if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
				{
					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
					return 1;
				}
				bool flag2 = true;
				foreach (KeyValuePair<int, int> keyValuePair2 in dictionary)
				{
					if (keyValuePair2.Key != -9999 && client.Player.GetTemplateCount(keyValuePair2.Key) < keyValuePair2.Value)
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					string translateId2 = "UserBuyItemHandler.NoBuyItem";
					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation(translateId2));
					return 1;
				}
				if (gold >= 0 && money1 >= 0 && offer >= 0 && gifttoken >= 0 && petScore >= 0 && boguScore >= 0 && damageScore >= 0 && (gold > 0 || money1 > 0 || offer > 0 || gifttoken > 0 || petScore > 0 || boguScore > 0  || damageScore > 0|| dictionary.Count > 0))
				{
					int money2 = client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock;
					if (gold <= client.Player.PlayerCharacter.Gold &&
					    money1 <= client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock &&
					    offer <= client.Player.PlayerCharacter.Offer &&
					    gifttoken <= client.Player.PlayerCharacter.GiftToken &&
					    petScore <= client.Player.PlayerCharacter.petScore &&
					    boguScore <= client.Player.PlayerCharacter.Score &&
					    damageScore <= client.Player.PlayerCharacter.damageScores)
					{
						client.Player.RemoveMoney(money1, isConsume: true);
						client.Player.RemoveGold(gold);
						client.Player.RemoveOffer(offer);
						client.Player.RemoveGiftToken(gifttoken);
						client.Player.RemovePetScore(petScore);
						client.Player.RemoveScore(boguScore);
						client.Player.RemoveDamageScores(damageScore);
						int money3 = client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock;
						string str2 =
							$"money: {money1} | gold: {gold} | offter: {offer} | gifttoken: {gifttoken} | petScore: {petScore} | boguScore: {boguScore} | damageScore: {damageScore} | moneyBefore: {money2} | moneyAfter: {money3}";
						/*if (money1 > 0 && client.Player.Extra.CheckNoviceActiveOpen(NoviceActiveType.USE_MONEY_ACTIVE))
						{
							client.Player.Extra.UpdateEventCondition(3, money1, isPlus: true, 0);
						}
						if (money1 > 0 && client.Player.Extra.CheckNoviceActiveOpen(NoviceActiveType.USE_MONEY_SPECIAL))
						{
							Console.WriteLine("public class UserBuyItemHandler : IPacketHandler");
							client.Player.Extra.UpdateEventCondition(8, money1, isPlus: true, 0);
						}*/
						foreach (KeyValuePair<int, int> keyValuePair in dictionary)
						{
							if (keyValuePair.Key != -9999)
							{
								client.Player.RemoveTemplateInShop(keyValuePair.Key, keyValuePair.Value);
							}

							stringBuilder1.Append(keyValuePair.Key + ",");
						}

						if (dictionary.Count > 0)
						{
							client.Player.UpdateProperties();
						}

						string str5 = str2 + " | itemNeed: " + string.Join(",", dictionary.Keys.ToArray());
						string str6 = "";
						int num3 = 0;
						MailInfo mail = new MailInfo();
						StringBuilder stringBuilder3 = new StringBuilder();
						stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));
						for (int index = 0; index < itemInfoList.Count; index++)
						{
							string str13 = str6;
							string str7;
							if (!(str6 == ""))
							{
								string str10 = itemInfoList[index].TemplateID.ToString();
								str7 = "," + str10;
							}
							else
							{
								str7 = itemInfoList[index].TemplateID.ToString();
							}

							str6 = str13 + str7;
							if (client.Player.AddTemplate(itemInfoList[index], itemInfoList[index].Template.BagType,
								    itemInfoList[index].Count, backToMail: false))
							{
								if (!boolList[index] || !itemInfoList[index].CanEquip())
								{
									continue;
								}

								int itemEpuipSlot =
									client.Player.EquipBag.FindItemEpuipSlot(itemInfoList[index].Template);
								if ((itemEpuipSlot != 9 && itemEpuipSlot != 10) ||
								    (intList1[index] != 9 && intList1[index] != 10))
								{
									if ((itemEpuipSlot == 7 || itemEpuipSlot == 8) &&
									    (intList1[index] == 7 || intList1[index] == 8))
									{
										itemEpuipSlot = intList1[index];
									}
								}
								else
								{
									itemEpuipSlot = intList1[index];
								}

								client.Player.EquipBag.MoveItem(itemInfoList[index].Place, itemEpuipSlot, 0);
								translateId1 = "UserBuyItemHandler.Save";
								continue;
							}

							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								itemInfoList[index].UserID = 0;
								playerBussiness.AddGoods(itemInfoList[index]);
								num3++;
								stringBuilder3.Append(num3);
								stringBuilder3.Append("、");
								stringBuilder3.Append(itemInfoList[index].Template.Name);
								stringBuilder3.Append("x");
								stringBuilder3.Append(itemInfoList[index].Count);
								stringBuilder3.Append(";");
								switch (num3)
								{
									case 1:
									{
										string str8 = (mail.Annex1 = itemInfoList[index].ItemID.ToString());
										mail.Annex1Name = itemInfoList[index].Template.Name;
										break;
									}
									case 2:
									{
										string str9 = (mail.Annex2 = itemInfoList[index].ItemID.ToString());
										mail.Annex2Name = itemInfoList[index].Template.Name;
										break;
									}
									case 3:
									{
										string str11 = (mail.Annex3 = itemInfoList[index].ItemID.ToString());
										mail.Annex3Name = itemInfoList[index].Template.Name;
										break;
									}
									case 4:
									{
										string str12 = (mail.Annex4 = itemInfoList[index].ItemID.ToString());
										mail.Annex4Name = itemInfoList[index].Template.Name;
										break;
									}
									case 5:
									{
										string str3 = (mail.Annex5 = itemInfoList[index].ItemID.ToString());
										mail.Annex5Name = itemInfoList[index].Template.Name;
										break;
									}
								}

								if (num3 == 5)
								{
									num3 = 0;
									mail.AnnexRemark = stringBuilder3.ToString();
									stringBuilder3.Remove(0, stringBuilder3.Length);
									stringBuilder3.Append(
										LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));
									mail.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title") +
									               mail.Annex1Name + "]";
									mail.Gold = 0;
									mail.Money = 0;
									mail.Receiver = client.Player.PlayerCharacter.NickName;
									mail.ReceiverID = client.Player.PlayerCharacter.ID;
									mail.Sender = mail.Receiver;
									mail.SenderID = mail.ReceiverID;
									mail.Title = mail.Content;
									mail.Type = 8;
									playerBussiness.SendMail(mail);
									type1 = eMessageType.BIGBUGLE_NOTICE;
									translateId1 = "UserBuyItemHandler.Mail";
									mail.Revert();
								}
							}
						}

						string content = str5 + " | listsBuy: " + str6;
						if (num3 > 0)
						{
							using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
							{
								mail.AnnexRemark = stringBuilder3.ToString();
								mail.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title") +
								               mail.Annex1Name + "]";
								mail.Gold = 0;
								mail.Money = 0;
								mail.Receiver = client.Player.PlayerCharacter.NickName;
								mail.ReceiverID = client.Player.PlayerCharacter.ID;
								mail.Sender = mail.Receiver;
								mail.SenderID = mail.ReceiverID;
								mail.Title = mail.Content;
								mail.Type = 8;
								playerBussiness2.SendMail(mail);
								type1 = eMessageType.BIGBUGLE_NOTICE;
								translateId1 = "UserBuyItemHandler.Mail";
							}
						}

						if (type1 == eMessageType.BIGBUGLE_NOTICE)
						{
							client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
						}

						client.Player.OnPaid(money1, gold, offer, gifttoken, petScore, 0, stringBuilder1.ToString());
						client.Player.AddLog("Buy Shop", content);
						if (money1 > 0)
						{
							client.Player.AddLog("BuyShop", money1.ToString());
						}
					}
					else
					{
						if (money1 > client.Player.PlayerCharacter.Money)
                        {
							translateId1 = "UserBuyItemHandler.NoMoney";
						}
						if (gold > client.Player.PlayerCharacter.Gold)
						{
							translateId1 = "UserBuyItemHandler.NoGold";
						}
						if (offer > client.Player.PlayerCharacter.Offer)
						{
							translateId1 = "UserBuyItemHandler.NoOffer";
						}
						if (gifttoken > client.Player.PlayerCharacter.GiftToken)
						{
							translateId1 = "UserBuyItemHandler.GiftToken";
						}
						if (petScore > client.Player.PlayerCharacter.petScore)
                        {
							translateId1 = "UserBuyItemHandler.petScore";
                        }
						if (boguScore > client.Player.PlayerCharacter.Score)
                        {
							translateId1 = "UserBuyItemHandler.boguScore";
                        }
						if(damageScore > client.Player.PlayerCharacter.damageScores)
                        {
							translateId1 = "UserBuyItemHandler.damageScore";
						}
						type1 = eMessageType.BIGBUGLE_NOTICE;
					}
					client.Player.SaveNewItems();
					client.Out.SendMessage(type1, LanguageMgr.GetTranslation(translateId1));
					pkg.WriteInt(1);
					pkg.WriteInt(3);
					client.Player.SendTCP(pkg);
					return 0;
				}
				client.Player.SendMessage("Erro no sistema. O problema foi enviado para um administrador.");
				log.Error("username: " + client.Player.PlayerCharacter.UserName + " - hack money down.");
				return 0;
			}
			client.Player.SendMessage("rro no sistema. O problema foi enviado para um administrador.");
			log.Error("username: " + client.Player.PlayerCharacter.UserName + " - hack money down (count: " + num1 + ").");
			return 0;
		}

		static UserBuyItemHandler()
		{
			countConnect = 0;
			log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
