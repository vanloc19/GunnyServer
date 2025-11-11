using Bussiness;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server;
using Game.Server.GameObjects;
using Game.Server.GuildBattle.Action;
using Game.Server.Managers;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Game.Server.GuildBattle
{
	[Serializable]
	public class GuildBattleMgr
	{
		[NonSerialized]
		private readonly static ILog ilog_0;

		private static ArrayList arrayList_0;

		private long long_0;

		public int CurrentActionCount;

		private long long_1;

		public GuildBattleState State;

		public DateTime TimeStart;

		public DateTime TimeStop;

		public readonly DateTime GuildBattleStartTime;

		public readonly List<Point> GuildPointDefault;

		private Dictionary<int, UserGuildBattleInfo> dictionary_0;

		private Dictionary<int, GuildBattleConsortiaInfo> dictionary_1;

		private object object_0;

		public bool IsOpen
		{
			get;
			set;
		}

		public bool IsSendAward
		{
			get;
			set;
		}

		public bool IsSendMail
		{
			get;
			set;
		}

		public DateTime LastDateReloadTop
		{
			get;
			set;
		}

		static GuildBattleMgr()
		{

			GuildBattleMgr.ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public GuildBattleMgr()
		{

			this.GuildBattleStartTime = DateTime.Parse(GameProperties.GuildBattleStartTime);
			this.GuildPointDefault = new List<Point>()
			{
				new Point(649, 954),
				new Point(988, 584),
				new Point(1643, 611),
				new Point(2359, 600),
				new Point(2503, 987),
				new Point(2086, 1512),
				new Point(1599, 1445),
				new Point(868, 1362)
			};
			this.dictionary_0 = new Dictionary<int, UserGuildBattleInfo>();
			this.dictionary_1 = new Dictionary<int, GuildBattleConsortiaInfo>();
			this.object_0 = new object();

			GuildBattleMgr.arrayList_0 = new ArrayList();
			this.State = GuildBattleState.CLOSE;
			this.dictionary_0 = new Dictionary<int, UserGuildBattleInfo>();
			this.dictionary_1 = new Dictionary<int, GuildBattleConsortiaInfo>();
			this.IsSendMail = false;
			this.IsSendAward = false;
			this.TimeStart = DateTime.Now;
			this.TimeStop = DateTime.Now;
		}

		public void AddAction(GInterface13 action)
		{
			lock (GuildBattleMgr.arrayList_0)
			{
				GuildBattleMgr.arrayList_0.Add(action);
			}
		}

		public void AddAction(ArrayList actions)
		{
			lock (GuildBattleMgr.arrayList_0)
			{
				GuildBattleMgr.arrayList_0.AddRange(actions);
			}
		}

		public void AddCountDownRevive(UserGuildBattleInfo user, int timeout)
		{
			user.IsDead = true;
			user.TombStoneEndTime = DateTime.Now.AddSeconds((double)timeout);
			if (!this.CheckAction(typeof(PlayerReviveCountDownAction)))
			{
				this.AddAction(new PlayerReviveCountDownAction(user, timeout * 1000, false));
			}
		}

		public bool AddPlayer(GamePlayer p)
		{
			bool flag = false;
			GuildBattleConsortiaInfo guildBattleConsortiaInfo = this.FindConsortia(p.PlayerCharacter.ConsortiaID);
			if (p.PlayerCharacter.ConsortiaID != 0 && guildBattleConsortiaInfo != null && this.State == GuildBattleState.OPEN)
			{
				UserGuildBattleInfo userGuildBattleInfo = this.FindUser(p.PlayerId);
				if (userGuildBattleInfo == null)
				{
					userGuildBattleInfo = new UserGuildBattleInfo(p);
					flag = this.AddUser(userGuildBattleInfo);
				}
				else
				{
					userGuildBattleInfo.NickName = p.PlayerCharacter.NickName;
					userGuildBattleInfo.IsActive = true;
					userGuildBattleInfo.Player = p;
					flag = true;
				}
				userGuildBattleInfo.Postion = guildBattleConsortiaInfo.DefaultPoint;
				if (p.PlayerCharacter.ReduceStartBlood <= 0)
				{
					p.PlayerCharacter.ReduceStartBlood = p.PlayerCharacter.hp;
				}
				if (flag)
				{
					this.SendInitSeftInfo(userGuildBattleInfo);
				}
			}
			return flag;
		}

		public void AddScore(UserGuildBattleInfo p, int endStack)
		{
			int num = 0;
			if (p.WinStreak >= 3 && p.WinStreak < 6)
			{
				num += 50;
			}
			else if (p.WinStreak >= 6 && p.WinStreak < 10)
			{
				num += 70;
			}
			else if (p.WinStreak < 10)
			{
				num += 30;
			}
			else
			{
				num += 110;
			}
			if (endStack >= 6 && endStack < 10)
			{
				num += 50;
			}
			else if (endStack == 10)
			{
				num += 70;
			}
			else if (endStack > 10)
			{
				num += 90;
			}
			if (p.DupeScoreConsortiaBattle)
			{
				num *= 2;
			}
			UserGuildBattleInfo score = p;
			score.Score = score.Score + num;
			GuildBattleConsortiaInfo guildBattleConsortiaInfo = this.FindConsortia(p.ConsortiaID);
			guildBattleConsortiaInfo.Score = guildBattleConsortiaInfo.Score + num;
		}

		public bool AddUser(UserGuildBattleInfo user)
		{
			bool flag;
			lock (this.dictionary_0)
			{
				if (this.dictionary_0.ContainsKey(user.UserID))
				{
					flag = false;
				}
				else
				{
					this.dictionary_0[user.UserID] = user;
					flag = true;
				}
			}
			return flag;
		}

		public bool CanEnterGame(int consortiaId)
		{
			if (this.FindConsortia(consortiaId) != null && this.State == GuildBattleState.OPEN)
			{
				return true;
			}
			return false;
		}

		public bool CanStartGame(DateTime now)
		{
			TimeSpan timeOfDay = this.GuildBattleStartTime.TimeOfDay;
			if (timeOfDay <= now.TimeOfDay && now.TimeOfDay.Hours < timeOfDay.Hours + 1)
			{
				return true;
			}
			return false;
		}

		public void ChallengeGame(GamePlayer p1, GamePlayer p2)
		{
			lock (this.object_0)
			{
				if (p1.CurrentRoom == null || p2.CurrentRoom == null || p1.CurrentRoom.RoomType != eRoomType.ConsortiaBattle || p2.CurrentRoom.RoomType != eRoomType.ConsortiaBattle || p1.CurrentRoom.IsPlaying || p2.CurrentRoom.IsPlaying)
				{
					Console.WriteLine($"TH1");
					p1.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.RoomStillWaiting"));
				}
				else
				{
                    Console.WriteLine($"TH2");
                    UserGuildBattleInfo userGuildBattleInfo = this.FindUser(p1.PlayerId);
					UserGuildBattleInfo userGuildBattleInfo1 = this.FindUser(p2.PlayerId);
					if (userGuildBattleInfo == null || userGuildBattleInfo1 == null || userGuildBattleInfo.Status != UserGuildBattleStatus.NORMAL || userGuildBattleInfo1.Status != UserGuildBattleStatus.NORMAL || userGuildBattleInfo.IsDead || userGuildBattleInfo1.IsDead)
					{
						Console.WriteLine($"{userGuildBattleInfo.NickName}_{userGuildBattleInfo.Status}_{userGuildBattleInfo.IsDead}");
                        Console.WriteLine($"{userGuildBattleInfo1.NickName}_{userGuildBattleInfo1.Status}_{userGuildBattleInfo1.IsDead}");
                        p1.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.RoomStillWaiting"));
					}
					else
					{
						userGuildBattleInfo.Status = UserGuildBattleStatus.FIGHTING;
						userGuildBattleInfo1.Status = UserGuildBattleStatus.FIGHTING;
						if (userGuildBattleInfo.FailBuffCount < 1)
						{
							userGuildBattleInfo.Player.PlayerCharacter.AgiPlusGuildBattle = 0;
							userGuildBattleInfo.Player.PlayerCharacter.AttPlusGuildBattle = 0;
						}
						else
						{
							userGuildBattleInfo.Player.PlayerCharacter.AgiPlusGuildBattle = 30;
							userGuildBattleInfo.Player.PlayerCharacter.AttPlusGuildBattle = 30;
						}
						if (userGuildBattleInfo1.FailBuffCount < 1)
						{
							userGuildBattleInfo1.Player.PlayerCharacter.AgiPlusGuildBattle = 0;
							userGuildBattleInfo1.Player.PlayerCharacter.AttPlusGuildBattle = 0;
						}
						else
						{
							userGuildBattleInfo1.Player.PlayerCharacter.AgiPlusGuildBattle = 30;
							userGuildBattleInfo1.Player.PlayerCharacter.AttPlusGuildBattle = 30;
						}
						this.SendUpdatePlayerStatus(userGuildBattleInfo);
						this.SendUpdatePlayerStatus(userGuildBattleInfo1);
						RoomMgr.StartConsortiaBattle(p1.CurrentRoom, p2.CurrentRoom);
					}
				}
			}
		}

		public void ChangeOpenClose(bool _isOpen)
		{
			this.ChangeOpenClose(_isOpen, DateTime.Now);
		}

		public void ChangeOpenClose(bool _isOpen, DateTime timeEnd)
		{
			if (!_isOpen)
			{
				this.State = GuildBattleState.CLOSE;
				this.IsSendMail = false;
			}
			else
			{
				this.State = GuildBattleState.OPEN;
				this.IsSendAward = false;
				this.dictionary_0 = new Dictionary<int, UserGuildBattleInfo>();
				if (this.LastDateReloadTop.Date != DateTime.Now.Date)
				{
					//this.InstallConsortiaList(this.GetTOPConsortiaDayOnline());
					this.InstallConsortiaList(this.GetTOPConsortiaWeekRiches());
				}
			}
			this.IsOpen = _isOpen;
			this.TimeStart = DateTime.Now;
			this.TimeStop = timeEnd;
			this.SendAllOpenClose();
		}

		public bool CheckAction(GInterface13 action)
		{
			bool flag;
			lock (GuildBattleMgr.arrayList_0)
			{
				flag = (!GuildBattleMgr.arrayList_0.Contains(action) ? false : true);
			}
			return flag;
		}

		public bool CheckAction(Type type)
		{
			bool flag;
			lock (GuildBattleMgr.arrayList_0)
			{
				flag = (GuildBattleMgr.arrayList_0.ToArray().SingleOrDefault<object>((object a) => a.GetType().Equals(type)) == null ? false : true);
			}
			return flag;
		}

		public void CheckState(int delay)
		{
			this.AddAction(new CheckGuildBattleStateAction(delay));
		}

		public GuildBattleConsortiaInfo FindConsortia(int consortiaId)
		{
			GuildBattleConsortiaInfo item;
			lock (this.dictionary_1)
			{
				if (!this.dictionary_1.ContainsKey(consortiaId))
				{
					item = null;
				}
				else
				{
					item = this.dictionary_1[consortiaId];
				}
			}
			return item;
		}

		public UserGuildBattleInfo FindUser(int userid)
		{
			UserGuildBattleInfo item;
			lock (this.dictionary_0)
			{
				if (!this.dictionary_0.ContainsKey(userid))
				{
					item = null;
				}
				else
				{
					item = this.dictionary_0[userid];
				}
			}
			return item;
		}

		public UserGuildBattleInfo[] GetAllActiveUser()
		{
			return (
				from a in this.dictionary_0.Values
				where a.IsActive
				select a).ToArray<UserGuildBattleInfo>();
		}

		public UserGuildBattleInfo[] GetAllActiveUser(int consortiaId)
		{
			return this.dictionary_0.Values.Where<UserGuildBattleInfo>((UserGuildBattleInfo a) => {
				if (!a.IsActive)
				{
					return false;
				}
				return a.ConsortiaID == consortiaId;
			}).ToArray<UserGuildBattleInfo>();
		}

		public GuildBattleConsortiaInfo[] GetAllConsortia()
		{
			return this.dictionary_1.Values.ToArray<GuildBattleConsortiaInfo>();
		}

		public UserGuildBattleInfo[] GetAllUser()
		{
			return this.dictionary_0.Values.ToArray<UserGuildBattleInfo>();
		}

		public ConsortiaInfo[] GetTOPConsortiaDayOnline()
		{
			ConsortiaInfo[] rankDayOnlineConsortias = null;
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				rankDayOnlineConsortias = consortiaBussiness.GetRankDayOnlineConsortias();
			}
			return rankDayOnlineConsortias;
		}

		public ConsortiaInfo[] GetTOPConsortiaDayRiches()
		{
			ConsortiaInfo[] rankDayRichesConsortias = null;
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				rankDayRichesConsortias = consortiaBussiness.GetRankDayRichesConsortias();
			}
			return rankDayRichesConsortias;
		}

		public ConsortiaInfo[] GetTOPConsortiaWeekRiches()
		{
			ConsortiaInfo[] rankWeekRichesConsortias = null;
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				rankWeekRichesConsortias = consortiaBussiness.GetRankWeekRichesConsortias();
			}
			return rankWeekRichesConsortias;
		}

		public UserGuildBattleInfo[] GetUserByConsortia(int consortiaId)
		{
			return (
				from a in this.dictionary_0.Values
				where a.ConsortiaID == consortiaId
				select a).ToArray<UserGuildBattleInfo>();
		}

		public string InstallConsortiaList(ConsortiaInfo[] list)
		{
			List<string> strs = new List<string>();
			lock (this.dictionary_1)
			{
				this.dictionary_1.Clear();
				int num = 0;
				ConsortiaInfo[] consortiaInfoArray = list;
				for (int i = 0; i < (int)consortiaInfoArray.Length; i++)
				{
					ConsortiaInfo consortiaInfo = consortiaInfoArray[i];
					if (!this.dictionary_1.ContainsKey(consortiaInfo.ConsortiaID))
					{
						GuildBattleConsortiaInfo guildBattleConsortiaInfo = new GuildBattleConsortiaInfo()
						{
							ConsortiaID = consortiaInfo.ConsortiaID,
							ConsortiaName = consortiaInfo.ConsortiaName,
							Rank = 0,
							Score = 0,
							DefaultPoint = this.GuildPointDefault[num]
						};
						this.dictionary_1.Add(guildBattleConsortiaInfo.ConsortiaID, guildBattleConsortiaInfo);
						strs.Add(consortiaInfo.ConsortiaName);
						num++;
					}
				}
			}
			this.LastDateReloadTop = DateTime.Now;
			return string.Join(",", strs.ToArray());
		}

		private void method_0(byte byte_0, int int_0, string string_0, string string_1)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(19);
			gSPacketIn.WriteByte(byte_0);
			if (byte_0 != 1)
			{
				gSPacketIn.WriteInt(int_0);
				gSPacketIn.WriteString(string_1);
				gSPacketIn.WriteString(string_0);
			}
			else
			{
				gSPacketIn.WriteInt(int_0);
				gSPacketIn.WriteString(string_0);
			}
			this.SendToAll(gSPacketIn);
		}

		private GSPacketIn method_1(bool CanEnter)
		{
			if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
				CanEnter = true;
			else
				CanEnter = false;

			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteBoolean(IsOpen);
			gSPacketIn.WriteDateTime(TimeStart);
			gSPacketIn.WriteDateTime(TimeStop);
			gSPacketIn.WriteBoolean(CanEnter);
			return gSPacketIn;
		}

		public void QuickRevive(UserGuildBattleInfo user, bool stay)
		{
			this.AddAction(new PlayerReviveCountDownAction(user, 0, stay));
		}

		public void RemoveAllPlayerInRoom()
		{
			lock (this.dictionary_0)
			{
				foreach (UserGuildBattleInfo value in this.dictionary_0.Values)
				{
					if (value != null && value.IsActive && value.Player != null && value.Player.CurrentRoom != null)
					{
						value.Player.CurrentRoom.RemovePlayerUnsafe(value.Player);
					}
					value.IsActive = false;
					value.Player = null;
				}
			}
		}

		public void RemovePlayer(GamePlayer p)
		{
			UserGuildBattleInfo userGuildBattleInfo = this.FindUser(p.PlayerId);
			if (userGuildBattleInfo != null)
			{
				this.RemovePlayer(userGuildBattleInfo);
			}
		}

		public void RemovePlayer(UserGuildBattleInfo p)
		{
			this.SendRemovePlayer(p);
			p.IsActive = false;
			p.WinStreak = 0;
			p.FailBuffCount = 0;
			p.LostCount = 0;
			p.Status = UserGuildBattleStatus.NORMAL;
		}

		public void SaveCurrentRankToDatabase(bool clear)
		{
			int i;
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (clear)
				{
					consortiaBussiness.RemoveAllConsortiaBattleRank();
					consortiaBussiness.RemoveAllConsortiaBattlePlayerRank();
				}
				GuildBattleConsortiaInfo[] array = (
					from a in (IEnumerable<GuildBattleConsortiaInfo>)this.GetAllConsortia()
					orderby a.Score descending
					select a).ToArray<GuildBattleConsortiaInfo>();
				UserGuildBattleInfo[] userGuildBattleInfoArray = (
					from a in (IEnumerable<UserGuildBattleInfo>)this.GetAllUser()
					orderby a.Score descending
					select a).ToArray<UserGuildBattleInfo>();
				int num = 1;
				GuildBattleConsortiaInfo[] guildBattleConsortiaInfoArray = array;
				for (i = 0; i < (int)guildBattleConsortiaInfoArray.Length; i++)
				{
					GuildBattleConsortiaInfo guildBattleConsortiaInfo = guildBattleConsortiaInfoArray[i];
					ConsortiaWarRankInfo consortiaWarRankInfo = new ConsortiaWarRankInfo()
					{
						ConsortiaID = guildBattleConsortiaInfo.ConsortiaID,
						ConsortiaName = guildBattleConsortiaInfo.ConsortiaName,
						Rank = num,
						Score = guildBattleConsortiaInfo.Score,
						TimeCreate = DateTime.Now
					};
					consortiaBussiness.AddConsortiaBattleRank(consortiaWarRankInfo);
					num++;
				}
				UserGuildBattleInfo[] userGuildBattleInfoArray1 = userGuildBattleInfoArray;
				for (i = 0; i < (int)userGuildBattleInfoArray1.Length; i++)
				{
					UserGuildBattleInfo userGuildBattleInfo = userGuildBattleInfoArray1[i];
					ConsortiaWarPlayerRankInfo consortiaWarPlayerRankInfo = new ConsortiaWarPlayerRankInfo()
					{
						UserID = userGuildBattleInfo.UserID,
						NickName = userGuildBattleInfo.NickName,
						ZoneID = GameServer.Instance.Configuration.ZoneId,
						ZoneName = GameServer.Instance.Configuration.ZoneName,
						ConsortiaID = userGuildBattleInfo.ConsortiaID,
						Score = userGuildBattleInfo.Score,
						TimeCreate = DateTime.Now
					};
					consortiaBussiness.AddConsortiaBattlePlayerRank(consortiaWarPlayerRankInfo);
				}
				if (clear)
				{
					this.dictionary_1 = new Dictionary<int, GuildBattleConsortiaInfo>();
					this.dictionary_0 = new Dictionary<int, UserGuildBattleInfo>();
				}
			}
		}

		public void SendAllOpenClose()
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GuildBattleConsortiaInfo[] allConsortia = this.GetAllConsortia();
			GamePlayer[] gamePlayerArray = allPlayers;
			for (int i = 0; i < (int)gamePlayerArray.Length; i++)
			{
				GamePlayer gamePlayer = gamePlayerArray[i];
				GSPacketIn gSPacketIn = this.method_1(allConsortia.SingleOrDefault<GuildBattleConsortiaInfo>((GuildBattleConsortiaInfo a) => a.ConsortiaID == gamePlayer.PlayerCharacter.ConsortiaID) != null);
				gamePlayer.SendTCP(gSPacketIn);
			}
		}

		public void SendAllPlayerList(UserGuildBattleInfo ex, bool toAll)
		{
			UserGuildBattleInfo[] allActiveUser = this.GetAllActiveUser();
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt((int)allActiveUser.Length);
			UserGuildBattleInfo[] userGuildBattleInfoArray = allActiveUser;
			for (int i = 0; i < (int)userGuildBattleInfoArray.Length; i++)
			{
				UserGuildBattleInfo userGuildBattleInfo = userGuildBattleInfoArray[i];
				gSPacketIn.WriteInt(userGuildBattleInfo.UserID);
				gSPacketIn.WriteDateTime(userGuildBattleInfo.TombStoneEndTime);
				gSPacketIn.WriteByte((byte)userGuildBattleInfo.Status);
				gSPacketIn.WriteInt(userGuildBattleInfo.Postion.X);
				gSPacketIn.WriteInt(userGuildBattleInfo.Postion.Y);
				gSPacketIn.WriteBoolean(userGuildBattleInfo.Player.PlayerCharacter.Sex);
				gSPacketIn.WriteInt(userGuildBattleInfo.ConsortiaID);
				gSPacketIn.WriteString(userGuildBattleInfo.ConsortiaName);
				gSPacketIn.WriteInt(userGuildBattleInfo.WinStreak);
				gSPacketIn.WriteInt(userGuildBattleInfo.FailBuffCount);
			}
			if (toAll)
			{
				this.SendToAll(gSPacketIn, ex);
				return;
			}
			ex.Player.SendTCP(gSPacketIn);
		}

		public void SendInitSeftInfo(UserGuildBattleInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteBoolean(true);
			gSPacketIn.WriteDateTime(user.TombStoneEndTime);
			gSPacketIn.WriteInt(user.Postion.X);
			gSPacketIn.WriteInt(user.Postion.Y);
			gSPacketIn.WriteInt(user.Player.PlayerCharacter.ReduceStartBlood);
			gSPacketIn.WriteInt(user.VictoryCount);
			gSPacketIn.WriteInt(user.WinStreak);
			gSPacketIn.WriteInt(user.Score);
			gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.ActivePowFirstGame);
			gSPacketIn.WriteBoolean(user.DupeScoreConsortiaBattle);
			user.Player.SendTCP(gSPacketIn);
		}

		public void SendPlayerMove(UserGuildBattleInfo user, string moveStr)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(user.UserID);
			gSPacketIn.WriteInt(user.Postion.X);
			gSPacketIn.WriteInt(user.Postion.Y);
			gSPacketIn.WriteString(moveStr);
			this.SendToAll(gSPacketIn, user);
		}

		public void SendRemovePlayer(UserGuildBattleInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteInt(user.UserID);
			this.SendToAll(gSPacketIn);
		}

		public void SendSingleOpenClose(GamePlayer p)
		{
			GSPacketIn gSPacketIn = this.method_1(this.FindConsortia(p.PlayerCharacter.ConsortiaID) != null);
			p.SendTCP(gSPacketIn);
		}

		public void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}

		public void SendToAll(GSPacketIn pkg, UserGuildBattleInfo except)
		{
			UserGuildBattleInfo[] allActiveUser = this.GetAllActiveUser();
			for (int i = 0; i < (int)allActiveUser.Length; i++)
			{
				UserGuildBattleInfo userGuildBattleInfo = allActiveUser[i];
				if (userGuildBattleInfo != null && userGuildBattleInfo != except)
				{
					userGuildBattleInfo.Player.SendTCP(pkg);
				}
			}
		}

		public void SendToTeam(GSPacketIn pkg, int consortiaId)
		{
			UserGuildBattleInfo[] allActiveUser = this.GetAllActiveUser(consortiaId);
			for (int i = 0; i < (int)allActiveUser.Length; i++)
			{
				UserGuildBattleInfo userGuildBattleInfo = allActiveUser[i];
				if (userGuildBattleInfo != null && userGuildBattleInfo.Player != null)
				{
					userGuildBattleInfo.Player.SendTCP(pkg);
				}
			}
		}

		public void SendToTeam(GSPacketIn pkg, UserGuildBattleInfo except)
		{
			UserGuildBattleInfo[] allActiveUser = this.GetAllActiveUser(except.ConsortiaID);
			for (int i = 0; i < (int)allActiveUser.Length; i++)
			{
				UserGuildBattleInfo userGuildBattleInfo = allActiveUser[i];
				if (userGuildBattleInfo != null && userGuildBattleInfo.Player != null && userGuildBattleInfo != except)
				{
					userGuildBattleInfo.Player.SendTCP(pkg);
				}
			}
		}

		public void SendUpdatePlayerStatus(UserGuildBattleInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(7);
			gSPacketIn.WriteInt(user.UserID);
			gSPacketIn.WriteDateTime(user.TombStoneEndTime);
			gSPacketIn.WriteByte((byte)user.Status);
			gSPacketIn.WriteInt(user.Postion.X);
			gSPacketIn.WriteInt(user.Postion.Y);
			gSPacketIn.WriteInt(user.WinStreak);
			gSPacketIn.WriteInt(user.FailBuffCount);
			this.SendToAll(gSPacketIn);
		}

		public void SendUpdateSceneInfo(UserGuildBattleInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(9);
			gSPacketIn.WriteInt(user.Player.PlayerCharacter.ReduceStartBlood);
			gSPacketIn.WriteInt(user.VictoryCount);
			gSPacketIn.WriteInt(user.WinStreak);
			gSPacketIn.WriteInt(user.Score);
			gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.ActivePowFirstGame);
			gSPacketIn.WriteBoolean(user.DupeScoreConsortiaBattle);
			gSPacketIn.WriteDateTime(user.TombStoneEndTime);
			user.Player.SendTCP(gSPacketIn);
		}

		public void SendUpdateScore(UserGuildBattleInfo user, byte type)
		{
			int i;
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(16);
			gSPacketIn.WriteByte(type);
			if (type != 1)
			{
				UserGuildBattleInfo[] array = (
					from a in (IEnumerable<UserGuildBattleInfo>)this.GetUserByConsortia(user.ConsortiaID)
					orderby a.Score descending
					select a).ToArray<UserGuildBattleInfo>();
				gSPacketIn.WriteInt((int)array.Length);
				int num = 1;
				UserGuildBattleInfo[] userGuildBattleInfoArray = array;
				for (i = 0; i < (int)userGuildBattleInfoArray.Length; i++)
				{
					UserGuildBattleInfo userGuildBattleInfo = userGuildBattleInfoArray[i];
					gSPacketIn.WriteString(userGuildBattleInfo.NickName);
					gSPacketIn.WriteInt(num);
					gSPacketIn.WriteInt(userGuildBattleInfo.Score);
					num++;
				}
			}
			else
			{
				GuildBattleConsortiaInfo[] guildBattleConsortiaInfoArray = (
					from a in (IEnumerable<GuildBattleConsortiaInfo>)this.GetAllConsortia()
					orderby a.Score descending
					select a).ToArray<GuildBattleConsortiaInfo>();
				gSPacketIn.WriteInt((int)guildBattleConsortiaInfoArray.Length);
				int num1 = 1;
				GuildBattleConsortiaInfo[] guildBattleConsortiaInfoArray1 = guildBattleConsortiaInfoArray;
				for (i = 0; i < (int)guildBattleConsortiaInfoArray1.Length; i++)
				{
					GuildBattleConsortiaInfo guildBattleConsortiaInfo = guildBattleConsortiaInfoArray1[i];
					gSPacketIn.WriteString(guildBattleConsortiaInfo.ConsortiaName);
					gSPacketIn.WriteInt(num1);
					gSPacketIn.WriteInt(guildBattleConsortiaInfo.Score);
					num1++;
				}
			}
			user.Player.SendTCP(gSPacketIn);
		}

		public void Update(long tick)
		{
			ArrayList arrayLists;
			if (this.long_0 >= tick)
			{
				return;
			}
			lock (GuildBattleMgr.arrayList_0)
			{
				arrayLists = (ArrayList)GuildBattleMgr.arrayList_0.Clone();
				GuildBattleMgr.arrayList_0.Clear();
			}
			this.CurrentActionCount = arrayLists.Count;
			if (arrayLists.Count <= 0)
			{
				if (this.long_1 < tick)
				{
					this.CheckState(0);
				}
				return;
			}
			ArrayList arrayLists1 = new ArrayList();
			foreach (GInterface13 gInterface13 in arrayLists)
			{
				try
				{
					gInterface13.Execute(this, tick);
					if (!gInterface13.IsFinished(tick))
					{
						arrayLists1.Add(gInterface13);
					}
				}
				catch (Exception exception)
				{
					GuildBattleMgr.ilog_0.Error("Map update error:", exception);
				}
			}
			this.AddAction(arrayLists1);
		}

		public void UpdateScoreMatch(int winId, int lostId)
		{
			UserGuildBattleInfo userGuildBattleInfo = this.FindUser(winId);
			UserGuildBattleInfo userGuildBattleInfo1 = this.FindUser(lostId);
			if (userGuildBattleInfo != null && userGuildBattleInfo1 != null && (!userGuildBattleInfo.IsActive || userGuildBattleInfo.Status == UserGuildBattleStatus.FIGHTING) && (!userGuildBattleInfo1.IsActive || userGuildBattleInfo1.Status == UserGuildBattleStatus.FIGHTING))
			{
				userGuildBattleInfo.Status = UserGuildBattleStatus.NORMAL;
				userGuildBattleInfo1.Status = UserGuildBattleStatus.NORMAL;
				int winStreak = userGuildBattleInfo1.WinStreak;
				if (winStreak >= 3)
				{
					this.method_0(2, winStreak, userGuildBattleInfo.NickName, userGuildBattleInfo1.NickName);
				}
				UserGuildBattleInfo winStreak1 = userGuildBattleInfo;
				winStreak1.WinStreak = winStreak1.WinStreak + 1;
				UserGuildBattleInfo victoryCount = userGuildBattleInfo;
				victoryCount.VictoryCount = victoryCount.VictoryCount + 1;
				userGuildBattleInfo.FailBuffCount = 0;
				userGuildBattleInfo.LostCount = 0;
				userGuildBattleInfo1.WinStreak = 0;
				UserGuildBattleInfo lostCount = userGuildBattleInfo1;
				lostCount.LostCount = lostCount.LostCount + 1;
				if (userGuildBattleInfo1.LostCount >= 3)
				{
					userGuildBattleInfo1.FailBuffCount = 1;
				}
				if (userGuildBattleInfo.WinStreak == 3 || userGuildBattleInfo.WinStreak == 6 || userGuildBattleInfo.WinStreak >= 10)
				{
					this.method_0(1, winStreak, userGuildBattleInfo.NickName, userGuildBattleInfo1.NickName);
				}
				this.AddScore(userGuildBattleInfo, winStreak);
			}
		}

		public void WaitTime(int delay)
		{
			this.long_1 = Math.Max(this.long_1, TickHelper.GetTickCount() + (long)delay);
		}
	}
}