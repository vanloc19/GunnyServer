using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(53, "获取箱子")]
	public class UserGetBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GamePlayer player = client.Player;
			int num = packet.ReadInt();
			LoadUserBoxInfo box = null;
			if (num == 0)
			{
				int time = packet.ReadInt();
				int onlineTime2 = (int)DateTime.Now.Subtract(player.BoxBeginTime).TotalMinutes;
				box = UserBoxMgr.FindTemplateByCondition(0, player.PlayerCharacter.Grade, player.PlayerCharacter.BoxProgression);
				if (box != null && onlineTime2 >= time && box.Condition == time)
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						playerBussiness.UpdateBoxProgression(player.PlayerCharacter.ID, player.PlayerCharacter.BoxProgression, player.PlayerCharacter.GetBoxLevel, player.PlayerCharacter.AddGPLastDate, DateTime.Now, time);
						player.PlayerCharacter.AlreadyGetBox = time;
						player.PlayerCharacter.BoxGetDate = DateTime.Now;
					}
				}
				return 0;
			}
			int type = packet.ReadInt();
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			bool updatedb = false;
			bool result = true;
			if (type == 0)
			{
				int onlineTime = (int)DateTime.Now.Subtract(player.BoxBeginTime).TotalMinutes;
				box = UserBoxMgr.FindTemplateByCondition(0, player.PlayerCharacter.Grade, player.PlayerCharacter.BoxProgression);
				if (box != null && (onlineTime >= box.Condition || player.PlayerCharacter.AlreadyGetBox == box.Condition))
				{
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						if (playerBussiness2.UpdateBoxProgression(player.PlayerCharacter.ID, box.Condition, player.PlayerCharacter.GetBoxLevel, player.PlayerCharacter.AddGPLastDate, DateTime.Now.Date, 0))
						{
							player.PlayerCharacter.BoxProgression = box.Condition;
							player.PlayerCharacter.BoxGetDate = DateTime.Now.Date;
							player.PlayerCharacter.AlreadyGetBox = 0;
							updatedb = true;
						}
					}
				}
			}
			else
			{
				box = UserBoxMgr.FindTemplateByCondition(1, player.PlayerCharacter.GetBoxLevel, Convert.ToInt32(player.PlayerCharacter.Sex));
				if (box != null && player.PlayerCharacter.Grade >= box.Level)
				{
					using (PlayerBussiness db = new PlayerBussiness())
					{
						if (db.UpdateBoxProgression(player.PlayerCharacter.ID, player.PlayerCharacter.BoxProgression, box.Level, player.PlayerCharacter.AddGPLastDate, player.PlayerCharacter.BoxGetDate, 0))
						{
							player.PlayerCharacter.GetBoxLevel = box.Level;
							updatedb = true;
						}
					}
				}
			}
			if (updatedb)
			{
				if (box != null)
				{
					List<ItemInfo> mailList = new List<ItemInfo>();
					List<ItemInfo> items = new List<ItemInfo>();
					int gold = 0;
					int money = 0;
					int giftToken = 0;
					int gp = 0;
					ItemBoxMgr.CreateItemBox(Convert.ToInt32(box.TemplateID), items, ref gold, ref money, ref giftToken, ref gp);
					if (gold > 0)
					{
						player.AddGold(gold);
					}
					if (money > 0)
					{
						player.AddMoney(money);
					}
					if (giftToken > 0)
					{
						player.AddGiftToken(giftToken);
					}
					if (gp > 0)
					{
						player.AddGP(gp, false, false);
					}
					foreach (ItemInfo item in items)
					{
						item.RemoveType = 120;
						if (!player.AddItem(item))
						{
							mailList.Add(item);
						}
					}
					if (type == 0)
					{
						player.BoxBeginTime = DateTime.Now;
						box = UserBoxMgr.FindTemplateByCondition(0, player.PlayerCharacter.Grade, player.PlayerCharacter.BoxProgression);
						if (box != null)
						{
							player.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Nhận quà từ rương thời gian {0} phút.", box.Condition));
						}
						else
						{
							player.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Bạn đã nhận hết của ngày hôm nay"));
						}
					}
					else
					{
						box = UserBoxMgr.FindTemplateByCondition(1, player.PlayerCharacter.GetBoxLevel, Convert.ToInt32(player.PlayerCharacter.Sex));
						if (box != null)
						{
							player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.level", box.Level));
						}
						else
						{
							player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.over"));
						}
					}
					if (mailList.Count > 0 && player.SendItemsToMail(mailList, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.mail"), LanguageMgr.GetTranslation("UserGetTimeBoxHandler.title"), eMailType.OpenUpArk))
					{
						player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserGetTimeBixHandler..full"));
						result = true;
						player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
				else
				{
					result = false;
				}
			}
			else
			{
				player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.fail"));
			}
			if (type == 0)
			{
				pkg.WriteBoolean(result);
				pkg.WriteInt(player.PlayerCharacter.BoxProgression);
				player.SendTCP(pkg);
			}
			return 0;
		}
	}
}
