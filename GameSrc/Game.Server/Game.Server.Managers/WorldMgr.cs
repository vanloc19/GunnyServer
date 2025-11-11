using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public sealed class WorldMgr
	{
		private static readonly ILog ilog_0;

		private static ReaderWriterLock readerWriterLock_0;

		private static Dictionary<int, GamePlayer> dictionary_0;

		private static Dictionary<int, GamePlayer> AllPlayedPlayers;

		private static Dictionary<int, AreaConfigInfo> dictionary_1;

		private static Dictionary<int, EdictumInfo> dictionary_2;

		private static Dictionary<int, ShopFreeCountInfo> dictionary_3;

		private static Dictionary<int, UserExitRoomLogInfo> dictionary_4;

		private static AreaConfigInfo areaConfigInfo_0;

		public static DateTime LastTimeUpdateCaddyRank;

		public static Scene _marryScene;

		public static Scene _hotSpringScene;

		private static RSACryptoServiceProvider rsacryptoServiceProvider_0;

		private static string[] string_0;

		public static Scene MarryScene => _marryScene;

		public static Scene HotSpringScene => _hotSpringScene;

		public static RSACryptoServiceProvider RsaCryptor => rsacryptoServiceProvider_0;

		public static Dictionary<int, UsersExtraInfo> CaddyRank;

		public static Dictionary<int, PlayerInfo> LeagueRank;


		public static bool m_IsLeagueOpen;

		public static bool IsLeagueOpen
		{
			get { return m_IsLeagueOpen; }
			set { m_IsLeagueOpen = value; }
		}

		public static bool Init()
		{
			bool flag = false;
			try
			{
				rsacryptoServiceProvider_0 = new RSACryptoServiceProvider();
				rsacryptoServiceProvider_0.FromXmlString(GameServer.Instance.Configuration.PrivateKey);
				dictionary_0.Clear();
				AllPlayedPlayers = new Dictionary<int, GamePlayer>();
				AllPlayedPlayers.Clear();
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					ServerInfo serviceSingle = serviceBussiness.GetServiceSingle(GameServer.Instance.Configuration.ServerID);
					if (serviceSingle != null)
					{
						_marryScene = new Scene(serviceSingle);
						_hotSpringScene = new Scene(serviceSingle);
						flag = true;
					}
				}
				Dictionary<int, EdictumInfo> dictionary = smethod_0();
				if (dictionary.Values.Count > 0)
				{
					Interlocked.Exchange(ref dictionary_2, dictionary);
				}
				UpdateCaddyRank();
				smethod_2();
				return flag;
			}
			catch (Exception ex)
			{
				ilog_0.Error("WordMgr Init", ex);
				return flag;
			}
		}

		public static bool ReloadEdictum()
		{
			bool flag = false;
			try
			{
				Dictionary<int, EdictumInfo> dictionary = smethod_0();
				if (dictionary.Values.Count > 0)
				{
					Interlocked.Exchange(ref dictionary_2, dictionary);
				}
				flag = true;
				return flag;
			}
			catch (Exception ex)
			{
				ilog_0.Error("WordMgr ReloadEdictum Init", ex);
				return flag;
			}
		}

		private static Dictionary<int, EdictumInfo> smethod_0()
		{
			Dictionary<int, EdictumInfo> dictionary = new Dictionary<int, EdictumInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				EdictumInfo[] allEdictum = produceBussiness.GetAllEdictum();
				foreach (EdictumInfo edictumInfo in allEdictum)
				{
					if (!dictionary.ContainsKey(edictumInfo.ID))
					{
						dictionary.Add(edictumInfo.ID, edictumInfo);
					}
				}
				return dictionary;
			}
		}

		public static bool AddPlayer(int playerId, GamePlayer player)
		{
			readerWriterLock_0.AcquireWriterLock(-1);
			try
			{
				if (dictionary_0.ContainsKey(playerId))
				{
					return false;
				}
				dictionary_0.Add(playerId, player);
			}
			finally
			{
				readerWriterLock_0.ReleaseWriterLock();
			}
			return true;
		}

		public static bool RemovePlayer(int playerId)
		{
			readerWriterLock_0.AcquireWriterLock(-1);
			GamePlayer gamePlayer = null;
			try
			{
				if (dictionary_0.ContainsKey(playerId))
				{
					gamePlayer = dictionary_0[playerId];
					dictionary_0.Remove(playerId);
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseWriterLock();
			}
			if (gamePlayer == null)
			{
				return false;
			}
			GameServer.Instance.LoginServer.SendUserOffline(playerId, gamePlayer.PlayerCharacter.ConsortiaID);
			return true;
		}

		public static GamePlayer GetPlayerById(int playerId)
		{
			GamePlayer gamePlayer = null;
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				if (dictionary_0.ContainsKey(playerId))
				{
					return dictionary_0[playerId];
				}
				return gamePlayer;
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
		}

		public static GamePlayer GetClientByPlayerNickName(string nickName)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer allPlayer in allPlayers)
			{
				if (allPlayer.PlayerCharacter.NickName == nickName)
				{
					return allPlayer;
				}
			}
			return null;
		}

		public static GamePlayer GetClientByPlayerUserName(string userName)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer allPlayer in allPlayers)
			{
				if (allPlayer.PlayerCharacter.UserName == userName)
				{
					return allPlayer;
				}
			}
			return null;
		}

		public static GamePlayer[] GetAllPlayers()
		{
			List<GamePlayer> gamePlayerList = new List<GamePlayer>();
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				foreach (GamePlayer gamePlayer in dictionary_0.Values)
				{
					if (gamePlayer != null && gamePlayer.PlayerCharacter != null)
					{
						gamePlayerList.Add(gamePlayer);
					}
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
			return gamePlayerList.ToArray();
		}

		public static string smethod_1(GamePlayer p)
		{
			return (p.Client.Socket.RemoteEndPoint as IPEndPoint)?.Address.ToString();
		}


		public static bool IsAccountLimit(GamePlayer p)
		{
			if (p.Client == null)
			{
				return false;
			}
			bool result = false;
			string hWID = p.Client.HWID;
			if (hWID != null && hWID.Length > 0)
			{
				GamePlayer[] allPlayerWithHWID = GetAllPlayerWithHWID(hWID);
				if (allPlayerWithHWID.Length > 3)
				{
					p.BlockReceiveMoney = true;
					p.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("GameServer.LimitIPConnentLauncher.Msg"));
					string text = "";
					GamePlayer[] array = allPlayerWithHWID;
					foreach (GamePlayer gamePlayer in array)
					{
						text = text + gamePlayer.PlayerCharacter.UserName + "(" + gamePlayer.PlayerCharacter.NickName + ").";
					}
					p.AddLog("OnlineMore", "LAUNCHER Username: " + p.PlayerCharacter.UserName + "|NickName: " + p.PlayerCharacter.NickName + "|ListAccountSameIP: " + text);
					result = true;
				}
			}
			else
			{
				string text2 = smethod_1(p);
				if (text2 != null)
				{
					GamePlayer[] allPlayerWithIP = GetAllPlayerWithIP(text2);
					if (allPlayerWithIP.Length > 3)
					{
						foreach (GamePlayer psingle in allPlayerWithIP)
						{
							psingle.BlockReceiveMoney = true;
							psingle.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("GameServer.LimitIPConnent.Msg"));
						}
						string text3 = "";
						GamePlayer[] array = allPlayerWithIP;
						foreach (GamePlayer gamePlayer2 in array)
						{
							text3 = text3 + gamePlayer2.PlayerCharacter.UserName + "(" + gamePlayer2.PlayerCharacter.NickName + ").";
						}
						p.AddLog("OnlineMore", "Username: " + p.PlayerCharacter.UserName + "|NickName: " + p.PlayerCharacter.NickName + "|ListAccountSameIP: " + text3);
						result = true;
					}
				}
			}
			return result;
		}

		public static GamePlayer[] GetAllPlayerWithHWID(string hwid)
		{
			List<GamePlayer> gamePlayerList = new List<GamePlayer>();
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				foreach (GamePlayer gamePlayer in dictionary_0.Values)
				{
					if (gamePlayer != null && gamePlayer.PlayerCharacter != null && gamePlayer.Client != null && gamePlayer.Client.IsConnected && gamePlayer.Client.HWID != null && gamePlayer.Client.HWID.Length > 0 && gamePlayer.Client.HWID == hwid)
					{
						gamePlayerList.Add(gamePlayer);
					}
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
			return gamePlayerList.ToArray();
		}

		public static GamePlayer[] GetAllPlayerWithIP(string ip)
		{
			List<GamePlayer> gamePlayerList = new List<GamePlayer>();
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				foreach (GamePlayer gamePlayer in dictionary_0.Values)
				{
					if (gamePlayer != null && gamePlayer.PlayerCharacter != null && gamePlayer.Client != null && gamePlayer.Client.Socket != null && gamePlayer.Client.IsConnected)
					{
						IPEndPoint remoteEndPoint = gamePlayer.Client.Socket.RemoteEndPoint as IPEndPoint;
						if (remoteEndPoint != null && remoteEndPoint.Address.ToString().Contains(ip))
						{
							gamePlayerList.Add(gamePlayer);
						}
					}
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
			return gamePlayerList.ToArray();
		}

		public static GamePlayer[] GetAllPlayersNoGame()
		{
			List<GamePlayer> gamePlayerList = new List<GamePlayer>();
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				GamePlayer[] allPlayers = GetAllPlayers();
				foreach (GamePlayer allPlayer in allPlayers)
				{
					if (allPlayer.CurrentRoom == null)
					{
						gamePlayerList.Add(allPlayer);
					}
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
			return gamePlayerList.ToArray();
		}

		public static GamePlayer GetSinglePlayerWithConsortia(int ConsortiaID)
		{
			GamePlayer gamePlayer1 = null;
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				foreach (GamePlayer gamePlayer2 in dictionary_0.Values)
				{
					if (gamePlayer2 != null && gamePlayer2.PlayerCharacter != null && gamePlayer2.PlayerCharacter.ConsortiaID == ConsortiaID)
					{
						gamePlayer1 = gamePlayer2;
					}
				}
				return gamePlayer1;
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
		}

		public static GamePlayer[] GetAllPlayersWithConsortia(int ConsortiaID)
		{
			List<GamePlayer> gamePlayerList = new List<GamePlayer>();
			readerWriterLock_0.AcquireReaderLock(-1);
			try
			{
				foreach (GamePlayer gamePlayer in dictionary_0.Values)
				{
					if (gamePlayer != null && gamePlayer.PlayerCharacter != null && gamePlayer.PlayerCharacter.ConsortiaID == ConsortiaID)
					{
						gamePlayerList.Add(gamePlayer);
					}
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseReaderLock();
			}
			return gamePlayerList.ToArray();
		}

		public static string GetPlayerStringByPlayerNickName(string nickName)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer allPlayer in allPlayers)
			{
				if (allPlayer.PlayerCharacter.NickName == nickName)
				{
					return allPlayer.ToString();
				}
			}
			return nickName + " is not online!";
		}

		public static string DisconnectPlayerByName(string nickName)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer allPlayer in allPlayers)
			{
				if (allPlayer.PlayerCharacter.NickName == nickName)
				{
					allPlayer.Disconnect();
					return "OK";
				}
			}
			return nickName + " is not online!";
		}

		public static void OnPlayerOffline(int playerid, int consortiaID)
		{
			ChangePlayerState(playerid, 0, consortiaID);
		}

		public static void OnPlayerOnline(int playerid, int consortiaID)
		{
			ChangePlayerState(playerid, 1, consortiaID);
		}

		public static void ChangePlayerState(int playerID, int state, int consortiaID)
		{
			GSPacketIn pkg = null;
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer allPlayer in allPlayers)
			{
				if ((allPlayer.Friends != null && allPlayer.Friends.ContainsKey(playerID) && allPlayer.Friends[playerID] == 0) || (allPlayer.PlayerCharacter.ConsortiaID != 0 && allPlayer.PlayerCharacter.ConsortiaID == consortiaID))
				{
					if (pkg == null)
					{
						pkg = allPlayer.Out.SendFriendState(playerID, state, allPlayer.PlayerCharacter.typeVIP, allPlayer.PlayerCharacter.VIPLevel);
					}
					else
					{
						allPlayer.SendTCP(pkg);
					}
				}
			}
		}

		public static void UpdateExitGame(int userId)
		{
			lock (dictionary_4)
			{
				if (dictionary_4.ContainsKey(userId))
				{
					if (dictionary_4[userId].TotalExitTime <= 3)
					{
						dictionary_4[userId].TotalExitTime++;
					}
					else
					{
						dictionary_4[userId].TimeBlock = DateTime.Now.AddMinutes(30.0);
					}
					dictionary_4[userId].LastLogout = DateTime.Now;
				}
				else
				{
					dictionary_4.Add(userId, new UserExitRoomLogInfo
					{
						UserID = userId,
						TimeBlock = DateTime.MinValue,
						TotalExitTime = 1,
						LastLogout = DateTime.Now
					});
				}
			}
		}

		public static DateTime CheckTimeEnterRoom(int userid)
		{
			lock (dictionary_4)
			{
				if (dictionary_4.ContainsKey(userid))
				{
					if (dictionary_4[userid].TimeBlock > DateTime.Now)
					{
						return dictionary_4[userid].TimeBlock;
					}
					if (dictionary_4[userid].TotalExitTime > 3)
					{
						dictionary_4[userid].TotalExitTime = 0;
					}
				}
				return DateTime.MinValue;
			}
		}

		public static bool UpdateShopFreeCount(int shopId, int total)
		{
			bool flag = false;
			lock (dictionary_3)
			{
				if (dictionary_3.ContainsKey(shopId))
				{
					if (dictionary_3[shopId].Count <= 0)
					{
						return flag;
					}
					dictionary_3[shopId].Count--;
					return true;
				}
				dictionary_3.Add(shopId, new ShopFreeCountInfo
				{
					ShopID = shopId,
					Count = total - 1,
					CreateDate = DateTime.Now
				});
				return true;
			}
		}

		private static void smethod_2()
		{
			WorldMgrDataInfo worldMgrDataInfo = Marshal.LoadDataFile<WorldMgrDataInfo>("shopfreecount", isEncrypt: true);
			if (worldMgrDataInfo != null && worldMgrDataInfo.ShopFreeCount != null)
			{
				dictionary_3 = worldMgrDataInfo.ShopFreeCount;
			}
		}

		private static void smethod_3()
		{
			Marshal.SaveDataFile(new WorldMgrDataInfo
			{
				ShopFreeCount = dictionary_3
			}, "shopfreecount", isEncrypt: true);
		}

		public static void ScanShopFreeVaildDate()
		{
			lock (dictionary_3)
			{
				bool flag = false;
				foreach (ShopFreeCountInfo value in dictionary_3.Values)
				{
					DateTime date3 = value.CreateDate.Date;
					DateTime date2 = DateTime.Now.Date;
					if (date3 != date2)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					dictionary_3.Clear();
				}
			}
		}

		public static List<ShopFreeCountInfo> GetAllShopFreeCount()
		{
			List<ShopFreeCountInfo> shopFreeCountInfoList = new List<ShopFreeCountInfo>();
			lock (dictionary_3)
			{
				foreach (ShopFreeCountInfo shopFreeCountInfo in dictionary_3.Values)
				{
					shopFreeCountInfoList.Add(shopFreeCountInfo);
				}
				return shopFreeCountInfoList;
			}
		}

		public static GSPacketIn SendSysNotice(eMessageType type, string msg, int ItemID, int TemplateID, string key)
		{
			int val = msg.IndexOf(TemplateID.ToString(), StringComparison.Ordinal);
			GSPacketIn pkg = new GSPacketIn(10);
			pkg.WriteInt((int)type);
			pkg.WriteString(msg.Replace(TemplateID.ToString(), ""));
			pkg.WriteByte(1);
			pkg.WriteInt(val);
			pkg.WriteInt(TemplateID);
			pkg.WriteInt(ItemID);
			if (!string.IsNullOrEmpty(key))
			{
				pkg.WriteString(key);
			}
			SendToAll(pkg);
			return pkg;
		}

		public static GSPacketIn SendSysTipNotice(string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(10);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteString(msg);
			SendToAll(gSPacketIn);
			return gSPacketIn;
		}

		public static GSPacketIn SendSysNotice(string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(10);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteString(msg);
			SendToAll(gSPacketIn);
			return gSPacketIn;
		}

		public static GSPacketIn SendOnlineNotice(string msg)
        {
			GSPacketIn pkg = new GSPacketIn(10);
			pkg.WriteInt(0);
			pkg.WriteString(msg);
			SendToAll(pkg);
			return pkg;
        }

		public static GSPacketIn SendSysNotice(string msg, bool isHide)
        {
			GSPacketIn pkg = new GSPacketIn(10);
			if (!isHide)
            {
				pkg.WriteInt(3);
				pkg.WriteString(msg);
				SendToAll(pkg);
            }
			return pkg;
        }

		public static void SendSysNotice(string msg, int consortiaId)
		{
			GamePlayer[] allPlayersWithConsortia = GetAllPlayersWithConsortia(consortiaId);
			foreach (GamePlayer obj in allPlayersWithConsortia)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(10);
				gSPacketIn.WriteInt(3);
				gSPacketIn.WriteString(msg);
				GSPacketIn pkg = gSPacketIn;
				obj.SendTCP(pkg);
			}
		}

		public static GSPacketIn SendSysNotice(eMessageType type, string msg, List<ItemInfo> items, int zoneID)
		{
			List<int> intList = smethod_4(msg, "@");
			GSPacketIn pkg = null;
			if (intList.Count == items.Count)
			{
				pkg = new GSPacketIn(10);
				pkg.WriteInt((int)type);
				pkg.WriteString(msg.Replace("@", ""));
				if (type == eMessageType.CROSS_NOTICE)
				{
					pkg.WriteInt(zoneID);
				}
				int index = 0;
				pkg.WriteByte((byte)intList.Count);
				foreach (int val in intList)
				{
					ItemInfo itemInfo = items[index];
					pkg.WriteInt(val);
					pkg.WriteInt(itemInfo.TemplateID);
					pkg.WriteInt(itemInfo.ItemID);
					pkg.WriteString("");
					index++;
				}
				SendToAll(pkg);
			}
			else
			{
				ilog_0.Error("wrong msg: " + msg + ": itemcount: " + items.Count);
			}
			return pkg;
		}

		private static List<int> smethod_4(string string_1, string string_2)
		{
			List<int> intList = new List<int>();
			int length = string_2.Length;
			int num = -length;
			while (true)
			{
				num = string_1.IndexOf(string_2, num + length);
				if (num == -1)
				{
					break;
				}
				intList.Add(num);
			}
			return intList;
		}

		public static void UpdateCaddyRank()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				UsersExtraInfo[] rankCaddy = playerBussiness.GetRankCaddy();
				CaddyRank = new Dictionary<int, UsersExtraInfo>();
				UsersExtraInfo[] array = rankCaddy;
				foreach (UsersExtraInfo UsersExtraInfo in array)
				{
					if (!CaddyRank.ContainsKey(UsersExtraInfo.UserID))
					{
						CaddyRank.Add(UsersExtraInfo.UserID, UsersExtraInfo);
					}
				}
				LastTimeUpdateCaddyRank = DateTime.Now;
			}
		}


		public static void AddAreaConfig(AreaConfigInfo[] Areas)
		{
			foreach (AreaConfigInfo area in Areas)
			{
				if (!dictionary_1.ContainsKey(area.AreaID))
				{
					if (area.AreaID == GameServer.Instance.Configuration.ZoneId)
					{
						areaConfigInfo_0 = area;
					}
					dictionary_1.Add(area.AreaID, area);
				}
			}
		}

		public static AreaConfigInfo FindAreaConfig(int zoneId)
		{
			readerWriterLock_0.AcquireWriterLock(-1);
			try
			{
				if (dictionary_1.ContainsKey(zoneId))
				{
					return dictionary_1[zoneId];
				}
			}
			finally
			{
				readerWriterLock_0.ReleaseWriterLock();
			}
			return null;
		}

		public static AreaConfigInfo[] GetAllAreaConfig()
		{
			List<AreaConfigInfo> areaConfigInfoList = new List<AreaConfigInfo>();
			foreach (AreaConfigInfo areaConfigInfo in dictionary_1.Values)
			{
				areaConfigInfoList.Add(areaConfigInfo);
			}
			return areaConfigInfoList.ToArray();
		}

		public static EdictumInfo[] GetAllEdictumVersion()
		{
			List<EdictumInfo> edictumInfoList = new List<EdictumInfo>();
			foreach (EdictumInfo edictumInfo in dictionary_2.Values)
			{
				DateTime date3 = edictumInfo.EndDate.Date;
				DateTime date2 = DateTime.Now.Date;
				if (date3 > date2)
				{
					edictumInfoList.Add(edictumInfo);
				}
			}
			return edictumInfoList.ToArray();
		}

		public static void SendToAll(GSPacketIn pkg)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			Console.WriteLine("Send to all " + allPlayers.Length + " players");
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].SendTCP(pkg);
			}
		}

		public static bool CheckBadWord(string msg)
		{
			string[] array = string_0;
			foreach (string str in array)
			{
				if (msg.ToLower().Contains(str.ToLower()))
				{
					return true;
				}
			}
			return false;
		}

		public static void UpdateUserRank()
		{
			using PlayerBussiness db = new PlayerBussiness();
			db.UpdateRank();
			foreach (GamePlayer p in GetAllPlayers())
			{
				if (p != null)
				{
					p.UpdatePublicPlayer();
				}
			}
		}

		public static void Test()
        {
			using (PlayerBussiness pb = new PlayerBussiness())
            {

            }
        }

		public static void UpdateIsLeagueOpen(bool IsOpen)
		{
			Console.WriteLine($"isOpen = {IsOpen}");
			m_IsLeagueOpen = IsOpen;
			foreach (GamePlayer p in GetAllPlayers())
			{
				if (p != null)
				{
					if (IsOpen)
					{
						p.Out.SendLeagueNotice(p.PlayerCharacter.ID);
					}
				}
			}
		}

		public static void ResetLeague(bool IsOpen)
		{
			foreach (GamePlayer p in GetAllPlayers())
			{
				if (p != null)
				{
					p.UpdateLeage();
				}
			}
		}


		/*public static void SendLeagueAward()
		{
			int level = 0;
			int type = 0;
			List<PlayerInfo> playersList = m_leagueRank.Where(a => a.Value.WeeklyScore >= 1).Select(a => a.Value).ToList();

			foreach(PlayerInfo in4 in playersList)
            {
				Console.WriteLine($"Name = {in4.NickName}, | Grade = {in4.Grade} | WeeklyScore = {in4.WeeklyScore}");
            }

			foreach (PlayerInfo info in playersList)
			{
				if (info.Grade >= 20 && info.Grade < 30)
				{
					level = 20;
					if (info.WeeklyScore >= 10000 && info.WeeklyScore < 20000)
					{
						type = 1;
					}
					else if (info.WeeklyScore >= 20000 && info.WeeklyScore < 35000)
					{
						type = 2;
					}
					else if (info.WeeklyScore >= 35000 && info.WeeklyScore < 60000)
					{
						type = 3;
					}
					else if (info.WeeklyScore >= 60000)
					{
						type = 4;
					}
					else
					{
						type = 0;
					}
				}
				else if (info.Grade >= 30 && info.Grade < 40)
				{
					level = 30;
					if (info.WeeklyScore >= 10000 && info.WeeklyScore < 20000)
					{
						type = 5;
					}
					else if (info.WeeklyScore >= 20000 && info.WeeklyScore < 35000)
					{
						type = 6;
					}
					else if (info.WeeklyScore >= 35000 && info.WeeklyScore < 60000)
					{
						type = 7;
					}
					else if (info.WeeklyScore >= 60000)
					{
						type = 8;
					}
					else
					{
						type = 0;
					}
				}
				else if (info.Grade >= 40)
				{
					level = 40;
					if (info.WeeklyScore >= 10000 && info.WeeklyScore < 20000)
					{
						type = 9;
					}
					else if (info.WeeklyScore >= 20000 && info.WeeklyScore < 35000)
					{
						type = 10;
					}
					else if (info.WeeklyScore >= 35000 && info.WeeklyScore < 60000)
					{
						type = 11;
					}
					else if (info.WeeklyScore >= 60000)
					{
						type = 12;
					}
					else
					{
						type = 0;
					}
				}
				else
				{
					level = 1;
					type = 1;
				}
				DailyLeagueAwardInfo[] awards = AwardMgr.GetLeagueAwardWithID(level, type);
				if (awards == null)
				{
					return;
				}
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					List<ItemInfo> items = new List<ItemInfo>();
					foreach (DailyLeagueAwardInfo a in awards)
					{
						ItemTemplateInfo temp = ItemMgr.FindItemTemplate(a.TemplateID);
						if (temp == null)
							continue;

						ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, 106);
						item.Count = a.Count;
						item.ValidDate = a.ItemValid;
						item.IsBinds = a.IsBind;
						item.StrengthenLevel = a.StrengthenLevel;
						item.AgilityCompose = a.AgilityCompose;
						item.LuckCompose = a.LuckCompose;
						item.DefendCompose = a.DefendCompose;
						item.AttackCompose = a.AttackCompose;
						item.Hole1 = a.Hole1;
						item.Hole2 = a.Hole2;
						item.Hole3 = a.Hole3;
						item.Hole4 = a.Hole4;
						item.Hole5 = a.Hole5;
						item.Hole5Exp = a.Hole5Exp;
						item.Hole5Level = a.Hole5Level;
						item.Hole6 = a.Hole6;
						item.Hole6Exp = a.Hole6Exp;
						item.Hole6Level = a.Hole6Level;
						items.Add(item);
					}
					pb.SendItemsToMail(items, info.ID, GameServer.Instance.Configuration.ZoneId, LanguageMgr.GetTranslation("GameServer.LeagueGame.Msg1"), LanguageMgr.GetTranslation("GameServer.LeagueGame.Msg1"));
				}
			}
		}

		public static void SendLeagueRankAward()
		{
			PlayerInfo players = m_leagueRank.Where(a => a.Value.WeeklyRanking == 1).Select(a => a.Value).FirstOrDefault();
			DailyLeagueAwardInfo[] awards = AwardMgr.GetLeagueAwardWithID(0, 0);
			if (awards == null)
			{
				return;
			}
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				List<ItemInfo> items = new List<ItemInfo>();
				foreach (DailyLeagueAwardInfo a in awards)
				{
					ItemTemplateInfo temp = ItemMgr.FindItemTemplate(a.TemplateID);
					if (temp == null)
						continue;

					ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, 106);
					item.Count = a.Count;
					item.ValidDate = a.ItemValid;
					item.IsBinds = a.IsBind;
					item.StrengthenLevel = a.StrengthenLevel;
					item.AgilityCompose = a.AgilityCompose;
					item.LuckCompose = a.LuckCompose;
					item.DefendCompose = a.DefendCompose;
					item.AttackCompose = a.AttackCompose;
					item.Hole1 = a.Hole1;
					item.Hole2 = a.Hole2;
					item.Hole3 = a.Hole3;
					item.Hole4 = a.Hole4;
					item.Hole5 = a.Hole5;
					item.Hole5Exp = a.Hole5Exp;
					item.Hole5Level = a.Hole5Level;
					item.Hole6 = a.Hole6;
					item.Hole6Exp = a.Hole6Exp;
					item.Hole6Level = a.Hole6Level;
					items.Add(item);
				}
				pb.SendItemsToMail(items, players.ID, GameServer.Instance.Configuration.ZoneId, LanguageMgr.GetTranslation("GameServer.LeagueGame.Msg3"), LanguageMgr.GetTranslation("GameServer.LeagueGame.Msg4"));
			}
		}*/



		public static void Stop()
		{
			smethod_3();
		}

		static WorldMgr()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			readerWriterLock_0 = new ReaderWriterLock();
			dictionary_0 = new Dictionary<int, GamePlayer>();
			CaddyRank = new Dictionary<int, UsersExtraInfo>();
			dictionary_1 = new Dictionary<int, AreaConfigInfo>();
			dictionary_2 = new Dictionary<int, EdictumInfo>();
			dictionary_3 = new Dictionary<int, ShopFreeCountInfo>();
			dictionary_4 = new Dictionary<int, UserExitRoomLogInfo>();
			LastTimeUpdateCaddyRank = DateTime.Now;
			string_0 = new string[17]
			{
				"gunny",
				"gun",
				"gunn",
				"g u n n y",
				"g unny",
				"g u nny",
				"g u n ny",
				"g un",
				"g u n",
				"com",
				"c om",
				"c o m",
				"net",
				"n et",
				"n e t",
				"ᶰ",
				"¥"
			};
		}
	}
}
