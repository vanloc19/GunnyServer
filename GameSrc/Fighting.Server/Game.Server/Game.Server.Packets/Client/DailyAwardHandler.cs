using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(13, "场景用户离开")]
	public class DailyAwardHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int type = packet.ReadInt();
			int point = 0;
			int gold = 0;
			int giftToken = 0;
			int medal = 0;
			int exp = 0;
			StringBuilder builder = new StringBuilder();
			List<ItemInfo> itemInfos = new List<ItemInfo>();
			ItemTemplateInfo temp;
			string translation = "";
			switch (type)
			{
				case 0:
					client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
					return 0;
					if (AwardMgr.AddDailyAward(client.Player))
					{
						using (PlayerBussiness bussiness = new PlayerBussiness())
						{
							if (bussiness.UpdatePlayerLastAward(client.Player.PlayerCharacter.ID, type))
							{
								builder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Success"));
							}
							else
							{
								builder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Fail"));
							}
						}
					}
					else
					{
						builder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Fail1"));
					}
					break;
				case 2:
					{
						client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
						return 0;
                        if (DateTime.Now.Date == client.Player.PlayerCharacter.LastGetEgg.Date)
                        {
                            builder.Append(string.Format("Bạn đã nhận 1 lần hôm nay!"));
                        }
                        else
                        {
							AwardMgr.AddEggAward(client.Player);
							ItemInfo Egg = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(112059), 1, 113);
							client.Player.PropBag.AddTemplate(Egg, 1);
							builder.Append(LanguageMgr.GetTranslation("GameServer.DailyEggReceive.Success"));
						}
                    }
					break;
				case 3:
					if (client.Player.PlayerCharacter.CanTakeVipReward)
					{
						int vIPLevel = client.Player.PlayerCharacter.VIPLevel;
						client.Player.LastVIPPackTime();
						if (!ItemBoxMgr.CreateItemBox(ItemMgr.FindItemTemplate(ItemMgr.FindItemBoxTypeAndLv(2, vIPLevel).TemplateID).TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp))
						{
							client.Player.SendMessage(LanguageMgr.GetTranslation("Error.ChangeChannel"));
							return 0;
						}
						using (PlayerBussiness bussiness3 = new PlayerBussiness())
						{
							bussiness3.UpdateLastVIPPackTime(client.Player.PlayerCharacter.ID);
						}
					}
					else
					{
						builder.Append("Você recebeu sua recompensa hoje!");
					}
					break;
				case 5:
					{
						using (PlayerBussiness bussiness4 = new PlayerBussiness())
						{
							DailyLogListInfo dailyLogListSingle = bussiness4.GetDailyLogListSingle(client.Player.PlayerCharacter.ID);
							if (dailyLogListSingle == null)
							{
								dailyLogListSingle = new DailyLogListInfo
								{
									UserID = client.Player.PlayerCharacter.ID,
									DayLog = "",
									UserAwardLog = 0,
									LastDate = DateTime.Now
								};
							}
							string dayLog = dailyLogListSingle.DayLog;
							dayLog.Split(',');
							if (string.IsNullOrEmpty(dayLog))
							{
								dayLog = "True";
								dailyLogListSingle.UserAwardLog = 0;
							}
							else
							{
								dayLog += ",True";
							}
							dailyLogListSingle.DayLog = dayLog;
							dailyLogListSingle.UserAwardLog++;
							bussiness4.UpdateDailyLogList(dailyLogListSingle);
						}
						break;
					}
			}
			if (point != 0)
			{
				builder.Append(point + LanguageMgr.GetTranslation("OpenUpArkHandler.Money"));
				client.Player.AddMoney(point);
			}
			if (gold != 0)
			{
				builder.Append(gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold"));
				client.Player.AddGold(gold);
			}
			if (giftToken != 0)
			{
				builder.Append(giftToken + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken"));
				client.Player.AddGiftToken(giftToken);
			}
			if (medal != 0)
			{
				builder.Append(medal + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal"));
				client.Player.AddMedal(medal);
			}
			StringBuilder builder2 = new StringBuilder();
			foreach (ItemInfo info3 in itemInfos)
			{
				builder2.Append(info3.Template.Name + "x" + info3.Count + ",");
				if (!client.Player.AddTemplate(info3, info3.Template.BagType, info3.Count, eGameView.RouletteTypeGet))
				{
					using (PlayerBussiness bussiness5 = new PlayerBussiness())
					{
						info3.UserID = 0;
						bussiness5.AddGoods(info3);
						MailInfo info4 = new MailInfo
						{
							Annex1 = info3.ItemID.ToString(),
							Content = LanguageMgr.GetTranslation("OpenUpArkHandler.Content1") + info3.Template.Name + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2"),
							Gold = 0,
							Money = 0,
							Receiver = client.Player.PlayerCharacter.NickName,
							ReceiverID = client.Player.PlayerCharacter.ID,
							Sender = "Sistema",
							SenderID = 1,
							Title = LanguageMgr.GetTranslation("OpenUpArkHandler.Title") + info3.Template.Name + "]",
							Type = 12
						};
						bussiness5.SendMail(info4);
						translation = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail");
					}
				}
			}
			if (builder2.Length > 0)
			{
				builder2.Remove(builder2.Length - 1, 1);
				string[] strArray = builder2.ToString().Split(',');
				for (int i = 0; i < strArray.Length; i++)
				{
					int num15 = 1;
					for (int j = i + 1; j < strArray.Length; j++)
					{
						if (strArray[i].Contains(strArray[j]) && strArray[j].Length == strArray[i].Length)
						{
							num15++;
							strArray[j] = j.ToString();
						}
					}
					if (num15 > 1)
					{
						strArray[i] = strArray[i].Remove(strArray[i].Length - 1, 1);
						string[] strArray2;
						IntPtr ptr;
						(strArray2 = strArray)[(int)(ptr = (IntPtr)i)] = strArray2[(int)ptr] + num15;
					}
					if (strArray[i] != i.ToString())
					{
						string[] strArray2;
						IntPtr ptr;
						(strArray2 = strArray)[(int)(ptr = (IntPtr)i)] = strArray2[(int)ptr] + ",";
						builder.Append(strArray[i]);
					}
				}
			}
			if (builder.Length - 1 > 0)
			{
				builder.Remove(builder.Length - 1, 1);
				builder.Append(".");
			}
			client.Out.SendMessage(eMessageType.GM_NOTICE, translation + builder.ToString());
			if (!string.IsNullOrEmpty(translation))
			{
				client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return 2;
		}
	}
}
