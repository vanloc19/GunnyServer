using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Fighting.Server.Games;
using Fighting.Server.Guild;
using Game.Logic;
using Game.Server.Managers;
using log4net;

namespace Fighting.Server.Rooms
{
	public class ProxyRoomMgr
	{
		private static readonly ILog ilog_0;

		public static readonly int THREAD_INTERVAL;

		public static readonly int PICK_UP_INTERVAL;

		public static readonly int CLEAR_ROOM_INTERVAL;

		private static bool startWithNpc;

		private static int serverId;

		private static Queue<IAction> queue_0;

		private static Thread thread_0;

		private static Dictionary<int, ProxyRoom> dictionary_0;

		private static int int_1;

		private static long long_0;

		private static long long_1;

		public static bool Setup()
		{
			thread_0 = new Thread(smethod_0);
			return true;
		}

		public static void Start()
		{
			if (!startWithNpc)
			{
				startWithNpc = true;
				thread_0.Start();
			}
		}

		public static void Stop()
		{
			if (startWithNpc)
			{
				startWithNpc = false;
				thread_0.Join();
			}
		}

		public static void AddAction(IAction action)
		{
			lock (queue_0)
			{
				queue_0.Enqueue(action);
			}
		}

		private static void smethod_0()
		{
			long num = 0L;
			long_1 = TickHelper.GetTickCount();
			long_0 = TickHelper.GetTickCount();
			while (startWithNpc)
			{
				long tickCount1 = TickHelper.GetTickCount();
				try
				{
					smethod_1();
					if (long_0 <= tickCount1)
					{
						long_0 += PICK_UP_INTERVAL;
						PickUpRooms(tickCount1);
					}
					if (long_1 <= tickCount1)
					{
						long_1 += CLEAR_ROOM_INTERVAL;
						smethod_4(tickCount1);
						smethod_5();
					}
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

		private static void smethod_1()
		{
			IAction[] array = null;
			lock (queue_0)
			{
				if (queue_0.Count > 0)
				{
					array = new IAction[queue_0.Count];
					queue_0.CopyTo(array, 0);
					queue_0.Clear();
				}
			}
			if (array == null)
			{
				return;
			}
			IAction[] array2 = array;
			IAction[] array3 = array2;
			foreach (IAction action in array3)
			{
				try
				{
					action.Execute();
				}
				catch (Exception ex)
				{
					ilog_0.Error("RoomMgr execute action error:", ex);
				}
			}
		}

		private static void PickUpRooms(long tick)
        {
			List<ProxyRoom> rooms = GetWaitMatchRoomUnsafe();
			foreach (ProxyRoom red in rooms)
            {
				ProxyRoom matchRoom = null;
				if (red.IsPlaying)
				{
					break;
				}

				if (red.IsCrossZone)
                {
					switch (red.RoomType)
                    {
						case eRoomType.Match:
                            {
								if (red.GameType == eGameType.Guild || red.GameType == eGameType.GuildLeage)
                                {
									foreach (ProxyRoom blue in rooms)
                                    {
										if (blue.GuildId != 0 && blue.GuildId == red.GuildId)
										{
											continue;
										}
										if (blue != red && blue.PlayerCount == red.PlayerCount && !blue.IsPlaying && blue.IsCrossZone && (blue.GameType == eGameType.Guild || blue.GameType == eGameType.GuildLeage))
										{
											matchRoom = blue;
										}
									}
								}
								else
                                {
									foreach (ProxyRoom blue in rooms)
									{
										if (blue != red && blue.PlayerCount == red.PlayerCount && !blue.IsPlaying && blue.IsCrossZone && (blue.GameType == eGameType.ALL || blue.GameType == eGameType.Free || blue.GameType == eGameType.Leage))
										{
											matchRoom = blue;
										}
									}
								}
							}
							break;
					}
					if (matchRoom != null)
					{
						smethod_6(red, matchRoom);
					}
					else
					{
						red.PickUpCount++;
					}
				}
				else
                {
					Console.WriteLine($"ProxyRoomMgr = {red.RoomType}");
					switch (red.RoomType)
                    {
                        case eRoomType.Match:
                            {
								if (red.GameType == eGameType.Guild || red.GameType == eGameType.GuildLeage)
								{
									foreach (ProxyRoom blue in rooms)
									{
										if (blue.GuildId != 0 && blue.GuildId == red.GuildId)
										{
											continue;
										}

										if (blue != red && blue.PlayerCount == red.PlayerCount && !blue.IsPlaying && !blue.IsCrossZone && (blue.GameType == eGameType.Guild || blue.GameType == eGameType.GuildLeage))
										{
											matchRoom = blue;
										}
									}
								}
								else
								{
									foreach (ProxyRoom blue in rooms)
									{
										if (blue != red && !red.isAutoBot && blue.PlayerCount == red.PlayerCount && !blue.IsPlaying && !blue.IsCrossZone && (blue.GameType == eGameType.ALL || blue.GameType == eGameType.Free || blue.GameType == eGameType.Leage))
										{
											//Console.WriteLine($"red.AvgLevel = {red.AvgLevel}, {red.GetPlayers()[0].CurrentEnemyId}");
											//Console.WriteLine($"blue.AvgLevel = {blue.AvgLevel}, {blue.GetPlayers()[0].CurrentEnemyId}");

											if (Convert.ToBoolean(ConfigurationManager.AppSettings["FindLevelMatch"]))
											{
												//Console.WriteLine("True");
                                                if (red.AvgLevel <= blue.AvgLevel + 2 && red.AvgLevel >= blue.AvgLevel - 2)
                                                {
                                                    //Console.WriteLine("RED");
                                                    matchRoom = blue;
                                                }
                                                if (blue.AvgLevel <= red.AvgLevel + 2 && blue.AvgLevel >= red.AvgLevel - 2)
                                                {
                                                    //Console.WriteLine("Blue");
                                                    matchRoom = blue;
                                                }
                                            }
											else
											{
                                                //Console.WriteLine("False");
                                                matchRoom = blue;
											}
										}
									}
								}
							}
							break;
						case eRoomType.EliteGameScore:
                            {
								foreach(ProxyRoom blue in rooms)
                                {
									if (blue != red && !red.isAutoBot && blue.PlayerCount == red.PlayerCount && !blue.IsPlaying && !blue.IsCrossZone && (blue.GameType == eGameType.ALL || blue.GameType == eGameType.Free))
									{
										if(red.AvgLevel >= 30 && red.AvgLevel <= 40 && (blue.AvgLevel >= 30 && blue.AvgLevel <= 40))
                                        {
											matchRoom = blue;
                                        }
										else if(red.AvgLevel >= 41 && red.AvgLevel <= 50 && (blue.AvgLevel >= 41 && blue.AvgLevel <= 50))
                                        {
											matchRoom = blue;
                                        }
                                    }

								}
                            }
                            break;
						case eRoomType.EliteGameChampion:
                            {
								foreach (ProxyRoom blue in rooms)
								{
									//if (blue != red && !blue.isAutoBot && blue.RoomType == eRoomType.EliteGameChampion && !red.isAutoBot && !blue.IsPlaying && blue.PlayerCount == red.PlayerCount && blue.GetPlayers()[0].CurrentEnemyId == red.GetPlayers()[0].CurrentEnemyId)
									bool Test1 = blue != red;
									//bool Test2 = !red.isAutoBot;
									//bool Test3 = blue.PlayerCount == red.PlayerCount;
									//bool Test4 = !blue.IsPlaying;
									bool Test5 = blue.GetPlayers()[0].CurrentEnemyId == red.GetPlayers()[0].CurrentEnemyId;
									//Console.WriteLine($"NickName = {blue.GetPlayers()[0].PlayerCharacter} | T1 = {Test1} | T5 = {Test5} | {blue.RoomType} | {blue.GameType}");
									if (Test1)
									{
										Console.WriteLine($"Blue=>NickName = {blue.GetPlayers()[0].PlayerCharacter.NickName}___CurrentEnemyId = {blue.GetPlayers()[0].CurrentEnemyId}");
										Console.WriteLine($"Red=>NickName = {red.GetPlayers()[0].PlayerCharacter.NickName}___CurrentEnemyId = {red.GetPlayers()[0].CurrentEnemyId}");
									}
									if (blue != red && !red.isAutoBot && blue.PlayerCount == red.PlayerCount && !blue.IsPlaying && blue.GetPlayers()[0].CurrentEnemyId == red.GetPlayers()[0].CurrentEnemyId && (blue.GameType == eGameType.ALL || blue.GameType == eGameType.Free))
									{
										//Console.WriteLine($"{blue.GetPlayers()[0].CurrentEnemyId} == {red.GetPlayers()[0].PlayerCharacter.ID}");
										//Console.WriteLine($"{red.GetPlayers()[0].CurrentEnemyId} == {blue.GetPlayers()[0].PlayerCharacter.ID}");
										matchRoom = blue;
									}


                                }

                            }
							break;
						case eRoomType.ConsortiaBattle:
                            {
								foreach (ProxyRoom blue in rooms)
                                {
									if (blue != red && blue.RoomType == eRoomType.ConsortiaBattle && !blue.IsPlaying && blue.PlayerCount == red.PlayerCount)
                                    {
										matchRoom = blue;
                                    }
                                }
							}
							break;
                    }
					if(matchRoom != null)
                    {
						smethod_6(red, matchRoom);
                    }
					else if (red.RoomType == eRoomType.Match)
                    {
						if(red.NpcId != -1 && (DateTime.Now - red.createDate).TotalSeconds >= 40 && !red.startWithNpc && !red.isAutoBot)
                        {
							red.Client.SendBeginFightNpc(red.selfId, (int)red.RoomType, (int)red.GameType, red.NpcId);
							//Console.WriteLine("--1.?????ID? No.{0}", red.NpcId);
						}
                    }
					else if ((DateTime.Now - red.createDate).TotalSeconds >= 40 && red.startWithNpc && !red.isAutoBot)
                    {
						foreach (ProxyRoom autoBot in rooms)
						{
							if (autoBot != red && autoBot.PlayerCount == red.PlayerCount && !autoBot.IsPlaying && autoBot.isAutoBot && red.NpcId == autoBot.NpcId)
							{
								//Console.WriteLine("--2.???????ID? No.{0}", red.NpcId);
								smethod_6(red, autoBot);
							}
						}
					}
					if (red.isAutoBot && !red.IsPlaying)
					{
						red.PickUpCount--;
					}
					else
					{
						red.PickUpCount++;
					}

				}
			}
        }

		/*private static void smethod_2(long long_2)
		{
			List<ProxyRoom> waitMatchRoomUnsafe = GetWaitMatchRoomUnsafe();
			foreach (ProxyRoom proxyRoom in waitMatchRoomUnsafe)
			{
				int minValue = int.MinValue;
				ProxyRoom proxyRoom_1_1 = null;
				if (proxyRoom.IsPlaying)
				{
					continue;
				}
				Console.WriteLine($"roomType = {proxyRoom.RoomType} | gameType = {proxyRoom.GameType}");
				if (proxyRoom.RoomType == eRoomType.Match)
				{
					switch (proxyRoom.GameType)
					{
						case eGameType.Guild:
						case eGameType.GuildLeage:
							foreach (ProxyRoom current in waitMatchRoomUnsafe)
							{
								if ((current.GuildId == 0 || current.GuildId != proxyRoom.GuildId) && current != proxyRoom && (current.GameType == eGameType.Guild || current.GameType == eGameType.GuildLeage) && (proxyRoom.GameType == eGameType.Guild || proxyRoom.GameType == eGameType.GuildLeage) && !current.IsPlaying && current.PlayerCount == proxyRoom.PlayerCount && !proxyRoom.isAutoBot && !current.isAutoBot && proxyRoom.ZoneId == current.ZoneId)
								{
									proxyRoom_1_1 = current;
								}
							}
							break;
						case eGameType.ALL:
						case eGameType.Leage:
							foreach (ProxyRoom current2 in waitMatchRoomUnsafe)
							{
								if ((current2.GuildId == 0 || current2.GuildId != proxyRoom.GuildId) && current2 != proxyRoom && !current2.IsPlaying && current2.PlayerCount == proxyRoom.PlayerCount)
								{
									proxyRoom_1_1 = current2;
								}
							}
							break;
						default:
							if (!proxyRoom.isAutoBot && !proxyRoom.startWithNpc)
							{
								ProxyRoom unsafeWithResult = GetMathRoomUnsafeWithResult(proxyRoom);
								if (unsafeWithResult != null)
								{
									proxyRoom_1_1 = unsafeWithResult;
									Console.WriteLine("StartMatch in rate: {0}% FP and ratelevel: {1}", proxyRoom.PickUpRate, proxyRoom.PickUpRateLevel);
								}
								else
								{
									proxyRoom.PickUpRateLevel++;
									proxyRoom.PickUpRate += 10;
								}
							}
							break;
					}
				}
				if (proxyRoom.RoomType == eRoomType.ConsortiaBattle)
				{
					foreach (ProxyRoom current6 in waitMatchRoomUnsafe)
					{
						if (current6 != proxyRoom && current6.RoomType == eRoomType.ConsortiaBattle && !current6.IsPlaying && current6.PlayerCount == proxyRoom.PlayerCount)
						{
							proxyRoom_1_1 = current6;
						}
					}
				}
				else if (proxyRoom.RoomType == eRoomType.EliteGameScore)
				{
					foreach (ProxyRoom blue in waitMatchRoomUnsafe)
					{
						if (blue != proxyRoom && blue.GameType == eGameType.EliteGameScore && !blue.IsPlaying && blue.PlayerCount == proxyRoom.PlayerCount && !proxyRoom.isAutoBot && !blue.isAutoBot && blue.EliteGameType == proxyRoom.EliteGameType)
						{
							if (proxyRoom.AvgLevel >= 30 && proxyRoom.AvgLevel <= 40 && (blue.AvgLevel >= 30 && blue.AvgLevel <= 40))
							{
								proxyRoom_1_1 = blue;
							}
							else if (proxyRoom.AvgLevel >= 41 && proxyRoom.AvgLevel <= 50 && (blue.AvgLevel >= 41 && blue.AvgLevel <= 50))
							{
								proxyRoom_1_1 = blue;
							}
						}
					}
				}
				else if (proxyRoom.RoomType == eRoomType.EliteGameChampion)
				{
					foreach (ProxyRoom blue in waitMatchRoomUnsafe)
					{
						if (blue != proxyRoom && !blue.isAutoBot && blue.GameType == eGameType.EliteGameChampion && !proxyRoom.isAutoBot && !blue.IsPlaying && blue.PlayerCount == proxyRoom.PlayerCount && blue.GetPlayers()[0].CurrentEnemyId == proxyRoom.GetPlayers()[0].CurrentEnemyId)
						{
							proxyRoom_1_1 = blue;
						}

					}
				}
				if (proxyRoom_1_1 != null)
				{
					smethod_6(proxyRoom, proxyRoom_1_1);
					continue;
				}
				if (!proxyRoom.IsCrossZone)
				{
					if (proxyRoom.PickUpCount >= 4 && !proxyRoom.startWithNpc && !proxyRoom.isAutoBot && proxyRoom.RoomType == eRoomType.Match && proxyRoom.PlayerCount == 1)
					{
						proxyRoom.startWithNpc = true;
						proxyRoom.Client.SendBeginFightNpc(proxyRoom.selfId, (int)proxyRoom.RoomType, (int)proxyRoom.GameType, proxyRoom.NpcId);
						Console.WriteLine("Call AutoBot No.{0}", proxyRoom.NpcId);
					}
					else if (proxyRoom.startWithNpc && !proxyRoom.isAutoBot)
					{
						bool flag = false;
						foreach (ProxyRoom proxyRoom_1_2 in waitMatchRoomUnsafe)
						{
							if (proxyRoom_1_2 != proxyRoom && proxyRoom_1_2.PlayerCount == proxyRoom.PlayerCount && !proxyRoom_1_2.IsPlaying && proxyRoom_1_2.isAutoBot && proxyRoom.NpcId == proxyRoom_1_2.NpcId)
							{
								flag = true;
								Console.WriteLine("Start fight with AutoBot No.{0}. RoomType: {1}, GameType: {2}", proxyRoom.NpcId, proxyRoom.RoomType, proxyRoom.GameType);
								smethod_6(proxyRoom, proxyRoom_1_2);
								break;
							}
						}
						if (!flag)
						{
							proxyRoom.PickUpNPCTotal++;
							Console.WriteLine("Fight with AutoBot No.{0} - Step: {1} is error no room", proxyRoom.NpcId, proxyRoom.PickUpNPCTotal);
							if (proxyRoom.PickUpNPCTotal > 3)
							{
								proxyRoom.startWithNpc = false;
								proxyRoom.PickUpNPCTotal = 0;
							}
						}
					}
				}
				if (proxyRoom.isAutoBot && !proxyRoom.IsPlaying)
				{
					proxyRoom.PickUpCount--;
				}
				else
				{
					proxyRoom.PickUpCount++;
				}
			}
		}

		/*private static bool OnEliteTime(ProxyRoom red, ProxyRoom blue)
		{
			return OnEliteTime(red, blue, true);
		}*/
		/*private static bool OnEliteTime(ProxyRoom red, ProxyRoom blue, bool checkZone)
		{
			if (LiveMgr.LeagueOpen)
			{
				if (red.PlayerCount == 1 && blue.PlayerCount == 1)
				{
					return red.AreaId == blue.AreaId;
				}
				if (red.GetGrade(20).Count == 2 && blue.GetGrade(20).Count == 2)
				{
					return checkZone;
				}

				//Console.WriteLine("OnEliteTime Chien Than blue.PlayerCount:{0}, red.PlayerCount:{1}, red.ZoneId == blue.ZoneId:{2}", blue.PlayerCount, red.PlayerCount, red.ZoneId == blue.ZoneId);
				return red.PlayerCount == blue.PlayerCount;
			}

			//Console.WriteLine("OnEliteTime blue.PlayerCount:{0}, red.PlayerCount:{1}, red.ZoneId == blue.ZoneId:{2}", blue.PlayerCount, red.PlayerCount, red.ZoneId == blue.ZoneId);

			return red.AreaId == blue.AreaId && checkZone && blue.PlayerCount == red.PlayerCount;
		}*/

		private static void smethod_4(long long_2)
		{
			List<ProxyRoom> proxyRoomList = new List<ProxyRoom>();
			foreach (ProxyRoom proxyRoom2 in dictionary_0.Values)
			{
				if (!proxyRoom2.IsPlaying && proxyRoom2.Game != null)
				{
					proxyRoomList.Add(proxyRoom2);
				}
			}
			foreach (ProxyRoom proxyRoom in proxyRoomList)
			{
				dictionary_0.Remove(proxyRoom.RoomId);
				try
				{
					proxyRoom.Dispose();
				}
				catch (Exception ex)
				{
					ilog_0.Error("Room dispose error:", ex);
				}
			}
		}

		private static void smethod_5()
		{
			List<ProxyRoom> proxyRoomList = new List<ProxyRoom>();
			foreach (ProxyRoom proxyRoom2 in dictionary_0.Values)
			{
				if (!proxyRoom2.IsPlaying && proxyRoom2.PickUpCount < -1)
				{
					proxyRoomList.Add(proxyRoom2);
				}
			}
			foreach (ProxyRoom proxyRoom in proxyRoomList)
			{
				dictionary_0.Remove(proxyRoom.RoomId);
				try
				{
					proxyRoom.Dispose();
				}
				catch (Exception ex)
				{
					ilog_0.Error("Room dispose error:", ex);
				}
			}
		}

		private static void smethod_6(ProxyRoom proxyRoom_0, ProxyRoom proxyRoom_1)
		{
			int mapIndex = MapMgr.GetMapIndex(0, 0, serverId);
			eGameType gameType = eGameType.Free;
			eRoomType roomType = eRoomType.Match;
            Console.WriteLine($"red = {proxyRoom_0.RoomType}, blue = {proxyRoom_1.RoomType}");
            

			if (proxyRoom_0.GameType == proxyRoom_1.GameType)
			{
				gameType = proxyRoom_0.GameType;
			}
			//if (gameType == eGameType.EliteGameScore || gameType == eGameType.EliteGameChampion)
			//{
			//	roomType = eRoomType.Match;
			//}
			else if ((proxyRoom_0.GameType == eGameType.ALL && proxyRoom_1.GameType == eGameType.Guild) || (proxyRoom_1.GameType == eGameType.ALL && proxyRoom_0.GameType == eGameType.Guild))
			{
				gameType = eGameType.Guild;
			}

			if(proxyRoom_0.RoomType == eRoomType.EliteGameScore && proxyRoom_1.RoomType == eRoomType.EliteGameScore)
			{
				roomType = eRoomType.EliteGameScore;
				gameType = eGameType.Free;
			}
            if (proxyRoom_0.RoomType == eRoomType.EliteGameChampion && proxyRoom_1.RoomType == eRoomType.EliteGameChampion)
            {
                roomType = eRoomType.EliteGameChampion;
                gameType = eGameType.Free;
            }

            Console.WriteLine($"Room nay la = {roomType}");

			BaseGame game = GameMgr.StartBattleGame(proxyRoom_0.GetPlayers(), proxyRoom_0, proxyRoom_1.GetPlayers(), proxyRoom_1, mapIndex, roomType, gameType, 2);
			if (game != null)
			{
				if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSoloBattle"]))
				{
					proxyRoom_0.SetDefaultDamageAll();
					proxyRoom_1.SetDefaultDamageAll();
				}
				proxyRoom_1.StartGame(game);
				proxyRoom_0.StartGame(game);
				if (game.GameType == eGameType.Guild || game.GameType == eGameType.GuildLeage)
				{
					proxyRoom_0.Client.SendConsortiaAlly(proxyRoom_0.GetPlayers()[0].PlayerCharacter.ConsortiaID, proxyRoom_1.GetPlayers()[0].PlayerCharacter.ConsortiaID, game.Id);
				}
			}
		}

		public static void StartWithNpcUnsafe(ProxyRoom room)
		{
			int npcId = room.NpcId;
			ProxyRoom roomUnsafe = GetRoomUnsafe(room.RoomId);
			foreach (ProxyRoom proxyRoom_1 in GetWaitMatchRoomUnsafe())
			{
				if (proxyRoom_1.isAutoBot && !proxyRoom_1.IsPlaying && proxyRoom_1.Game == null && proxyRoom_1.NpcId == npcId)
				{
					Console.WriteLine("Start fight with AutoBot or VPlayer No.{0} ", npcId);
					smethod_6(roomUnsafe, proxyRoom_1);
				}
			}
		}

		public static bool AddRoomUnsafe(ProxyRoom room)
		{
			if (dictionary_0.ContainsKey(room.RoomId))
			{
				return false;
			}
			dictionary_0.Add(room.RoomId, room);
			return true;
		}

		public static bool RemoveRoomUnsafe(int roomId)
		{
			if (!dictionary_0.ContainsKey(roomId))
			{
				return false;
			}
			dictionary_0.Remove(roomId);
			return true;
		}

		public static ProxyRoom GetRoomUnsafe(int roomId)
		{
			if (dictionary_0.ContainsKey(roomId))
			{
				return dictionary_0[roomId];
			}
			return null;
		}

		public static ProxyRoom[] GetAllRoom()
		{
			lock (dictionary_0)
			{
				return GetAllRoomUnsafe();
			}
		}

		public static ProxyRoom[] GetAllRoomUnsafe()
		{
			ProxyRoom[] array = new ProxyRoom[dictionary_0.Values.Count];
			dictionary_0.Values.CopyTo(array, 0);
			return array;
		}

		public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
		{
			List<ProxyRoom> proxyRoomList = new List<ProxyRoom>();
			foreach (ProxyRoom proxyRoom in dictionary_0.Values)
			{
				if (!proxyRoom.IsPlaying && proxyRoom.Game == null)
				{
					proxyRoomList.Add(proxyRoom);
				}
			}
			return proxyRoomList;
		}

		public static List<ProxyRoom> GetWaitMatchRoomWithoutBotUnsafe(ProxyRoom roomCompare)
		{
			List<ProxyRoom> proxyRoomList = new List<ProxyRoom>();
			foreach (ProxyRoom proxyRoom in dictionary_0.Values)
			{
				if (!proxyRoom.IsPlaying && proxyRoom.Game == null && !proxyRoom.isAutoBot && !proxyRoom.startWithNpc && proxyRoom.IsCrossZone == roomCompare.IsCrossZone && (proxyRoom.IsCrossZone || proxyRoom.ZoneId == roomCompare.ZoneId))
				{
					proxyRoomList.Add(proxyRoom);
				}
			}
			return proxyRoomList;
		}

		public static ProxyRoom GetMathRoomUnsafeWithResult(ProxyRoom roomCompare)
		{
			List<ProxyRoom> source = new List<ProxyRoom>();
			bool flag = false;
			List<ProxyRoom> withoutBotUnsafe = GetWaitMatchRoomWithoutBotUnsafe(roomCompare);
			foreach (ProxyRoom proxyRoom2 in withoutBotUnsafe)
			{
				if (!proxyRoom2.IsPlaying && proxyRoom2.Game == null && proxyRoom2 != roomCompare && (proxyRoom2.PickUpRateLevel >= roomCompare.PickUpRateLevel || roomCompare.PickUpRateLevel == 1) && proxyRoom2.PlayerCount == roomCompare.PlayerCount)
				{
					int num2 = roomCompare.AvgLevel - roomCompare.PickUpRateLevel;
					int num4 = roomCompare.AvgLevel + roomCompare.PickUpRateLevel;
					if (proxyRoom2.AvgLevel >= num2 && proxyRoom2.AvgLevel <= num4)
					{
						source.Add(proxyRoom2);
						flag = true;
					}
				}
			}
			if (source.Count == 0)
			{
				foreach (ProxyRoom proxyRoom in withoutBotUnsafe)
				{
					if (!proxyRoom.IsPlaying && proxyRoom.Game == null && proxyRoom != roomCompare && (proxyRoom.PickUpRate >= roomCompare.PickUpRate || roomCompare.PickUpRate == 1) && proxyRoom.PlayerCount == roomCompare.PlayerCount)
					{
						int num1 = roomCompare.FightPower - roomCompare.FightPower / 100 * roomCompare.PickUpRate;
						int num3 = roomCompare.FightPower + roomCompare.FightPower / 100 * roomCompare.PickUpRate;
						if (proxyRoom.FightPower >= num1 && proxyRoom.FightPower <= num3)
						{
							source.Add(proxyRoom);
						}
					}
				}
			}
			if (source.Count <= 0)
			{
				return null;
			}
			List<ProxyRoom> obj = (flag ? source.OrderBy((ProxyRoom a) => a.AvgLevel).ToList() : source.OrderBy((ProxyRoom a) => a.FightPower).ToList());
			return obj[obj.Count / 2];
		}

		public static int NextRoomId()
		{
			return Interlocked.Increment(ref int_1);
		}

		public static void AddRoom(ProxyRoom room)
		{
			AddAction(new AddRoomAction(room));
		}

		public static void RemoveRoom(ProxyRoom room)
		{
			AddAction(new RemoveRoomAction(room));
		}

		static ProxyRoomMgr()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			THREAD_INTERVAL = 20;
			PICK_UP_INTERVAL = 5000;
			CLEAR_ROOM_INTERVAL = 250;
			startWithNpc = false;
			serverId = 1;
			queue_0 = new Queue<IAction>();
			dictionary_0 = new Dictionary<int, ProxyRoom>();
			int_1 = 0;
			long_0 = 0L;
			long_1 = 0L;
		}
	}
}
