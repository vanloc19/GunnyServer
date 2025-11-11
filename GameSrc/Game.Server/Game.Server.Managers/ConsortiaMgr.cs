using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class ConsortiaMgr
	{
		private static Dictionary<string, int> _ally;

		private static Dictionary<int, ConsortiaInfo> _consortia;

		private static Dictionary<int, ConsortiaBossConfigInfo> _consortiaBossConfigInfos;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static ReaderWriterLock m_lock;

		public static bool AddConsortia(int consortiaID)
		{
			m_lock.AcquireWriterLock(-1);
			try
			{
				if (!_consortia.ContainsKey(consortiaID))
				{
					ConsortiaInfo info = new ConsortiaInfo
					{
						BuildDate = DateTime.Now,
						Level = 1,
						IsExist = true,
						ConsortiaName = "",
						ConsortiaID = consortiaID
					};
					_consortia.Add(consortiaID, info);
				}
			}
			catch (Exception exception)
			{
				log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
			return false;
		}

		public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
		{
			if (consortiaID1 == 0 || consortiaID2 == 0 || consortiaID1 == consortiaID2)
			{
				return -1;
			}
			ConsortiaInfo info = FindConsortiaInfo(consortiaID1);
			ConsortiaInfo info2 = FindConsortiaInfo(consortiaID2);
			if (info == null || info2 == null || info.Level < 3 || info2.Level < 3)
			{
				return -1;
			}
			return FindConsortiaAlly(consortiaID1, consortiaID2);
		}

		public static int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
		{
			if (roomType != 0)
			{
				return 0;
			}
			int playerCount = playercount / 2;
			int riches = 0;
			int state = 2;
			int num4 = 1;
			int num5 = 3;
			if (gameClass == eGameType.Guild)
			{
				num5 = 10;
				num4 = (int)RateMgr.GetRate(eRateType.Offer_Rate);
			}
			float rate = RateMgr.GetRate(eRateType.Riches_Rate);
			using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
			{
				if (gameClass == eGameType.Free)
				{
					playerCount = 0;
				}
				else
				{
					bussiness.ConsortiaFight(consortiaWin, consortiaLose, playerCount, out riches, state, totalKillHealth, rate);
				}
				List<string> list = new List<string>();
				ConsortiaInfo consortiaSingle = bussiness.GetConsortiaSingle(consortiaWin);
				ConsortiaInfo consortiaSingle2 = bussiness.GetConsortiaSingle(consortiaLose);
				if (riches <= 0)
				{
					riches = 1;
				}
				bussiness.ConsortiaRichAdd(consortiaWin, ref riches);
				foreach (KeyValuePair<int, Player> pair in players)
				{
					if (pair.Value != null)
					{
						if (pair.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaWin)
						{
							pair.Value.PlayerDetail.AddOffer((playerCount + num5) * num4);
							pair.Value.PlayerDetail.PlayerCharacter.RichesRob += riches;
							list.Add($"[{pair.Value.PlayerDetail.PlayerCharacter.NickName}]");
						}
						else if (pair.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaLose)
						{
							pair.Value.PlayerDetail.AddOffer((int)Math.Round((double)playerCount * 0.5) * num4);
							pair.Value.PlayerDetail.RemoveOffer(num5);
						}
					}
				}
				return riches;
			}
		}

		public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
		{
			m_lock.AcquireWriterLock(-1);
			try
			{
				if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
				{
					_consortia[consortiaID].ShopLevel = shopLevel;
				}
			}
			catch (Exception exception)
			{
				log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
			return false;
		}

		public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
		{
			m_lock.AcquireWriterLock(-1);
			try
			{
				if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
				{
					_consortia[consortiaID].SmithLevel = smithLevel;
				}
			}
			catch (Exception exception)
			{
				log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
			return false;
		}

		public static bool ConsortiaSkillUpGrade(int consortiaID, int skillLevel)
		{
			bool result = false;
			if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
			{
				_consortia[consortiaID].SkillLevel = skillLevel;
			}

			return result;
		}

		public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
		{
			m_lock.AcquireWriterLock(-1);
			try
			{
				if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
				{
					_consortia[consortiaID].StoreLevel = storeLevel;
				}
			}
			catch (Exception exception)
			{
				log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
			return false;
		}

		public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
		{
			bool flag = false;
			m_lock.AcquireWriterLock(-1);
			try
			{
				if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
				{
					_consortia[consortiaID].Level = consortiaLevel;
					return flag;
				}
				ConsortiaInfo info = new ConsortiaInfo
				{
					BuildDate = DateTime.Now,
					Level = consortiaLevel,
					IsExist = true
				};
				_consortia.Add(consortiaID, info);
				return flag;
			}
			catch (Exception exception)
			{
				log.Error("ConsortiaUpGrade", exception);
				return flag;
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
		}

		public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
		{
			if (cosortiaID1 == 0 || consortiaID2 == 0 || cosortiaID1 == consortiaID2)
			{
				return -1;
			}
			string str = ((cosortiaID1 >= consortiaID2) ? (consortiaID2 + "&" + cosortiaID1) : (cosortiaID1 + "&" + consortiaID2));
			m_lock.AcquireReaderLock(10000);
			try
			{
				if (_ally.ContainsKey(str))
				{
					return _ally[str];
				}
			}
			catch
			{
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			return 0;
		}

		public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
		{
			m_lock.AcquireReaderLock(10000);
			try
			{
				if (_consortia.ContainsKey(consortiaID))
				{
					return _consortia[consortiaID];
				}
			}
			catch
			{
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			return null;
		}

		private static int GetOffer(int state, eGameType gameType)
		{
			switch (gameType)
			{
				case eGameType.Free:
					switch (state)
					{
						case 0:
							return 1;
						case 1:
							return 0;
						case 2:
							return 3;
					}
					break;
				case eGameType.Guild:
					switch (state)
					{
						case 0:
							return 5;
						case 1:
							return 0;
						case 2:
							return 10;
					}
					break;
			}
			return 0;
		}

		public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
		{
			return GetOffer(FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
		}

		public static bool Init()
		{
			try
			{
				m_lock = new ReaderWriterLock();
				_ally = new Dictionary<string, int>();
				if (!Load(_ally))
				{
					return false;
				}
				_consortia = new Dictionary<int, ConsortiaInfo>();
				_consortiaBossConfigInfos = new Dictionary<int, ConsortiaBossConfigInfo>();
				if (!LoadConsortia(_consortia, _consortiaBossConfigInfos))
				{
					return false;
				}
				return true;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ConsortiaMgr", exception);
				}
				return false;
			}
		}

		public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
		{
			if (roomType != 0)
			{
				return -1;
			}
			int state = FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);
			if (state != -1)
			{
				int offer = GetOffer(state, gameClass);
				if (lose.PlayerCharacter.Offer < offer)
				{
					offer = lose.PlayerCharacter.Offer;
				}
				if (offer != 0)
				{
					players[win].GainOffer = offer;
					players[lose].GainOffer = -offer;
				}
			}
			return state;
		}

		private static bool Load(Dictionary<string, int> ally)
		{
			using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
			{
				ConsortiaAllyInfo[] consortiaAllyAll = bussiness.GetConsortiaAllyAll();
				foreach (ConsortiaAllyInfo info in consortiaAllyAll)
				{
					if (info.IsExist)
					{
						string str = ((info.Consortia1ID >= info.Consortia2ID) ? (info.Consortia2ID + "&" + info.Consortia1ID) : (info.Consortia1ID + "&" + info.Consortia2ID));
						if (!ally.ContainsKey(str))
						{
							ally.Add(str, info.State);
						}
					}
				}
			}
			return true;
		}

		private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia, Dictionary<int, ConsortiaBossConfigInfo> consortiaBossConfig)
		{
			using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
			{
				ConsortiaInfo[] consortiaAll = bussiness.GetConsortiaAll();
				foreach (ConsortiaInfo info in consortiaAll)
				{
					if (info.IsExist && !consortia.ContainsKey(info.ConsortiaID))
					{
						consortia.Add(info.ConsortiaID, info);
					}
				}
				ConsortiaBossConfigInfo[] consortiaBossConfigAll = bussiness.GetConsortiaBossConfigAll();
				foreach (ConsortiaBossConfigInfo info2 in consortiaBossConfigAll)
				{
					if (!consortiaBossConfig.ContainsKey(info2.BossLevel))
					{
						consortiaBossConfig.Add(info2.BossLevel, info2);
					}
				}
			}
			return true;
		}

		public static bool ReLoad()
		{
			try
			{
				Dictionary<string, int> ally = new Dictionary<string, int>();
				Dictionary<int, ConsortiaInfo> consortia = new Dictionary<int, ConsortiaInfo>();
				Dictionary<int, ConsortiaBossConfigInfo> consortiaBossConfig = new Dictionary<int, ConsortiaBossConfigInfo>();
				if (Load(ally) && LoadConsortia(consortia, consortiaBossConfig))
				{
					m_lock.AcquireWriterLock(-1);
					try
					{
						_ally = ally;
						_consortia = consortia;
						_consortiaBossConfigInfos = consortiaBossConfig;
						return true;
					}
					catch
					{
					}
					finally
					{
						m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ConsortiaMgr", exception);
				}
			}
			return false;
		}

		public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
		{
			string str = ((cosortiaID1 >= consortiaID2) ? (consortiaID2 + "&" + cosortiaID1) : (cosortiaID1 + "&" + consortiaID2));
			m_lock.AcquireWriterLock(-1);
			try
			{
				if (!_ally.ContainsKey(str))
				{
					_ally.Add(str, state);
				}
				else
				{
					_ally[str] = state;
				}
			}
			catch
			{
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
			return 0;
		}

		public static bool AddBuffConsortia(GamePlayer Player, ConsortiaBuffTempInfo ConsortiaBuffInfo, int consortiaId, int id, int validate)
		{
			switch (ConsortiaBuffInfo.group)
			{
				case 1:
					BufferList.CreatePayBuffer(101, ConsortiaBuffInfo.value, validate, id)?.Start(Player);
					break;
				case 3:
					BufferList.CreatePayBuffer(103, ConsortiaBuffInfo.value, validate, id)?.Start(Player);
					break;
				case 6:
					BufferList.CreatePayBuffer(106, ConsortiaBuffInfo.value, validate, id)?.Start(Player);
					break;
				case 8:
					Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Consortia.Msg2"));
					return false;
				case 11:
					BufferList.CreatePayBuffer(111, ConsortiaBuffInfo.value, validate, id)?.Start(Player);
					break;
				case 12:
                    Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Consortia.Msg2"));
                    return false;
                default:
					{
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							ConsortiaUserInfo[] memberByConsortia = playerBussiness.GetAllMemberByConsortia(consortiaId);
							AbstractBuffer abstractBuffer = null;
							switch (ConsortiaBuffInfo.group)
							{
								case 2:
									abstractBuffer = BufferList.CreatePayBuffer(102, ConsortiaBuffInfo.value, validate, id);
									break;
								case 4:
									abstractBuffer = BufferList.CreatePayBuffer(104, ConsortiaBuffInfo.value, validate, id);
									break;
								case 5:
									abstractBuffer = BufferList.CreatePayBuffer(105, ConsortiaBuffInfo.value, validate, id);
									break;
								case 7:
									abstractBuffer = BufferList.CreatePayBuffer(107, ConsortiaBuffInfo.value, validate, id);
									break;
								case 9:
									abstractBuffer = BufferList.CreatePayBuffer(109, ConsortiaBuffInfo.value, validate, id);
									break;
								case 10:
									abstractBuffer = BufferList.CreatePayBuffer(110, ConsortiaBuffInfo.value, validate, id);
									break;
							}
							ConsortiaUserInfo[] array = memberByConsortia;
							for (int i = 0; i < array.Length; i++)
							{
								GamePlayer playerById = WorldMgr.GetPlayerById(array[i].UserID);
								if (playerById != null)
								{
									abstractBuffer?.Start(playerById);
									if (playerById != Player)
									{
										playerById.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Consortia.Msg3"));
									}
								}
							}
							if (abstractBuffer != null)
							{
								ConsortiaBufferInfo info = playerBussiness.GetUserConsortiaBufferSingle(ConsortiaBuffInfo.id);
								if (info == null)
								{
									info = new ConsortiaBufferInfo();
									info.ConsortiaID = consortiaId;
									info.IsOpen = true;
									info.BufferID = ConsortiaBuffInfo.id;
									info.Type = abstractBuffer.Info.Type;
									info.Value = abstractBuffer.Info.Value;
									info.ValidDate = abstractBuffer.Info.ValidDate;
									info.BeginDate = abstractBuffer.Info.BeginDate;
								}
								else
								{
									info.BufferID = ConsortiaBuffInfo.id;
									info.Value = abstractBuffer.Info.Value;
									info.ValidDate += abstractBuffer.Info.ValidDate;
								}
								playerBussiness.SaveConsortiaBuffer(info);
							}
						}
						break;
					}
			}
			return true;
		}
	}
}
