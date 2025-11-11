using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PlayerExtra
	{
		protected Timer _hotSpringTimer;

		protected Timer _onlineGameTimer;

		public const int COUPLE_NUM = 20;
		public const int DUNGEON_NUM = 4;
		public const int PROPS_NUM = 5;

		private int[] buffData = new int[7]
		{
			1,
			2,
			3,
			4,
			5,
			6,
			7
		};

		private Dictionary<int, EventRewardProcessInfo> dictionary_0;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Dictionary<int, int> m_kingBlessInfo;

		private UsersExtraInfo m_Info;

		protected object m_lock = new object();

		protected GamePlayer m_player;

		private bool m_saveToDb;

		private List<EventAwardInfo> m_searchGoodItems;

		public int MapId = 1;

		private int[] positions = new int[34];

		private static ThreadSafeRandom rand = new ThreadSafeRandom();

		public readonly DateTime reChangeEnd;

		public readonly DateTime reChangeStart;

		public const int STRENGTH_ENCHANCE = 1;

		public readonly DateTime strengthenEnd;

		public readonly DateTime strengthenStart;

		public readonly DateTime upGradeEnd;

		public readonly DateTime upGradeStart;

		public readonly DateTime useMoneyEnd;

		public readonly DateTime useMoneyStart;

		public readonly DateTime upVipStart;

		public readonly DateTime upVipEnd;

		public UsersExtraInfo Info
		{
			get
			{
				return m_Info;
			}
			set
			{
				m_Info = value;
			}
		}

		public Dictionary<int, int> KingBlessInfo
		{
			get
			{
				return m_kingBlessInfo;
			}
			set
			{
				m_kingBlessInfo = value;
			}
		}

		public GamePlayer Player => m_player;

		public List<EventAwardInfo> SearchGoodItems
		{
			get
			{
				return m_searchGoodItems;
			}
			set
			{
				m_searchGoodItems = value;
			}
		}

		public PlayerExtra(GamePlayer player, bool saveTodb)
		{
			m_player = player;
			m_kingBlessInfo = new Dictionary<int, int>();
			m_searchGoodItems = new List<EventAwardInfo>();
			m_saveToDb = saveTodb;
		}

		public void BeginHotSpringTimer()
		{
			int dueTime = 60000;
			if (_hotSpringTimer == null)
			{
				_hotSpringTimer = new Timer(HotSpringCheck, null, dueTime, dueTime);
			}
			else
			{
				_hotSpringTimer.Change(dueTime, dueTime);
			}
		}

		public void BeginOnlineGameTimer()
        {
			int dueTime = 60000;
			if (_onlineGameTimer == null)
            {
				_onlineGameTimer = new Timer(OnlineGameCheck, null, dueTime, dueTime);
            }
			else
            {
				_onlineGameTimer.Change(dueTime, dueTime);
            }
        }

		public UsersExtraInfo CreateUserExtra(int UserID)
		{
			UsersExtraInfo obj = new UsersExtraInfo
			{
				UserID = UserID,
				LastTimeHotSpring = DateTime.Now,
				LastFreeTimeHotSpring = DateTime.Now,
				MinHotSpring = 30,
				FreeSendMailCount = 0
			};
			DateTime dateTime = DateTime.Now.AddDays(-1.0);
			obj.LeftRoutteCount = 1;
			obj.LeftRoutteRate = 0f;
			return obj;
		}

		public bool CheckNoviceActiveOpen(NoviceActiveType activeType)
		{
			bool flag = false;
			switch (activeType)
			{
				/*case NoviceActiveType.GRADE_UP_ACTIVE:
					return true;
				case NoviceActiveType.STRENGTHEN_WEAPON_ACTIVE:
					return true;
				case NoviceActiveType.USE_MONEY_ACTIVE:
					return true;
				case NoviceActiveType.RECHANGE_MONEY_ACTIVE:
					return true;
				case NoviceActiveType.UPGRADE_VIP_ACTIVE:
					return true;
				case NoviceActiveType.UPDATE_FIGHTPOWER:
					return true;
				case NoviceActiveType.RECHARGE_SPECIAL:
					return true;
				case NoviceActiveType.USE_MONEY_SPECIAL:
					return true;*/
				case NoviceActiveType.TANG_CAP_VIP:
					return true;
				case NoviceActiveType.TIEUXU_TICHLUY:
					return true;
				case NoviceActiveType.NAPXU_TICHLUY:
					return true;
				case NoviceActiveType.TIEUXU_DACBIET:
					return true;
				case NoviceActiveType.NAPXU_DACBIET:
					return true;
				default:
					return flag;
			}
		}

		public string GetNoviceActivityName(NoviceActiveType activeType)
		{
			string translateId = "Unknown";
			switch (activeType)
			{
                #region OLD
                /*case NoviceActiveType.GRADE_UP_ACTIVE:
					translateId = "Tăng cấp nhận thưởng";
					break;
				case NoviceActiveType.STRENGTHEN_WEAPON_ACTIVE:
					translateId = "Cường hóa tặng quà";
					break;
				case NoviceActiveType.USE_MONEY_ACTIVE:
					translateId = "Tiêu phí thưởng mỗi ngày";
					break;
				case NoviceActiveType.RECHANGE_MONEY_ACTIVE:
					translateId = "Nạp thưởng mỗi ngày";
					break;
				case NoviceActiveType.UPGRADE_VIP_ACTIVE:
					translateId = "Tăng vip nhận quà";
					break;
				case NoviceActiveType.UPDATE_FIGHTPOWER:
					translateId = "Quà lực chiến";
					break;
				case NoviceActiveType.RECHARGE_SPECIAL:
					translateId = "Nạp đặc biệt";
					break;
				case NoviceActiveType.USE_MONEY_SPECIAL:
					translateId = "Tiêu phí đặc biệt";
					break;*/
                #endregion
                case NoviceActiveType.TANG_CAP_VIP:
					translateId = "Tăng vip nhận quà";
					break;
				case NoviceActiveType.TIEUXU_TICHLUY:
					translateId = "Tiêu phí thưởng mỗi ngày";
					break;
				case NoviceActiveType.NAPXU_TICHLUY:
					translateId = "Nạp thưởng mỗi ngày";
					break;
				case NoviceActiveType.TIEUXU_DACBIET:
					translateId = "Tiêu phí đặc biệt";
					break;
				case NoviceActiveType.NAPXU_DACBIET:
					translateId = "Nạp xu đặc biệt";
					break;

			}
			return string.Format(translateId);
		}

		public EventRewardProcessInfo GetEventProcess(int activeType)
		{
			lock (m_lock)
			{
				if (!dictionary_0.ContainsKey(activeType))
				{
					dictionary_0.Add(activeType, method_0(activeType));
				}
				return dictionary_0[activeType];
			}
		}

		public EventAwardInfo GetSpecialTem(int type, int pos)
		{
			return new EventAwardInfo
			{
				TemplateID = type,
				Position = pos,
				Count = 1
			};
		}

		public void ResetNoviceEvent(NoviceActiveType activeType)
		{
			EventRewardProcessInfo eventProcess = GetEventProcess((int)activeType);
			eventProcess.AwardGot = 0;
			eventProcess.Conditions = 0;
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				bussiness.UpdateUsersEventProcess(eventProcess);
			}
		}

		protected void HotSpringCheck(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				if (m_player.CurrentHotSpringRoom == null)
				{
					StopHotSpringTimer();
					return;
				}
				if (Info.MinHotSpring <= 0)
                {
					m_player.CurrentHotSpringRoom.RemovePlayer(m_player);
					GSPacketIn pkg = new GSPacketIn(169);
					pkg.WriteString(string.Format("Hết thời gian vào suối nước nóng vui lòng gia hạn!!!"));
					m_player.SendTCP(pkg);
					return;
				}					
				int expWithLevel = HotSpringMgr.GetExpWithLevel(m_player.PlayerCharacter.Grade) / 10;
				int ExpHotSpring = 0;
				if (expWithLevel > 0)
				{
					Info.MinHotSpring--;
					m_player.OnPlayerSpa(1);
					if (Info.MinHotSpring <= 5)
					{
						m_player.SendMessage("Bạn chỉ còn " + Info.MinHotSpring + " phút.");
					}				
					if (m_player.CurrentHotSpringRoom.Info.roomID <= 4)
                    {
						ExpHotSpring = expWithLevel * 2;
						m_player.SendMessage(string.Format("|Phòng Thường|-EXP nhận được là {0}", expWithLevel * 2));
					}
					else
                    {
						ExpHotSpring = expWithLevel * 4;
						m_player.SendMessage(string.Format("|Phòng VIP|-EXP nhận được là {0}", expWithLevel * 4));
					}
					m_player.AddGP(ExpHotSpring, false, false);
					m_player.Out.SendHotSpringUpdateTime(m_player, ExpHotSpring);
					m_player.OnHotSpingExpAdd(Info.MinHotSpring, ExpHotSpring);
				}
				Thread.CurrentThread.Priority = priority;
				tickCount = Environment.TickCount - tickCount;
			}
			catch (Exception exception)
			{
				Console.WriteLine("HotSpringCheck: " + exception);
			}
		}

		protected void OnlineGameCheck(object sender)
        {
			try
            {
				int tickCount = Environment.TickCount;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				m_player.OnOnlineGameAdd();
				GmActivityMgr.OnUpdateOnline(m_player, 1);
				m_player.PlayerCharacter.CheckTimer++;
				Thread.CurrentThread.Priority = priority;
				tickCount = Environment.TickCount - tickCount;
            }
			catch (Exception exception)
            {
				Console.WriteLine("OnlineGameCheck: " + exception);
            }
        }

		public virtual void LoadFromDatabase()
		{
			if (!m_saveToDb)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				m_Info = playerBussiness.GetSingleUsersExtra(m_player.PlayerCharacter.ID);
				if (m_Info == null)
				{
					m_Info = CreateUserExtra(Player.PlayerCharacter.ID);
				}
				dictionary_0 = new Dictionary<int, EventRewardProcessInfo>();
				EventRewardProcessInfo[] userEventProcess = playerBussiness.GetUserEventProcess(m_player.PlayerCharacter.ID);
				foreach (EventRewardProcessInfo rewardProcessInfo in userEventProcess)
				{
					if (!dictionary_0.ContainsKey(rewardProcessInfo.ActiveType))
					{
						dictionary_0.Add(rewardProcessInfo.ActiveType, rewardProcessInfo);
					}
				}
			}
		}

		private EventRewardProcessInfo method_0(int int_0)
		{
			return new EventRewardProcessInfo
			{
				UserID = m_player.PlayerCharacter.ID,
				ActiveType = int_0,
				Conditions = 0,
				AwardGot = 0
			};
		}

		public virtual void SaveToDatabase()
		{
			if (!m_saveToDb)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				lock (m_lock)
				{
					if (m_Info != null && m_Info.IsDirty)
					{
						playerBussiness.UpdateUserExtra(m_Info);
					}
				}
			}
		}

		public void StopHotSpringTimer()
		{
			if (_hotSpringTimer != null)
			{
				_hotSpringTimer.Dispose();
				_hotSpringTimer = null;
			}
		}

		public void StopOnlineGameTimer()
        {
			if (_onlineGameTimer != null)
            {
				_onlineGameTimer.Dispose();
				_onlineGameTimer = null;
            }
        }

		public void StopAllTimer()
        {
			StopHotSpringTimer();
			StopOnlineGameTimer();
        }

		public void UpdateEventCondition(int activeType, int value)
		{
			UpdateEventCondition(activeType, value, isPlus: false, 0);
		}

		public void UpdateEventCondition(int activeType, int value, bool isPlus, int awardGot)
		{
			PlayerBussiness playerBussiness = new PlayerBussiness();
			EventRewardProcessInfo eventProcess = GetEventProcess(activeType);
			if (eventProcess == null)
			{
				eventProcess = method_0(activeType);
			}
			if (isPlus)
			{
				eventProcess.Conditions += value;
			}
			else if (eventProcess.Conditions < value)
			{
				eventProcess.Conditions = value;
			}
			if (awardGot != 0)
			{
				eventProcess.AwardGot = awardGot;
			}
			DateTime now = DateTime.Now;
			DateTime endTime = DateTime.Now.AddYears(2);
			playerBussiness.UpdateUsersEventProcess(eventProcess);
			m_player.Out.SendOpenNoviceActive(0, activeType, eventProcess.Conditions, eventProcess.AwardGot, now, endTime);
		}

		public bool CanRingExp(int type)
		{
			if (Info == null)
				return false;

			switch (type)
			{
				case -1:
					return true;
				case 1:
					return Info.dungeonNum < DUNGEON_NUM;
				case 2:
					return Info.propsNum < PROPS_NUM;
				default:
					return Info.coupleNum < COUPLE_NUM;
			}
		}

		public void UpdateLoveScoreLimit(int type)
		{
			if (Info == null)
				return;

			switch (type)
			{
				case 1:
					if (Info.dungeonNum < DUNGEON_NUM)
						Info.dungeonNum++;
					break;
				case 2:
					if (Info.propsNum < PROPS_NUM)
						Info.propsNum++;
					break;
				default:
					if (Info.coupleNum < COUPLE_NUM)
						Info.coupleNum++;
					break;
			}
		}
	}
}
