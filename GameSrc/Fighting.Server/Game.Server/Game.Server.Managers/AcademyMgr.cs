using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class AcademyMgr
	{
		public static readonly int LEVEL_GAP;

		public static readonly int TARGET_PLAYER_MIN_LEVEL;

		public static readonly int ACADEMY_LEVEL_MIN;

		public static readonly int ACADEMY_LEVEL_MAX;

		public static readonly int RECOMMEND_MAX_NUM;

		public static readonly int NONE_STATE;

		public static readonly int APPRENTICE_STATE;

		public static readonly int MASTER_STATE;

		public static readonly int MASTER_FULL_STATE;

		public static object m_object;

		private static List<AcademyRequestInfo> Requests;

		public static bool Init()
		{
			Requests = new List<AcademyRequestInfo>();
			return true;
		}

		public static void AddRequest(AcademyRequestInfo request)
		{
			lock (Requests)
			{
				Requests.Add(request);
			}
		}

		public static AcademyRequestInfo GetRequest(int senderid, int receiveid)
		{
			AcademyRequestInfo academyRequestInfo1 = null;
			lock (Requests)
			{
				foreach (AcademyRequestInfo academyRequestInfo2 in Requests)
				{
					if (academyRequestInfo2.SenderID == senderid && academyRequestInfo2.ReceiderID == receiveid)
					{
						return academyRequestInfo2;
					}
				}
				return academyRequestInfo1;
			}
		}

		public static void RemoveRequest(AcademyRequestInfo request)
		{
			lock (Requests)
			{
				Requests.Remove(request);
			}
		}

		public static void RemoveOldRequest()
		{
			List<AcademyRequestInfo> academyRequestInfoList = new List<AcademyRequestInfo>();
			lock (Requests)
			{
				foreach (AcademyRequestInfo academyRequestInfo in Requests)
				{
					if (academyRequestInfo.CreateTime.AddHours(1.0) < DateTime.Now)
					{
						academyRequestInfoList.Add(academyRequestInfo);
					}
				}
			}
			if (academyRequestInfoList.Count <= 0)
			{
				return;
			}
			foreach (AcademyRequestInfo item in academyRequestInfoList)
			{
				RemoveRequest(item);
			}
		}

		public static bool FireApprentice(GamePlayer player, int uid, bool isSilent)
		{
			bool flag = false;
			lock (m_object)
			{
				if (flag = player.PlayerCharacter.RemoveMasterOrApprentices(uid))
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						GamePlayer playerById = WorldMgr.GetPlayerById(uid);
						PlayerInfo player2 = ((playerById == null) ? playerBussiness.GetUserSingleByUserID(uid) : playerById.PlayerCharacter);
						if (player2 == null)
						{
							return flag;
						}
						player2.RemoveMasterOrApprentices(player2.masterID);
						player2.masterID = 0;
						playerBussiness.UpdateAcademyPlayer(player2);
						if (playerById == null)
						{
							return flag;
						}
						if (!isSilent)
						{
							playerById.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("Game.Server.AppSystem.BreakApprenticeShipMsg.Apprentice", player.PlayerCharacter.NickName), isAlert: true);
						}
						playerById.Out.SendAcademyAppState(player2, uid);
						return flag;
					}
				}
				return flag;
			}
		}

		public static bool FireMaster(GamePlayer player, bool isComplete)
		{
			bool flag = false;
			lock (m_object)
			{
				if (flag = player.PlayerCharacter.RemoveMasterOrApprentices(player.PlayerCharacter.masterID))
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						GamePlayer playerById = WorldMgr.GetPlayerById(player.PlayerCharacter.masterID);
						PlayerInfo player2 = ((playerById == null) ? playerBussiness.GetUserSingleByUserID(player.PlayerCharacter.masterID) : playerById.PlayerCharacter);
						if (player2 == null)
						{
							return flag;
						}
						if (isComplete)
						{
							player2.graduatesCount++;
						}
						player2.RemoveMasterOrApprentices(player.PlayerId);
						playerBussiness.UpdateAcademyPlayer(player2);
						if (playerById != null)
						{
							if (!isComplete)
							{
								playerById.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("Game.Server.AppSystem.BreakApprenticeShipMsg.Master", player.PlayerCharacter.NickName), isAlert: true);
							}
							playerById.Out.SendAcademyAppState(player2, player.PlayerCharacter.ID);
						}
						player.PlayerCharacter.masterID = 0;
						return flag;
					}
				}
				return flag;
			}
		}

		public static bool AddApprentice(GamePlayer master, GamePlayer app)
		{
			bool flag = false;
			lock (m_object)
			{
				if (flag = master.PlayerCharacter.AddMasterOrApprentices(app.PlayerCharacter.ID, app.PlayerCharacter.NickName) && app.PlayerCharacter.masterID == 0)
				{
					app.PlayerCharacter.masterID = master.PlayerCharacter.ID;
					app.PlayerCharacter.AddMasterOrApprentices(master.PlayerCharacter.ID, master.PlayerCharacter.NickName);
					app.Out.SendAcademyAppState(app.PlayerCharacter, -1);
					master.Out.SendAcademyAppState(master.PlayerCharacter, -1);
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						playerBussiness.UpdateAcademyPlayer(app.PlayerCharacter);
						playerBussiness.UpdateAcademyPlayer(master.PlayerCharacter);
					}
					app.OnAcademyEvent(master, 0);
					master.OnAcademyEvent(app, 1);
					return flag;
				}
				return flag;
			}
		}

		public static void UpdateAwardApp(GamePlayer player, int oldLevel)
		{
			Dictionary<int, int> dictionary1 = GameProperties.AcademyApprenticeAwardArr();
			Dictionary<int, int> dictionary2 = GameProperties.AcademyMasterAwardArr();
			int masterId = player.PlayerCharacter.masterID;
			string titleGraduated = LanguageMgr.GetTranslation("Game.Server.Managers.AcademyMgr.TitleGraduated");
			GamePlayer playerById = WorldMgr.GetPlayerById(masterId);
			for (int key = oldLevel + 1; key <= player.PlayerCharacter.Grade; key++)
			{
				if (dictionary1.ContainsKey(key))
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(dictionary1[key]), 1, 103);
					itemInfo.IsBinds = true;
					WorldEventMgr.SendItemToMail(itemInfo, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, player.ZoneId, null, LanguageMgr.GetTranslation("Game.Server.AppSystem.GraduateBox.Success", key));
				}
				if (dictionary2.ContainsKey(key))
				{
					ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(dictionary2[key]), 1, 103);
					itemInfo2.IsBinds = true;
					WorldEventMgr.SendItemToMail(itemInfo2, masterId, player.PlayerCharacter.MasterOrApprenticesArr[masterId], player.ZoneId, null, LanguageMgr.GetTranslation("Game.Server.AppSystem.TakeAppBox.Success", player.PlayerCharacter.NickName, key));
				}
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					if (playerById != null)
					{
						playerById.SendMailToUser(playerBussiness, LanguageMgr.GetTranslation("Game.Server.AppSystem.ApprenticeLevelUp.mailTitle", player.PlayerCharacter.NickName, key), LanguageMgr.GetTranslation("Game.Server.AppSystem.ApprenticeLevelUp.mailTitle", player.PlayerCharacter.NickName, key), eMailType.ItemOverdue);
						continue;
					}
					MailInfo mailInfo = new MailInfo();
					mailInfo.Content = LanguageMgr.GetTranslation("Game.Server.AppSystem.ApprenticeLevelUp.mailTitle", player.PlayerCharacter.NickName, key);
					mailInfo.Title = LanguageMgr.GetTranslation("Game.Server.AppSystem.ApprenticeLevelUp.mailTitle", player.PlayerCharacter.NickName, key);
					mailInfo.Gold = 0;
					mailInfo.IsExist = true;
					mailInfo.Money = 0;
					mailInfo.GiftToken = 0;
					mailInfo.Receiver = player.PlayerCharacter.MasterOrApprenticesArr[masterId];
					mailInfo.ReceiverID = masterId;
					mailInfo.Sender = player.PlayerCharacter.MasterOrApprenticesArr[masterId];
					mailInfo.SenderID = masterId;
					mailInfo.Type = 9;
					MailInfo mail = mailInfo;
					mail.Annex1 = "";
					mail.Annex1Name = "";
					playerBussiness.SendMail(mail);
				}
			}
			if (player.PlayerCharacter.Grade < ACADEMY_LEVEL_MIN)
			{
				return;
			}
			PlayerInfo playerInfo = null;
			if (playerById != null)
			{
				playerById.Out.SendAcademyGradute(player, 1);
				playerInfo = playerById.PlayerCharacter;
			}
			else
			{
				using (PlayerBussiness playerBussiness_ = new PlayerBussiness())
				{
					playerInfo = playerBussiness_.GetUserSingleByUserID(masterId);
				}
			}
			string[] array = GameProperties.AcademyAppAwardComplete.Split(',')[player.PlayerCharacter.Sex ? 1 : 0].Split('|');
			List<ItemInfo> infos = new List<ItemInfo>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(int.Parse(array2[i])), 1, 103);
				fromTemplate.ValidDate = 365;
				fromTemplate.IsBinds = true;
				infos.Add(fromTemplate);
			}
			WorldEventMgr.SendItemsToMails(infos, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, player.ZoneId, null, LanguageMgr.GetTranslation("Game.Server.AppSystem.GraduateBox.Success"));
			string[] array3 = GameProperties.AcademyMasAwardComplete.Split(',')[playerInfo.Sex ? 1 : 0].Split('|');
			infos.Clear();
			array2 = array3;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemInfo fromTemplate2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(int.Parse(array2[i])), 1, 103);
				fromTemplate2.ValidDate = 3;
				fromTemplate2.IsBinds = true;
				infos.Add(fromTemplate2);
			}
			WorldEventMgr.SendItemsToMail(infos, playerInfo.ID, playerInfo.NickName, LanguageMgr.GetTranslation("Game.Server.AppSystem.GraduateBoxForMaster.MailTitle", player.PlayerCharacter.NickName), LanguageMgr.GetTranslation("Game.Server.AppSystem.GraduateBoxForMaster.MailContert"));
			FireMaster(player, isComplete: true);
			using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
			{
				playerBussiness2.UpdateAcademyPlayer(player.PlayerCharacter);
			}
			player.Out.SendAcademyAppState(player.PlayerCharacter, masterId);
			player.Out.SendAcademyGradute(player, 0);
			player.Rank.AddAchievementRank(titleGraduated);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.MasterGainHonour.content", playerInfo.NickName, player.PlayerCharacter.NickName, titleGraduated));
			}
		}

		public static bool CheckCanApp(int levelApp)
		{
			if (levelApp >= TARGET_PLAYER_MIN_LEVEL)
			{
				return levelApp <= ACADEMY_LEVEL_MAX;
			}
			return false;
		}

		public static bool CheckCanMaster(int levelMaster)
		{
			return levelMaster >= ACADEMY_LEVEL_MIN;
		}

		static AcademyMgr()
		{
			LEVEL_GAP = 5;
			TARGET_PLAYER_MIN_LEVEL = 6;
			ACADEMY_LEVEL_MIN = 20;
			ACADEMY_LEVEL_MAX = 16;
			RECOMMEND_MAX_NUM = 3;
			NONE_STATE = 0;
			APPRENTICE_STATE = 1;
			MASTER_STATE = 2;
			MASTER_FULL_STATE = 3;
			m_object = new object();
		}
	}
}
