using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievement
{
	public class AchievementInventory
	{
		private object m_lock;

		protected List<AchievementDataInfo> m_data;

		protected List<UsersRecordInfo> m_userRecord;

		private GamePlayer m_player;

		public AchievementInventory(GamePlayer player)
		{
			m_player = player;
			m_lock = new object();
			m_userRecord = new List<UsersRecordInfo>();
			m_data = new List<AchievementDataInfo>();
		}

		public void LoadFromDatabase(int playerId)
		{
			lock (m_lock)
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					m_userRecord = db.GetUserRecord(m_player.PlayerId);
					m_data = db.GetUserAchievementData(m_player.PlayerId);
					InitUserRecord();
					if (m_userRecord != null && m_userRecord.Count > 0)
					{
						m_player.Out.SendInitAchievements(m_userRecord);
					}
					if (m_data != null && m_data.Count > 0)
					{
						m_player.Out.SendUpdateAchievementData(m_data);
					}
				}
				BaseUserRecord.CreateCondition(AchievementMgr.ItemRecordType, m_player);
			}
		}

		public List<AchievementDataInfo> GetSuccessAchievement()
		{
			lock (m_data)
			{
				return m_data.ToList();
			}
		}

		public List<UsersRecordInfo> GetProccessAchievement()
		{
			lock (m_userRecord)
			{
				return m_userRecord.ToList();
			}
		}

		public void InitUserRecord()
		{
			Hashtable ht = AchievementMgr.ItemRecordType;
			lock (m_userRecord)
			{
				if (m_userRecord.Count >= ht.Count)
				{
					return;
				}
				DictionaryEntry de;
				foreach (DictionaryEntry item in ht)
				{
					de = item;
					UsersRecordInfo temp = new UsersRecordInfo();
					temp.UserID = m_player.PlayerId;
					DictionaryEntry de2 = de;
					temp.RecordID = int.Parse(de2.Key.ToString());
					temp.Total = 0;
					temp.IsDirty = true;
					if (m_userRecord.Where(delegate(UsersRecordInfo s)
					{
						int recordID = s.RecordID;
						DictionaryEntry dictionaryEntry = de;
						return recordID == int.Parse(dictionaryEntry.Key.ToString());
					}).ToList().Count <= 0)
					{
						m_userRecord.Add(temp);
					}
				}
			}
		}

		public int UpdateUserAchievement(int type, int value)
		{
			lock (m_userRecord)
			{
				foreach (UsersRecordInfo info in m_userRecord)
				{
					if (info.RecordID == type)
					{
						info.Total += value;
						info.IsDirty = true;
						m_player.Out.SendUpdateAchievements(info);
					}
				}
			}
			return 0;
		}

		public int UpdateUserAchievement(int type, int value, int mode)
		{
			lock (m_userRecord)
			{
				foreach (UsersRecordInfo info in m_userRecord)
				{
					if (info.RecordID == type && info.Total < value)
					{
						info.Total = value;
						info.IsDirty = true;
						m_player.Out.SendUpdateAchievements(info);
					}
				}
			}
			return 0;
		}

		public bool Finish(AchievementInfo achievementInfo)
		{
			if (!CanCompleted(achievementInfo))
			{
				return false;
			}
			AddAchievementData(achievementInfo);
			SendReward(achievementInfo);
			return true;
		}

		private bool CheckAchievementData(AchievementInfo info)
		{
			if (info.EndDate < DateTime.Now)
			{
				return false;
			}
			if (info.NeedMaxLevel < m_player.Level)
			{
				return false;
			}
			if (info.IsOther == 1 && m_player.PlayerCharacter.ConsortiaID <= 0)
			{
				return false;
			}
			if (info.IsOther == 2 && m_player.PlayerCharacter.SpouseID <= 0)
			{
				return false;
			}
			if (info.PreAchievementID != "0,")
			{
				string[] tempArry = info.PreAchievementID.Split(',');
				for (int i = 0; i < tempArry.Length; i++)
				{
					if (!IsAchievementFinish(AchievementMgr.GetSingleAchievement(Convert.ToInt32(tempArry[i]))))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public bool CanCompleted(AchievementInfo achievementInfo)
		{
			int count = 0;
			List<AchievementConditionInfo> conditions = AchievementMgr.GetAchievementCondition(achievementInfo);
			if (conditions != null && conditions.Count > 0)
			{
				foreach (AchievementConditionInfo condition in conditions)
				{
					foreach (UsersRecordInfo userRecord in m_userRecord)
					{
						if (condition.CondictionType == userRecord.RecordID && condition.Condiction_Para2 <= userRecord.Total)
						{
							count++;
						}
					}
				}
			}
			return count == conditions.Count;
		}

		public bool SendReward(AchievementInfo achievementInfo)
		{
			string msg = "";
			List<AchievementRewardInfo> achievementReward = AchievementMgr.GetAchievementReward(achievementInfo);
			new List<ItemInfo>();
			new List<ItemInfo>();
			foreach (AchievementRewardInfo reward in achievementReward)
			{
				if (reward.RewardType == 1)
				{
					m_player.Rank.AddAchievementRank(reward.RewardPara);
				}
			}
			if (achievementInfo.AchievementPoint != 0)
			{
				m_player.AddAchievementPoint(achievementInfo.AchievementPoint);
				msg = msg + LanguageMgr.GetTranslation("Game.Server.Achievement.FinishAchievement.AchievementPoint", achievementInfo.AchievementPoint) + " ";
			}
			return true;
		}

		public AchievementInfo FindAchievement(int id)
		{
			foreach (KeyValuePair<int, AchievementInfo> info in AchievementMgr.Achievement)
			{
				if (info.Value.ID == id)
				{
					return info.Value;
				}
			}
			return null;
		}

		public bool AddAchievementData(AchievementInfo achievementInfo)
		{
			if (!IsAchievementFinish(achievementInfo))
			{
				AchievementDataInfo achievementData = new AchievementDataInfo();
				achievementData.UserID = m_player.PlayerId;
				achievementData.AchievementID = achievementInfo.ID;
				achievementData.IsComplete = true;
				achievementData.CompletedDate = DateTime.Now;
				achievementData.IsDirty = true;
				lock (m_data)
				{
					m_data.Add(achievementData);
				}
				return true;
			}
			return false;
		}

		private bool IsAchievementFinish(AchievementInfo achievementInfo)
		{
			IEnumerable<AchievementDataInfo> data = m_data.Where((AchievementDataInfo s) => s.AchievementID == achievementInfo.ID);
			if (data != null)
			{
				return data.ToList().Count > 0;
			}
			return false;
		}

		public void SaveToDatabase()
		{
			if (m_userRecord != null && m_userRecord.Count > 0)
			{
				lock (m_userRecord)
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						foreach (UsersRecordInfo userRecord in m_userRecord)
						{
							if (userRecord.IsDirty)
							{
								playerBussiness.UpdateDbUserRecord(userRecord);
							}
						}
					}
				}
			}
			if (m_data == null || m_data.Count <= 0)
			{
				return;
			}
			lock (m_data)
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					foreach (AchievementDataInfo achievementData in m_data)
					{
						if (achievementData.IsDirty)
						{
							db.UpdateDbAchievementDataInfo(achievementData);
						}
					}
				}
			}
		}
	}
}
