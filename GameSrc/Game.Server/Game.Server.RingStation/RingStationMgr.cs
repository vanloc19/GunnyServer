using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.RingStation.Battle;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.RingStation
{
	public sealed class RingStationMgr
	{
		private static RingstationConfigInfo _ringstationConfigInfo;

		private static VirtualPlayerInfo _virtualPlayerInfo = new VirtualPlayerInfo();

		private static Dictionary<int, UserRingStationInfo> dictionary_1 = new Dictionary<int, UserRingStationInfo>();

		private static Dictionary<int, List<RingstationBattleFieldInfo>> dictionary_2 = new Dictionary<int, List<RingstationBattleFieldInfo>>();

		private static Func<RingstationBattleFieldInfo, DateTime> func_0;

		private static Func<UserRingStationInfo, bool> func_1;

		private static Func<UserRingStationInfo, int> func_2;

		private static Func<UserRingStationInfo, int> func_3;

		private static Func<UserRingStationInfo, bool> func_4;

		private static Func<UserRingStationInfo, int> func_5;

		private static int int_0 = 10;

		private static List<VirtualPlayerInfo> list_0 = new List<VirtualPlayerInfo>();

		private static List<UserRingStationInfo> list_1 = new List<UserRingStationInfo>();

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

		protected static object m_lock = new object();

		private static Dictionary<int, RingStationGamePlayer> m_players = new Dictionary<int, RingStationGamePlayer>();

		private static RingStationBattleServer m_server = null;

		protected static Timer m_statusScanTimer;

		private static string[] NickName;

		private static ThreadSafeRandom rand = new ThreadSafeRandom();

		private static readonly string weaklessGuildProgressStr = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

		private static readonly string[] weapons = new string[15]
		{
			"7017|Sdart",
			"7018|Selectricbar",
			"7019|Sbrick",
			"7016|Sbomb",
			"7008|dart",
			"7020|Sfruit",
			"7021|SOPSbox",
			"7020|Sfruit",
			"7046|Scar",
			"7021|SOPSbox",
			"7016|Sbomb",
			"7056|bazooka",
			"7024|boomerang",
			"7009|electricbar",
			"7027|ghostaxe"
		};

		public static RingstationConfigInfo ConfigInfo => _ringstationConfigInfo;

		public static VirtualPlayerInfo NormalPlayer
		{
			get
			{
				return _virtualPlayerInfo;
			}
			set
			{
				_virtualPlayerInfo = value;
			}
		}

		public static bool AddPlayer(int playerId, RingStationGamePlayer player)
		{
			m_clientLocker.AcquireWriterLock(-1);
			try
			{
				if (m_players.ContainsKey(playerId))
				{
					return false;
				}
				m_players.Add(playerId, player);
			}
			finally
			{
				m_clientLocker.ReleaseWriterLock();
			}
			return true;
		}

		public static void BeginTimer()
		{
			int dueTime = 60000;
			if (m_statusScanTimer == null)
			{
				m_statusScanTimer = new Timer(StatusScan, null, dueTime, dueTime);
			}
			else
			{
				m_statusScanTimer.Change(dueTime, dueTime);
			}
		}

		public static void CreateAutoBot(int roomtype, int gametype, int npcId)
		{
			BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId())
			{
				RoomType = roomtype,
				GameType = gametype,
				PickUpNpcId = npcId,
				IsAutoBot = true,
				IsFreedom = true
			};
			RingStationGamePlayer player = new RingStationGamePlayer();
			string str = RingStationConfiguration.RandomName[rand.Next(0, RingStationConfiguration.RandomName.Length - 1)];
			player.NickName = str + "1";
			player.GP = 1283;
			player.Grade = 5;
			player.Attack = 100;
			player.Defence = 100;
			player.Luck = 100;
			player.Agility = 100;
			player.hp = 1000;
			player.FightPower = 1200;
			player.BaseAttack = 100.0;
			player.BaseDefence = 50.0;
			player.BaseAgility = 1.0 - (double)player.Agility * 0.001;
			player.BaseBlood = 1000.0;
			string str2 = weapons[rand.Next(weapons.Length)];
			player.Style = GetAutoBotStyleRandom(str2);
			player.Colors = ",,,,,,,,,,,,,,,";
			player.Hide = 1111111111;
			player.TemplateID = int.Parse(str2.Split('|')[0]);
			player.StrengthLevel = 0;
			player.WeaklessGuildProgressStr = "R/O/DeABAtgWdWsIAAAAAAAAgCAECwAAAAAAABgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
			player.ID = npcId;
			if (m_server != null)
			{
				AddPlayer(player.ID, player);
				room.AddPlayer(player);
				m_server.AddRoom(room);
			}
			Console.WriteLine("Running AutoBot Default");
		}

		public static int GetAutoBot(GamePlayer player, int roomtype, int gametype)
		{
			int playerId = RingStationConfiguration.NextPlayerID();
			BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId());
			room.RoomType = roomtype;
			room.GameType = gametype;
			room.PickUpNpcId = playerId;
			room.IsAutoBot = true;
			room.IsFreedom = false;
			RingStationGamePlayer player2 = new RingStationGamePlayer();
			player2.NickName = RingStationConfiguration.RandomName[rand.Next(0, RingStationConfiguration.RandomName.Length)];
			player2.GP = player.PlayerCharacter.GP;
			player2.Grade = player.PlayerCharacter.Grade;
			player2.Attack = player.PlayerCharacter.Attack / 2;
			player2.Defence = player.PlayerCharacter.Defence / 2;
			player2.Luck = player.PlayerCharacter.Luck / 2;
			player2.Agility = player.PlayerCharacter.Agility / 2;
			player2.hp = player.PlayerCharacter.hp / 2;
			player2.FightPower = player.PlayerCharacter.FightPower;
			player2.BaseAttack = player.GetBaseAttack() / 2.0;
			player2.BaseDefence = player.GetBaseDefence() / 2.0;
			player2.BaseAgility = player.GetBaseAgility() / 2.0;
			player2.BaseBlood = player.GetBaseBlood() / 1.65;
			string str = weapons[rand.Next(weapons.Length)];
			player2.Style = $"1214|head13,,3244|hair44,,5276|cloth76,6204|face3,{str},,,,,,,,,";
			player2.Colors = ",,,,,,,,,,,,,,,";
			player2.Hide = 1111111111;
			player2.TemplateID = int.Parse(str.Split('|')[0]);
			player2.StrengthLevel = 0;
			player2.WeaklessGuildProgressStr = "R/O/DeABAtgWdWsIAAAAAAAAgCAECwAAAAAAABgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
			player2.ID = playerId;
			if (m_server != null)
			{
				AddPlayer(playerId, player2);
				room.AddPlayer(player2);
				m_server.AddRoom(room);
			}
			Console.WriteLine("Running AutoBot Easy");
			return playerId;
		}

		public static void CreateAutoBot(GamePlayer player, int roomtype, int gametype, int npcId)
		{
			BaseRoomRingStation room = new BaseRoomRingStation(RingStationConfiguration.NextRoomId())
			{
				RoomType = roomtype,
				GameType = gametype,
				PickUpNpcId = npcId,
				IsAutoBot = true,
				IsFreedom = true
			};
			RingStationGamePlayer player2 = new RingStationGamePlayer();
			player2.NickName = RingStationConfiguration.RandomName[rand.Next(0, RingStationConfiguration.RandomName.Length - 1)];
			player2.GP = player.PlayerCharacter.GP;
			player2.Grade = player.PlayerCharacter.Grade;
			RingStationGamePlayer stationGamePlayer1 = player2;
			int attack1 = player.PlayerCharacter.Attack;
			int attack2 = player.PlayerCharacter.Attack;
			int num1 = (stationGamePlayer1.Attack = attack1 / 3);
			RingStationGamePlayer stationGamePlayer2 = player2;
			int defence1 = player.PlayerCharacter.Defence;
			int defence2 = player.PlayerCharacter.Defence;
			int num2 = (stationGamePlayer2.Defence = defence1 / 3);
			RingStationGamePlayer stationGamePlayer3 = player2;
			int luck1 = player.PlayerCharacter.Luck;
			int luck2 = player.PlayerCharacter.Luck;
			int num3 = (stationGamePlayer3.Luck = luck1 / 3);
			RingStationGamePlayer stationGamePlayer4 = player2;
			int agility1 = player.PlayerCharacter.Agility;
			int agility2 = player.PlayerCharacter.Agility;
			int num4 = (stationGamePlayer4.Agility = agility1 / 3);
			RingStationGamePlayer stationGamePlayer5 = player2;
			int hp1 = player.PlayerCharacter.hp;
			int hp2 = player.PlayerCharacter.hp;
			int num5 = (stationGamePlayer5.hp = hp1 / 3);
			RingStationGamePlayer stationGamePlayer6 = player2;
			int fightPower1 = player.PlayerCharacter.FightPower;
			int fightPower2 = player.PlayerCharacter.FightPower;
			int num6 = (stationGamePlayer6.FightPower = fightPower1 / 3);
			player2.BaseAttack = player.GetBaseAttack() - 0.0 * player.GetBaseAttack() / 3.0;
			player2.BaseDefence = player.GetBaseDefence() - 0.0 * player.GetBaseDefence() / 3.0;
			player2.BaseAgility = player.GetBaseAgility() - 0.0 * player.GetBaseAgility() / 3.0;
			player2.BaseBlood = player.GetBaseBlood() - 0.0 * player.GetBaseBlood() / 1.65;
			string weapon = weapons[rand.Next(weapons.Length)];
			player2.Style = GetAutoBotStyleRandom(weapon);
			player2.Colors = ",,,,,,,,,,,,,,,";
			player2.Hide = 1111111111;
			player2.TemplateID = int.Parse(weapon.Split('|')[0]);
			player2.StrengthLevel = 0;
			player2.WeaklessGuildProgressStr = weaklessGuildProgressStr;
			player2.ID = npcId;
			if (m_server != null)
			{
				AddPlayer(player2.ID, player2);
				room.AddPlayer(player2);
				m_server.AddRoom(room);
			}
			Console.WriteLine("Running AutoBot Normal");
		}

		public static List<RingStationGamePlayer> GetAllPlayer()
		{
			List<RingStationGamePlayer> list = new List<RingStationGamePlayer>();
			m_clientLocker.AcquireReaderLock(-1);
			try
			{
				foreach (RingStationGamePlayer player in m_players.Values)
				{
					list.Add(player);
				}
				return list;
			}
			finally
			{
				m_clientLocker.ReleaseReaderLock();
			}
		}

		public static RingStationGamePlayer GetPlayerById(int playerId)
		{
			RingStationGamePlayer player = null;
			m_clientLocker.AcquireReaderLock(-1);
			try
			{
				if (m_players.ContainsKey(playerId))
				{
					return m_players[playerId];
				}
				return player;
			}
			finally
			{
				m_clientLocker.ReleaseReaderLock();
			}
		}

		public static UserRingStationInfo[] GetRingStationRanks()
		{
			List<UserRingStationInfo> list = new List<UserRingStationInfo>();
			foreach (UserRingStationInfo info in list_1)
			{
				list.Add(info);
				if (list.Count >= 50)
				{
					break;
				}
			}
			return list.ToArray();
		}

		public static UserRingStationInfo GetSingleRingStationInfos(int playerId)
		{
			if (dictionary_1.ContainsKey(playerId))
			{
				return dictionary_1[playerId];
			}
			return null;
		}

		public static VirtualPlayerInfo GetVirtualPlayerInfo()
		{
			int num = rand.Next(list_0.Count);
			return list_0[num];
		}

		public static int GetWeaponID(string style)
		{
			if (!string.IsNullOrEmpty(style))
			{
				string str = style.Split(',')[6];
				if (str.IndexOf("|") != -1)
				{
					return int.Parse(str.Split('|')[0]);
				}
			}
			return 7008;
		}

		public static bool Init()
		{
			bool flag = false;
			try
			{
				m_players.Clear();
				BattleServer server = BattleMgr.GetServer(GameServer.Instance.Configuration.ServerID);
				if (server == null)
				{
					return false;
				}
				m_server = new RingStationBattleServer(RingStationConfiguration.ServerID, server.Ip, server.Port, "1,7road");
				try
				{
					NickName = GameProperties.VirtualName.Split(',');
					flag = m_server.Start();
					if (!SetupVirtualPlayer())
					{
						return false;
					}
					using (new PlayerBussiness())
					{
						_ringstationConfigInfo = null;
						if (_ringstationConfigInfo == null)
						{
							_ringstationConfigInfo = new RingstationConfigInfo();
							_ringstationConfigInfo.buyCount = 10;
							_ringstationConfigInfo.buyPrice = 8000;
							_ringstationConfigInfo.cdPrice = 10000;
							_ringstationConfigInfo.AwardTime = DateTime.Now.AddDays(3.0);
							_ringstationConfigInfo.AwardNum = 450;
							_ringstationConfigInfo.AwardFightWin = "1-50,25|51-100,20|101-1000000,15";
							_ringstationConfigInfo.AwardFightLost = "1-50,15|51-100,10|101-1000000,5";
							_ringstationConfigInfo.ChampionText = "";
							_ringstationConfigInfo.ChallengeNum = 10;
							_ringstationConfigInfo.IsFirstUpdateRank = true;
						}
					}
					BeginTimer();
					ReLoadUserRingStation();
					ReLoadBattleField();
					return flag;
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
					return flag;
				}
			}
			catch (Exception exception2)
			{
				log.Error("RingStationMgr Init", exception2);
				return flag;
			}
		}

		public static RingstationBattleFieldInfo[] LoadRingstationBattleFieldDb()
		{
			using (new PlayerBussiness())
			{
				return new List<RingstationBattleFieldInfo>().ToArray();
			}
		}

		public static Dictionary<int, List<RingstationBattleFieldInfo>> LoadRingstationBattleFields(RingstationBattleFieldInfo[] RingstationBattleField)
		{
			Dictionary<int, List<RingstationBattleFieldInfo>> dictionary = new Dictionary<int, List<RingstationBattleFieldInfo>>();
			for (int i = 0; i < RingstationBattleField.Length; i++)
			{
				RingstationBattleFieldInfo info = RingstationBattleField[i];
				if (!dictionary.Keys.Contains(info.UserID))
				{
					IEnumerable<RingstationBattleFieldInfo> source = RingstationBattleField.Where((RingstationBattleFieldInfo s) => s.UserID == info.UserID);
					dictionary.Add(info.UserID, source.ToList());
				}
			}
			return dictionary;
		}

		public static void LoadRingStationInfo(PlayerInfo player, int dame, int guard)
		{
			if (player == null)
			{
				return;
			}
			using (new PlayerBussiness())
			{
				if (dictionary_1.ContainsKey(player.ID))
				{
					bool flag3 = false;
					UserRingStationInfo info = dictionary_1[player.ID];
					if (dame != info.BaseDamage && info.BaseGuard != guard)
					{
						info.BaseDamage = dame;
						info.BaseGuard = guard;
						info.BaseEnergy = (int)(1.0 - (double)player.Agility * 0.001);
						flag3 = true;
					}
					int weaponID = GetWeaponID(player.Style);
					if (info.WeaponID != weaponID)
					{
						info.WeaponID = weaponID;
						flag3 = true;
					}
				}
				else
				{
					UserRingStationInfo info2 = new UserRingStationInfo
					{
						UserID = player.ID,
						WeaponID = GetWeaponID(player.Style),
						BaseDamage = dame,
						BaseGuard = guard,
						BaseEnergy = (int)(1.0 - (double)player.Agility * 0.001),
						signMsg = LanguageMgr.GetTranslation("RingStation.signMsg"),
						ChallengeNum = _ringstationConfigInfo.ChallengeNum,
						buyCount = _ringstationConfigInfo.buyCount,
						ChallengeTime = DateTime.Now,
						LastDate = DateTime.Now,
						Info = player
					};
					dictionary_1.Add(player.ID, info2);
				}
			}
		}

		public static UserRingStationInfo[] LoadUserRingStationDb()
		{
			using (new PlayerBussiness())
			{
				return new List<UserRingStationInfo>().ToArray();
			}
		}

		public static Dictionary<int, UserRingStationInfo> LoadUserRingStations(UserRingStationInfo[] UserRingStation)
		{
			Dictionary<int, UserRingStationInfo> dictionary = new Dictionary<int, UserRingStationInfo>();
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				for (int i = 0; i < UserRingStation.Length; i++)
				{
					UserRingStationInfo info = UserRingStation[i];
					if (!dictionary.Keys.Contains(info.UserID))
					{
						info.Info = bussiness.GetUserSingleByUserID(info.UserID);
						if (info.Info != null)
						{
							info.WeaponID = GetWeaponID(info.Info.Style);
							dictionary.Add(info.UserID, info);
						}
					}
				}
				return dictionary;
			}
		}

		public static bool ReLoadBattleField()
		{
			try
			{
				RingstationBattleFieldInfo[] array = LoadRingstationBattleFieldDb();
				Dictionary<int, List<RingstationBattleFieldInfo>> dictionary = LoadRingstationBattleFields(array);
				if (array.Length != 0)
				{
					Interlocked.Exchange(ref dictionary_2, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ReLoad RingstationBattleField", exception);
				}
				return false;
			}
		}

		public static bool ReLoadUserRingStation()
		{
			try
			{
				UserRingStationInfo[] userRingStation = LoadUserRingStationDb();
				Dictionary<int, UserRingStationInfo> dictionary = LoadUserRingStations(userRingStation);
				if (userRingStation.Length != 0)
				{
					Interlocked.Exchange(ref dictionary_1, dictionary);
					if (func_1 == null)
					{
						func_1 = smethod_1;
					}
					IEnumerable<UserRingStationInfo> source = userRingStation.Where(func_1);
					if (func_2 == null)
					{
						func_2 = smethod_2;
					}
					list_1 = source.OrderBy(func_2).ToList();
				}
				return true;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ReLoad All UserRingStation", exception);
				}
				return false;
			}
		}

		public static bool SetupVirtualPlayer()
		{
			int[] numArray = new int[16]
			{
				7001,
				7006,
				7007,
				7008,
				7009,
				7010,
				7011,
				7012,
				7013,
				7014,
				7026,
				7025,
				7046,
				7024,
				7035,
				7037
			};
			int[] numArray2 = new int[18]
			{
				1111,
				1112,
				1113,
				1114,
				1115,
				1117,
				1116,
				1119,
				1120,
				1121,
				1122,
				1123,
				1124,
				1125,
				1127,
				1128,
				1129,
				1130
			};
			int[] numArray3 = new int[18]
			{
				2131,
				2132,
				2133,
				2134,
				2135,
				2136,
				2137,
				2138,
				2139,
				2140,
				2141,
				2142,
				2143,
				2144,
				2146,
				2145,
				2147,
				2148
			};
			int[] numArray4 = new int[19]
			{
				3131,
				3132,
				3133,
				3134,
				3135,
				3136,
				3137,
				3138,
				3140,
				3139,
				3141,
				3142,
				3143,
				3144,
				3145,
				3147,
				3148,
				3146,
				3149
			};
			int[] numArray5 = new int[18]
			{
				4120,
				4121,
				4122,
				4123,
				4124,
				4125,
				4126,
				4129,
				4130,
				4131,
				4132,
				4133,
				4134,
				4135,
				4136,
				4137,
				4138,
				4139
			};
			int[] numArray6 = new int[18]
			{
				5110,
				5111,
				5112,
				5113,
				5114,
				5115,
				5116,
				5117,
				5118,
				5119,
				5120,
				5121,
				5122,
				5123,
				5124,
				5125,
				5126,
				5127
			};
			int[] numArray7 = new int[20]
			{
				6110,
				6111,
				6112,
				6113,
				6114,
				6115,
				6116,
				6117,
				6118,
				6119,
				6120,
				6121,
				6122,
				6123,
				6124,
				6125,
				6126,
				6128,
				6127,
				6129
			};
			int[] numArray8 = new int[26]
			{
				15009,
				15018,
				15019,
				15020,
				15021,
				15022,
				15023,
				15024,
				15025,
				15026,
				15027,
				15028,
				15029,
				15049,
				15031,
				15032,
				15033,
				15034,
				15035,
				15036,
				15037,
				15038,
				15039,
				15040,
				15041,
				15042
			};
			int length = numArray.Length;
			for (int i = 0; i < length; i++)
			{
				ItemTemplateInfo info = ItemMgr.FindItemTemplate(numArray[i]);
				ItemTemplateInfo info2 = ItemMgr.FindItemTemplate(numArray2[i]);
				ItemTemplateInfo info3 = ItemMgr.FindItemTemplate(numArray3[i]);
				ItemTemplateInfo info4 = ItemMgr.FindItemTemplate(numArray4[i]);
				ItemTemplateInfo info5 = ItemMgr.FindItemTemplate(numArray5[i]);
				ItemTemplateInfo info6 = ItemMgr.FindItemTemplate(numArray6[i]);
				ItemTemplateInfo info7 = ItemMgr.FindItemTemplate(numArray7[i]);
				ItemTemplateInfo info8 = ItemMgr.FindItemTemplate(numArray8[i]);
				if (info != null && info2 != null && info3 != null && info4 != null && info5 != null && info6 != null && info7 != null && info8 != null)
				{
					string str = $"{numArray[i]}|{info.Pic}";
					string str2 = $"{numArray2[i]}|{info2.Pic}";
					string str3 = $"{numArray3[i]}|{info3.Pic}";
					string str4 = $"{numArray4[i]}|{info4.Pic}";
					string str5 = $"{numArray5[i]}|{info5.Pic}";
					string str6 = $"{numArray6[i]}|{info6.Pic}";
					string str7 = $"{numArray7[i]}|{info7.Pic}";
					string str8 = $"{numArray8[i]}|{info8.Pic}";
					string str9 = $"{str2},{str3},{str4},{str5},{str6},{str7},{str},,{str8},,,,,,,,,";
					VirtualPlayerInfo item = new VirtualPlayerInfo
					{
						Style = str9,
						Weapon = numArray[i]
					};
					list_0.Add(item);
				}
			}
			return list_0.Count > Math.Abs(length / 2);
		}

		private static DateTime smethod_0(RingstationBattleFieldInfo ringstationBattleFieldInfo_0)
		{
			return ringstationBattleFieldInfo_0.BattleTime;
		}

		private static bool smethod_1(UserRingStationInfo userRingStationInfo_0)
		{
			return userRingStationInfo_0.Rank != 0;
		}

		private static int smethod_2(UserRingStationInfo userRingStationInfo_0)
		{
			return userRingStationInfo_0.Rank;
		}

		private static int smethod_3(UserRingStationInfo userRingStationInfo_0)
		{
			return userRingStationInfo_0.Total;
		}

		private static bool smethod_4(UserRingStationInfo userRingStationInfo_0)
		{
			return userRingStationInfo_0.Rank != 0;
		}

		private static int smethod_5(UserRingStationInfo userRingStationInfo_0)
		{
			return userRingStationInfo_0.Rank;
		}

		protected static void StatusScan(object sender)
		{
			try
			{
				log.Info("Begin Scan RingStation Info....");
				int tickCount = Environment.TickCount;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				bool flag = false;
				if (ReLoadUserRingStation())
				{
					List<UserRingStationInfo> list = new List<UserRingStationInfo>();
					foreach (UserRingStationInfo info in dictionary_1.Values)
					{
						list.Add(info);
					}
					if (_ringstationConfigInfo.IsFirstUpdateRank && list.Count > int_0)
					{
						if (func_3 == null)
						{
							func_3 = smethod_3;
						}
						List<UserRingStationInfo> list2 = list.OrderByDescending(func_3).ToList();
						lock (m_lock)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								UserRingStationInfo userRingStationInfo = list2[i];
								userRingStationInfo.Rank = i + 1;
								UpdateRingStationInfo(userRingStationInfo);
							}
						}
						_ringstationConfigInfo.IsFirstUpdateRank = false;
						flag = true;
					}
					if (func_4 == null)
					{
						func_4 = smethod_4;
					}
					IEnumerable<UserRingStationInfo> source = list.Where(func_4);
					if (func_5 == null)
					{
						func_5 = smethod_5;
					}
					list_1 = source.OrderBy(func_5).ToList();
					if (list_1.Count > 0)
					{
						UserRingStationInfo info2 = list_1[0];
						if (info2.Info != null)
						{
							_ringstationConfigInfo.ChampionText = info2.Info.NickName;
							flag = true;
						}
					}
					if (_ringstationConfigInfo.IsEndTime())
					{
						lock (m_lock)
						{
							_ringstationConfigInfo.AwardTime = DateTime.Now;
							_ringstationConfigInfo.AwardTime = DateTime.Now.AddDays(3.0);
							flag = true;
						}
						if (list.Count > 0)
						{
							ItemTemplateInfo goods = ItemMgr.FindItemTemplate(-1000);
							string translation = LanguageMgr.GetTranslation("RingStation.RankAward");
							if (goods != null)
							{
								foreach (UserRingStationInfo info3 in list)
								{
									int num4 = _ringstationConfigInfo.AwardNumByRank(info3.Rank);
									List<ItemInfo> infos = new List<ItemInfo>();
									if (num4 > 0)
									{
										ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 102);
										item.Count = num4;
										item.ValidDate = 0;
										item.IsBinds = true;
										infos.Add(item);
										if (WorldEventMgr.SendItemsToMail(infos, info3.UserID, info3.Info.NickName, translation))
										{
											WorldMgr.GetPlayerById(info3.UserID)?.Out.SendMailResponse(info3.UserID, eMailRespose.Receiver);
										}
									}
								}
							}
						}
					}
				}
				Thread.CurrentThread.Priority = priority;
				tickCount = Environment.TickCount - tickCount;
				log.Info("End Scan RingStation Info....");
			}
			catch (Exception exception)
			{
				log.Error("StatusScan ", exception);
			}
		}

		public static bool UpdateRingStationFight(UserRingStationInfo ring)
		{
			if (ring == null)
			{
				return false;
			}
			lock (m_lock)
			{
				if (dictionary_1.ContainsKey(ring.UserID))
				{
					dictionary_1[ring.UserID] = ring;
					return true;
				}
			}
			return false;
		}

		public static bool UpdateRingStationInfo(UserRingStationInfo ring)
		{
			if (ring != null)
			{
				using (new PlayerBussiness())
				{
					lock (m_lock)
					{
						if (dictionary_1.ContainsKey(ring.UserID))
						{
							dictionary_1[ring.UserID] = ring;
							return true;
						}
					}
				}
			}
			return false;
		}

		public static string GetAutoBotStyleRandom(string weapon)
		{
			string style = $"1142|head51,,3107|hair6,,5160|cloth60,6103|face2,{weapon},,,,,,,,,";
			switch (new Random().Next(1, 20))
			{
			case 1:
				style = $"1142|head51,,3158|hair58,,5106|cloth5,6103|face2,{weapon},,,,,,,,,";
				break;
			case 2:
				style = $"1142|head51,,3158|hair58,,5180|cloth80,6103|face2,{weapon},,,,,,,,,";
				break;
			case 3:
				style = $",,3150|hair50,,5106|cloth5,,{weapon},,,,,,,,,";
				break;
			case 4:
				style = $"1144|head53,,3150|hair50,,5180|cloth80,6103|face2,{weapon},,,,,,,,,";
				break;
			case 5:
				style = $",,3150|hair50,,5106|cloth5,,{weapon},,,,17002|offhand2,,,,,";
				break;
			case 6:
				style = $"1137|head42,2106|glass5,3150|hair50,,5130|cloth30,,{weapon},13144|suits44,,";
				break;
			case 7:
				style = $",,3170|hair70,4104|eff4,5303|cloth103,6134|face33,{weapon},,,,17002|offhand2,9522|ring55,,,,";
				break;
			case 8:
				style = $",,3308|hair108,4104|eff4,5145|cloth45,6131|face30,{weapon},13136|suits36,15048|wing48,,17002|offhand2,9522|ring55,,,,";
				break;
			case 9:
				style = $",,3164|hair64,4104|eff4,5112|cloth11,6128|face27,{weapon},13136|suits36,15048|wing48,,17002|offhand2,9522|ring55,,,,";
				break;
			case 10:
				style = $",,3160|hair60,4104|eff4,5140|cloth40,6135|face34,{weapon},13136|suits36,15026|wing026,,17002|offhand2,9522|ring55,,,,";
				break;
			case 11:
				style = $",,,,,,{weapon},13145|suits45,,,,,,,,";
				break;
			case 12:
				style = $",,3163|hair63,4104|eff4,5107|cloth6,6120|face19,{weapon},13136|suits36,15004|wing004,,17002|offhand2,9522|ring55,,,,";
				break;
			case 13:
				style = $",,3150|hair50,4148|eff48,5136|cloth36,6115|face14,{weapon},13136|suits36,15026|wing026,,17002|offhand2,9522|ring55,,,,";
				break;
			case 14:
				style = $",,3138|hair38,4104|eff4,5107|cloth6,6120|face19,{weapon},13136|suits36,15026|wing026,,17002|offhand2,9522|ring55,,,,";
				break;
			case 15:
				style = $",,3143|hair43,4117|eff17,5137|cloth37,6105|face4,{weapon},13136|suits36,15026|wing026,,17002|offhand2,9522|ring55,,,,";
				break;
			case 16:
				style = $",,3125|hair25,4118|eff18,5128|cloth28,6116|face15,{weapon},13136|suits36,15026|wing026,,17002|offhand2,9522|ring55,,,,";
				break;
			case 17:
				style = $",,3110|hair9,4117|eff17,5137|cloth37,6120|face19,{weapon},13136|suits36,15026|wing026,,17002|offhand2,9522|ring55,,,,";
				break;
			case 18:
				style = $",,3135|hair35,4117|eff17,5131|cloth31,6105|face4,{weapon},13136|suits36,15004|wing004,,17002|offhand2,9522|ring55,,,,";
				break;
			case 19:
				style = $",,3151|hair51,4117|eff17,5141|cloth41,6110|face9,{weapon},13136|suits36,15003|wing003,,17002|offhand2,9522|ring55,,,,";
				break;
			case 20:
				style = $",,3107|hair6,4117|eff17,5135|cloth35,6120|face19,{weapon},13136|suits36,15003|wing003,,17002|offhand2,9522|ring55,,,,";
				break;
			}
			return style;
		}
	}
}
