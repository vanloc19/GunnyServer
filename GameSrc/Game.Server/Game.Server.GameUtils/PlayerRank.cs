//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using Bussiness;
//using Game.Server.GameObjects;
//using Game.Server.Managers;
//using Game.Server.Packets;
//using log4net;
//using SqlDataProvider.Data;

//namespace Game.Server.GameUtils
//{
//	public class PlayerRank
//	{
//		private static readonly ILog log;

//		private UserRankInfo m_currentRank;

//		protected object m_lock = new object();

//		protected GamePlayer m_player;

//		private List<UserRankInfo> m_rank;

//		private bool m_saveToDb;

//		public UserRankInfo CurrentRank
//		{
//			get
//			{
//				return m_currentRank;
//			}
//			set
//			{
//				m_currentRank = value;
//			}
//		}

//		public GamePlayer Player => m_player;

//		public List<UserRankInfo> Ranks
//		{
//			get
//			{
//				return m_rank;
//			}
//			set
//			{
//				m_rank = value;
//			}
//		}

//		public PlayerRank(GamePlayer player, bool saveTodb)
//		{
//			m_player = player;
//			m_saveToDb = saveTodb;
//			m_rank = new List<UserRankInfo>();
//			m_currentRank = GetRank(m_player.PlayerCharacter.Honor);
//		}

//		public void AddRank(UserRankInfo info)
//		{
//			lock (m_rank)
//			{
//				m_rank.Add(info);
//			}
//		}

//		public void AddRank(string honor)
//		{
//			UserRankInfo info = new UserRankInfo
//			{
//				ID = 0,
//				UserID = m_player.PlayerCharacter.ID,
//				UserRank = honor,
//				Attack = 0,
//				Defence = 0,
//				Luck = 0,
//				Agility = 0,
//				HP = 0,
//				Damage = 0,
//				Guard = 0,
//				BeginDate = DateTime.Now,
//				Validate = 0,
//				IsExit = true
//			};
//			AddRank(info);
//		}

//		public void CreateRank(int UserID)
//		{
//			new List<UserRankInfo>();
//			UserRankInfo info = new UserRankInfo
//			{
//				ID = 0,
//				UserID = UserID,
//				UserRank = "Gunny Member",
//				Attack = 0,
//				Defence = 0,
//				Luck = 0,
//				Agility = 0,
//				HP = 0,
//				Damage = 0,
//				Guard = 0,
//				BeginDate = DateTime.Now,
//				Validate = 0,
//				IsExit = true
//			};
//			AddRank(info);
//		}

//		public List<UserRankInfo> GetRank()
//		{
//			List<UserRankInfo> list = new List<UserRankInfo>();
//			foreach (UserRankInfo info in m_rank)
//			{
//				if (info.IsExit)
//				{
//					list.Add(info);
//				}
//			}
//			return list;
//		}

//		public bool UpdateCurrentRank()
//		{
//			lock (this.m_lock)
//			{
//				this.m_currentRank = this.GetSingleRank();
//			}
//			return this.CurrentRank != null;
//		}

//		public UserRankInfo GetSingleRank()
//		{
//			UserRankInfo rankByHonnor = this.GetRankByHonnor(this.m_player.PlayerCharacter.Honor);
//			if (rankByHonnor == null || rankByHonnor.IsValidRank())
//			{
//				return rankByHonnor;
//			}
//			this.m_player.PlayerCharacter.Honor = "";
//			this.RemoveRank(rankByHonnor);
//			return null;
//		}

//		public UserRankInfo GetRankByHonnor(string honor)
//		{
//			UserRankInfo userRankInfo;
//			List<UserRankInfo>.Enumerator enumerator = this.m_rank.GetEnumerator();
//			try
//			{
//				while (enumerator.MoveNext())
//				{
//					UserRankInfo current = enumerator.Current;
//					if (current.UserRank != honor)
//					{
//						continue;
//					}
//					userRankInfo = current;
//					return userRankInfo;
//				}
//				return null;
//			}
//			finally
//			{
//				((IDisposable)enumerator).Dispose();
//			}
//			return userRankInfo;
//		}

//		public UserRankInfo GetRank(string honor)
//		{
//			foreach (UserRankInfo info in m_rank)
//			{
//				if (info.UserRank == honor)
//				{
//					return info;
//				}
//			}
//			return null;
//		}

//		public bool IsRank(string honor)
//		{
//			foreach (UserRankInfo item in m_rank)
//			{
//				if (item.UserRank == honor)
//				{
//					return true;
//				}
//			}
//			return false;
//		}

//		public virtual void LoadFromDatabase()
//		{
//			if (!m_saveToDb)
//			{
//				return;
//			}
//			using (PlayerBussiness bussiness = new PlayerBussiness())
//			{
//				List<UserRankInfo> singleUserRank = bussiness.GetSingleUserRank(Player.PlayerCharacter.ID);
//				if (singleUserRank.Count == 0)
//				{
//					CreateRank(Player.PlayerCharacter.ID);
//					return;
//				}
//				foreach (UserRankInfo info in singleUserRank)
//				{
//					if (info.IsValidRank())
//					{
//						AddRank(info);
//					}
//					else
//					{
//						RemoveRank(info);
//					}
//				}
//			}
//		}

//		public void RemoveRank(UserRankInfo item)
//		{
//			item.IsExit = false;
//			AddRank(item);
//		}

//		public virtual void SaveToDatabase()
//		{
//			if (!m_saveToDb)
//			{
//				return;
//			}
//			using (PlayerBussiness bussiness = new PlayerBussiness())
//			{
//				lock (m_lock)
//				{
//					for (int i = 0; i < m_rank.Count; i++)
//					{
//						UserRankInfo item = m_rank[i];
//						if (item != null && item.IsDirty)
//						{
//							if (item.ID > 0)
//							{
//								bussiness.UpdateUserRank(item);
//							}
//							else
//							{
//								bussiness.AddUserRank(item);
//							}
//						}
//					}
//				}
//			}
//		}

//		public void AddRankNew(GameClient client, string rank, int Attack, int Defence, int Agility, int Luck, int HP, int Guard, int Damage, int Validate)
//		{
//			client.Player.Rank.AddRank(new UserRankInfo()
//			{
//				Attack = Attack,
//				Defence = Defence,
//				Agility = Agility,
//				Luck = Luck,
//				HP = HP,
//				UserRank = rank,
//				Guard = Guard,
//				Damage = Damage,
//				IsExit = true,
//				Validate = Validate,
//				UserID = client.Player.PlayerCharacter.ID,
//				BeginDate = DateTime.Now
//			});
//			this.m_player.Out.SendUserRanks(this.m_rank);
//			client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Bạn nhận được danh hiệu \"{0}\"", (object)rank));
//			WorldMgr.SendSysNotice(string.Format("[{0}] mở được danh hiệu \"{1}\"", (object)client.Player.PlayerCharacter.NickName, (object)rank));
//		}

//		static PlayerRank()
//		{
//			log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//		}
//	}
//}

using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
    public class PlayerRank
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected object m_lock = new object();

        protected GamePlayer m_player;

        private List<UserRankInfo> m_rank;

        private List<UserRankInfo> m_removeRank;

        private UserRankInfo m_currentRank;

        private bool m_saveToDb;

        public GamePlayer Player
        {
            get { return m_player; }
        }

        public UserRankInfo CurrentRank
        {
            get { return m_currentRank; }
        }

        public List<UserRankInfo> ListRank
        {
            get { return m_rank; }
        }

        public PlayerRank(GamePlayer player, bool saveToDb)
        {
            m_player = player;
            m_saveToDb = saveToDb;
            m_rank = new List<UserRankInfo>();
            m_removeRank = new List<UserRankInfo>();
        }

        public virtual void LoadFromDatabase()
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    try
                    {
                        List<UserRankInfo> lists = pb.GetSingleUserRank(Player.PlayerCharacter.ID);
                        if (lists.Count == 0)
                        {
                            CreateRank(Player.PlayerCharacter.ID);
                        }
                        else
                        {
                            foreach (UserRankInfo info in lists)
                            {
                                if (info.IsValidRank())
                                {
                                    AddRank(info);
                                }
                                else
                                {
                                    RemoveRank(info);
                                }
                            }
                        }
                    }
                    finally
                    {
                        UserRankDateInfo rank = RankMgr.FindRankDate(Player.PlayerCharacter.ID);
                        if (rank != null)
                        {
                            if (rank.FightPower == 1)
                            {
                                AddNewRank(602, 1);
                            }
                        }
                        UpdateCurrentRank();
                    }
                }
            }
        }

        public void SendUserRanks()
        {
            Player.Out.SendUserRanks(Player.PlayerCharacter.ID, m_rank);
        }

        public bool UpdateCurrentRank()
        {
            lock (m_lock)
            {
                m_currentRank = GetSingleRank();
            }
            return CurrentRank != null;
        }

        public void AddNewRank(int id)
        {
            AddNewRank(id, 0);
        }

        public void AddNewRank(int id, int days)
        {
            NewTitleInfo newTile = NewTitleMgr.FindNewTitle(id);
            if (newTile != null)
            {
                UserRankInfo temp = GetRankByHonnor(id);
                if (temp == null)
                {
                    UserRankInfo item = new UserRankInfo();
                    item.Info = newTile;
                    item.Name = newTile.Name;
                    item.NewTitleID = newTile.ID;
                    item.UserID = Player.PlayerCharacter.ID;
                    item.BeginDate = DateTime.Now;
                    item.EndDate = DateTime.Now.AddDays((double)days);
                    item.Validate = days;
                    item.IsExit = true;
                    AddRank(item);
                }
                else
                {
                    temp.IsExit = true;
                    if (days == 0)
                    {
                        temp.Validate = 0;
                        temp.EndDate = temp.EndDate.AddYears(1);
                    }
                    else if (days > 0 && temp.Validate != 0)
                    {
                        temp.Validate += days;
                        Console.WriteLine($"Before: {temp.EndDate}");
                        temp.EndDate = temp.EndDate.AddDays(days);
                        Console.WriteLine($"After: {temp.EndDate}");
                    }    
                    /*Console.WriteLine($"Before: {temp.EndDate} = {temp.EndDate.AddDays(2)}");
                    //temp.EndDate = temp.EndDate.AddDays(2);//DateTime.Now.AddDays((double)days);
                    
                    Console.WriteLine($"After: {temp.EndDate} = {temp.EndDate.AddDays(2)}");
                    Console.WriteLine($"EndDate = {temp.EndDate}");*/
                       
                    
                }
                SendUserRanks();
            }
        }

        public void AddAchievementRank(string name)
        {
            NewTitleInfo newTile = NewTitleMgr.FindNewTitleByName(name);
            if (newTile != null)
            {
                UserRankInfo temp = GetSingleRank(name);
                if (temp == null)
                {
                    UserRankInfo item = new UserRankInfo();
                    item.Info = newTile;
                    item.Name = newTile.Name;
                    item.NewTitleID = newTile.ID;
                    item.UserID = Player.PlayerCharacter.ID;
                    item.BeginDate = DateTime.Now;
                    item.EndDate = DateTime.Now;
                    item.Validate = 0;
                    item.IsExit = true;
                    AddRank(item);
                }
                else
                {
                    temp.IsExit = true;
                    temp.EndDate = DateTime.Now;
                }

                SendUserRanks();
            }
        }

        public void AddRank(UserRankInfo rank)
        {
            lock(m_rank)
            {
                NewTitleInfo newTile = NewTitleMgr.FindNewTitle(rank.NewTitleID);
                if (newTile != null)
                {
                    rank.Info = newTile;
                }
                m_rank.Add(rank);
            }
        }

        public void RemoveRank(UserRankInfo item)
        {
            bool result = false;
            lock (m_rank)
            {
                result = m_rank.Remove(item);
            }
            if (result)
            {
                item.IsExit = false;
                lock (m_removeRank)
                {
                    m_removeRank.Add(item);
                }
            }
        }

        public UserRankInfo GetSingleRank()
        {
            UserRankInfo info = GetRankByHonnor(m_player.PlayerCharacter.honorId);
            if (info != null && !info.IsValidRank())
            {
                m_player.PlayerCharacter.honorId = 0;
                m_player.PlayerCharacter.Honor = "";
                RemoveRank(info);
                return null;
            }
            return info;
        }

        public UserRankInfo GetSingleRank(string honor)
        {
            foreach (UserRankInfo info in m_rank)
            {
                if (info.Name.Contains(honor) && info.IsValidRank())
                {
                    return info;
                }
            }
            return null;
        }

        public UserRankInfo GetRankByHonnor(int honor)
        {
            foreach (UserRankInfo info in m_rank)
            {
                if (info.NewTitleID == honor)
                {
                    return info;
                }
            }
            return null;
        }

        public void CreateRank(int UserID)
        {
            List<UserRankInfo> infos = new List<UserRankInfo>();
            NewTitleInfo newTile = NewTitleMgr.FindNewTitle(744);
            if (newTile != null)
            {
                UserRankInfo item = new UserRankInfo();
                item.Info = newTile;
                item.ID = 0;
                item.UserID = UserID;
                item.NewTitleID = newTile.ID;
                item.Name = newTile.Name;
                item.BeginDate = DateTime.Now;
                item.EndDate = DateTime.Now;
                item.Validate = 0;
                item.IsExit = true;
                AddRank(item);
            }
        }

        public virtual void SaveToDatabase()
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    lock (m_lock)
                    {
                        foreach (UserRankInfo item in m_rank)
                        {
                            if (item != null && item.IsDirty)
                            {
                                if (item.ID > 0)
                                {
                                    pb.UpdateUserRank(item);
                                }
                                else
                                {
                                    pb.AddUserRank(item);
                                }
                            }
                        }

                        foreach (UserRankInfo item in m_removeRank)
                        {
                            pb.UpdateUserRank(item);
                        }

                        m_removeRank.Clear();
                    }
                }
            }
        }
    }
}