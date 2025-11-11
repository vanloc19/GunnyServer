using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Fighting.Server.GameObjects;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Fighting.Server.Games
{
	public class GameMgr
	{
		private static readonly ILog ilog_0;

		public static readonly long THREAD_INTERVAL;

		private static Dictionary<int, BaseGame> dictionary_0;

		private static Thread thread_0;

		private static bool bool_0;

		private static int int_0;

		private static int int_1;

		private static int int_2;

		private static readonly int int_3;

		private static long long_0;

		public static int BoxBroadcastLevel => int_1;

		public static bool Setup(int serverId, int boxBroadcastLevel)
		{
			thread_0 = new Thread(smethod_0);
			dictionary_0 = new Dictionary<int, BaseGame>();
			int_0 = serverId;
			int_1 = boxBroadcastLevel;
			int_2 = 0;
			return true;
		}

		public static void Start()
		{
			if (!bool_0)
			{
				bool_0 = true;
				thread_0.Start();
			}
		}

		public static void Stop()
		{
			if (bool_0)
			{
				bool_0 = false;
				thread_0.Join();
			}
		}

		private static void smethod_0()
		{
			long num = 0L;
			long_0 = TickHelper.GetTickCount();
			while (bool_0)
			{
				long tickCount1 = TickHelper.GetTickCount();
				try
				{
					smethod_1(tickCount1);
					smethod_2(tickCount1);
				}
				catch (Exception ex)
				{
					ilog_0.Error("Room Mgr Thread Error:", ex);
				}
				long tickCount2 = TickHelper.GetTickCount();
				num += THREAD_INTERVAL - (tickCount2 - tickCount1);
				if (num > 0)
				{
					Thread.Sleep((int)num);
					num = 0L;
				}
				else if (num < -1000)
				{
					ilog_0.WarnFormat("Room Mgr is delay {0} ms!", num);
					num += 1000;
				}
			}
		}

		private static void smethod_1(long long_1)
		{
			IList games = GetGames();
			if (games == null)
			{
				return;
			}
			foreach (BaseGame baseGame in games)
			{
				try
				{
					baseGame.Update(long_1);
				}
				catch (Exception ex)
				{
					ilog_0.Error("Game  updated error:", ex);
				}
			}
		}

		private static void smethod_2(long long_1)
		{
			if (long_0 > long_1)
			{
				return;
			}
			long_0 += int_3;
			ArrayList arrayList = new ArrayList();
			lock (dictionary_0)
			{
				foreach (BaseGame baseGame2 in dictionary_0.Values)
				{
					if (baseGame2.GameState == eGameState.Stopped)
					{
						arrayList.Add(baseGame2);
					}
				}
				foreach (BaseGame baseGame in arrayList)
				{
					dictionary_0.Remove(baseGame.Id);
					try
					{
						baseGame.Dispose();
					}
					catch (Exception ex)
					{
						ilog_0.Error("game dispose error:", ex);
					}
				}
			}
		}

		public static List<BaseGame> GetGames()
		{
			List<BaseGame> baseGameList = new List<BaseGame>();
			lock (dictionary_0)
			{
				baseGameList.AddRange(dictionary_0.Values);
				return baseGameList;
			}
		}

		public static BaseGame FindGame(int id)
		{
			lock (dictionary_0)
			{
				if (dictionary_0.ContainsKey(id))
				{
					return dictionary_0[id];
				}
			}
			return null;
		}

		public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			try
			{
				Map map = MapMgr.CloneMap(MapMgr.GetMapIndex(mapIndex, (byte)roomType, int_0));
				if (map == null)
				{
					return null;
				}
				PVPGame pvpGame = new PVPGame(int_2++, 0, red, blue, map, roomType, gameType, timeType);
				lock (dictionary_0)
				{
					dictionary_0.Add(pvpGame.Id, pvpGame);
				}
				pvpGame.Prepare();
				return pvpGame;
			}
			catch (Exception ex)
			{
				ilog_0.Error("Create game error:", ex);
				return null;
			}
		}

		public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			try
			{
				Map map = MapMgr.CloneMap(MapMgr.GetMapIndex(mapIndex, (byte)roomType, int_0));
				if (map == null)
				{
					return null;
				}
				BattleGame game = new BattleGame(int_2++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType);
				lock (dictionary_0)
				{
					dictionary_0.Add(game.Id, game);
				}
				game.Prepare();
				SendStartMessage(game);
				return game;
			}
			catch (Exception ex)
			{
				ilog_0.Error("Create battle game error:", ex);
				return null;
			}
		}

		public static void SendStartMessage(BattleGame game)
		{
			GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CHAT);
			pkg.WriteInt(2);
			if (game.GameType == eGameType.Free)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.free", new object[0]));
			}
			else if (game.GameType == eGameType.Leage)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.Leage", new object[0]));
			}
			else if (game.GameType == eGameType.GuildLeage)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.GuildLeage", new object[0]));
			}
			else if (game.GameType == eGameType.Guild)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.Consortia", new object[0]));
			}
			else if (game.RoomType == eRoomType.EliteGameScore)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.Score", new object[0]));
			}
			else if (game.RoomType == eRoomType.EliteGameChampion)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.Rank", new object[0]));
			}
			else
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.free", new object[0]));
			}
			game.SendToAll(pkg);
            #region OLD
            /*Console.WriteLine($"SendStartMessage = {game.GameType} | {game.RoomType}");
			GSPacketIn pkg1 = new GSPacketIn((short)3);
			pkg1.WriteInt(2);
			if (game.GameType != eGameType.EliteGameScore || game.GameType != eGameType.EliteGameChampion)
			{
				if (game.GameType == eGameType.Free)
				{
					foreach (Player allFightPlayer in game.GetAllFightPlayers())
					{
						(allFightPlayer.PlayerDetail as ProxyPlayer).m_antiAddictionRate = 1.0;
						GSPacketIn pkg2 = GameMgr.SendBufferList(allFightPlayer, (allFightPlayer.PlayerDetail as ProxyPlayer).Buffers);
						game.SendToAll(pkg2);
					}
					GSPacketIn pkg3 = GameMgr.smethod_3(DateTime.Now);
					game.SendToAll(pkg3);
					pkg1.WriteString(LanguageMgr.GetTranslation("FightServer.MergeRoom.Match.Success"));
				}
				else if (game.GameType == eGameType.Leage)
				{
					pkg1.WriteString("K?t h?p tr?n ??u chi?n th?n th�nh c�ng");
				}
				else
				{
					pkg1.WriteString(LanguageMgr.GetTranslation("FightServer.MergeRoom.Guild.Success"));
				}
			}
			else
			{
				pkg1.WriteString(LanguageMgr.GetTranslation("FightServer.MergeRoom.EliteGame.Success"));
			}
			game.SendToAll(pkg1, (IGamePlayer)null);*/
            #endregion
        }

        public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
		{
			GSPacketIn gsPacketIn = new GSPacketIn(186, player.Id);
			gsPacketIn.WriteInt(infos.Count);
			foreach (BufferInfo info in infos)
			{
				gsPacketIn.WriteInt(info.Type);
				gsPacketIn.WriteBoolean(info.IsExist);
				gsPacketIn.WriteDateTime(info.BeginDate);
				gsPacketIn.WriteInt(info.ValidDate);
				gsPacketIn.WriteInt(info.Value);
			}
			return gsPacketIn;
		}

		private static GSPacketIn smethod_3(DateTime dateTime_0)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(999);
			gSPacketIn.WriteDateTime(dateTime_0);
			return gSPacketIn;
		}

		static GameMgr()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			THREAD_INTERVAL = 40L;
			int_3 = 60000;
		}
	}
}
