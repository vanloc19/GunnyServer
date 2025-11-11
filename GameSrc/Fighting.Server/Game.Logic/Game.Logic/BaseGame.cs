using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic
{
	public class BaseGame : AbstractGame
	{
		public delegate void GameNpcDieEventHandle(int NpcId);

		public delegate void GameOverLogEventHandle(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar);

		public int blueScore;

		public string BossWarField;

		public int[] Cards;

		public int ConsortiaAlly;

		public int CurrentActionCount;

		public int CurrentTurnTotalDamage;

		public TurnedLiving LastTurnLiving;

        public bool Is3d = false;

        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public readonly int[] EquipPlace;

		private long long_1;

		private ArrayList m_actions;

		protected TurnedLiving m_currentLiving;

		protected eGameState m_gameState;

		private bool m_GetBlood;

		private int m_lifeTime;

		protected List<Living> m_livings;

		private List<LoadingFileInfo> m_loadingFiles;

		public Map m_map;

		protected int m_nextPlayerId;

		protected int m_nextWind;

		public bool m_confineWind;

		private long m_passTick;

		protected Dictionary<int, Player> m_players;

		protected Random m_random;

        private int m_roomId;

		private List<Ball> m_tempBall;

		private List<Box> m_tempBox;

		private List<Point> m_tempPoints;

		private List<Point> m_tempGhostPoints;

		private List<TurnedLiving> m_turnQueue;

		protected Dictionary<int, string> m_logStartIps;

		private long m_waitTimer;

		public int PhysicalId;

		public int redScore;

		public int RichesRate;

		public int TotalCostGold;

		public int TotalCostMoney;

		public int TotalHurt;

        public int BloodBuff = 0;

        protected int turnIndex;

		public bool FrozenWind;

		protected List<Living> m_decklivings;

		public bool ConFineWind
		{
			get
			{
				return m_confineWind;
			}
			set
			{
				m_confineWind = value;
			}
		}

		public TurnedLiving CurrentLiving => m_currentLiving;

		public eGameState GameState => m_gameState;

		public bool GetBlood
		{
			get
			{
				return m_GetBlood;
			}
			set
			{
				m_GetBlood = value;
			}
		}

		public bool HasPlayer => m_players.Count > 0;

		public int LifeTime => m_lifeTime;

		protected int m_turnIndex
		{
			get
			{
				return turnIndex;
			}
			set
			{
				turnIndex = value;
			}
		}

		public Map Map => m_map;

		public int nextPlayerId
		{
			get
			{
				return m_nextPlayerId;
			}
			set
			{
				m_nextPlayerId = value;
			}
		}

		public int PlayerCount
		{
			get
			{
				lock (m_players)
				{
					return this.m_players.Where(p => !p.Value.IsQuanChien).Count();
				}
			}
		}

		private bool m_isWrong;
		public bool IsWrong
        {
			get
            {
				return m_isWrong;
            }
			set
            {
				m_isWrong = value;
            }
        }

		public Dictionary<int, Player> Players => m_players;

		public Random Random => m_random;

		public int RoomId => m_roomId;

		public int TurnIndex
		{
			get
			{
				return m_turnIndex;
			}
			set
			{
				m_turnIndex = value;
			}
		}

		public List<TurnedLiving> TurnQueue => m_turnQueue;

		public float Wind => m_map.wind;

		public event GameEventHandle BeginNewTurn;

		public event GameNpcDieEventHandle GameNpcDie;

		public event GameOverLogEventHandle GameOverLog;

		public event GameEventHandle GameOverred;

		public BaseGame(int id, int roomId, Map map, eRoomType roomType, eGameType gameType, int timeType)
			: base(id, roomType, gameType, timeType)
		{
            m_loadingFiles = new List<LoadingFileInfo>();
            long_1 = 0L;
            m_roomId = roomId;
            m_players = new Dictionary<int, Player>();
            m_turnQueue = new List<TurnedLiving>();
            m_livings = new List<Living>();
            m_random = new Random();
			m_logStartIps = new Dictionary<int, string>();
            m_map = map;
            m_actions = new ArrayList();
            PhysicalId = 0;
            EquipPlace = new int[15]
            {
                1,
                2,
                3,
                4,
                5,
                6,
                11,
                13,
                14,
                15,
                16,
                17,
                18,
                19,
                20
            };
            m_confineWind = false;
            FrozenWind = false;
            BossWarField = "";
            m_tempBox = new List<Box>();
            m_tempGhostPoints = new List<Point>();
            m_tempBall = new List<Ball>();
            m_tempPoints = new List<Point>();
            m_decklivings = new List<Living>();
            if (base.RoomType == eRoomType.Dungeon)
            {
                Cards = new int[21];
            }
            else
            {
                Cards = new int[9];
            }
            m_gameState = eGameState.Inited;
        }

		public Player[] GetSameTeamPlayer(Player player)
		{
			List<Player> teammates = new List<Player>();
			foreach (Player p in GetAllFightPlayers())
			{
				if (p != player && p.Team == player.Team)
				{
					teammates.Add(p);
				}
			}
			return teammates.ToArray();
		}

		public bool CoupleFight(Player player)
		{
			Player[] array = GetSameTeamPlayer(player);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].PlayerDetail.PlayerCharacter.SpouseID == player.PlayerDetail.PlayerCharacter.ID)
				{
					return true;
				}
			}
			return false;
		}

		public void AddAction(IAction action)
		{
			lock (m_actions)
			{
				m_actions.Add(action);
			}
		}

		public void AddAction(ArrayList actions)
		{
			lock (m_actions)
			{
				m_actions.AddRange(actions);
			}
		}

		public virtual void AddLiving(Living living)
		{
			m_map.AddPhysical(living);
			if (living is Player)
			{
				Player p = living as Player;
				if (p.Weapon == null)
				{
					return;
				}
				else
                {
					m_turnQueue.Add(living as TurnedLiving);
					return;
                }
			}
			if (living is TurnedLiving)
			{
				m_turnQueue.Add(living as TurnedLiving);
			}
			else
			{
				m_livings.Add(living);
			}
			/*if (!(living is Player))*/
			SendAddLiving(living);
			#region OLD
			/*if (!(living is Player) || (living as Player).Weapon != null)
			{
				if (living is TurnedLiving)
				{
					m_turnQueue.Add(living as TurnedLiving);
				}
				else
				{
					m_livings.Add(living);
				}
				if (!(living is Player))
				{
					SendAddLiving(living);
				}
			}*/
			#endregion
		}

        public virtual void AddGhostBoxObj(PhysicalObj phy)
		{
			m_map.AddPhysical(phy);
			phy.SetGame(this);
		}

		public void AddLoadingFile(int type, string file, string className)
		{
			if (file != null && className != null)
			{
				m_loadingFiles.Add(new LoadingFileInfo(type, file, className));
			}
		}

		public virtual void AddNormalBoss(Living living)
		{
			m_map.AddPhysical(living);
			if (!(living is Player) || (living as Player).Weapon != null)
			{
				m_livings.Add(living);
				SendAddLiving(living);
			}
		}

		protected void AddPlayer(IGamePlayer gp, Player fp)
		{
			lock (m_players)
			{
				m_players.Add(fp.Id, fp);
				if (fp.Weapon != null)
				{
					m_turnQueue.Add(fp);
				}
			}
		}

		public virtual void AddPhysicalObj(PhysicalObj phy, bool sendToClient)
		{
			m_map.AddPhysical(phy);
			phy.SetGame(this);
			if (sendToClient)
			{
				SendAddPhysicalObj(phy);
			}
		}

		public virtual void AddPhysicalTip(PhysicalObj phy, bool sendToClient)
		{
			m_map.AddPhysical(phy);
			phy.SetGame(this);
			if (sendToClient)
			{
				SendAddPhysicalTip(phy);
			}
		}

		public void AddTempPoint(int x, int y)
		{
			m_tempPoints.Add(new Point(x, y));
		}

		public void AfterUseItem(ItemInfo item)
		{
		}

		internal void capnhattrangthai(Living player, string loai1, string loai2)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id);
			pkg.WriteByte(41);
			pkg.WriteString(loai1);
			pkg.WriteString(loai2);
			SendToAll(pkg);
		}

        public void ClearAllChild()
        {
            List<Living> list = new List<Living>();
            foreach (Living living in m_livings)
            {
                if (living.IsLiving && living is SimpleNpc)
                {
                    list.Add(living);
                }
            }
            foreach (Living living2 in list)
            {
                m_livings.Remove(living2);
                living2.Dispose();
                RemoveLiving(living2.Id);
            }
        }

		public void ClearAllChildByID(int ID)
		{
			List<Living> livings = new List<Living>();
			foreach (SimpleNpc mLiving in this.m_livings)
			{
				if (!mLiving.IsLiving || !(mLiving is SimpleNpc) || mLiving.NpcInfo.ID != ID)
				{
					continue;
				}
				livings.Add(mLiving);
			}
			foreach (Living living in livings)
			{
				this.m_livings.Remove(living);
				living.Dispose();
				this.RemoveLiving(living.Id);
			}
		}

		public void ClearAllChildByIDs(int[] ID)
		{
			List<Living> livings = new List<Living>();
			for (int i = 0; i < ID.Length; i++)
			{
				foreach (SimpleNpc mLiving in this.m_livings)
				{
					if (!mLiving.IsLiving || !(mLiving is SimpleNpc) || mLiving.NpcInfo.ID != ID[i])
					{
						continue;
					}
					livings.Add(mLiving);
				}
			}
			foreach (Living living in livings)
			{
				this.m_livings.Remove(living);
				living.Dispose();
				this.RemoveLiving(living.Id);
			}
		}

		public List<SimpleNpc> GetNPCLivingWithProperties(int properties)
		{
			var simpleNpcs = new List<SimpleNpc>();
			foreach (Living mLiving in this.m_livings)
			{
				if (!(mLiving is SimpleNpc) || !mLiving.IsLiving || (mLiving as SimpleNpc).Properties1 != properties)
				{
					continue;
				}
				simpleNpcs.Add(mLiving as SimpleNpc);
			}
			return simpleNpcs;
		}

		public TurnedLiving FindNextTurnedLiving()
		{
            #region OLD
            /*List<TurnedLiving> list = FindAllTurnLiving();
			if (list.Count == 0)
			{
				return null;
			}
			else
			{
				list = list.Where(p => !p.IsQuanChien).ToList();
				int index = m_random.Next(list.Count);
				TurnedLiving turnedLiving = list[index];
				int delay = turnedLiving.Delay;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Delay < delay && list[i].IsLiving)
					{
						delay = list[i].Delay;
						turnedLiving = list[i];
					}
				}
				turnedLiving.TurnNum++;
			}
			return turnedLiving;*/
            #endregion
            List<TurnedLiving> allTurnLiving = this.FindAllTurnLiving();
			TurnedLiving turnedLiving1;
			if (allTurnLiving.Count == 0)
			{
				turnedLiving1 = (TurnedLiving)null;
			}
			else
			{
				allTurnLiving = allTurnLiving.Where(p => !p.IsQuanChien).ToList();
				int index1 = this.m_random.Next(allTurnLiving.Count);
				TurnedLiving turnedLiving2 = allTurnLiving[index1];
				int delay = turnedLiving2.Delay;
				for (int index2 = 0; index2 < allTurnLiving.Count; ++index2)
				{
					if (allTurnLiving[index2].Delay < delay && allTurnLiving[index2].IsLiving)
					{
						delay = allTurnLiving[index2].Delay;
						turnedLiving2 = allTurnLiving[index2];
					}
				}
				++turnedLiving2.TurnNum;
				turnedLiving1 = turnedLiving2;
			}
			return turnedLiving1;
		}

		public void ClearDiedPhysicals()
		{
			List<Living> list = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (!living.IsLiving)
				{
					list.Add(living);
				}
			}
			foreach (Living item2 in list)
			{
				m_livings.Remove(item2);
				item2.Dispose();
			}
			List<Living> list2 = new List<Living>();
			foreach (TurnedLiving item3 in m_turnQueue)
			{
				if (!item3.IsLiving)
				{
					list2.Add(item3);
				}
			}
			foreach (TurnedLiving item4 in list2)
			{
				m_turnQueue.Remove(item4);
			}
			foreach (Physics item5 in m_map.GetAllPhysicalSafe())
			{
				if (!item5.IsLiving && !(item5 is Player))
				{
					m_map.RemovePhysical(item5);
				}
			}
			#region Old Died
			//         List<Living> list = new List<Living>();
			//foreach (Living living in m_livings)
			//{
			//	if (!living.IsLiving)
			//	{
			//		list.Add(living);
			//	}
			//}
			//foreach (Living living2 in list)
			//{
			//	m_livings.Remove(living2);
			//	living2.Dispose();
			//}
			//List<Living> list2 = new List<Living>();
			//foreach (TurnedLiving living3 in m_turnQueue)
			//{
			//	if (!living3.IsLiving)
			//	{
			//		list2.Add(living3);
			//	}
			//}
			//foreach (TurnedLiving living4 in list2)
			//{
			//	m_turnQueue.Remove(living4);
			//}
			//foreach (Physics physics in m_map.GetAllPhysicalSafe())
			//{
			//	if (!physics.IsLiving && !(physics is Player))
			//	{
			//		m_map.RemovePhysical(physics);
			//	}
			//}
			#endregion
		}

        public void ClearLoadingFiles()
		{
			m_loadingFiles.Clear();
		}

		public void ClearWaitTimer()
		{
			m_waitTimer = 0L;
		}

		public Box AddBox(ItemInfo item, Point pos, bool sendToClient)
		{
			Box box = new Box(PhysicalId++, "1", item, 1);
			box.SetXY(pos);
			AddPhysicalObj(box, sendToClient);
			return AddBox(box, sendToClient);
		}

		public Box AddBox(Box box, bool sendToClient)
		{
			m_tempBox.Add(box);
			AddPhysicalObj(box, sendToClient);
			return box;
		}

		public void AddTempGhostPoint(int x, int y)
		{
			m_tempGhostPoints.Add(new Point(x, y));
		}

		public Box AddGhostBox(Point pos, int type)
		{
			Box box = new Box(PhysicalId++, "1", null, type);
			box.SetXY(pos);
			return AddGhostBox(box);
		}

		public Box AddGhostBox(Box box)
		{
			m_tempBox.Add(box);
			AddGhostBoxObj(box);
			return box;
		}

		public int CheckGhostBox()
		{
			List<Box> source = new List<Box>();
			foreach (Box simpleBox in m_tempBox)
			{
				if (simpleBox.Type > 1)
				{
					source.Add(simpleBox);
				}
			}
			return source.Count();
		}

		public void CheckBox()
		{
			List<Box> temp = new List<Box>();
			foreach (Box b2 in m_tempBox)
			{
				if (!b2.IsLiving)
				{
					temp.Add(b2);
				}
			}
			foreach (Box b in temp)
			{
				m_tempBox.Remove(b);
				RemovePhysicalObj(b, true);
			}
		}

		public List<Point> DrawCirclePoints(int points, int dis, double radius, Point center)
		{
			List<Point> pointList = new List<Point>();
			double num1 = Math.PI * 2.0 / (double)points;
			for (double num2 = radius; num2 > (double)dis; num2 -= (double)dis)
			{
				for (int index = 0; index < points; index++)
				{
					double num3 = num1 * (double)index;
					Point point = new Point((int)((double)center.X + num2 * Math.Cos(num3)), (int)((double)center.Y + num2 * Math.Sin(num3)));
					pointList.Add(point);
				}
			}
			return pointList;
		}

		public void CreateGhostPoints()
		{
			int backroundHeight = m_map.Info.BackroundHeight;
			Point center = new Point(m_map.Info.BackroundWidht / 2, backroundHeight / 2);
			m_tempGhostPoints = DrawCirclePoints(backroundHeight, 30, backroundHeight - 180, center);
		}

		public List<Box> CreateBox()
		{
			int num1 = m_players.Count + 2;
			int num2 = 0;
			List<ItemInfo> info = null;
			if (CurrentTurnTotalDamage > 0)
			{
				num2 = m_random.Next(1, 3);
				if (m_tempBox.Count + num2 > num1)
				{
					num2 = num1 - m_tempBox.Count;
				}
				if (num2 > 0)
				{
					DropInventory.BoxDrop(m_roomType, ref info);
				}
			}
			int diedPlayerCount = GetDiedPlayerCount();
			int num3 = 0;
			List<Box> simpleBoxList = new List<Box>();
			if (diedPlayerCount > 0)
			{
				num3 = m_random.Next(diedPlayerCount);
				if (m_tempGhostPoints.Count < num1)
				{
					CreateGhostPoints();
				}
				for (int index4 = 0; index4 < m_tempGhostPoints.Count; index4++)
				{
					int index6 = m_random.Next(m_tempGhostPoints.Count);
					Point point2 = m_tempGhostPoints[index6];
					m_tempGhostPoints[index6] = m_tempGhostPoints[index4];
					m_tempGhostPoints[index4] = point2;
				}
				int num5 = diedPlayerCount + num1 - CheckGhostBox();
				if (m_tempGhostPoints.Count > num5)
				{
					int[] numArray = new int[2]
					{
						2,
						3
					};
					for (int index3 = 0; index3 < num5; index3++)
					{
						int index7 = m_random.Next(numArray.Length);
						int index8 = m_random.Next(m_tempGhostPoints.Count);
						simpleBoxList.Add(AddGhostBox(m_tempGhostPoints[index8], numArray[index7]));
					}
				}
			}
			if (m_tempBox.Count + num2 + num3 > num1)
			{
				_ = m_tempBox.Count;
			}
			if (info != null)
			{
				for (int index2 = 0; index2 < m_tempPoints.Count; index2++)
				{
					int index5 = m_random.Next(m_tempPoints.Count);
					Point point = m_tempPoints[index5];
					m_tempPoints[index5] = m_tempPoints[index2];
					m_tempPoints[index2] = point;
				}
				int num4 = Math.Min(info.Count, m_tempPoints.Count);
				for (int index = 0; index < num4; index++)
				{
					simpleBoxList.Add(AddBox(info[index], m_tempPoints[index], false));
				}
			}
			m_tempPoints.Clear();
			m_tempGhostPoints.Clear();
			return simpleBoxList;
		}

		public virtual void CheckState(int delay)
		{
			AddAction(new CheckPVPGameStateAction(delay));
		}

		public SimpleBoss[] FindAllBoss()
		{
			List<SimpleBoss> list = new List<SimpleBoss>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleBoss)
				{
					list.Add(living as SimpleBoss);
				}
			}
			return list.ToArray();
		}

		public List<SimpleBoss> FindAllBossTurned()
		{
			List<SimpleBoss> list = new List<SimpleBoss>();
			foreach (TurnedLiving living in m_turnQueue)
			{
				if (living is SimpleBoss)
				{
					list.Add(living as SimpleBoss);
				}
			}
			return list;
		}

		public SimpleNpc[] FindAllNpc()
		{
			List<SimpleNpc> list = new List<SimpleNpc>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleNpc)
				{
					list.Add(living as SimpleNpc);
				}
			}
			return list.ToArray();
		}

		public SimpleNpc[] FindAllNpcLiving()
		{
			List<SimpleNpc> list = new List<SimpleNpc>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleNpc && living.IsLiving)
				{
					list.Add(living as SimpleNpc);
				}
			}
			return list.ToArray();
		}

		public List<SimpleBoss> FindAllTurnBoss()
		{
			List<SimpleBoss> list = new List<SimpleBoss>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleBoss)
				{
					list.Add(living as SimpleBoss);
				}
			}
			return list;
		}

		public List<Living> FindAllTurnBossLiving()
		{
			List<Living> list = new List<Living>();
			foreach (TurnedLiving living in m_turnQueue)
			{
				if (living is SimpleBoss && living.IsLiving)
				{
					list.Add(living);
				}
			}
			return list;
		}

		public Player FindFarPlayer(int x, int y)
		{
			lock (m_players)
			{
				double minValue = double.MinValue;
				Player player = null;
				foreach (Player player2 in m_players.Values)
				{
					if (player2.IsLiving)
					{
						double num2 = player2.Distance(x, y);
						if (num2 > minValue)
						{
							minValue = num2;
							player = player2;
						}
					}
				}
				return player;
			}
		}

		public int GainCoupleGP(Player player, int gp)
		{
			Player[] array = GetSameTeamPlayer(player);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].PlayerDetail.PlayerCharacter.SpouseID == player.PlayerDetail.PlayerCharacter.SpouseID)
				{
					return (int)((double)gp * 1.5);
				}
			}
			return gp;
		}

		public int FindlivingbyDir(Living npc)
		{
			int num = 0;
			int num2 = 0;
			foreach (Player player in m_players.Values)
			{
				if (player.IsLiving)
				{
					if (player.X > npc.X)
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
			}
			if (num2 > num)
			{
				return 1;
			}
			if (num2 < num)
			{
				return -1;
			}
			return -npc.Direction;
		}

		public SimpleBoss[] FindLivingTurnBossWithID(int id)
		{
			List<SimpleBoss> list = new List<SimpleBoss>();
			foreach (TurnedLiving living in m_turnQueue)
			{
				if (living is SimpleBoss && living.IsLiving && (living as SimpleBoss).NpcInfo.ID == id)
				{
					list.Add(living as SimpleBoss);
				}
			}
			return list.ToArray();
		}

		public Living FindNearestHelper(int x, int y)
		{
			double maxValue = double.MaxValue;
			Living living = null;
			foreach (TurnedLiving living2 in m_turnQueue)
			{
				if (living2.IsLiving && (living2 is Player || living2.Config.IsHelper))
				{
					double num2 = living2.Distance(x, y);
					if (num2 < maxValue)
					{
						maxValue = num2;
						living = living2;
					}
				}
			}
			return living;
		}

		public Player FindNearestPlayer(int x, int y)
		{
			double maxValue = double.MaxValue;
			Player player = null;
			foreach (Player player2 in m_players.Values)
			{
				if (player2.IsQuanChien)
					continue;

				if (player2.IsLiving)
				{
					double num2 = player2.Distance(x, y);
					if (num2 < maxValue)
					{
						maxValue = num2;
						player = player2;
					}
				}
			}
			return player;
		}

        public List<TurnedLiving> FindAllTurnLiving()
		{
			List<TurnedLiving> list = new List<TurnedLiving>();
			foreach (TurnedLiving item in m_turnQueue)
			{
				if ((item is Player && item.IsLiving && (item as Player).IsActive) || (item is SimpleBoss && item.Config.IsTurn && item.IsLiving))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public Player FindPlayer(int id)
		{
			lock (m_players)
			{
				if (m_players.ContainsKey(id))
				{
					return m_players[id];
				}
			}
			return null;
		}

		public PhysicalObj[] FindPhysicalObjByName(string name)
		{
			List<PhysicalObj> list = new List<PhysicalObj>();
			foreach (PhysicalObj obj2 in m_map.GetAllPhysicalObjSafe())
			{
				if (obj2.Name == name)
				{
					list.Add(obj2);
				}
			}
			return list.ToArray();
		}

		public Living FindRandomLiving()
		{
			List<Living> list = new List<Living>();
			Living living = null;
			foreach (Living living2 in m_livings)
			{
				if (living2.IsLiving)
				{
					list.Add(living2);
				}
			}
			int num = Random.Next(0, list.Count);
			if (list.Count > 0)
			{
				living = list[num];
			}
			return living;
		}

		/*public Player FindRandomPlayer()
		{
			lock (m_players)
			{
				if (m_players.Count > 0)
				{
					List<Player> list = new List<Player>();
					foreach (Player value in m_players.Values)
					{
						if (value.IsLiving && value.IsActive && !value.IsQuanChien)
						{
							list.Add(value);
						}
					}
					int index = Random.Next(0, list.Count);
					return list[index];
				}
				return null;
			}
		}*/

		public Player FindRandomPlayer()
        {
			List<Player> list = new List<Player>();
			Player player = null;
			foreach(Player p in m_players.Values)
            {
				if(p.IsLiving)
                {
					list.Add(p);
                }
            }
			if(list.Count > 0)
            {
				int next = Random.Next(0, list.Count);
				player = list[next];
            }
			return player;
        }

		public Player[] FindRandomPlayer(int max)
		{
			List<Player> listrt = new List<Player>();

			if (m_players.Count > 0)
			{
				List<Player> list = new List<Player>();
				foreach (Player player in m_players.Values)
				{
					if (player.IsLiving)
					{
						list.Add(player);
					}
				}

				for (int i = 0; i < max; i++)
				{
					int next = Random.Next(0, list.Count);

					listrt.Add(list[next]);

					list.RemoveAt(next);

					if (list.Count <= 0)
						break;
				}
			}

			return listrt.ToArray();
		}

		public Player FindRandomPlayerNotLock()
		{
			List<Player> source = new List<Player>();
			foreach (Player player in m_players.Values)
			{
				if (player.IsLiving && player.State != 9)
				{
					source.Add(player);
				}
			}
			if (source.Count > 0)
			{
				int index = Random.Next(0, source.Count);
				return source.ElementAt(index);
			}
			return null;
		}

		public List<Player> GetAllEnemyPlayers(Living living)
		{
			List<Player> list = new List<Player>();
			lock (m_players)
			{
				foreach (Player player in m_players.Values)
				{
					if (player.Team != living.Team)
					{
						list.Add(player);
					}
				}
				return list;
			}
		}

		public List<Player> GetAllFightPlayers()
		{
			List<Player> list = new List<Player>();
			lock (m_players)
            {
				list.AddRange(m_players.Values);
				return list;
            }
		}

		public List<Player> GetAllLivingPlayers()
		{
			lock (m_players)
			{
				List<Player> list = new List<Player>();
				foreach (Player player in m_players.Values)
				{
					if (player.IsLiving)
					{
						list.Add(player);
					}
				}
				return list;
			}
		}

		public List<Player> GetAllLivingPlayersByProperties(int prop)
		{
			List<Player> players = new List<Player>();
			lock (this.m_players)
			{
				foreach (Player value in this.m_players.Values)
				{
					if (!value.IsLiving || value.Properties1 != 2)
					{
						continue;
					}
					players.Add(value);
				}
			}
			return players;
		}

		public Player[] GetAllPlayers()
		{
			return GetAllFightPlayers().ToArray();
		}

		public List<Player> GetAllTeamPlayers(Living living)
		{
			List<Player> list = new List<Player>();
			lock (m_players)
			{
				foreach (Player player in m_players.Values)
				{
					if (player.Team == living.Team)
					{
						list.Add(player);
					}
				}
				return list;
			}
		}

		public List<Living> GetBossLivings()
		{
			List<Living> list = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (living.IsLiving && living is SimpleBoss)
				{
					list.Add(living);
				}
			}
			return list;
		}

		public List<Living> FindAppointDeGreeNpc(int degree)
		{
			List<Living> npc = new List<Living>();
			foreach (Living p3 in m_livings)
			{
				if (p3.IsLiving && p3.Degree == degree)
				{
					npc.Add(p3);
				}
			}
			foreach (Living p2 in m_decklivings)
			{
				if (p2.IsLiving && p2.Degree == degree)
				{
					npc.Add(p2);
				}
			}
			foreach (TurnedLiving p in m_turnQueue)
			{
				if (p.IsLiving && p.Degree == degree)
				{
					npc.Add(p);
				}
			}
			return npc;
		}

		public int GetDiedBossCount()
		{
			int num = 0;
			SimpleBoss[] array = FindAllBoss();
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsLiving)
				{
					num++;
				}
			}
			return num;
		}

		public int GetDiedCount()
		{
			return GetDiedNPCCount() + GetDiedBossCount();
		}

		public int GetDiedNPCCount()
		{
			int num = 0;
			SimpleNpc[] array = FindAllNpc();
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsLiving)
				{
					num++;
				}
			}
			return num;
		}

		public int GetDiedPlayerCount()
		{
			int num = 0;
			foreach (Player player in m_players.Values)
			{
				if (player.IsQuanChien)
					continue;

				if (player.IsActive && !player.IsLiving)
				{
					num++;
				}
			}
			return num;
		}

		public List<Living> GetFightFootballLivings()
		{
			List<Living> list = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleNpc)
				{
					list.Add(living);
				}
			}
			return list;
		}

		public Player GetFrostPlayerRadom()
		{
			List<Player> allFightPlayers = GetAllFightPlayers();
			List<Player> source = new List<Player>();
			foreach (Player player in allFightPlayers)
			{
				if (player.IsFrost)
				{
					source.Add(player);
				}
			}
			if (source.Count > 0)
			{
				int index = Random.Next(0, source.Count);
				return source.ElementAt(index);
			}
			return null;
		}

		public int GetHighDelayTurn()
		{
			new List<Living>();
			int delay = int.MinValue;
			foreach (TurnedLiving living in m_turnQueue)
			{
				if (living != null && living.Delay > delay)
				{
					delay = living.Delay;
				}
			}
			return delay;
		}

		public List<Living> GetLivedLivings()
		{
			List<Living> list = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (living.IsLiving)
				{
					list.Add(living);
				}
			}
			return list;
		}

		public List<Living> GetLivedLivingsHadTurn()
		{
			List<Living> list = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (living.IsLiving && living is SimpleNpc && living.Config.IsTurn)
				{
					list.Add(living);
				}
			}
			return list;
		}

		public List<Living> GetLivedNpcs(int npcId)
		{
			List<Living> list = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (living.IsLiving && living is SimpleNpc && (living as SimpleNpc).NpcInfo.ID == npcId)
				{
					list.Add(living);
				}
			}
			return list;
		}

		public int GetLowDelayTurn()
		{
			new List<Living>();
			int num = int.MaxValue;
			foreach (TurnedLiving item in m_turnQueue)
			{
				if (item != null && item.Delay < num)
				{
					num = item.Delay;
				}
			}
			return num;
		}

		public float GetNextWind()
		{
			if (FrozenWind)
            {
				return 0f;
            }
			int num = (int)(Wind * 10f);
			int num2;
			if (num > m_nextWind)
			{
				num2 = num - m_random.Next(11);
				if (num <= m_nextWind)
				{
					m_nextWind = m_random.Next(-40, 40);
				}
			}
			else
			{
				num2 = num + m_random.Next(11);
				if (num >= m_nextWind)
				{
					m_nextWind = m_random.Next(-40, 40);
				}
			}
			return (float)num2 / 10f;
		}

		public Player GetNoHolePlayerRandom()
		{
			List<Player> allFightPlayers = GetAllFightPlayers();
			List<Player> source = new List<Player>();
			foreach (Player player in allFightPlayers)
			{
				if (player.IsNoHole)
				{
					source.Add(player);
				}
			}
			if (source.Count > 0)
			{
				int index = Random.Next(0, source.Count);
				return source.ElementAt(index);
			}
			return null;
		}

		public Player GetPlayer(IGamePlayer gp)
		{
			Player player = null;
			lock (m_players)
			{
				foreach (Player player2 in m_players.Values)
				{
					if (player2.PlayerDetail == gp)
					{
						return player2;
					}
				}
				return player;
			}
		}

		public Player GetPlayerByIndex(int index)
		{
			return m_players.ElementAt(index).Value;
		}

		public int GetPlayerCount()
		{
			return GetAllFightPlayers().Count;
		}

		protected Point GetPlayerPoint(MapPoint mapPos, int team)
		{
			List<Point> list = ((team == 1) ? mapPos.PosX : mapPos.PosX1);
			int num = m_random.Next(list.Count);
			Point item = list[num];
			list.Remove(item);
			return item;
		}

		public bool GetSameTeam()
		{
			bool result = false;
			Player[] players = GetAllPlayers();
			foreach (Player p in players)
            {
				if (p.Team == players[0].Team)
					result = true;
				else
                {
					result = false;
					break;
                }
            }
			return result;
		}

		public int getTurnTime()
		{
			switch (m_timeType)
			{
				case 1:
					return 8;
				case 2:
					return 10;
				case 3:
					return 12;
				case 4:
					return 16;
				case 5:
					return 21;
				case 6:
					return 31;
				default:
					return -1;
			}
		}

		public int GetTurnWaitTime()
		{
			return m_timeType;
		}

		public byte GetVane(int Wind, int param)
		{
			int wind = Math.Abs(Wind);
			switch (param)
			{
			case 1:
				return WindMgr.GetWindID(wind, 1);
			case 3:
				return WindMgr.GetWindID(wind, 3);
			default:
				return 0;
			}
		}

		public long GetWaitTimer()
		{
			return m_waitTimer;
		}

		public int GetWaitTimerLeft()
		{
			if (long_1 <= 0)
			{
				return 0;
			}
			long num = ((TickHelper.GetTickCount() > long_1) ? (TickHelper.GetTickCount() - long_1) : (long_1 - TickHelper.GetTickCount()));
			if (num > 10000)
			{
				return 1000;
			}
			return (int)num;
		}

		public bool IsAllComplete()
		{
			foreach (Player allFightPlayer in GetAllFightPlayers())
			{
				if (allFightPlayer.LoadingProcess < 100)
				{
					return false;
				}
			}
			return true;
		}

		internal bool isTrainer()
		{
			return base.RoomType == eRoomType.Freshman;
		}

		internal void method_10(int int_3, int int_4, int int_5)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(62);
			pkg.WriteInt(int_5);
			pkg.WriteInt(int_3);
			pkg.WriteInt(int_4);
			SendToAll(pkg);
		}

		internal void method_18(Living living_0, int int_4, int int_5, int int_6, int int_7, string string_0, string string_1, int int_8)
		{
			GSPacketIn pkg = new GSPacketIn(91, living_0.Id)
			{
				Parameter1 = living_0.Id
			};
			pkg.WriteByte(55);
			pkg.WriteInt(int_4);
			pkg.WriteInt(int_5);
			pkg.WriteInt(int_6);
			pkg.WriteInt(int_7);
			pkg.WriteInt(int_8);
			pkg.WriteString((!string.IsNullOrEmpty(string_0)) ? string_0 : "");
			pkg.WriteString((!string.IsNullOrEmpty(string_1)) ? string_1 : "");
			SendToAll(pkg);
		}

		internal void method_24(Living living_0)
		{
			GSPacketIn pkg = new GSPacketIn(91, living_0.Id)
			{
				Parameter1 = living_0.Id
			};
			pkg.WriteByte(72);
			pkg.WriteInt(living_0.X);
			pkg.WriteInt(living_0.Y);
			SendToAll(pkg);
		}

		internal void SendLivingActionMapping(int id, string source, string value)
        {
			GSPacketIn pkg = new GSPacketIn(91, id);
			pkg.Parameter1 = id;
			pkg.WriteByte(223);
			pkg.WriteInt(id);
			pkg.WriteString(source);
			pkg.WriteString(value);
			SendToAll(pkg);
        }

		internal void method_30(Living living_0)
		{
			GSPacketIn pkg = new GSPacketIn(91, living_0.Id)
			{
				Parameter1 = living_0.Id
			};
			pkg.WriteByte(33);
			pkg.WriteBoolean(living_0.IsFrost);
			SendToAll(pkg);
		}

		internal void method_39(Player player_0, int int_4, int int_5, int int_6)
		{
			GSPacketIn pkg = new GSPacketIn(91, player_0.Id)
			{
				Parameter1 = player_0.Id
			};
			pkg.WriteByte(32);
			pkg.WriteByte((byte)int_4);
			pkg.WriteInt(int_5);
			pkg.WriteInt(int_6);
			pkg.WriteInt(player_0.Id);
			pkg.WriteBoolean(val: false);
			SendToAll(pkg);
		}

		internal void SendLockFocus(bool IsLock)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(69);
			pkg.WriteBoolean(IsLock);
			SendToAll(pkg);
		}

		internal void method_47(Living living_0, int int_4, bool bool_0)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = living_0.Id
			};
			pkg.WriteByte(128);
			pkg.WriteInt(int_4);
			pkg.WriteBoolean(bool_0);
			SendToAll(pkg);
		}

		internal void method_59(Living living_0, int int_3, bool bool_1)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.Parameter1 = living_0.Id;
			gSPacketIn.WriteByte(128);
			gSPacketIn.WriteInt(int_3);
			gSPacketIn.WriteBoolean(bool_1);
			SendToAll(gSPacketIn);
		}

		internal void SendLivingBoltMove(Living living)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id);
			pkg.Parameter1 = living.Id;
			pkg.WriteByte(72);
			pkg.WriteInt(living.X);
			pkg.WriteInt(living.Y);
			SendToAll(pkg);
		}

		public virtual void MinusDelays(int lowestDelay)
		{
			foreach (TurnedLiving item in m_turnQueue)
			{
				if (!item.IsQuanChien)
				{
					item.Delay -= lowestDelay;
				}
			}
		}

		public List<Player> GetAllFightPlayersNoQuanChien()
		{
			List<Player> list = new List<Player>();
			lock (this.m_players)
			{
				foreach (var item in this.m_players)
					if (!item.Value.IsQuanChien)
					{
						list.Add(item.Value);
					}
			}
			return list;
		}

		protected void OnBeginNewTurn()
		{
			if (this.BeginNewTurn != null)
			{
				this.BeginNewTurn(this);
			}
		}

		public void OnGameNpcDie(int Id)
		{
			if (this.GameNpcDie != null)
			{
				this.GameNpcDie(Id);
			}
		}

		public void OnGameOverLog(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
		{
			if (this.GameOverLog != null)
			{
				this.GameOverLog(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, BossWarField);
			}
		}

		protected void OnGameOverred()
		{
			if (this.GameOverred != null)
			{
				this.GameOverred(this);
			}
		}

		public override void Pause(int time)
		{
			m_passTick = Math.Max(m_passTick, TickHelper.GetTickCount() + time);
		}

		internal void PedSuikAov(Living living_0, int int_4)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = living_0.Id
			};
			pkg.WriteByte(80);
			pkg.WriteInt(living_0.Id);
			pkg.WriteInt(int_4);
			SendToAll(pkg);
		}

		public override void ProcessData(GSPacketIn packet)
		{
			if (m_players.ContainsKey(packet.Parameter1))
			{
				Player player = m_players[packet.Parameter1];
				AddAction(new ProcessPacketAction(player, packet));
			}
		}

		public Player FindPlayerWithUserId(int userid)
		{
			lock (m_players)
			{
				if (m_players.Count > 0)
				{
					foreach (Player player in m_players.Values)
					{
						if (player.PlayerDetail.PlayerCharacter.ID == userid)
						{
							return player;
						}
					}
				}
				return null;
			}
		}

		public void RemoveLivings(int id)
		{
			SendRemoveLiving(id);
		}

		public void RemoveLiving(int id)
		{
            SendRemoveLiving(id);
		}

		public void RemoveLiving(Living living, bool sendToClient)
		{
			m_map.RemovePhysical(living);
			if (sendToClient)
			{
				SendRemoveLiving(living.Id);
			}
		}

		public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
		{
			Player player = null;
			lock (m_players)
			{
				foreach (Player p in m_players.Values)
				{
					if (p.PlayerDetail == gp)
					{
						player = p;
						m_players.Remove(p.Id);
						break;
					}
				}
			}
			lock (m_turnQueue)
			{
				if (player != null)
				{
					m_turnQueue.Remove(player);
				}
			}
			if (player != null)
			{
				AddAction(new RemovePlayerAction(player));
			}
			return player;
		}

		public void RemovePhysicalObj(PhysicalObj phy, bool sendToClient)
		{
			m_map.RemovePhysical(phy);
			phy.SetGame(null);
			if (sendToClient)
			{
				SendRemovePhysicalObj(phy);
			}
		}

		public override void Resume()
		{
			m_passTick = 0L;
		}

		public void SelectObject(int id, int zoneId)
		{
			lock (m_players)
			{
			}
		}

		internal void SendShowBloodItem(int livingId)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(73);
			pkg.WriteInt(livingId);
			SendToAll(pkg);
		}

		internal void SendAddLiving(Living living)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.Parameter1 = living.Id;
			pkg.WriteByte(64);
			pkg.WriteByte((byte)living.Type);
			pkg.WriteInt(living.Id);
			pkg.WriteString(living.Name);
			pkg.WriteString(living.ModelId);
			pkg.WriteString(living.ActionStr);
			pkg.WriteInt(living.X);
			pkg.WriteInt(living.Y);
			pkg.WriteInt(living.Blood);
			pkg.WriteInt(living.MaxBlood);
			pkg.WriteInt(living.Team);
			pkg.WriteByte((byte)living.Direction);
			pkg.WriteByte(living.Config.isBotom);
			pkg.WriteBoolean(living.Config.isShowBlood);
			pkg.WriteBoolean(living.Config.isShowSmallMapPoint);
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteBoolean(living.IsFrost);
			pkg.WriteBoolean(living.IsHide);
			pkg.WriteBoolean(living.IsNoHole);
			pkg.WriteBoolean(val: false);
			pkg.WriteInt(0);
			SendToAll(pkg);
		}

		internal void SendAddPhysicalObj(PhysicalObj obj)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(48);
			pkg.WriteInt(obj.Id);
			pkg.WriteInt(obj.Type);
			pkg.WriteInt(obj.X);
			pkg.WriteInt(obj.Y);
			pkg.WriteString(obj.Model);
			pkg.WriteString(obj.CurrentAction);
			pkg.WriteInt(obj.Scale);
			pkg.WriteInt(obj.Scale);
			pkg.WriteInt(obj.Rotation);
			pkg.WriteInt(obj.phyBringToFront);
			pkg.WriteInt(obj.typeEffect);
			SendToAll(pkg);
		}

		internal void SendAddPhysicalTip(PhysicalObj obj)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(68);
			pkg.WriteInt(obj.Id);
			pkg.WriteInt(obj.Type);
			pkg.WriteInt(obj.X);
			pkg.WriteInt(obj.Y);
			pkg.WriteString(obj.Model);
			pkg.WriteString(obj.CurrentAction);
			pkg.WriteInt(obj.Scale);
			pkg.WriteInt(obj.Rotation);
			SendToAll(pkg);
		}

		internal void SendAttackEffect(Living player, int type)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(129);
			pkg.WriteBoolean(val: true);
			pkg.WriteInt(type);
			SendToAll(pkg);
		}

		internal void method_10(Living living_0, PetSkillElementInfo petSkillElementInfo_0, bool bool_1)
		{
			method_12(living_0, petSkillElementInfo_0, bool_1, 3, null);
		}

		internal void method_11(Living living_0, PetSkillElementInfo petSkillElementInfo_0, bool bool_1, int int_4)
		{
			method_12(living_0, petSkillElementInfo_0, bool_1, int_4, null);
		}

		internal void method_12(Living living_0, PetSkillElementInfo petSkillElementInfo_0, bool bool_1, int int_4, IGamePlayer igamePlayer_0)
		{
			GSPacketIn pkg = new GSPacketIn(91, living_0.Id);
			pkg.Parameter1 = living_0.Id;
			pkg.WriteByte(145);
			pkg.WriteInt(petSkillElementInfo_0.ID);
			pkg.WriteString(petSkillElementInfo_0.Name);
			pkg.WriteString(petSkillElementInfo_0.Description);
			pkg.WriteString(petSkillElementInfo_0.Pic.ToString());
			pkg.WriteString(petSkillElementInfo_0.EffectPic);
			pkg.WriteBoolean(bool_1);
			SendToAll(pkg);
		}

		internal void SendCreateGame()
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(101);
			pkg.WriteInt((byte)m_roomType);
			pkg.WriteInt((byte)m_gameType);
			pkg.WriteInt(m_timeType);
			List<Player> allFightPlayers = GetAllFightPlayers();
			pkg.WriteInt(allFightPlayers.Count);
			foreach (Player player in allFightPlayers)
			{
				IGamePlayer playerDetail = player.PlayerDetail;
				pkg.WriteInt(playerDetail.ZoneId);
				pkg.WriteString(playerDetail.ZoneName);
				pkg.WriteInt(playerDetail.PlayerCharacter.ID);
				pkg.WriteString(playerDetail.PlayerCharacter.NickName);				
				pkg.WriteBoolean(false);
				pkg.WriteInt(playerDetail.Place);
				pkg.WriteByte(playerDetail.PlayerCharacter.typeVIP);
				pkg.WriteInt(playerDetail.PlayerCharacter.VIPLevel);
				pkg.WriteBoolean(playerDetail.PlayerCharacter.Sex);
				pkg.WriteInt(playerDetail.PlayerCharacter.Hide);
				pkg.WriteString(playerDetail.PlayerCharacter.Style);
				pkg.WriteString(playerDetail.PlayerCharacter.Colors);
				pkg.WriteString(playerDetail.PlayerCharacter.Skin);
				pkg.WriteInt(playerDetail.PlayerCharacter.Grade);
				pkg.WriteInt(playerDetail.PlayerCharacter.Repute);
				if (playerDetail.MainWeapon == null)
				{
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(playerDetail.MainWeapon.TemplateID);
					pkg.WriteInt(playerDetail.MainWeapon.RefineryLevel);
					pkg.WriteString(playerDetail.MainWeapon.Template.Name);
					pkg.WriteDateTime(DateTime.MinValue);
				}
				if (playerDetail.SecondWeapon == null)
				{
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(playerDetail.SecondWeapon.TemplateID);
				}
				pkg.WriteInt(playerDetail.PlayerCharacter.Nimbus);
				pkg.WriteBoolean(playerDetail.PlayerCharacter.IsShowConsortia);
				pkg.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
				pkg.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
				pkg.WriteInt(playerDetail.PlayerCharacter.badgeID);
				pkg.WriteInt(playerDetail.PlayerCharacter.ConsortiaLevel);
				pkg.WriteInt(playerDetail.PlayerCharacter.ConsortiaRepute);
				pkg.WriteInt(playerDetail.PlayerCharacter.Win);
				pkg.WriteInt(playerDetail.PlayerCharacter.Total);
				pkg.WriteInt(playerDetail.PlayerCharacter.FightPower);
				pkg.WriteInt(playerDetail.PlayerCharacter.apprenticeshipState);
				pkg.WriteInt(playerDetail.PlayerCharacter.masterID);
				pkg.WriteString(playerDetail.PlayerCharacter.masterOrApprentices);
				pkg.WriteInt(playerDetail.PlayerCharacter.AchievementPoint);
				pkg.WriteString(playerDetail.PlayerCharacter.Honor);
				pkg.WriteInt(playerDetail.PlayerCharacter.Offer);
				pkg.WriteBoolean(playerDetail.PlayerCharacter.DailyLeagueFirst);//(player.PlayerDetail.MatchInfo.DailyLeagueFirst);
				pkg.WriteInt(playerDetail.PlayerCharacter.DailyLeagueLastScore);//(player.PlayerDetail.MatchInfo.DailyLeagueLastScore);
				pkg.WriteBoolean(playerDetail.PlayerCharacter.IsMarried);
				if (playerDetail.PlayerCharacter.IsMarried)
				{
					pkg.WriteInt(playerDetail.PlayerCharacter.SpouseID);
					pkg.WriteString(playerDetail.PlayerCharacter.SpouseName);
				}
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(player.Team);
				pkg.WriteInt(player.Id);
				pkg.WriteInt(player.MaxBlood);
				if (player.Pet == null)
				{
					pkg.WriteInt(0);
					continue;
				}
				pkg.WriteInt(1);
				pkg.WriteInt(player.Pet.Place);
				pkg.WriteInt(player.Pet.TemplateID);
				pkg.WriteInt(player.Pet.ID);
				pkg.WriteString(player.Pet.Name);
				pkg.WriteInt(player.Pet.UserID);
				pkg.WriteInt(player.Pet.Level);
				string[] skillEquips = player.Pet.SkillEquip.Split('|');
				pkg.WriteInt(skillEquips.Length);
				foreach (string skill in skillEquips)
                {
					pkg.WriteInt(int.Parse(skill.Split(',')[1]));
					pkg.WriteInt(int.Parse(skill.Split(',')[0]));
				}
			}
			SendToAll(pkg);
		}

		internal void SendEquipEffect(Living player, string buffer)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(3);
			pkg.WriteString(buffer);
			SendToAll(pkg);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="living"></param>
		/// <param name="achievID">
		/// 1:Vua phản kích
		/// 2:Hạt nhân thần chưởng
		/// 3:Siêu chuẩn
		/// 4:Tức nước vở bờ 
		/// 5:Kỹ thuật siêu cao
		/// 7:Đòn chí mạng 
		/// 6:Hộ giáp thần công
		/// 8:tấn công siêu chuẩn
		/// </param>
		/// <param name="num"></param>
		/// <param name="delay"></param>
		internal void SendFightAchievement(Living living, int achievID, int dis, int delay)
		{
			if (living.Game.RoomType == eRoomType.Match || living.Game.RoomType == eRoomType.Freedom || living.Game.RoomType == eRoomType.EliteGameChampion || living.Game.RoomType == eRoomType.EliteGameScore)
			{
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(238);
				pkg.WriteInt(achievID);
				pkg.WriteInt(dis);
				pkg.WriteInt(delay);
				SendToAll(pkg);
			}
		}

		internal void SendGameActionMapping(Living player, Ball ball)
		{
			string currentAction = ball.CurrentAction;
			GSPacketIn pkg = new GSPacketIn(91, player.Id);
			pkg.WriteByte(223);
			pkg.WriteInt(ball.Id);
			pkg.WriteString(currentAction);
			pkg.WriteString(ball.ActionMapping[currentAction]);
			SendToAll(pkg);
		}

		internal void SendGameBigBox(Living player, List<int> listTemplate)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id);
			pkg.WriteByte(136);
			pkg.WriteInt(listTemplate.Count);
			foreach (int num in listTemplate)
			{
				pkg.WriteInt(num);
			}
			SendToAll(pkg);
		}

		internal void SendGameNextTurn(Living living, BaseGame game, List<Box> newBoxes)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id);
			pkg.Parameter1 = living.Id;
			pkg.WriteByte(6);
			int Wind = (int)((double)game.Wind * 10.0);
			pkg.WriteBoolean(Wind > 0);
			pkg.WriteByte(GetVane(Wind, 1));
			pkg.WriteByte(GetVane(Wind, 2));
			pkg.WriteByte(GetVane(Wind, 3));
			pkg.WriteBoolean(living.IsHide);
			pkg.WriteInt(getTurnTime());
			pkg.WriteInt(newBoxes.Count);
			foreach (Box simpleBox in newBoxes)
			{
				pkg.WriteInt(simpleBox.Id);
				pkg.WriteInt(simpleBox.X);
				pkg.WriteInt(simpleBox.Y);
				pkg.WriteInt(simpleBox.Type);
			}
			List<Player> allFightPlayers = game.GetAllFightPlayers();
			pkg.WriteInt(allFightPlayers.Count);
			foreach (Player player in allFightPlayers)
			{
				pkg.WriteInt(player.Id);
				pkg.WriteBoolean(player.IsLiving);
				pkg.WriteInt(player.X);
				pkg.WriteInt(player.Y);
				pkg.WriteInt(player.Blood);
				pkg.WriteBoolean(player.IsNoHole);
				pkg.WriteInt(player.Energy);
				pkg.WriteInt(player.psychic);
				pkg.WriteInt(player.Dander);
				if (player.Pet == null)
				{
					pkg.WriteInt(0);
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(player.PetMaxMP);
					pkg.WriteInt(player.PetMP);
				}
				pkg.WriteInt(player.ShootCount);
				pkg.WriteInt(player.flyCount);
			}
			pkg.WriteInt(game.TurnIndex);
			SendToAll(pkg);
		}

		internal void method_50(Player player_0, int int_4)
		{
			method_51(player_0, player_0.PetEffects.CurrentUseSkill, player_0.PetEffects.CurrentUseSkill != 0, int_4);
		}

		internal void method_51(Player player_0, int int_4, bool bool_1, int int_5)
		{
			GSPacketIn pkg = new GSPacketIn(91, player_0.Id);
			pkg.Parameter1 = player_0.Id;
			pkg.WriteByte(144);
			pkg.WriteInt(int_4);
			pkg.WriteBoolean(bool_1);
			pkg.WriteInt(int_5);
			SendToAll(pkg);
		}

		internal void method_51(Living living_0)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(4);
			(living_0 as Player).PlayerDetail.SendTCP(pkg);
		}

		internal void SendGamePickBox(Living player, int index, int arkType, string goods)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id);
			pkg.WriteByte(49);
			pkg.WriteByte((byte)index);
			pkg.WriteByte((byte)arkType);
			pkg.WriteString(goods);
			SendToAll(pkg);
		}

		internal void SendGamePlayerProperty(Living living, string type, string state)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(41);
			pkg.WriteString(type);
			pkg.WriteString(state);
			SendToAll(pkg);
		}

		internal void SendGamePlayerTakeCard(Player player, bool isAuto, int index, int templateID, int count, bool isVip)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id);
			pkg.Parameter1 = player.Id;
			pkg.WriteByte((byte)98);
			pkg.WriteBoolean(isAuto);
			pkg.WriteByte((byte)index);
			pkg.WriteInt(templateID);
			pkg.WriteInt(count);
			pkg.WriteBoolean(isVip);
			SendToAll(pkg);
		}

		internal void SendGameUpdateBall(Player player, bool Special)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(20);
			pkg.WriteBoolean(Special);
			pkg.WriteInt(player.CurrentBall.ID);
			SendToAll(pkg);
		}

		internal void SendGameUpdateDander(TurnedLiving player)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(14);
			pkg.WriteInt(player.Dander);
			SendToAll(pkg);
		}

		internal void SendGameUpdateFrozenState(Living player)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(33);
			pkg.WriteBoolean(player.IsFrost);
			SendToAll(pkg);
		}

		internal void SendGameUpdateHealth(Living living, int type, int value)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id);
			pkg.Parameter1 = living.Id;
			pkg.WriteByte(11);
			pkg.WriteByte((byte)type);
			pkg.WriteInt(living.Blood);
			pkg.WriteInt(value);
			this.SendToAll(pkg);
		}

		internal void SendGameUpdateHideState(Living player)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(35);
			pkg.WriteBoolean(player.IsHide);
			SendToAll(pkg);
		}

		internal void SendGameUpdateNoHoleState(Living player)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(82);
			pkg.WriteBoolean(player.IsNoHole);
			SendToAll(pkg);
		}

		internal void SendGameUpdateSealState(Living player, int type)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.Parameter1 = player.Id;
			pkg.WriteByte(41);
			pkg.WriteByte((byte)type);
			pkg.WriteBoolean(player.GetSealState());
			SendToAll(pkg);
		}

		internal void SendGameUpdateShootCount(Player player)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(46);
			pkg.WriteByte((byte)player.ShootCount);
			SendToAll(pkg);
		}

		internal void SendGameUpdateWind(float wind)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(38);
			int val = (int)(wind * 10f);
			pkg.WriteInt(val);
			pkg.WriteBoolean(val > 0);
			pkg.WriteByte(GetVane(val, 1));
			pkg.WriteByte(GetVane(val, 2));
			pkg.WriteByte(GetVane(val, 3));
			SendToAll(pkg);
		}

		internal void SendGameWindPic(byte windId, byte[] windpic)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(241);
			pkg.WriteByte(windId);
			pkg.Write(windpic);
			this.SendToAll(pkg);
		}

		internal void SendIsLastMission(bool isLast)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(160);
			pkg.WriteBoolean(isLast);
			SendToAll(pkg);
		}

		internal void SendLivingBeat(Living living, Living target, int totalDemageAmount, string action, int livingCount, int attackEffect)
		{
			int dander = 0;
			if (target is Player)
            {
				Player p = target as Player;
				dander = p.Dander;
            }

			GSPacketIn pkg = new GSPacketIn(91, living.Id);
			pkg.Parameter1 = living.Id;
			pkg.WriteByte(58);
			pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
			pkg.WriteInt(livingCount);
			for (int i = 1; i <= livingCount; i++)
            {
				pkg.WriteInt(target.Id);
				pkg.WriteInt(totalDemageAmount);
				pkg.WriteInt(target.Blood);
				pkg.WriteInt(dander);
				pkg.WriteInt(attackEffect);
			}
			SendToAll(pkg);
		}

		internal void SendLivingFall(Living living, int toX, int toY, int speed, string action, int type)
        {
			GSPacketIn pkg = new GSPacketIn(91, living.Id);
			pkg.Parameter1 = living.Id;
			pkg.WriteByte((byte)eTankCmdType.LIVING_FALLING);
			pkg.WriteInt(toX);
			pkg.WriteInt(toY);
			pkg.WriteInt(speed);
			pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
			pkg.WriteInt(type);
			SendToAll(pkg);
        }

		internal void SendLivingJump(Living living, int toX, int toY, int speed, string action, int type)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(57);
			pkg.WriteInt(toX);
			pkg.WriteInt(toY);
			pkg.WriteInt(speed);
			pkg.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			pkg.WriteInt(type);
			SendToAll(pkg);
		}

		internal void method_26(Living living_0, int int_3, int int_4, int int_5, int int_6, string string_0, string string_1, int int_7)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living_0.Id);
			gSPacketIn.Parameter1 = living_0.Id;
			gSPacketIn.WriteByte(55);
			gSPacketIn.WriteInt(int_3);
			gSPacketIn.WriteInt(int_4);
			gSPacketIn.WriteInt(int_5);
			gSPacketIn.WriteInt(int_6);
			gSPacketIn.WriteInt(int_7);
			gSPacketIn.WriteString((!string.IsNullOrEmpty(string_0)) ? string_0 : "");
			gSPacketIn.WriteString((!string.IsNullOrEmpty(string_1)) ? string_1 : "");
			SendToAll(gSPacketIn);
		}

		internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(55);
			pkg.WriteInt(fromX);
			pkg.WriteInt(fromY);
			pkg.WriteInt(toX);
			pkg.WriteInt(toY);
			pkg.WriteInt(speed);
			pkg.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			pkg.WriteString("");
			SendToAll(pkg);
		}

		internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed, string sAction)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(55);
			pkg.WriteInt(fromX);
			pkg.WriteInt(fromY);
			pkg.WriteInt(toX);
			pkg.WriteInt(toY);
			pkg.WriteInt(speed);
			pkg.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			pkg.WriteString((!string.IsNullOrEmpty(sAction)) ? sAction : "");
			SendToAll(pkg);
		}

		internal void SendLivingPlayMovie(Living living, string action)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(60);
			pkg.WriteString(action);
			SendToAll(pkg);
		}

		internal void SendLivingSay(Living living, string msg, int type)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(59);
			pkg.WriteString(msg);
			pkg.WriteInt(type);
			SendToAll(pkg);
		}

		internal void SendLivingShowBlood(Living living, int isShow)
		{
			GSPacketIn pkg = new GSPacketIn(91, living.Id);
			pkg.WriteByte(80);
			pkg.WriteInt(living.Id);
			pkg.WriteInt(isShow);
			SendToAll(pkg);
		}

		internal void SendLivingShowBlood(Player player, long blood, int x, int y)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id);
			pkg.WriteByte(39);
			pkg.WriteInt(player.Id);
			pkg.WriteLong(blood);
			pkg.WriteInt(x);
			pkg.WriteInt(y);
			SendToAll(pkg);
		}

		internal void SendLivingTurnRotation(Player player, int rotation, int speed, string endPlay)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(85);
			pkg.WriteInt(rotation);
			pkg.WriteInt(speed);
			pkg.WriteString(endPlay);
			SendToAll(pkg);
		}

		public void LivingChangeAngle(Living living, int Speed, int Angle, string endPlay)
        {
			GSPacketIn gSPacketIn = new GSPacketIn(91)
			{
				Parameter1 = living.Id
			};
			gSPacketIn.WriteByte(85);
			gSPacketIn.WriteInt(Angle);
			gSPacketIn.WriteInt(Speed);
			gSPacketIn.WriteString(endPlay);
			this.SendToAll(gSPacketIn);
		}

		internal void SendLivingUpdateAngryState(Living living)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(118);
			pkg.WriteInt(living.State);
			SendToAll(pkg);
		}

		internal void SendLivingUpdateDirection(Living living)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(7);
			pkg.WriteInt(living.Direction);
			SendToAll(pkg);
		}

		internal void SendMessage(IGamePlayer player, string msg, string msg1, int type)
		{
			if (msg != null)
			{
				GSPacketIn pkg = new GSPacketIn(3);
				pkg.WriteInt(type);
				pkg.WriteString(msg);
				player.SendTCP(pkg);
			}
			if (msg1 != null)
			{
				GSPacketIn in2 = new GSPacketIn(3);
				in2.WriteInt(type);
				in2.WriteString(msg1);
				SendToAll(in2, player);
			}
		}

		internal void SendOpenSelectLeaderWindow(int maxTime)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(102);
			pkg.WriteInt(maxTime);
			SendToAll(pkg);
		}

		internal void SendPetBuff(Living player, PetSkillElementInfo info, bool isActive)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(145);
			pkg.WriteInt(info.ID);
			pkg.WriteString("");
			pkg.WriteString("");
			pkg.WriteString(info.Pic.ToString());
			pkg.WriteString(info.EffectPic);
			pkg.WriteBoolean(isActive);
			SendToAll(pkg);
		}

		internal void SendPetSkillCd(Living player, int skillInfoID, int ColdDown)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(147);
			pkg.WriteInt(skillInfoID);
			pkg.WriteInt(ColdDown);
			(player as Player).PlayerDetail.SendTCP(pkg);
		}

		/*internal void SendPetUseKill(Player player, int achievID)
		{
			SendPetUseKill(player, player.PetEffects.CurrentUseSkill, player.PetEffects.CurrentUseSkill != 0, achievID);
		}*/

		internal void SendPetUseKill(Player player, int type)
		{
			SendPetUseKill(player, player.PetEffects.CurrentUseSkill, player.PetEffects.CurrentUseSkill != 0, type);
		}

		internal void SendPetUseKill(Player player, int skillId, bool isUse, int type)
		{
			GSPacketIn pkg = new GSPacketIn((byte)ePackageTypeLogic.GAME_CMD, player.Id);
			pkg.Parameter1 = player.Id;
			pkg.WriteByte(144);
			pkg.WriteInt(skillId);
			pkg.WriteBoolean(isUse);
			pkg.WriteInt(type);
			SendToAll(pkg);
		}

		internal void SendSelfTurn(Living living)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(4);
			(living as Player).PlayerDetail.SendTCP(pkg);
		}

		internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving, bool sendExcept)
        {
            GSPacketIn pkg = new GSPacketIn(91, player.Id)
            {
                Parameter1 = player.Id
            };
            pkg.WriteByte(9);
            pkg.WriteBoolean(val: false);
            pkg.WriteByte((byte)type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteByte(dir);
            pkg.WriteBoolean(isLiving);
            if (type == 2)
            {
                pkg.WriteInt(m_tempBox.Count);
                foreach (Box box in m_tempBox)
                {
                    pkg.WriteInt(box.X);
                    pkg.WriteInt(box.Y);
                }
            }
            if (sendExcept)
            {
                SendToAll(pkg, player.PlayerDetail);
            }
            else
            {
                SendToAll(pkg);
            }
        }

        internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving)
        {
			GSPacketIn pkg = new GSPacketIn((byte)ePackageTypeLogic.GAME_CMD, player.Id);
			pkg.Parameter1 = player.Id;
			pkg.WriteByte((byte)eTankCmdType.MOVESTART);
			pkg.WriteBoolean(false);
			pkg.WriteByte((byte)type);
			pkg.WriteInt(x);
			pkg.WriteInt(y);
			pkg.WriteByte(dir);
			pkg.WriteBoolean(isLiving);
			if (type == 2)
			{
				pkg.WriteInt(m_tempBox.Count);
				foreach (Box box in m_tempBox)
				{
					pkg.WriteInt(box.X);
					pkg.WriteInt(box.Y);
				}
			}

			SendToAll(pkg);
		}

		internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving, string action)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(9);
			pkg.WriteByte((byte)type);
			pkg.WriteInt(x);
			pkg.WriteInt(y);
			pkg.WriteByte(dir);
			pkg.WriteBoolean(isLiving);
			pkg.WriteString((!string.IsNullOrEmpty(action)) ? action : "move");
			SendToAll(pkg);
		}

		internal void SendPlayerMove2(Player player, int type, int x, int y, byte dir, bool isLiving)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(9);
			pkg.WriteByte((byte)type);
			pkg.WriteInt(x);
			pkg.WriteInt(y);
			pkg.WriteByte(dir);
			pkg.WriteBoolean(isLiving);
			if (type == 2)
			{
				pkg.WriteInt(m_tempBox.Count);
				foreach (Box box in m_tempBox)
				{
					pkg.WriteInt(box.X);
					pkg.WriteInt(box.Y);
				}
			}
			SendToAll(pkg);
		}

		internal void SendPlayerMove(Player player, int type, int x, int y, byte dir)
		{
			GSPacketIn pkg = new GSPacketIn((byte)ePackageTypeLogic.GAME_CMD, player.PlayerDetail.PlayerCharacter.ID);
			pkg.Parameter1 = player.Id;
			pkg.WriteByte((byte)eTankCmdType.MOVESTART);
			pkg.WriteBoolean(true);
			pkg.WriteByte((byte)type);
			pkg.WriteInt(x);
			pkg.WriteInt(y);
			pkg.WriteByte(dir);
			pkg.WriteBoolean(player.IsLiving);
			if (type == 2)
			{
				pkg.WriteInt(m_tempBox.Count);
				foreach (Box box in m_tempBox)
				{
					pkg.WriteInt(box.X);
					pkg.WriteInt(box.Y);
				}
			}

			SendToAll(pkg, player.PlayerDetail);
		}

		internal void SendPlayerPicture(Living living, int type, bool state)
		{
			GSPacketIn pkg = new GSPacketIn(91)
			{
				Parameter1 = living.Id
			};
			pkg.WriteByte(128);
			pkg.WriteInt(type);
			pkg.WriteBoolean(state);
			SendToAll(pkg);
		}

		internal void SendPlayerRemove(Player player)
		{
			GSPacketIn pkg = new GSPacketIn(94, player.PlayerDetail.PlayerCharacter.ID);
			pkg.WriteByte(5);
			pkg.WriteInt(player.PlayerDetail.ZoneId);
			SendToAll(pkg);
		}

        internal void SendLoading()
        {
            foreach (Player allFightPlayer in GetAllFightPlayers())
            {
                GSPacketIn pkg = new GSPacketIn(91);
                pkg.WriteByte(16);
                pkg.WriteInt(allFightPlayer.LoadingProcess);
                pkg.WriteInt(allFightPlayer.PlayerDetail.ZoneId);
                pkg.WriteInt(allFightPlayer.PlayerDetail.PlayerCharacter.ID);
                SendToAll(pkg);
            }
        }

        internal void SendPlayerUseProp(Player player, int type, int place, int templateID)
		{
			SendPlayerUseProp(player, type, place, templateID, player);
		}

		internal void SendPlayerUseProp(Living player, int type, int place, int templateID, Player p)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(32);
			pkg.WriteByte((byte)type);
			pkg.WriteInt(place);
			pkg.WriteInt(templateID);
			pkg.WriteInt(p.Id);
			pkg.WriteBoolean(templateID == 10017);
			SendToAll(pkg);
		}

		internal void SendPhysicalObjFocus (Physics physics, int type)
        {
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(62);
			pkg.WriteInt(type);
			pkg.WriteInt(physics.X);
			pkg.WriteInt(physics.Y);
			SendToAll(pkg);
        }

		internal void SendPhysicalObjPlayAction(PhysicalObj obj)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(66);
			pkg.WriteInt(obj.Id);
			pkg.WriteString(obj.CurrentAction);
			SendToAll(pkg);
		}

		internal void SendRemoveLiving(int id)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(53);
			pkg.WriteInt(id);
			SendToAll(pkg);
		}

		internal void SendRemovePhysicalObj(PhysicalObj obj)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(53);
			pkg.WriteInt(obj.Id);
			SendToAll(pkg);
		}

		internal void SendSkipNext(Player player)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(12);
			player.PlayerDetail.SendTCP(pkg);
		}

		internal void SendStartLoading(int maxTime)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(103);
			pkg.WriteInt(maxTime);
			pkg.WriteInt(m_map.Info.ID);
			pkg.WriteInt(m_loadingFiles.Count);
			foreach (LoadingFileInfo info in m_loadingFiles)
			{
				pkg.WriteInt(info.Type);
				pkg.WriteString(info.Path);
				pkg.WriteString(info.ClassName);
			}
			if (IsSpecialPVE())
			{
				pkg.WriteInt(0);
			}
			else
			{
				GameNeedPetSkillInfo[] list = PetMgr.GetGameNeedPetSkill();
				pkg.WriteInt(list.Length);
				foreach (GameNeedPetSkillInfo info in list)
                {
					pkg.WriteString(info.Pic.ToString()); //_loc_7.pic = event.pkg.readUTF();
					pkg.WriteString(info.EffectPic); //_loc_7.effect = event.pkg.readUTF();
				}
			}
			SendToAll(pkg);
		}

		internal void method_53(Living living_0, string string_0)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(0);
			pkg.WriteString(string_0);
			SendToAll(pkg);
		}

		public void SendSyncLifeTime()
		{
			GSPacketIn pkg = new GSPacketIn(91);
			//pkg.Parameter2 = -1;
			pkg.WriteByte(131);
			pkg.WriteInt(m_lifeTime);
			SendToAll(pkg);
		}

		internal void method_34(Living living_0, List<int> BhV2sgMkjBpUnK7WcH)
		{
			GSPacketIn pkg = new GSPacketIn(91, living_0.Id);
			pkg.WriteByte(136);
			pkg.WriteInt(BhV2sgMkjBpUnK7WcH.Count);
			foreach (int val in BhV2sgMkjBpUnK7WcH)
			{
				pkg.WriteInt(val);
			}
			SendToAll(pkg);
		}

		public virtual void SendToAll(GSPacketIn pkg)
		{
			SendToAll(pkg, null);
		}

		public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
			if (pkg.Parameter2 == 0)
			{
				pkg.Parameter2 = LifeTime;
			}
			foreach (Player player in GetAllFightPlayers())
			{
				if (player.IsActive && player.PlayerDetail != except)
				{
					player.PlayerDetail.SendTCP(pkg);
				}
			}
		}
		public virtual void SendToAll2(GSPacketIn pkg, IGamePlayer except)
		{
			if (pkg.Parameter2 == 0)
			{
				pkg.Parameter2 = LifeTime;
			}
			foreach (Player player in GetAllFightPlayersNoQuanChien())
			{
				if (player.IsActive && player.PlayerDetail != except)
				{
					player.PlayerDetail.SendTCP(pkg);
				}
			}
		}

		public virtual void SendToTeam(GSPacketIn pkg, int team)
		{
			SendToTeam(pkg, team, null);
		}

		public virtual void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
		{
			foreach (Player player in GetAllFightPlayers())
			{
				if (player.IsActive && player.PlayerDetail != except && player.Team == team)
				{
					player.PlayerDetail.SendTCP(pkg);
				}
			}
		}

		public void SendChatToTeam(GSPacketIn pkg, Player p)
		{
			if (pkg.Parameter2 == 0)
			{
				pkg.Parameter2 = LifeTime;
			}
			foreach (Player current in GetAllTeamPlayers(p))
			{
				if (current.IsActive && current.PlayerDetail != p)
				{
					current.PlayerDetail.SendTCP(pkg);
				}
			}
		}

		internal void SendUseDeputyWeapon(Player player, int ResCount)
		{
			GSPacketIn pkg = new GSPacketIn(91, player.Id)
			{
				Parameter1 = player.Id
			};
			pkg.WriteByte(84);
			pkg.WriteInt(ResCount);
			player.PlayerDetail.SendTCP(pkg);
		}

		public bool SetMap(int mapId)
		{
			if (GameState != eGameState.Playing)
			{
				Map map = MapMgr.CloneMap(mapId);
				if (map != null)
				{
					m_map = map;
					return true;
				}
			}
			return false;
		}

		public void SetWind(int wind)
		{
			m_map.wind = wind;
		}

		public void Shuffer<T>(T[] array)
		{
			for (int i = array.Length; i > 1; i--)
			{
				int index = Random.Next(i);
				T local = array[index];
				array[index] = array[i - 1];
				array[i - 1] = local;
			}
		}

		public virtual bool TakeCard(Player player)
		{
			return false;
		}

		public virtual bool TakeCard(Player player, int index, bool isAuto)
		{
			return false;
		}

		public int FindBombPlayerX(int blowArea)
		{
			Dictionary<int, int> tempFind = new Dictionary<int, int>();
			Dictionary<int, int> tempFindTow = new Dictionary<int, int>();
			List<int> endBlowX = new List<int>();
			List<Player> listPlayer = GetAllFightPlayers();
			int tempCheck = 0;
			foreach (Player player2 in listPlayer)
			{
				if (!player2.IsLiving)
				{
					continue;
				}
				for (int i = 0; i < 10; i++)
				{
					int x;
					do
					{
						x = Random.Next(player2.X - blowArea, player2.X + blowArea);
					}
					while (tempFind.ContainsKey(x));
					tempFind.Add(x, 0);
				}
			}
			foreach (int blowX3 in tempFind.Keys)
			{
				foreach (Player player in listPlayer)
				{
					if (player.X > blowX3 - blowArea && player.X < blowX3 + blowArea)
					{
						if (tempFindTow.ContainsKey(blowX3))
						{
							tempFindTow[blowX3]++;
						}
						else
						{
							tempFindTow.Add(blowX3, 1);
						}
					}
				}
			}
			foreach (int blowX2 in tempFindTow.Values)
			{
				if (blowX2 > tempCheck)
				{
					tempCheck = blowX2;
				}
			}
			foreach (int blowX in tempFindTow.Keys)
			{
				if (tempFindTow[blowX] == tempCheck)
				{
					endBlowX.Add(blowX);
				}
			}
			int ret = Random.Next(0, endBlowX.Count);
			return endBlowX[ret];
		}

		public Player Timnguoichoigannhat()
		{
			List<Player> list = new List<Player>();
			List<int> list2 = new List<int>();
			foreach (Player player in m_players.Values)
			{
				if (player.IsLiving)
				{
					list.Add(player);
					list2.Add(player.X);
				}
			}
			int num = list2.Max();
			foreach (Player player2 in list)
			{
				if (player2.X == num)
				{
					return player2;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return $"Id:{base.Id},player:{PlayerCount},state:{GameState},current:{CurrentLiving},turnIndex:{m_turnIndex},actions:{m_actions.Count}";
		}

		public bool IsSpecialPVE()
		{
			eRoomType roomType = base.RoomType;
			return roomType == eRoomType.FightLab || roomType == eRoomType.Freshman;
		}

		public bool IsSpecialPVEForBuffer()
        {
			eRoomType roomType = base.RoomType;
			if (roomType != eRoomType.FightLab && roomType != eRoomType.Freshman && roomType != eRoomType.Freedom && roomType != eRoomType.Match)
            {
				return false;
            }
			return true;
        }

		public void Update(long tick)
		{
			if (m_passTick >= tick) return;

			m_lifeTime++;

			if (GameState == eGameState.Stopped) return;

			ArrayList temp;

			lock (m_actions)
			{
				temp = (ArrayList)m_actions.Clone();
				m_actions.Clear();
			}

			CurrentActionCount = temp.Count;
			if (temp.Count > 0)
			{
				ArrayList left = new ArrayList();
				foreach (IAction action in temp)
				{
					try
					{
						action.Execute(this, tick);
						if (action.IsFinished(tick) == false)
						{
							left.Add(action);
						}
					}
					catch (Exception ex)
					{
						log.Error("Map update error:", ex);
					}
				}

				AddAction(left);
			}
			else if (m_waitTimer < tick)
			{
				CheckState(0);
			}
		}

		public void UpdateWind(float wind, bool sendToClient)
		{
			if (!m_confineWind && m_map.wind != wind)
			{
				m_map.wind = wind;
				if (sendToClient)
				{
					SendGameUpdateWind(wind);
				}
			}
		}

		public void VaneLoading()
		{
			List<WindInfo> listWinds = WindMgr.GetWind();
			foreach (WindInfo info in listWinds)
            {
				SendGameWindPic((byte)info.WindID, info.WindPic);
            }
		}

		public void WaitTime(int delay)
		{
			m_waitTimer = Math.Max(m_waitTimer, TickHelper.GetTickCount() + delay);
			long_1 = m_waitTimer;
		}

		internal void SendLivingWalkTo(Living m_living, int p1, int p2, int p3, int p4, string m_action, int m_speed)
		{
			throw new NotImplementedException();
		}

		public SimpleNpc[] GetNPCLivingWithID(int id)
		{
			List<SimpleNpc> list = new List<SimpleNpc>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleNpc && living.IsLiving && (living as SimpleNpc).NpcInfo.ID == id)
				{
					list.Add(living as SimpleNpc);
				}
			}
			return list.ToArray();
		}

		public SimpleBoss FindSingleSimpleBossID(int id)
        {
			foreach (TurnedLiving item in m_turnQueue)
            {
				if (item is SimpleBoss && (item as SimpleBoss).NpcInfo.ID == id)
                {
					return item as SimpleBoss;
                }
            }
			return null;
        }

		public List<Player> FindRangePlayers(int minX, int maxX)
		{
			lock (m_players)
			{
				List<Player> players = new List<Player>();
				foreach (Player p in m_players.Values)
				{
					if (p.IsLiving && p.X >= minX && p.X <= maxX)
					{
						players.Add(p);
					}
				}

				return players;
			}
		}

		public string ListPlayersName()
		{
			List<string> nameArr = new List<string>();

			List<Player> lists = GetAllLivingPlayers();

			foreach (Player player in lists)
			{
				nameArr.Add(player.PlayerDetail.PlayerCharacter.NickName);
			}

			return string.Join(",", nameArr);
		}

		public Player FindPlayerWithId(int id)
		{
			lock (m_players)
			{
				if (m_players.Count > 0)
				{
					foreach (Player player in m_players.Values)
					{
						if (player.IsLiving && player.Id == id)
						{
							return player;
						}
					}
				}

				return null;
			}
		}

		public void SendOpenPopupQuestionFrame(int id, int hasAnsw, int needAnsw, int totalQuestion, int timeLimit, string title, string question, string aw1, string aw2, string aw3)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(24);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(id);
			gSPacketIn.WriteInt(hasAnsw);
			gSPacketIn.WriteInt(needAnsw);
			gSPacketIn.WriteInt(totalQuestion);
			gSPacketIn.WriteInt(timeLimit);
			gSPacketIn.WriteString(title);
			gSPacketIn.WriteString(question);
			gSPacketIn.WriteString(aw1);
			gSPacketIn.WriteString(aw2);
			gSPacketIn.WriteString(aw3);
			SendToAll(gSPacketIn);
		}

		public void SendClosePopupQuestionFrame()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(24);
			gSPacketIn.WriteBoolean(val: false);
			SendToAll(gSPacketIn);
		}

		public List<Player> GetAllLivingPlayerOnRange(int minX, int maxX)
		{
			List<Player> list = new List<Player>();
			foreach (Player allLivingPlayer in GetAllLivingPlayers())
			{
				if (allLivingPlayer.X >= minX && allLivingPlayer.X <= maxX)
				{
					list.Add(allLivingPlayer);
				}
			}
			return list;
		}

		public void ShowBloodItem(int id)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(73);
			gSPacketIn.WriteInt(id);
			this.SendToAll(gSPacketIn);
		}

		public SimpleBoss FindBossWithID(int id)
        {
			SimpleBoss simpleBosses = null;
            foreach (Living list0 in this.m_turnQueue)
            {
                if (!(list0 is SimpleBoss) || !list0.IsLiving || (list0 as SimpleBoss).NpcInfo.ID != id)
                {
                    continue;
                }
                simpleBosses = list0 as SimpleBoss;
            }
            return simpleBosses;
        }

		public SimpleNpc FindHealthyHelper()
		{
			SimpleNpc simpleNpc = null;
			foreach (SimpleNpc mLiving in this.m_livings)
			{
				if (!mLiving.Config.IsHelper || mLiving.Config.CanHeal)
				{
					continue;
				}
				simpleNpc = mLiving;
			}
			return simpleNpc;
		}

        public List<Player> FindRandomPlayerInPoint(int X1, int X2)
        {
            List<Player> list = new List<Player>();
            Player result = null;
            foreach (Player current in this.m_players.Values)
            {
                if (current.IsLiving && current.X > X1 && current.X < X2)
                {
                    list.Add(current);
                }
            }
            return list;
        }

        public bool CanEnd = false;

		public bool CanEndGame = false;

		protected void AddLogIp(int id, string ip)
		{
			lock (m_logStartIps)
            {
				if (!m_logStartIps.ContainsKey(id))
					m_logStartIps.Add(id, ip);
            }
        }

		public bool ArenaBoss()
        {
			switch(RoomType)
            {
				case eRoomType.CryptBoss:
					return true;
            }
			return false;
        }

		public int GetTimeMoving(int X, int width)
		{
			return X * 6000 / width;
		}

		public void RemoveLiving2(int id)
        {
			foreach (Living current in this.m_livings.ToList())
            {
				if(current.Id == id)
                {
					this.m_map.RemovePhysical(current);
                    if(current is TurnedLiving)
                    {
						this.m_turnQueue.Remove(current as TurnedLiving);
                    }
					else
                    {
						this.m_livings.Remove(current);
                    }
                }
            }
        }

        public int GetDelayDistance(int x1, int x2, int speed)
        {
            int dis = Math.Abs(x1 - x2);
            int result = ((1000 / speed) * dis) / 22;
            return result;
        }


        public List<SimpleNpc> FindAllNpc2()
		{
			List<SimpleNpc> list = new List<SimpleNpc>();
			foreach (Living current in this.m_livings)
			{
				if (current is SimpleNpc)
				{
					list.Add(current as SimpleNpc);
				}
			};
			return list;
		}

		public void SendEffectIcon(Living living, int type, bool state)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91)
			{
				Parameter1 = living.Id
			};
			gSPacketIn.WriteByte(128);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteBoolean(state);
			gSPacketIn.WriteInt(1);
			this.SendToAll(gSPacketIn);
		}

        public List<SimpleNpc> FindAllNpcByName(string Name)
        {
            List<SimpleNpc> list = new List<SimpleNpc>();
            foreach (Living current in this.m_livings)
            {
                if (current is SimpleNpc && ((SimpleNpc)current).NpcInfo.Name == Name)
                {
                    list.Add(current as SimpleNpc);
                }
            };
            return list;
        }

        public void ClearAllNpc()
		{
			List<Living> temp = new List<Living>();
			foreach (Living living in m_livings)
			{
				if (living is SimpleNpc)
					temp.Add(living);
			}

			foreach (Living living in temp)
			{
				m_livings.Remove(living);
				living.Dispose();
				SendRemoveLiving(living.Id);
			}

			List<Physics> phys = m_map.GetAllPhysicalSafe();
			foreach (Physics phy in phys)
			{
				if (phy is SimpleNpc)
				{
					m_map.RemovePhysical(phy);
				}
			}
		}
	}
}
