using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Server.GuildBattle;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Games
{
	public class GameMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly long THREAD_INTERVAL = 40L;
		private static List<BaseGame> m_games;
		private static GuildBattleMgr m_guildBattle;
		private static Thread m_thread;
		private static bool m_running;
		private static int m_serverId;
		private static int m_boxBroadcastLevel;
		private static int m_gameId;

		private static readonly int CLEAR_GAME_INTERVAL = 5 * 1000;
		private static readonly int UPDATE_GAME_INTERVAL = 60 * 1000;
		private static long m_clearGamesTimer;
		private static long m_updateGamesTimer;
		private static DateTime m_synDate;

		public static GuildBattleMgr GuildBattle
		{
			get { return m_guildBattle; }
		}

		public static int SynDate
		{
			get { return DateTime.Compare(m_synDate.AddSeconds(THREAD_INTERVAL), DateTime.Now); }
		}

		public static int BoxBroadcastLevel
		{
			get { return m_boxBroadcastLevel; }
		}

		public static bool Setup(int serverId, int boxBroadcastLevel)
		{
			GameMgr.m_thread = new Thread(new ThreadStart(GameMgr.GameThread));
			GameMgr.m_games = new List<BaseGame>();
			m_guildBattle = new GuildBattleMgr();
			GameMgr.m_serverId = serverId;
			GameMgr.m_boxBroadcastLevel = boxBroadcastLevel;
			GameMgr.m_gameId = 0;
			m_synDate = DateTime.Now;
			return true;
		}
		public static bool Start()
		{
			if (!GameMgr.m_running)
			{
				GameMgr.m_running = true;
				GameMgr.m_thread.Start();
			}
			return true;
		}
		public static void Stop()
		{
			if (GameMgr.m_running)
			{
				GameMgr.m_running = false;
				GameMgr.m_thread.Join();
			}
		}
		private static void GameThread()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			long balance = 0;
			m_clearGamesTimer = TickHelper.GetTickCount();
			m_updateGamesTimer = TickHelper.GetTickCount();
			while (m_running)
			{
				long start = TickHelper.GetTickCount();
				try
				{
					UpdateGames(start);
					ClearStoppedGames(start);
					UpdateGuildBattle(start);
				}
				catch (Exception ex)
				{
					log.Error("Game Mgr Theard Error:", ex);
				}
				long end = TickHelper.GetTickCount();
				balance += THREAD_INTERVAL - (end - start);
				if (balance > 0)
				{
					if ((int)balance > UPDATE_GAME_INTERVAL)
					{
						Thread.Sleep(UPDATE_GAME_INTERVAL);
						if (log.IsErrorEnabled)
						{
							log.WarnFormat("Game Mgr current balance {0} ms!", balance);
						}
					}
					else
					{
						Thread.Sleep((int)balance);
					}
					balance = 0;
				}
				else
				{
					if (balance < -1000)
					{
						if (log.IsInfoEnabled)
							log.WarnFormat("Game Mgr is delay {0} ms!", balance);

						balance += 1000;
					}
				}
				m_synDate = DateTime.Now;
			}
			#region OLD
			//Thread.CurrentThread.Priority = ThreadPriority.Highest;
			//long num = 0L;
			//GameMgr.m_clearGamesTimer = TickHelper.GetTickCount();
			//while (GameMgr.m_running)
			//{
			//	long tickCount = TickHelper.GetTickCount();
			//	int num2 = 0;
			//	try
			//	{
			//		//num2 = GameMgr.UpdateGames(tickCount);
			//		num2 = GameMgr.UpdateGames(tickCount);
			//		GameMgr.UpdateGuildBattle(tickCount);
			//		if (GameMgr.m_clearGamesTimer <= tickCount)
			//		{
			//			GameMgr.m_clearGamesTimer += (long)GameMgr.CLEAR_GAME_INTERVAL;
			//			ArrayList arrayList = new ArrayList();
			//			foreach (BaseGame current in GameMgr.m_games)
			//			{
			//				if (current.GameState == eGameState.Stopped)
			//				{
			//					arrayList.Add(current);
			//				}
			//			}
			//			foreach (BaseGame item in arrayList)
			//			{
			//				GameMgr.m_games.Remove(item);
			//			}
			//			ThreadPool.QueueUserWorkItem(new WaitCallback(GameMgr.ClearStoppedGames), arrayList);
			//		}
			//	}
			//	catch (Exception exception)
			//	{
			//		GameMgr.log.Error("Game Mgr Thread Error:", exception);
			//	}
			//	long tickCount2 = TickHelper.GetTickCount();
			//	num += GameMgr.THREAD_INTERVAL - (tickCount2 - tickCount);
			//	if (tickCount2 - tickCount > GameMgr.THREAD_INTERVAL * 2L)
			//	{
			//		GameMgr.log.WarnFormat("Game Mgr spent too much times: {0} ms, count:{1}", tickCount2 - tickCount, num2);
			//	}
			//	if (num > 0L)
			//	{
			//		Thread.Sleep((int)num);
			//		num = 0L;
			//	}
			//	else
			//	{
			//		if (num < -1000L)
			//		{
			//			num += 1000L;
			//		}
			//	}
			//}
			#endregion
		}
		private static void ClearStoppedGames(long tick)
		{
			if (m_clearGamesTimer <= tick)
			{
				m_clearGamesTimer += CLEAR_GAME_INTERVAL;
				ArrayList temp = new ArrayList();
				lock (m_games)
				{
					foreach (BaseGame g in m_games)
					{
						if (g.GameState == eGameState.Stopped)
						{
							temp.Add(g);
						}
					}
					foreach (BaseGame g in temp)
					{
						m_games.Remove(g);
						try
						{
							g.Dispose();
						}
						catch (Exception ex)
						{
							log.Error("game dispose error:", ex);
						}
					}
				}
			}
		}
		public static void ClearAllGames()
		{
			ArrayList temp = new ArrayList();
			lock (m_games)
			{
				foreach (BaseGame g in m_games)
				{
					temp.Add(g);
				}
				foreach (BaseGame g in temp)
				{
					m_games.Remove(g);
					try
					{
						g.Dispose();
					}
					catch (Exception ex)
					{
						log.Error("game dispose error:", ex);
					}
				}
			}
		}
		private static int UpdateGames(long tick)
		{
			IList allGame = GameMgr.GetAllGame();
			if (allGame != null)
			{
				foreach (BaseGame baseGame in allGame)
				{
					try
					{
						baseGame.Update(tick);
					}
					catch (Exception exception)
					{
						GameMgr.log.Error("Game  updated error:", exception);
					}
				}

				if (m_updateGamesTimer <= tick)
				{
					m_updateGamesTimer += UPDATE_GAME_INTERVAL;
				}

				return allGame.Count;
			}
			return 0;
		}
		private static void UpdateGuildBattle(long tick)
		{
			try
			{
				m_guildBattle.Update(tick);
			}
			catch (Exception ex)
			{
				log.Error("Game GuildBattle Updated Error:", ex);
			}
		}
		public static List<BaseGame> GetAllGame()
		{
			List<BaseGame> list = new List<BaseGame>();
			List<BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			try
			{
				list.AddRange(GameMgr.m_games);
			}
			finally
			{
				Monitor.Exit(games);
			}
			return list;
		}
		public static BaseGame StartPVPGame(int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			BaseGame result;
			try
			{
				int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId);
				Map map = MapMgr.CloneMap(mapIndex2);
				if (map != null)
				{
					PVPGame pVPGame = new PVPGame(GameMgr.m_gameId++, roomId, red, blue, map, roomType, gameType, timeType);
					List<BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(pVPGame);
					}
					finally
					{
						Monitor.Exit(games);
					}
					pVPGame.Prepare();
					result = pVPGame;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception exception)
			{
				GameMgr.log.Error("Create game error:", exception);
				result = null;
			}
			return result;
		}
		public static BaseGame StartPVEGame(int roomId, List<IGamePlayer> players, int copyId, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int levelLimits, int currentFloor)
		{
			BaseGame result;
			try
			{
				PveInfo pveInfo;
				if (copyId == 0 || copyId == 100000)
				{
					pveInfo = PveInfoMgr.GetPveInfoByType(roomType, levelLimits);
				}
				else
				{
					pveInfo = PveInfoMgr.GetPveInfoById(copyId);
				}
				if (pveInfo != null)
				{
					PVEGame pVEGame = new PVEGame(GameMgr.m_gameId++, roomId, pveInfo, players, null, roomType, gameType, timeType, hardLevel, currentFloor);
					List<BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(pVEGame);
					}
					finally
					{
						Monitor.Exit(games);
					}
					pVEGame.Prepare();
					result = pVEGame;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception exception)
			{
				GameMgr.log.Error("Create game error:", exception);
				result = null;
			}
			return result;
		}

		public static BaseGame StartChallengePVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, BaseRoom redRoom, BaseRoom blueRoom, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			try
			{
				int index = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
				Map map = MapMgr.CloneMap(index);
				if (map != null)
				{
					BattleGame game = new BattleGame(m_gameId++, red, redRoom, blue, blueRoom, map, roomType, gameType, timeType);
					lock (m_games)
					{
						m_games.Add(game);
					}
					game.Prepare();
					Console.WriteLine("StartChallengePVPGame create {0} with {1} player, createID {2}", game.RoomType, game.PlayerCount, game.Id);
					return game;
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				log.Error("create Game error:", ex);
				return null;
			}
		}
	}
}
