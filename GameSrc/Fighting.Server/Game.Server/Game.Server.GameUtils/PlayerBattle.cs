using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PlayerBattle
	{
		public readonly int Agility = 1600;

		public readonly int Attack = 1700;

		public readonly int Blood = 25000;

		public readonly int Damage = 1000;

		public readonly int Defend = 1500;

		public readonly int Energy = 293;

		public readonly int fairBattleDayPrestige = 2000;

		public readonly int Guard = 500;

		public readonly int LevelLimit = 15;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public readonly int Lucky = 1500;

		protected object m_lock = new object();

		private UserMatchInfo m_matchInfo;

		protected GamePlayer m_player;

		private bool m_saveToDb;

		public readonly int maxCount = 30;

		public UserMatchInfo MatchInfo
		{
			get
			{
				return m_matchInfo;
			}
			set
			{
				m_matchInfo = value;
			}
		}

		public GamePlayer Player => m_player;

		public PlayerBattle(GamePlayer player, bool saveTodb)
		{
			m_player = player;
			m_saveToDb = saveTodb;
		}

		public void AddPrestige(bool isWin)
		{
			FairBattleRewardInfo battleDataByPrestige = FairBattleRewardMgr.GetBattleDataByPrestige(m_matchInfo.totalPrestige);
			if (battleDataByPrestige == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg5"));
			}
			else
			{
				int prestigeForWin = battleDataByPrestige.PrestigeForWin;
				string translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg3", prestigeForWin);
				if (isWin)
				{
					m_matchInfo.dailyWinCount++;
				}
				if (!isWin)
				{
					prestigeForWin = battleDataByPrestige.PrestigeForLose;
					translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg4", prestigeForWin);
				}
				if (m_matchInfo.addDayPrestge < fairBattleDayPrestige)
				{
					m_matchInfo.addDayPrestge += prestigeForWin;
					m_matchInfo.totalPrestige += prestigeForWin;
				}
				Player.SendMessage(translation);
			}
			m_matchInfo.dailyGameCount++;
			m_matchInfo.weeklyGameCount++;
		}

		public void CreateInfo(int UserID)
		{
			m_matchInfo = new UserMatchInfo();
			m_matchInfo.ID = 0;
			m_matchInfo.UserID = UserID;
			m_matchInfo.dailyScore = 0;
			m_matchInfo.dailyWinCount = 0;
			m_matchInfo.dailyGameCount = 0;
			m_matchInfo.DailyLeagueFirst = true;
			m_matchInfo.DailyLeagueLastScore = 0;
			m_matchInfo.weeklyScore = 0;
			m_matchInfo.weeklyGameCount = 0;
			m_matchInfo.weeklyRanking = 0;
			m_matchInfo.addDayPrestge = 0;
			m_matchInfo.totalPrestige = 0;
			m_matchInfo.restCount = 30;
			m_matchInfo.leagueGrade = 0;
			m_matchInfo.leagueItemsGet = 0;
			m_matchInfo.maxCount = maxCount;
		}

		public int GetRank()
		{
			return RankMgr.FindRank(Player.PlayerCharacter.ID)?.rank ?? 0;
		}

		public virtual void LoadFromDatabase()
		{
			if (!m_saveToDb)
			{
				return;
			}
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				m_matchInfo = bussiness.GetSingleUserMatchInfo(Player.PlayerCharacter.ID);
				if (m_matchInfo == null)
				{
					CreateInfo(Player.PlayerCharacter.ID);
				}
				m_matchInfo.maxCount = maxCount;
			}
		}

		public void Reset()
		{
			m_matchInfo.dailyScore = 0;
			m_matchInfo.dailyWinCount = 0;
			m_matchInfo.dailyGameCount = 0;
			m_matchInfo.addDayPrestge = 0;
			m_matchInfo.restCount = 30;
			m_matchInfo.leagueItemsGet = 0;
			m_saveToDb = true;
			SaveToDatabase();
		}

		public virtual void SaveToDatabase()
		{
			if (!m_saveToDb)
			{
				return;
			}
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				lock (m_lock)
				{
					if (m_matchInfo != null && m_matchInfo.IsDirty)
					{
						if (m_matchInfo.ID > 0)
						{
							bussiness.UpdateUserMatchInfo(m_matchInfo);
						}
						else
						{
							bussiness.AddUserMatchInfo(m_matchInfo);
						}
					}
				}
			}
		}

		public void UpdateLeagueGrade()
		{
			if (Player.PlayerCharacter.Grade < 30)
			{
				Player.MatchInfo.leagueGrade = 20;
			}
			else if (Player.PlayerCharacter.Grade < 40)
			{
				Player.MatchInfo.leagueGrade = 30;
			}
			else if (Player.PlayerCharacter.Grade <= 50)
			{
				Player.MatchInfo.leagueGrade = 40;
			}
			m_saveToDb = true;
			SaveToDatabase();
		}

		public void Update()
		{
			if (this.m_matchInfo.restCount > 0)
			{
				this.m_matchInfo.restCount--;
				//this.Player.Out.SendLeagueNotice(this.Player.PlayerCharacter.ID, this.MatchInfo.restCount, this.maxCount, 3);
			}
		}
	}
}
