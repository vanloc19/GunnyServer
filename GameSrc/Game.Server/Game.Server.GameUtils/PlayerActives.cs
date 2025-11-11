using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Packets;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PlayerActives
	{
		private ThreadSafeRandom rand = new ThreadSafeRandom();

		private static readonly ILog ilog_0;

		protected object m_lock;

		protected Timer _labyrinthTimer;

		protected Timer _christmasTimer;

		protected GamePlayer m_player;

		private Random random_0;

		private bool bool_0;

		private int int_0;

		public GamePlayer Player => m_player;

		private PyramidConfigInfo m_pyramidConfig;

		public PyramidConfigInfo PyramidConfig
        {
			get { return m_pyramidConfig; }
            set { m_pyramidConfig = value; }
        }

		private PyramidInfo m_pyramid;

		public PyramidInfo Pyramid
		{
			get { return m_pyramid; }
			set { m_pyramid = value; }
		}

		public PlayerActives(GamePlayer player, bool saveTodb)
		{
			m_lock = new object();
			random_0 = new Random();
			int_0 = GameProperties.WarriorFamRaidTimeRemain;
			m_player = player;
			bool_0 = saveTodb;
			this.m_openCardPrice = GameProperties.ConvertStringArrayToIntArray("NewChickenOpenCardPrice");
			this.m_eagleEyePrice = GameProperties.ConvertStringArrayToIntArray("NewChickenEagleEyePrice");
			this.m_flushPrice = GameProperties.NewChickenFlushPrice;
			this.m_freeFlushTime = 120;
			this.m_freeRefreshBoxCount = 0;
			this.m_freeOpenCardCount = 0;
			this.m_freeEyeCount = 0;
			this.m_RemoveChickenBoxRewards = new System.Collections.Generic.List<NewChickenBoxItemInfo>();
			SetupPyramidConfig();
			m_listCards = new List<GodCardUser>();
			m_listExchanges = new List<GodCardGroupUser>();
			m_listAwards = new List<int>();
		}

		private List<GodCardUser> m_listCards;
		private List<GodCardGroupUser> m_listExchanges;
		private List<int> m_listAwards;
		public readonly int SXRowLength = 7;
		public readonly int SXColumnLength = 7;

		public List<GodCardUser> ListCards
		{
			get { return m_listCards; }
		}
		public List<GodCardGroupUser> ListExchanges
		{
			get { return m_listExchanges; }
		}
		public List<int> ListAwards
		{
			get { return m_listAwards; }
		}

		private UserChristmasInfo m_christmas;

		public UserChristmasInfo Christmas
		{
			get { return m_christmas; }
			set { m_christmas = value; }
		}

		private void method_0()
		{
			int num = 1000;
			if (_labyrinthTimer == null)
			{
				_labyrinthTimer = new Timer(LabyrinthCheck, null, num, num);
			}
			else
			{
				_labyrinthTimer.Change(num, num);
			}
		}

		protected void LabyrinthCheck(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				UpdateLabyrinthTime();
				Thread.CurrentThread.Priority = priority;
				_ = Environment.TickCount;
			}
			catch (Exception ex)
			{
				Console.WriteLine("LabyrinthCheck: " + ex);
			}
		}

		public void StopLabyrinthTimer()
		{
			if (_labyrinthTimer != null)
			{
				_labyrinthTimer.Change(-1, -1);
				_labyrinthTimer.Dispose();
				_labyrinthTimer = null;
			}
		}

		public void UpdateLabyrinthTime()
		{
			UserLabyrinthInfo labyrinth = Player.Labyrinth;
			labyrinth.isCleanOut = true;
			labyrinth.isInGame = true;
			if (labyrinth.remainTime > 0 && labyrinth.currentRemainTime > 0)
			{
				labyrinth.remainTime--;
				labyrinth.currentRemainTime--;
				int_0--;
			}
			if (int_0 == 0)
			{
				method_1();
				int_0 = 120;
				labyrinth.currentFloor++;
				if (labyrinth.currentFloor > labyrinth.myProgress)
				{
					labyrinth.currentFloor = labyrinth.myProgress;
					labyrinth.isCleanOut = false;
					labyrinth.isInGame = false;
					labyrinth.completeChallenge = false;
					labyrinth.remainTime = 0;
					labyrinth.currentRemainTime = 0;
					labyrinth.cleanOutAllTime = 0;
					StopLabyrinthTimer();
				}
			}
			Player.Out.SendLabyrinthUpdataInfo(Player.PlayerId, labyrinth);
		}

		public void CleantOutLabyrinth()
		{
			method_0();
		}

		private void method_1()
		{
			int index = m_player.Labyrinth.currentFloor - 1;
			int exp = m_player.CreateExps()[index];
			string labyrinthGold = m_player.labyrinthGolds[index];
			int count = int.Parse(labyrinthGold.Split('|')[0]);
			int int_3 = int.Parse(labyrinthGold.Split('|')[1]);
			if (m_player.PropBag.GetItemByTemplateID(0, 11916) == null || !m_player.RemoveTemplate(11916, 1))
			{
				m_player.Labyrinth.isDoubleAward = false;
			}
			if (m_player.Labyrinth.isDoubleAward)
			{
				exp *= 2;
				count *= 2;
				int_3 *= 2;
			}
			m_player.Labyrinth.accumulateExp += exp;
			List<ItemInfo> itemInfoList = new List<ItemInfo>();
			if (method_2())
			{
				itemInfoList = m_player.CopyDrop(2, 40002);
				m_player.AddTemplate(itemInfoList, count, eGameView.dungeonTypeGet);
				m_player.AddHardCurrency(int_3);
			}
			m_player.AddGP(exp, false, false);
			method_3(m_player.Labyrinth.currentFloor, exp, int_3, itemInfoList);
		}

		private bool method_2()
		{
			bool flag = false;
			for (int num = 0; num <= m_player.Labyrinth.myProgress; num += 2)
			{
				if (num == m_player.Labyrinth.currentFloor)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		private void method_3(int int_1, int int_2, int int_3, List<ItemInfo> ht0VOBWhZvOfbkW24XT)
		{
			if (ht0VOBWhZvOfbkW24XT == null)
			{
				ht0VOBWhZvOfbkW24XT = new List<ItemInfo>();
			}
			GSPacketIn pkg = new GSPacketIn(131, m_player.PlayerId);
			pkg.WriteByte(7);
			pkg.WriteInt(int_1);
			pkg.WriteInt(int_2);
			pkg.WriteInt(ht0VOBWhZvOfbkW24XT.Count);
			foreach (ItemInfo itemInfo in ht0VOBWhZvOfbkW24XT)
			{
				pkg.WriteInt(itemInfo.TemplateID);
				pkg.WriteInt(itemInfo.Count);
			}
			pkg.WriteInt(int_3);
			m_player.SendTCP(pkg);
		}

		public void StopCleantOutLabyrinth()
		{
			UserLabyrinthInfo labyrinth = Player.Labyrinth;
			labyrinth.isCleanOut = false;
			Player.Out.SendLabyrinthUpdataInfo(Player.PlayerId, labyrinth);
			StopLabyrinthTimer();
		}

		public void SpeededUpCleantOutLabyrinth()
		{
			UserLabyrinthInfo labyrinth = Player.Labyrinth;
			labyrinth.isCleanOut = false;
			labyrinth.isInGame = false;
			labyrinth.completeChallenge = false;
			labyrinth.remainTime = 0;
			labyrinth.currentRemainTime = 0;
			labyrinth.cleanOutAllTime = 0;
			for (int currentFloor = labyrinth.currentFloor; currentFloor <= labyrinth.myProgress; currentFloor++)
			{
				method_1();
				labyrinth.currentFloor++;
			}
			labyrinth.currentFloor = labyrinth.myProgress;
			Player.Out.SendLabyrinthUpdataInfo(Player.PlayerId, labyrinth);
			StopLabyrinthTimer();
		}
		private int[] m_openCardPrice;
		private int[] m_eagleEyePrice;
		private int m_freeOpenCardCount;
		private int m_freeEyeCount;
		private int m_flushPrice;
		private int m_freeFlushTime;
		private int m_freeRefreshBoxCount;
		private readonly int ChikenBoxCount = 18;

		private ActiveSystemInfo m_activeInfo;
		private NewChickenBoxItemInfo[] m_ChickenBoxRewards;
		private System.Collections.Generic.List<NewChickenBoxItemInfo> m_RemoveChickenBoxRewards;

		public NewChickenBoxItemInfo[] ChickenBoxRewards
        {
			get { return this.m_ChickenBoxRewards; }
			set { this.m_ChickenBoxRewards = value; }
        }

		public ActiveSystemInfo Info
        {
			get { return this.m_activeInfo; }
			set { this.m_activeInfo = value; }
        }

		public int flushPrice
        {
			get { return this.m_flushPrice; }
			set { this.m_flushPrice = value; }
        }

		public int freeEyeCount
        {
			get { return this.m_freeEyeCount; }
			set { this.m_freeEyeCount = value; }
        }

		public int freeOpenCardCount
        {
			get { return this.m_freeOpenCardCount; }
			set { this.m_freeOpenCardCount = value; }
        }

		public int freeFlushTime
        {
			get { return this.m_freeFlushTime; }
			set { this.m_freeFlushTime = value; }
        }

		public int freeRefreshBoxCount
        {
			get { return this.m_freeRefreshBoxCount; }
			set { this.freeRefreshBoxCount = value; }
        }

		public int[] openCardPrice
        {
			get { return this.m_openCardPrice; }
			set { this.m_openCardPrice = value; }
        }

		public int[] eagleEyePrice
        {
            get { return this.m_eagleEyePrice; }
			set { this.m_eagleEyePrice = value; }
        }
        #region ChickenBox
        public void EnterChickenBox()
		{
			if (this.m_ChickenBoxRewards == null)
			{
				this.LoadChickenBox();
			}
		}

		public void LoadChickenBox()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				this.m_ChickenBoxRewards = playerBussiness.GetSingleNewChickenBox(this.Player.PlayerCharacter.ID);
				if (this.m_ChickenBoxRewards.Length == 0)
				{
					this.PayFlushView();
				}
			}
		}

		public bool UpdateChickenBoxAward(NewChickenBoxItemInfo box)
		{
			bool result;
			for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
			{
				if (this.m_ChickenBoxRewards[i].Position == box.Position)
				{
					this.m_ChickenBoxRewards[i] = box;
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public NewChickenBoxItemInfo ViewAward(int pos)
		{
			NewChickenBoxItemInfo[] chickenBoxRewards = this.m_ChickenBoxRewards;
			NewChickenBoxItemInfo result;
			for (int i = 0; i < chickenBoxRewards.Length; i++)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = chickenBoxRewards[i];
				if (newChickenBoxItemInfo.Position == pos && !newChickenBoxItemInfo.IsSeeded)
				{
					result = newChickenBoxItemInfo;
					return result;
				}
			}
			result = null;
			return result;
		}

		public NewChickenBoxItemInfo GetAward(int pos)
		{
			NewChickenBoxItemInfo[] chickenBoxRewards = this.m_ChickenBoxRewards;
			NewChickenBoxItemInfo result;
			for (int i = 0; i < chickenBoxRewards.Length; i++)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = chickenBoxRewards[i];
				if (newChickenBoxItemInfo.Position == pos && !newChickenBoxItemInfo.IsSelected)
				{
					result = newChickenBoxItemInfo;
					return result;
				}
			}
			result = null;
			return result;
		}

		public void RandomPosition()
		{
			System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
			for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
			{
				list.Add(this.m_ChickenBoxRewards[i].Position);
			}
			this.rand.Shuffer<NewChickenBoxItemInfo>(this.m_ChickenBoxRewards);
			for (int j = 0; j < list.Count; j++)
			{
				this.m_ChickenBoxRewards[j].Position = list[j];
			}
		}

		public NewChickenBoxItemInfo[] CreateChickenBoxAward(int count, eEventType DataId)
		{
			System.Collections.Generic.List<NewChickenBoxItemInfo> list = new System.Collections.Generic.List<NewChickenBoxItemInfo>();
			System.Collections.Generic.Dictionary<int, NewChickenBoxItemInfo> dictionary = new System.Collections.Generic.Dictionary<int, NewChickenBoxItemInfo>();
			int num = 0;
			int num2 = 0;
			while (list.Count < count)
			{
				System.Collections.Generic.List<NewChickenBoxItemInfo> newChickenBoxAward = EventAwardMgr.GetNewChickenBoxAward(DataId);
				if (newChickenBoxAward.Count > 0)
				{
					NewChickenBoxItemInfo newChickenBoxItemInfo = newChickenBoxAward[0];
					if (!dictionary.Keys.Contains(newChickenBoxItemInfo.TemplateID))
					{
						dictionary.Add(newChickenBoxItemInfo.TemplateID, newChickenBoxItemInfo);
						newChickenBoxItemInfo.Position = num;
						list.Add(newChickenBoxItemInfo);
						num++;
					}
				}
				num2++;
			}
			return list.ToArray();
		}

		public void RemoveChickenBoxRewards()
		{
			for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = this.m_ChickenBoxRewards[i];
				if (newChickenBoxItemInfo != null && newChickenBoxItemInfo.ID > 0)
				{
					newChickenBoxItemInfo.Position = -1;
					this.m_RemoveChickenBoxRewards.Add(newChickenBoxItemInfo);
				}
			}
		}

		public void PayFlushView()
		{
			this.m_activeInfo.lastFlushTime = System.DateTime.Now;
			this.m_activeInfo.isShowAll = true;
			this.m_activeInfo.canOpenCounts = 5;
			this.m_activeInfo.canEagleEyeCounts = 5;
			this.RemoveChickenBoxRewards();
			this.m_ChickenBoxRewards = this.CreateChickenBoxAward(this.ChikenBoxCount, eEventType.CHICKEN_BOX);
			for (int i = 0; i < this.m_ChickenBoxRewards.Length; i++)
			{
				this.m_ChickenBoxRewards[i].UserID = this.Player.PlayerCharacter.ID;
			}
		}

		public bool IsFreeFlushTime()
		{
			System.DateTime lastFlushTime = this.Info.lastFlushTime;
			System.DateTime d = lastFlushTime.AddMinutes((double)this.freeFlushTime);
			System.TimeSpan timeSpan = System.DateTime.Now - this.Info.lastFlushTime;
			double num = (d - lastFlushTime).TotalMinutes - timeSpan.TotalMinutes;
			return num > 0.0;
		}

		public void SendChickenBoxItemList()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(87);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteDateTime(this.Info.lastFlushTime);
			gSPacketIn.WriteInt(this.freeFlushTime);
			gSPacketIn.WriteInt(this.freeRefreshBoxCount);
			gSPacketIn.WriteInt(this.freeEyeCount);
			gSPacketIn.WriteInt(this.freeOpenCardCount);
			gSPacketIn.WriteBoolean(this.Info.isShowAll);
			gSPacketIn.WriteInt(this.ChickenBoxRewards.Length);
			NewChickenBoxItemInfo[] chickenBoxRewards = this.ChickenBoxRewards;
			for (int i = 0; i < chickenBoxRewards.Length; i++)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = chickenBoxRewards[i];
				gSPacketIn.WriteInt(newChickenBoxItemInfo.TemplateID);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.StrengthenLevel);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.Count);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.ValidDate);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.AttackCompose);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.DefendCompose);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.AgilityCompose);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.LuckCompose);
				gSPacketIn.WriteInt(newChickenBoxItemInfo.Position);
				gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSelected);
				gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSeeded);
				gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsBinds);
			}
			this.m_player.SendTCP(gSPacketIn);
		}

		public bool IsChickenBoxOpen()
		{
			System.Convert.ToDateTime(GameProperties.NewChickenBeginTime);
			System.DateTime dateTime = System.Convert.ToDateTime(GameProperties.NewChickenEndTime);
			return System.DateTime.Now.Date < dateTime.Date;
		}


		public void SendEvent()
        {
			if (this.IsChickenBoxOpen())
			{
				this.m_player.Out.SendChickenBoxOpen(this.m_player.PlayerId, this.flushPrice, this.openCardPrice, this.eagleEyePrice);
			}
			if (GameMgr.GuildBattle.IsOpen && Player.OpenConsortiaBattleT7())
            {
				GameMgr.GuildBattle.SendSingleOpenClose(m_player);
            }
			SendCryptBossInitAllData();
			SendGodCardRaiseOpenClose();
			SendDDTQiYuanOpenClose();
			//SendSanXiaoOpenClose();
		}

		public void CreateActiveSystemInfo(int UserID, string name)
		{
			lock(this.m_lock)
            {
				this.m_activeInfo = new ActiveSystemInfo();
				this.m_activeInfo.ID = 0;
				this.m_activeInfo.UserID = UserID;
				this.m_activeInfo.canEagleEyeCounts = 5;
				this.m_activeInfo.canOpenCounts = 5;
				this.m_activeInfo.isShowAll = true;
				this.m_activeInfo.lastFlushTime = DateTime.Now;
				this.m_activeInfo.cardScore = 0;
				this.m_activeInfo.cardChipCount = 0;
				this.m_activeInfo.cardFreeCount = 0;
				this.m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
				this.m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
				this.m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);
				this.m_activeInfo.SXCrystal = 0;
				this.m_activeInfo.SXStepRemain = 5;
				this.m_activeInfo.SXScore = 0;
				this.m_activeInfo.SXMapInfoData = "";
				this.m_activeInfo.MiniShopBuyCount = "";
				this.m_activeInfo.SXRewardsGet = "";
				this.m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
				this.m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
				this.m_activeInfo.DamageNum = 0;
				this.m_activeInfo.lastEnterYearMonter = DateTime.Now;
				this.m_activeInfo.ChickActiveData = "0";
				CreateSXMap();
				SetupCryptBossOpenDay();
				CreateYearMonterBoxState();
			}
		}

		public virtual void LoadFromDatabase()
        {
			if(bool_0)
            {
				using (PlayerBussiness pb = new PlayerBussiness())
                {
					try
                    {
						if(IsChristmasOpen())
                        {
							m_christmas = pb.GetSingleUserChristmas(Player.PlayerCharacter.ID);
							if(m_christmas == null)
                            {
								CreateChristmasInfo(Player.PlayerCharacter.ID);
							}
						}
						m_activeInfo = pb.GetSingleActiveSystem(Player.PlayerCharacter.ID);
						if(m_activeInfo == null)
                        {
							CreateActiveSystemInfo(Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName);
                        }
						else
                        {
							if(string.IsNullOrEmpty(m_activeInfo.cardListCard))
                            {
								m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
								m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
								m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);
							}
                        }
                    }
					finally
                    {
						SetupCryptBossOpenDay();
						m_listCards = JsonConvert.DeserializeObject<List<GodCardUser>>(Info.cardListCard);
						m_listExchanges = JsonConvert.DeserializeObject<List<GodCardGroupUser>>(Info.cardListExchange);
						m_listAwards = JsonConvert.DeserializeObject<List<int>>(Info.cardListAward);
					}
                }
            }
            #region OLD
            /*Console.WriteLine("LoadFromDatabase()");
			if (bool_0)
            {
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
                {
					try
					{
						this.m_activeInfo = playerBussiness.GetSingleActiveSystem(this.Player.PlayerCharacter.ID);
						if (this.m_activeInfo == null)
						{
							this.CreateActiveSystemInfo(this.Player.PlayerCharacter.ID, this.Player.PlayerCharacter.NickName);
						}
						else
                        {
							lock(m_lock)
                            {
								if (string.IsNullOrEmpty(m_activeInfo.cardListCard))
								{
									m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
									m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
									m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);
								}
							}
                        }
					}
					catch
                    {
						//SetupCryptBossOpenDay();
						m_listCards = JsonConvert.DeserializeObject<List<GodCardUser>>(Info.cardListCard);
						m_listExchanges = JsonConvert.DeserializeObject<List<GodCardGroupUser>>(Info.cardListExchange);
						m_listAwards = JsonConvert.DeserializeObject<List<int>>(Info.cardListAward);
					}
				}
			}*/
            #endregion
        }

        public virtual void SaveToDatabase()
		{
			if(bool_0)
            {
				using (PlayerBussiness pb = new PlayerBussiness())
                {
					lock (m_lock)
                    {
						if (m_cryptBoss != null)
						{
							m_activeInfo.CryptBoss = JsonConvert.SerializeObject(m_cryptBoss);
						}

						if (m_activeInfo != null && m_activeInfo.IsDirty)
						{
							m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
							m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
							m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);

							if (m_activeInfo.ID > 0)
								pb.UpdateActiveSystem(m_activeInfo);
							else
								pb.AddActiveSystem(m_activeInfo);
						}

						if(m_pyramid != null && m_pyramid.IsDirty)
                        {
							if (m_pyramid.ID > 0)
								pb.UpdatePyramid(m_pyramid);
							else
								pb.AddPyramid(m_pyramid);
                        }

						if (m_christmas != null && m_christmas.IsDirty)
						{
							if (m_christmas.ID > 0)
								pb.UpdateUserChristmas(m_christmas);
							else
								pb.AddUserChristmas(m_christmas);
						}

						if (m_ChickenBoxRewards != null)
						{
							foreach (NewChickenBoxItemInfo box in m_ChickenBoxRewards)
							{
								if (box != null && box.IsDirty)
								{
									if (box.ID > 0)
										pb.UpdateNewChickenBox(box);
									else
										pb.AddNewChickenBox(box);
								}
							}
						}
					}

					if (m_RemoveChickenBoxRewards.Count > 0)
					{
						foreach (NewChickenBoxItemInfo box in m_RemoveChickenBoxRewards)
						{
							pb.UpdateNewChickenBox(box);
						}
					}
				}
			}
			//???
            #region OLD
            //if (bool_0)
            //{
            //	using (PlayerBussiness playerBussiness = new PlayerBussiness())
            //	{
            //		if (m_cryptBoss != null)
            //		{
            //			m_activeInfo.CryptBoss = JsonConvert.SerializeObject(m_cryptBoss);
            //		}
            //		if (this.m_activeInfo != null && this.m_activeInfo.IsDirty)
            //		{
            //			m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
            //			m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
            //			m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);
            //			if (this.m_activeInfo.ID > 0)
            //			{
            //				playerBussiness.UpdateActiveSystem(this.m_activeInfo);
            //			}
            //			else
            //			{
            //				playerBussiness.AddActiveSystem(this.m_activeInfo);
            //			}
            //		}
            //		if (this.m_ChickenBoxRewards != null)
            //		{
            //			NewChickenBoxItemInfo[] chickenBoxRewards = this.m_ChickenBoxRewards;
            //			for (int i = 0; i < chickenBoxRewards.Length; i++)
            //			{
            //				NewChickenBoxItemInfo newChickenBoxItemInfo = chickenBoxRewards[i];
            //				if (newChickenBoxItemInfo != null && newChickenBoxItemInfo.IsDirty)
            //				{
            //					if (newChickenBoxItemInfo.ID > 0)
            //					{
            //						playerBussiness.UpdateNewChickenBox(newChickenBoxItemInfo);
            //					}
            //					else
            //					{
            //						playerBussiness.AddNewChickenBox(newChickenBoxItemInfo);
            //					}
            //				}
            //			}
            //		}
            //		if (this.m_RemoveChickenBoxRewards.Count > 0)
            //		{
            //			foreach (NewChickenBoxItemInfo current in this.m_RemoveChickenBoxRewards)
            //			{
            //				playerBussiness.UpdateNewChickenBox(current);
            //			}
            //		}
            //	}
            //}
            #endregion
        }
        #endregion
        #region GodCard
        public bool CheckCardExchangesCount(GodCardGroupDetailInfo[] groups)
		{
			foreach (GodCardGroupDetailInfo group in groups)
			{
				if (FindCardCount(group.CardID) <= 0)
				{
					return false;
				}
			}

			return true;
		}

		public int SaveListCard(int Id, int count)
		{
			int currCount = 0;
			lock (m_lock)
			{
				for (int i = 0; i < ListCards.Count; i++)
				{
					if (m_listCards[i].CardId == Id)
					{
						m_listCards[i].Count += count;
						currCount = m_listCards[i].Count;
					}
				}

				if (currCount == 0)
				{
					m_listCards.Add(new GodCardUser { CardId = Id, Count = count });
				}
			}

			return currCount;
		}

		public int RemoveListCard(int Id, int count)
		{
			lock (m_lock)
			{
				for (int i = 0; i < ListCards.Count; i++)
				{
					if (m_listCards[i].CardId == Id && m_listCards[i].Count > 0 && m_listCards[i].Count >= count)
					{
						m_listCards[i].Count -= count;
						return m_listCards[i].Count;
					}
				}
			}

			return count;
		}

		public int FindCardCount(int Id)
		{
			for (int i = 0; i < ListCards.Count; i++)
			{
				if (ListCards[i].CardId == Id)
				{
					return ListCards[i].Count;
				}
			}

			return 0;
		}

		public int SaveListCardExchanges(int Id, int count)
		{
			int currCount = 0;
			lock (m_lock)
			{
				for (int i = 0; i < ListExchanges.Count; i++)
				{
					if (m_listExchanges[i].GroupId == Id)
					{
						m_listExchanges[i].Count += count;
						currCount = m_listExchanges[i].Count;
					}
				}

				if (currCount == 0)
				{
					m_listExchanges.Add(new GodCardGroupUser { GroupId = Id, Count = 1 });
				}
			}

			return currCount;
		}

		public int FindCardExchangesCount(int Id)
		{
			for (int i = 0; i < ListExchanges.Count; i++)
			{
				if (ListExchanges[i].GroupId == Id)
				{
					return ListExchanges[i].Count;
				}
			}

			return 0;
		}

		public void ResetGodCard()
		{
			lock (m_lock)
			{
				if (DateTime.Now.Date > Convert.ToDateTime(GameProperties.SanXiaoEndTime).Date.AddDays(1))
				{
					Random rd = new Random();
					m_activeInfo.cardScore = 0;
					m_activeInfo.cardFreeCount = 0;
					m_activeInfo.cardChipCount = 0;
					m_activeInfo.cardListCard = "";
					m_activeInfo.cardListAward = "";
					m_activeInfo.cardListExchange = "";
				}
				else
				{
					m_activeInfo.cardFreeCount = 0;
				}
			}
		}

		public void SendGodCardRaiseOpenClose()
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)GodCardRaisePackageType.IS_OPEN);
			pkg.WriteBoolean(GlobalConstants.IsOpenActive(eActive.GodCardRaise));
			pkg.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
			Player.SendTCP(pkg);
		}

		public void SendDDTQiYuanOpenClose()
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)QiYuanPackageType.PACK_TYPE_PUSH_OPEN_CLOSE);
			pkg.WriteBoolean(GlobalConstants.IsOpenActive(eActive.QiYuan));
			pkg.WriteDateTime(Convert.ToDateTime(GameProperties.QiYuanEndTime));
			Player.SendTCP(pkg);
		}
		#endregion
		#region Sanxiao

		public void SendSXGainedDropItems(List<ItemInfo> listItems)
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)SanXiaoPackageType.DROP_OUT_GAINED_ITEM);
			pkg.WriteInt(listItems.Count);
			foreach (ItemInfo item in listItems)
			{
				pkg.WriteInt(item.TemplateID);
				pkg.WriteInt(item.Count);
			}

			m_player.SendTCP(pkg);
		}

		public void SendSXStoreData(Dictionary<int, int> datas)
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)SanXiaoPackageType.STORE_DATA);
			pkg.WriteInt(m_activeInfo.SXCrystal);
			pkg.WriteInt(datas.Count);
			foreach (KeyValuePair<int, int> d in datas)
			{
				pkg.WriteInt(d.Key);
				pkg.WriteInt(d.Value);
			}

			m_player.SendTCP(pkg);
		}

		public void SendSXRewardsData(List<int> datas)
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)SanXiaoPackageType.REWARDS_DATA);
			pkg.WriteInt(datas.Count);
			foreach (int i in datas)
			{
				pkg.WriteInt(i);
			}

			m_player.SendTCP(pkg);
		}

		public void SendSXBuyTime()
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)SanXiaoPackageType.BUY_TIMES);
			pkg.WriteInt(m_activeInfo.SXStepRemain); // bước đi
			m_player.SendTCP(pkg);
		}

		public void SendSanXiaoOpenClose()
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)SanXiaoPackageType.IS_OPEN);
			pkg.WriteBoolean(false);
			pkg.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
			pkg.WriteInt(0); //_discounts = _loc2_.readInt();
			Player.SendTCP(pkg);
		}

		public void SendSXData()
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
			pkg.WriteByte((byte)SanXiaoPackageType.DATA);
			pkg.WriteInt(m_activeInfo.SXScore); // điểm
			pkg.WriteInt(m_activeInfo.SXStepRemain); // bước đi
			pkg.WriteInt(m_activeInfo.SXCrystal); // thủy tinh
			pkg.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
			m_player.SendTCP(pkg);
		}

		public void SendSXRequireMap()
        {
			Dictionary<int, Dictionary<int, SXCellDataInfo>> mapInfos = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(m_activeInfo.SXMapInfoData);

			if (mapInfos == null)
				mapInfos = CreateSXMap();

			GSPacketIn pkg = new GSPacketIn((int)ePackageType.SAN_XIAO, m_player.PlayerId);
			pkg.WriteByte((byte)SanXiaoPackageType.REQUIRE_MAP);
			pkg.WriteInt(SXRowLength * SXColumnLength);
			for (int i = 0; i < SXRowLength; i++)
			{
				for (int j = 0; j < SXColumnLength; j++)
				{
					pkg.WriteInt(i);
					pkg.WriteInt(j);
					pkg.WriteInt(mapInfos[i][j].type);
				}
			}

			m_player.SendTCP(pkg);
		}

		private Dictionary<int, Dictionary<int, SXCellDataInfo>> InitSXMap()
		{
			Dictionary<int, Dictionary<int, SXCellDataInfo>> mapInfos =
				new Dictionary<int, Dictionary<int, SXCellDataInfo>>();
			for (int i = 0; i < SXRowLength; i++)
			{
				Dictionary<int, SXCellDataInfo> cell = new Dictionary<int, SXCellDataInfo>();

				mapInfos.Add(i, cell);

				for (int j = 0; j < SXColumnLength; j++)
				{
					mapInfos[i].Add(j, new SXCellDataInfo(i, j));
				}
			}

			return mapInfos;
		}

		public Dictionary<int, Dictionary<int, SXCellDataInfo>> CreateSXMap()
		{
			Dictionary<int, Dictionary<int, SXCellDataInfo>> mapInfos =
				JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(m_activeInfo
					.SXMapInfoData);
			if (mapInfos == null)
				mapInfos = InitSXMap();

			for (int i = 0; i < SXRowLength; i++)
			{
				for (int j = 0; j < SXColumnLength; j++)
				{
					int type = rand.Next(1, 6);
					int max = mapInfos[Math.Max(0, i - 1)][j].type;
					if (max == type)
						type = (max + 2) % 5 + 1;

					max = mapInfos[i][Math.Max(0, j - 2)].type;
					if (max == type)
						type = (type + 2) % 5 + 1;

					mapInfos[i][j].type = type;
				}
			}

			m_activeInfo.SXMapInfoData = JsonConvert.SerializeObject(mapInfos);

			return mapInfos;
		}

		public void ResetSanXiao()
        {
			lock(m_lock)
            {
				this.m_activeInfo.SXStepRemain = 5;
            }
        }
		#endregion

		#region CryptBoss

		private CryptBossItemInfo[] m_cryptBoss;

		private readonly string[] cryptBossOpenDay = GameProperties.CryptBossOpenDay.Split('|');

		public CryptBossItemInfo[] CryptBoss
		{
			get { return m_cryptBoss; }
        }

		public CryptBossItemInfo GetCryptBossData(int id)
        {
			lock (m_lock)
            {
				for (int i = 0; i < m_cryptBoss.Length; i++)
                {
					if (m_cryptBoss[i].id == id)
                    {
						return m_cryptBoss[i];
                    }
                }
            }
			return null;
        }
		public GSPacketIn SendCryptBossInitAllData()
        {
			GSPacketIn pkg = new GSPacketIn(275, m_player.PlayerId);
			pkg.WriteByte(1);
			pkg.WriteInt(CryptBoss.Length);
			foreach(CryptBossItemInfo info in CryptBoss)
            {
				pkg.WriteInt(info.id);
				pkg.WriteInt(info.star);
				pkg.WriteInt(info.state);
            }
			m_player.SendTCP(pkg);
			return pkg;
        }

		private void SetupCryptBossOpenDay()
        {
			List<CryptBossItemInfo> deserialize = new List<CryptBossItemInfo>();
			if (string.IsNullOrEmpty(m_activeInfo.CryptBoss))
            {
				for (int i = 0; i < cryptBossOpenDay.Length; i++)
                {
					string[] days = cryptBossOpenDay[i].Split(',');
					CryptBossItemInfo itemInfo = new CryptBossItemInfo();
					itemInfo.id = int.Parse(days[0]);
					itemInfo.star = 0;
					itemInfo.state = GetState(days);
					deserialize.Add(itemInfo);
				}

			}
			else
            {
				deserialize = JsonConvert.DeserializeObject<List<CryptBossItemInfo>>(m_activeInfo.CryptBoss);
			}
			lock(m_lock)
            {
				m_cryptBoss = deserialize.ToArray();
			}
        }

		public void ResetCryptBossData()
        {
			if (m_cryptBoss != null)
			{
				lock (m_lock)
				{
					for (int i = 0; i < CryptBoss.Length; i++)
					{
						if (CryptBoss[i] == null)
							continue;
						int id = m_cryptBoss[i].id;
						string[] days = GetOpenDay(id);
						m_cryptBoss[i].state = GetState(days);
					}
				}
			}
		}

		private string[] GetOpenDay(int id)
		{
			foreach (string openday in cryptBossOpenDay)
			{
				string[] days = openday.Split(',');
				if (id.ToString() == days[0])
				{
					return days;
				}
			}

			return null;
		}

		private int GetState(string[] days)
		{
			if (days == null)
				return 2;
			for (int i = 1; i < days.Length; i++)
			{
				string day = days[i];
				if ((int)DateTime.Now.DayOfWeek == int.Parse(day))
				{
					return 1;
				}
			}

			return 2;
		}

		public void UpdateStar(int id)
		{
			lock (m_lock)
			{
				for (int i = 0; i < m_cryptBoss.Length; i++)
				{
					CryptBossItemInfo info = m_cryptBoss[i];
					if (info.id == id)
					{
						if (info.star < 5)
						{
							info.star++;
						}

						info.state = 2;
						SendUpdateSingleData(info);
						m_cryptBoss[i] = info;
						SendCryptBossAward(id, info.star, Player.ZoneId);
					}
				}
			}
		}

		public GSPacketIn SendUpdateSingleData(CryptBossItemInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(275, m_player.PlayerId);
			pkg.WriteByte(2);
			pkg.WriteInt(info.id); //_loc_6.id = param1.readInt();
			pkg.WriteInt(info.star); //_loc_6.star = param1.readInt();
			pkg.WriteInt(info.state); //_loc_6.state = param1.readInt();
			m_player.SendTCP(pkg);
			return pkg;
		}

		private readonly int[] m_cryptBossAward1 = new int[] { 1120186, 1120187, 1120188, 1120189, 1120190 };
		private readonly int[] m_cryptBossAward2 = new int[] { 1120191, 1120192, 1120193, 1120194, 1120195 };
		private readonly int[] m_cryptBossAward3 = new int[] { 1120196, 1120197, 1120198, 1120199, 1120200 };
		private readonly int[] m_cryptBossAward4 = new int[] { 1120201, 1120202, 1120203, 1120204, 1120205 };
		private readonly int[] m_cryptBossAward5 = new int[] { 1120206, 1120207, 1120208, 1120209, 1120210 };
		private readonly int[] m_cryptBossAward6 = new int[] { 1120211, 1120212, 1120213, 1120214, 1120215 };

		public void SendCryptBossAward(int id, int star, int zoneId)
		{
			int dataId = 0;
			switch (id)
			{
				case 1:
					if (star > 0 && star <= m_cryptBossAward1.Length)
						dataId = m_cryptBossAward1[star - 1];
					break;
				case 2:
					if (star > 0 && star <= m_cryptBossAward2.Length)
						dataId = m_cryptBossAward2[star - 1];
					break;
				case 3:
					if (star > 0 && star <= m_cryptBossAward3.Length)
						dataId = m_cryptBossAward3[star - 1];
					break;
				case 4:
					if (star > 0 && star <= m_cryptBossAward4.Length)
						dataId = m_cryptBossAward4[star - 1];
					break;
				case 5:
					if (star > 0 && star <= m_cryptBossAward5.Length)
						dataId = m_cryptBossAward5[star - 1];
					break;
				case 6:
					if (star > 0 && star <= m_cryptBossAward6.Length)
						dataId = m_cryptBossAward6[star - 1];
					break;
			}

			ItemTemplateInfo template = ItemMgr.FindItemTemplate(dataId);
			if (template != null)
			{
				List<ItemInfo> infos = new List<ItemInfo>();
				ItemInfo item = ItemInfo.CreateFromTemplate(template, 1, 102);
				item.Count = 1;
				item.ValidDate = 0;
				item.IsBinds = true;
				infos.Add(item);
				string title = LanguageMgr.GetTranslation(string.Format("Phần thưởng vượt địa ngục hạng {0} độ khó {1} sao. ", id, star));
				WorldEventMgr.SendItemsToMails(infos, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, GameServer.Instance.Configuration.ZoneId, null, title);
			}
		}
		#endregion

		public void ResetActive()
        {
			this.ResetCryptBossData();
			this.ResetGodCard();
			this.ResetSanXiao();
			this.ResetChristmas();
        }

		public bool YearMonterValidate()
        {
			if (Info.lastEnterYearMonter.Date < DateTime.Now.Date)
			{
				lock (m_lock)
				{
					m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
					m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
					m_activeInfo.lastEnterYearMonter = DateTime.Now;
					m_activeInfo.DamageNum = 0;
					CreateYearMonterBoxState();
				}
			}

			return true;
		}

		public void CreateYearMonterBoxState()
		{
			string[] yearMonsterBoxInfo = GameProperties.YearMonsterBoxInfo.Split('|');
			int length = yearMonsterBoxInfo.Length;
			string[] chArray = new string[length];
			for (int i = 0; i < length; i++)
			{
				int box = int.Parse(yearMonsterBoxInfo[i].Split(',')[1]) * 10000;
				if (box <= m_activeInfo.DamageNum)
				{
					chArray[i] = "2";
				}
				else
				{
					chArray[i] = "1";
				}
			}

			m_activeInfo.BoxState = string.Join("-", chArray);
		}

		public void SetYearMonterBoxState(int id)
		{
			string[] BoxState = m_activeInfo.BoxState.Split('-');
			int length = BoxState.Length;
			string[] chArray = new string[length];
			for (int i = 0; i < length; i++)
			{
				if (i == id)
				{
					chArray[i] = "3";
				}
				else
				{
					chArray[i] = BoxState[i];
				}
			}

			m_activeInfo.BoxState = string.Join("-", chArray);
		}


        public void SendQXAward(int QXDropId)
        {
            if (QXDropId <= 0)
                return;
            int templateId = 0;
            switch (QXDropId)
            {
                case 70001:
                    templateId = 112308;
                    break;
                case 70002:
                    templateId = 112309;
                    break;
                case 70003:
                    templateId = 112310;
                    break;
                case 70006:
                    templateId = 112311;
                    break;
                case 70007:
                    templateId = 112312;
                    break;
                case 70008:
                    templateId = 112313;
                    break;
                case 70009:
                    templateId = 112314;
                    break;
                case 70010:
                    templateId = 112365;
                    break;
                case 70011:
                    templateId = 112366;
                    break;
                case 70099:
                    templateId = 1120299;
                    break;
            }
            ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId);
            if (itemTemplate != null)
            {
                List<SqlDataProvider.Data.ItemInfo> infos = new List<SqlDataProvider.Data.ItemInfo>();
                SqlDataProvider.Data.ItemInfo fromTemplate = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(itemTemplate, 1, 102);
                fromTemplate.Count = 1;
                fromTemplate.ValidDate = 0;
                fromTemplate.IsBinds = true;
                infos.Add(fromTemplate);
                int num = QXDropId - 70000;
                if (QXDropId >= 70006)
                    num -= 2;
                string translation = LanguageMgr.GetTranslation("GamePlayer.Msg22", (object)num);
                WorldEventMgr.SendItemsToMails(infos, this.Player.PlayerCharacter.ID, this.Player.PlayerCharacter.NickName, this.Player.ZoneId, null, translation);
                QXDropId = 0;
            }
        }

        public bool IsYearMonsterOpen()
        {
			DateTime end = Convert.ToDateTime(GameProperties.YearMonsterEndDate);
			return DateTime.Now.Date < end.Date;
        }

		public bool IsChristmasOpen()
		{
			DateTime end = Convert.ToDateTime(GameProperties.ChristmasEndDate);
			return DateTime.Now.Date < end.Date;
		}

		public void ResetChristmas()
		{
			lock (m_lock)
			{
				if (m_christmas != null)
				{
					m_christmas.dayPacks = 0;
					m_christmas.AvailTime = 0;
					m_christmas.isEnter = false;
				}
			}
		}

		public void BeginChristmasTimer()
		{
			int interval = 1 * 60 * 1000;
			if (_christmasTimer == null)
			{
				_christmasTimer = new Timer(new TimerCallback(ChristmasTimeCheck), null, interval, interval);
			}
			else
			{
				_christmasTimer.Change(interval, interval);
			}
		}

		protected void ChristmasTimeCheck(object sender)
		{
			try
			{
				int startTick = Environment.TickCount;
				ThreadPriority oldprio = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				//some code  
				UpdateChristmasTime();
				//end code
				Thread.CurrentThread.Priority = oldprio;
				startTick = Environment.TickCount - startTick;
			}
			catch (Exception e)
			{
				Console.WriteLine("ChristmasTimeCheck: " + e);
			}
		}

		public void UpdateChristmasTime()
		{
			TimeSpan usedTime = DateTime.Now - Christmas.gameBeginTime;
			TimeSpan availTime = Christmas.gameEndTime - Christmas.gameBeginTime;
			double timeLeft = availTime.TotalMinutes - usedTime.TotalMinutes;
			lock (m_christmas)
			{
				m_christmas.AvailTime = (int)timeLeft < 0 ? 0 : (int)timeLeft;
			}
		}

		public void StopChristmasTimer()
		{
			if (_christmasTimer != null)
			{
				_christmasTimer.Dispose();
				_christmasTimer = null;
			}
		}

		public void AddTime(int min)
		{
			lock (m_christmas)
			{
				m_christmas.AvailTime += min;
			}
		}

		public void CreateChristmasInfo(int UserID)
		{
			lock (m_lock)
			{
				m_christmas = new UserChristmasInfo();
				m_christmas.ID = 0;
				m_christmas.UserID = UserID;
				m_christmas.count = 0;
				m_christmas.exp = 0;
				m_christmas.awardState = 0;
				m_christmas.lastPacks = 1100;
				m_christmas.packsNumber = -1;
				m_christmas.gameBeginTime = DateTime.Now;
				m_christmas.gameEndTime = DateTime.Now.AddMinutes(60);
				m_christmas.isEnter = false;
				m_christmas.dayPacks = 0;
				m_christmas.AvailTime = 0;
			}
		}

		public bool AvailTime()
		{
			DateTime gameBeginTime = Christmas.gameBeginTime;
			DateTime gameEndTime = Christmas.gameEndTime;
			TimeSpan usedTime = DateTime.Now - gameBeginTime;
			TimeSpan availTime = gameEndTime - gameBeginTime;
			double timeLeft = availTime.TotalMinutes - usedTime.TotalMinutes;
			return timeLeft > 0;
		}

		#region Code Ga Hanh
		public string DefaultChickActiveData()
		{
			return string.Concat(new object[]
			{
				"0,0,",
				DateTime.Now.ToString("yyyy/MM/dd"),
				",",
				DateTime.Now.ToString("yyyy/MM/dd"),
				",",
				DateTime.Now.ToString("yyyy/MM/dd"),
				",",
				DateTime.Now.ToString("yyyy/MM/dd"),
				",0"
			});
		}

		public bool SaveChickActiveData(UserChickActiveInfo data)
		{
			if (data != null)
			{
				string[] chickDataArr = new string[]
				{
					data.IsKeyOpened.ToString(),
					data.KeyOpenedType.ToString(),
					data.KeyOpenedTime.ToString("yyyy/MM/dd"),
					data.EveryDay.ToString("yyyy/MM/dd"),
					data.Weekly.ToString("yyyy/MM/dd"),
					data.AfterThreeDays.ToString("yyyy/MM/dd"),
					data.CurrentLvAward.ToString()
				};
				m_activeInfo.ChickActiveData = string.Join(",", chickDataArr);
				return true;
			}
			return false;
		}

		public void SendUpdateChickActivation()
		{
			UserChickActiveInfo chickInfo = GetChickActiveData();
			if (chickInfo != null)
			{
				GSPacketIn pkg = new GSPacketIn((byte)ePackageType.ACTIVITY_PACKAGE);
				pkg.WriteInt((int)ChickActivationType.CHICKACTIVATION);
				pkg.WriteInt((int)ChickActivationType.CHICKACTIVATION_UPDATE);
				pkg.WriteInt(chickInfo.IsKeyOpened);
				pkg.WriteInt(1);
				pkg.WriteDateTime(chickInfo.KeyOpenedTime);
				pkg.WriteInt(chickInfo.KeyOpenedType);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Monday)? 0: 1);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)? 0: 1);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)? 0: 1);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Thursday)? 0: 1);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Friday)? 0: 1);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)? 0: 1);
				pkg.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)? 0: 1);
				pkg.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now))? 0: 1);
				pkg.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now))? 0: 1);
				pkg.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now))? 0: 1);
				pkg.WriteInt((chickInfo.Weekly < chickInfo.StartOfWeek(DateTime.Now, DayOfWeek.Saturday) && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)? 0: 1);
				pkg.WriteInt(chickInfo.CurrentLvAward);
				m_player.SendTCP(pkg);
			}
		}

		public UserChickActiveInfo GetChickActiveData()
		{
			UserChickActiveInfo userChick = null;
			if (m_activeInfo.ChickActiveData == "0")
			{
				m_activeInfo.ChickActiveData = DefaultChickActiveData();
			}
			string[] chickDataArr = m_activeInfo.ChickActiveData.Split(',');
			if (chickDataArr.Length > 0)
			{
				userChick = new UserChickActiveInfo();
				userChick.IsKeyOpened = int.Parse(chickDataArr[0]);
				userChick.KeyOpenedType = int.Parse(chickDataArr[1]);
				userChick.KeyOpenedTime = DateTime.Parse(chickDataArr[2]);
				userChick.EveryDay = DateTime.Parse(chickDataArr[3]);
				userChick.Weekly = DateTime.Parse(chickDataArr[4]);
				userChick.AfterThreeDays = DateTime.Parse(chickDataArr[5]);
				userChick.CurrentLvAward = int.Parse(chickDataArr[6]);
				//1,2,3/17/2022 12:07:13 AM,3/17/2022,3/5/2022,3/16/2022,0

			}
			return userChick;
		}
        #endregion

        #region Pyramid
        public bool IsPyramidOpen()
		{
			DateTime end = Convert.ToDateTime(GameProperties.PyramidEndTime);
			return DateTime.Now.Date < end.Date;
		}

		private void SetupPyramidConfig()
		{
			lock (m_lock)
			{
				m_pyramidConfig = new PyramidConfigInfo();
				m_pyramidConfig.isOpen = IsPyramidOpen();
				m_pyramidConfig.isScoreExchange = !IsPyramidOpen();
				m_pyramidConfig.beginTime = Convert.ToDateTime(GameProperties.PyramidBeginTime);
				m_pyramidConfig.endTime = Convert.ToDateTime(GameProperties.PyramidEndTime);
				m_pyramidConfig.freeCount = 3;
				m_pyramidConfig.revivePrice = GameProperties.ConvertStringArrayToIntArray("PyramidRevivePrice");
				m_pyramidConfig.turnCardPrice = GameProperties.PyramydTurnCardPrice;
			}
		}

		public bool LoadPyramid()
		{
			if (m_pyramid == null)
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					m_pyramid = pb.GetSinglePyramid(Player.PlayerCharacter.ID);
					if (m_pyramid == null)
					{
						CreatePyramidInfo();
					}
				}
			}

			return true;
		}

		public void CreatePyramidInfo()
		{
			lock (m_lock)
			{
				m_pyramid = new PyramidInfo();
				m_pyramid.ID = 0;
				m_pyramid.UserID = Player.PlayerCharacter.ID;
				m_pyramid.currentLayer = 1;
				m_pyramid.maxLayer = 1;
				m_pyramid.totalPoint = 0;
				m_pyramid.turnPoint = 0;
				m_pyramid.pointRatio = 0;
				m_pyramid.currentFreeCount = 0;
				m_pyramid.currentReviveCount = 0;
				m_pyramid.isPyramidStart = false;
				m_pyramid.LayerItems = "";
				m_pyramid.currentCountNow = 0;
			}
		}

		#endregion

		static PlayerActives()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
