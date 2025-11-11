using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Events;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.ConsortiaTask;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using Game.Server.Statics;
using Game.Server.RingStation;
using log4net;
using log4net.Config;
using Game.Logic.Game.Logic;
using Game.Server.LittleGame;
using Game.Server.GameObjects;
using Game.Server.Managers.EliteGame;
using Game.Server.GMActives;

namespace Game.Server
{
	public class GameServer : BaseServer
	{
		private LoginServerConnector _loginServer;

		//private WorldServerConnector _wordServer;

		private int _shutdownCount = 6;

		private Timer _shutdownTimer;

		private const int BUF_SIZE = 8192;

		Update_TOP upTOP = new Update_TOP();

		public static readonly string Edition = "2612558";

		public static bool KeepRunning = false;

		public static bool IsSendLeagueAward = false;

		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected Timer m_bagMailScanTimer;

		protected Timer m_buffScanTimer;

		private static bool m_compiled = false;

		private GameServerConfig m_config;

		private bool m_debugMenory;

		private static GameServer m_instance = null;

		private bool m_isRunning;

		private Queue m_packetBufPool;

		protected Timer m_pingCheckTimer;

		protected Timer m_qqTipScanTimer;

		protected Timer m_saveDbTimer;

		protected Timer m_saveRecordTimer;

		private static int m_tryCount = 4;

		private static int m_worldTryCount = 4;

		public GameServerConfig Configuration => m_config;

		public static GameServer Instance => m_instance;

		public LoginServerConnector LoginServer => _loginServer;

		//public WorldServerConnector WorldServer => _wordServer;


		public int PacketPoolSize => m_packetBufPool.Count;

		protected GameServer(GameServerConfig config)
		{
			m_config = config;
			if (log.IsDebugEnabled)
			{
				log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
				log.Debug("Gameserver root directory is: " + Configuration.RootDirectory);
				log.Debug("Changing directory to root directory");
			}
			Directory.SetCurrentDirectory(Configuration.RootDirectory);
		}

		public byte[] AcquirePacketBuffer()
		{
			lock (m_packetBufPool.SyncRoot)
			{
				if (m_packetBufPool.Count > 0)
				{
					return (byte[])m_packetBufPool.Dequeue();
				}
			}
			log.Warn("packet buffer pool is empty!");
			return new byte[8192];
		}

		private bool AllocatePacketBuffers()
		{
			int capacity = Configuration.MaxClientCount * 3;
			m_packetBufPool = new Queue(capacity);
			for (int i = 0; i < capacity; i++)
			{
				m_packetBufPool.Enqueue(new byte[8192]);
			}
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("allocated packet buffers: {0}", capacity.ToString());
			}
			return true;
		}

		protected void BuffScanTimerProc(object sender)
		{
			try
			{
				upTOP.UpdateCeleb();
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Buff Scaning ...");
					log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				int num2 = 0;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;

				EliteGameMgr.CheckEliteGameEvent();

				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer player in allPlayers)
				{
					if (player.BufferList != null)
					{
						player.BufferList.Update();
						num2++;
					}
					player.ResetTotem(false);
				}

				// save all gmactives
				GmActivityMgr.ScanAction();
				GmActivityMgr.DoAction();

				if (DateTime.Now.Hour == GameProperties.LittleGameStartHourse)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Little Game Running Sucess!");
					Console.ResetColor();
					if (!LittleGameWorldMgr.IsOpen)
					{
						TaskMgr.Init();
					}
				}

				if (DateTime.Now.Hour == GameProperties.LittleGameStartHourse + GameProperties.LittleGameTimeSpending)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Little Game Closed Sucess!");
					Console.ResetColor();
					if (LittleGameWorldMgr.IsOpen)
					{
						TaskMgr.Init();
					}
				}

				Thread.CurrentThread.Priority = priority;
				tickCount = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Buff Scan complete!");
					log.Info("Buff all " + num2 + " players in " + tickCount + "ms");
				}
				if (tickCount > 120000)
				{
					log.WarnFormat("Scan all Buff and {0} players in {1} ms", num2, tickCount);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("BuffScanTimerProc", exception);
				}
			}
			finally
            {
				if (log.IsErrorEnabled)
                {
					log.Info("GameMgr Scaning ...");
				}
				if (GameMgr.SynDate < 0)
                {
					GameMgr.ClearAllGames();
					GameMgr.Stop();
					GameMgr.Setup(Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION);
					GameMgr.Start();
					if (log.IsInfoEnabled)
					{
						log.Warn("Game PVE Restart Success!");
					}
				}
				else
				{
					if (log.IsInfoEnabled)
					{
						log.InfoFormat("GameMgr.SynDate: {0}", GameMgr.SynDate);
					}
				}
				if (log.IsInfoEnabled)
				{
					log.Info("GameMgr Scan complete!");
				}
			}
		}

		public static void CreateInstance(GameServerConfig config)
		{
			if (m_instance == null)
			{
				FileInfo configFile = new FileInfo(config.LogConfigFile);
				if (!configFile.Exists)
				{
					ResourceUtil.ExtractResource(configFile.Name, configFile.FullName, Assembly.GetAssembly(typeof(GameServer)));
				}
				XmlConfigurator.ConfigureAndWatch(configFile);
				m_instance = new GameServer(config);
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
				if (e.IsTerminating)
				{
					Stop();
				}
			}
			catch
			{
				try
				{
					using (FileStream stream = new FileStream("c:\\testme.log", FileMode.Append, FileAccess.Write))
					{
						using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
						{
							writer.WriteLine(e.ExceptionObject);
						}
					}
				}
				catch
				{
				}
			}
		}

		public new GameClient[] GetAllClients()
		{
			GameClient[] array = null;
			lock (_clients.SyncRoot)
			{
				array = new GameClient[_clients.Count];
				_clients.Keys.CopyTo(array, 0);
				return array;
			}
		}

		protected override BaseClient GetNewClient()
		{
			return new GameClient(this, AcquirePacketBuffer(), AcquirePacketBuffer());
		}

		protected bool InitComponent(bool componentInitState, string text)
		{
			if (m_debugMenory)
			{
				log.Debug("Start Memory " + text + ": " + GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024);
			}
			if (log.IsInfoEnabled)
			{
				log.Info(text + ": " + componentInitState);
			}
			if (!componentInitState)
			{
				Stop();
			}
			if (m_debugMenory)
			{
				log.Debug("Finish Memory " + text + ": " + GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024);
			}
			return componentInitState;
		}

		public bool InitGlobalTimer()
		{
			int dueTime = Configuration.DBSaveInterval * 60 * 1000;
			if (m_saveDbTimer == null)
			{
				m_saveDbTimer = new Timer(SaveTimerProc, null, dueTime, dueTime);
			}
			else
			{
				m_saveDbTimer.Change(dueTime, dueTime);
			}
			dueTime = Configuration.PingCheckInterval * 60 * 1000;
			if (m_pingCheckTimer == null)
			{
				m_pingCheckTimer = new Timer(PingCheck, null, dueTime, dueTime);
			}
			else
			{
				m_pingCheckTimer.Change(dueTime, dueTime);
			}
			dueTime = Configuration.SaveRecordInterval * 60 * 1000;
			if (m_saveRecordTimer != null)
			{
				m_saveRecordTimer.Change(dueTime, dueTime);
			}
			dueTime = 60000;
			if (m_buffScanTimer == null)
			{
				m_buffScanTimer = new Timer(BuffScanTimerProc, null, dueTime, dueTime);
			}
			else
			{
				m_buffScanTimer.Change(dueTime, dueTime);
			}
			return true;
		}

		private bool InitLoginServer()
		{
			_loginServer = new LoginServerConnector(m_config.LoginServerIp, m_config.LoginServerPort, m_config.ServerID, m_config.ServerName, AcquirePacketBuffer(), AcquirePacketBuffer());
			_loginServer.Disconnected += loginServer_Disconnected;
			return _loginServer.Connect();
		}

		/*private bool InitWorldServer()
        {
			_wordServer = new WorldServerConnector(this.m_config.WorldServerIp, this.m_config.WorldServerPort, m_config.ServerID, m_config.ServerName, AcquirePacketBuffer(), AcquirePacketBuffer());
			_wordServer.Disconnected += worldServer_Disconnected;
			return _wordServer.Connect();

		}*/

		private void loginServer_Disconnected(BaseClient client)
		{
			bool isRunning = m_isRunning;
			Stop();
			if (isRunning && m_tryCount > 0)
			{
				m_tryCount--;
				log.Error("Center Server Disconnect! Stopping Server");
				log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", m_tryCount);
				Thread.Sleep(1000);
				if (Start())
				{
					log.Error("Restart the game server success!");
				}
			}
			else
			{
				if (m_tryCount == 0)
				{
					log.ErrorFormat("Restart the game server failed after {0} times.", 4);
					log.Error("Server Stopped!");
				}
				LogManager.Shutdown();
			}
		}

		/*private void worldServer_Disconnected(BaseClient client)
        {
			bool isRunning = m_isRunning;
			Stop();
			if(isRunning && m_worldTryCount > 0)
            {
				m_worldTryCount--;
				log.Error("World Server Disconnected! Stopping Server");
				log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", m_worldTryCount);
				Thread.Sleep(1000);
				if(Start())
                {
					log.Error("Restart the game server success!");
				}
				else
                {
					if(m_worldTryCount == 0)
                    {
						log.ErrorFormat("Restart the game server failed after {0} times.", 4);
						log.Error("Server Stopped!");
					}
					LogManager.Shutdown();
                }
			}
        }*/

		protected void PingCheck(object sender)
		{
			try
			{
				log.Info("Begin ping check....");
				long num = (long)Configuration.PingCheckInterval * 60L * 1000 * 1000 * 10;
				GameClient[] allClients = GetAllClients();
				if (allClients != null)
				{
					GameClient[] array = allClients;
					foreach (GameClient client in array)
					{
						if (client == null)
						{
							continue;
						}
						if (client.IsConnected)
						{
							if (client.Player != null)
							{
								client.Out.SendPingTime(client.Player);
								if (AntiAddictionMgr.ISASSon && AntiAddictionMgr.count == 0)
								{
									AntiAddictionMgr.count++;
								}
							}
							else if (client.PingTime + num < DateTime.Now.Ticks)
							{
								client.Disconnect();
							}
						}
						else
						{
							client.Disconnect();
						}
					}
				}
				log.Info("End ping check....");
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("PingCheck callback", exception);
				}
			}
			try
			{
				log.Info("Begin ping center check....");
				Instance.LoginServer.SendPingCenter();
				log.Info("End ping center check....");
			}
			catch (Exception exception2)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("PingCheck center callback", exception2);
				}
			}
		}

		//public bool RecompileScripts()
		//{
		//	if (!m_compiled)
		//	{
		//		string path = Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
		//		if (!Directory.Exists(path))
		//		{
		//			Directory.CreateDirectory(path);
		//		}
		//		string[] strArray = Configuration.ScriptAssemblies.Split(',');
		//		m_compiled = ScriptMgr.CompileScripts(compileVB: false, path, Configuration.ScriptCompilationTarget, strArray);
		//	}
		//	return m_compiled;
		//}

		public bool RecompileScripts()
		{
			GameServer.m_compiled = false;
			if (!GameServer.m_compiled)
			{
				string path = this.Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string[] asm_names = this.Configuration.ScriptAssemblies.Split(new char[]
				{
					','
				});
				GameServer.m_compiled = ScriptMgr.CompileScripts(false, path, this.Configuration.ScriptCompilationTarget, asm_names);
			}
			return GameServer.m_compiled;
		}

		public void ReleasePacketBuffer(byte[] buf)
		{
			if (buf != null && GC.GetGeneration(buf) >= GC.MaxGeneration)
			{
				lock (m_packetBufPool.SyncRoot)
				{
					m_packetBufPool.Enqueue(buf);
				}
			}
		}

		protected void SaveRecordProc(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving Record...");
					log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				LogMgr.Save();
				Thread.CurrentThread.Priority = priority;
				tickCount = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving Record complete!");
				}
				if (tickCount > 120000)
				{
					log.WarnFormat("Saved all Record  in {0} ms", tickCount);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SaveRecordProc", exception);
				}
			}
		}

		protected void SaveTimerProc(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving database...");
					log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				int num2 = 0;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					allPlayers[i].SaveNewsItemIntoDatabase();
					num2++;
				}
				WorldMgr.UpdateCaddyRank();
				//WorldMgr.UpdateLeagueRank();
				WorldMgr.ScanShopFreeVaildDate();
				ConsortiaTaskMgr.ScanConsortiaTask();
				AcademyMgr.RemoveOldRequest();
				// save data in World
				GmActivityMgr.SaveData();
				Thread.CurrentThread.Priority = priority;
				tickCount = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving New Item to database complete!");
					log.Info("Saved all databases and " + num2 + " players in " + tickCount + "ms");
				}
				if (tickCount > 120000)
				{
					log.WarnFormat("Saved all databases and {0} players in {1} ms", num2, tickCount);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SaveTimerProc", exception);
				}
			}
			finally
			{
				GameEventMgr.Notify(GameServerEvent.WorldSave);
			}
		}

		public void Shutdown()
		{
			Instance.LoginServer.SendShutdown(isStoping: true);
			_shutdownTimer = new Timer(ShutDownCallBack, null, 0, 60000);
		}

		private void ShutDownCallBack(object state)
		{
			try
			{
				_shutdownCount--;
				Console.WriteLine($"Server will shutdown after {_shutdownCount} mins!");
				GameClient[] allClients = Instance.GetAllClients();
				foreach (GameClient client in allClients)
				{
					if (client.Out != null)
					{
						client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
					}
				}
				if (_shutdownCount == 0)
				{
					Console.WriteLine("Server has stopped!");
					Instance.LoginServer.SendShutdown(isStoping: false);
					_shutdownTimer.Dispose();
					_shutdownTimer = null;
					Instance.Stop();
				}
			}
			catch (Exception exception)
			{
				log.Error(exception);
			}
		}

		public override bool Start()
		{
			if (m_isRunning)
			{
				return false;
			}
			try
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				Thread.CurrentThread.Priority = ThreadPriority.Normal;
				GameProperties.Refresh();
				if (!InitComponent(RecompileScripts(), "Recompile Scripts"))
				{
					bool flag = false;
				}
				if (!InitComponent(ConsortiaLevelMgr.Init(), "ConsortiaLevelMgr Init"))
				{
					return false;
				}
				if (!InitComponent(StartScriptComponents(), "Script components"))
				{
					return false;
				}
				if (!InitComponent(GameProperties.EDITION == Edition, "Edition: " + Edition))
				{
					return false;
				}
				if (!InitComponent(InitSocket(IPAddress.Parse(Configuration.Ip), Configuration.Port), "InitSocket Port: " + Configuration.Port))
				{
					return false;
				}
				if (!InitComponent(AllocatePacketBuffers(), "AllocatePacketBuffers()"))
				{
					return false;
				}
				if (!InitComponent(LogMgr.Setup(Configuration.GAME_TYPE, Configuration.ServerID, Configuration.AreaID), "LogMgr Init"))
				{
					return false;
				}
				if (!InitComponent(WorldMgr.Init(), "WorldMgr Init"))
				{
					return false;
				}
				if (!InitComponent(MapMgr.Init(), "MapMgr Init"))
				{
					return false;
				}
				if (!InitComponent(ItemMgr.Init(), "ItemMgr Init"))
				{
					return false;
				}
				if (!InitComponent(ItemBoxMgr.Init(), "ItemBox Init"))
				{
					return false;
				}
				if (!InitComponent(BallMgr.Init(), "BallMgr Init"))
				{
					return false;
				}
				if (!InitComponent(ExerciseMgr.Init(), "ExerciseMgr Init"))
				{
					return false;
				}
				if (!InitComponent(LevelMgr.Init(), "levelMgr Init"))
				{
					return false;
				}
				if (!InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
				{
					return false;
				}
				if (!InitComponent(FusionMgr.Init(), "FusionMgr Init"))
				{
					return false;
				}
				if (!InitComponent(UserBoxMgr.Init(), "UserBoxMgr Init"))
				{
					return false;
				}
				if (!InitComponent(AwardMgr.Init(), "AwardMgr Init"))
				{
					return false;
				}
				if (!InitComponent(AchievementMgr.Init(), "AchievementMgr Init"))
				{
					return false;
				}
				if (!InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
				{
					return false;
				}
				if (!InitComponent(MissionInfoMgr.Init(), "MissionInfoMgr Init"))
				{
					return false;
				}
				if (!InitComponent(PveInfoMgr.Init(), "PveInfoMgr Init"))
				{
					return false;
				}
				if (!InitComponent(DropMgr.Init(), "Drop Init"))
				{
					return false;
				}
				if (!InitComponent(FightRateMgr.Init(), "FightRateMgr Init"))
				{
					return false;
				}
				if (!InitComponent(RefineryMgr.Init(), "RefineryMgr Init"))
				{
					return false;
				}
				if (!InitComponent(StrengthenMgr.Init(), "StrengthenMgr Init"))
				{
					return false;
				}
				if (!InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
				{
					return false;
				}
				if (!InitComponent(ShopMgr.Init(), "ShopMgr Init"))
				{
					return false;
				}
				if (!InitComponent(QuestMgr.Init(), "QuestMgr Init"))
				{
					return false;
				}

				if (!InitComponent(GmActivityMgr.Init(), "GmActivityMgr Init"))
					return false;

				if (!InitComponent(RoomMgr.Setup(Configuration.MaxRoomCount), "RoomMgr.Setup"))
				{
					return false;
				}
				if (!InitComponent(GameMgr.Setup(Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION), "GameMgr.Start()"))
				{
					return false;
				}
				if (!InitComponent(ConsortiaMgr.Init(), "ConsortiaMgr Init"))
				{
					return false;
				}
				if (!InitComponent(ConsortiaExtraMgr.Init(), "ConsortiaExtraMgr Init"))
				{
					return false;
				}
				if (!InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
				{
					return false;
				}
				if (!InitComponent(RateMgr.Init(Configuration), "ExperienceRateMgr Init"))
				{
					return false;
				}
				if (!InitComponent(WindMgr.Init(), "WindMgr Init"))
				{
					return false;
				}
				if (!InitComponent(CardMgr.Init(), "CardMgr Init"))
				{
					return false;
				}

				if (!InitComponent(CardBuffMgr.Init(), "CardBuffMgr Init"))
					return false;

				if (!InitComponent(FairBattleRewardMgr.Init(), "FairBattleRewardMgr Init"))
				{
					return false;
				}
				if (!InitComponent(ConsortiaTaskMgr.Init(), "ConsortiaTaskMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(PetMgr.Init(), "PetMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(MacroDropMgr.Init(), "MacroDropMgr Init"))
				{
					return false;
				}
				if (!InitComponent(MarryRoomMgr.Init(), "MarryRoomMgr Init"))
				{
					return false;
				}
				if (!InitComponent(RankMgr.Init(), "RankMgr Init"))
				{
					return false;
				}
				if (!InitComponent(CommunalActiveMgr.Init(), "CommunalActiveMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(QQTipsMgr.Init(), "QQTipsMgr Init"))
				{
					return false;
				}
				if (!InitComponent(SubActiveMgr.Init(), "SubActiveMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(EventAwardMgr.Init(), "EventAwardMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(EventLiveMgr.Init(), "EventLiveMgr Setup"))
				{
					return false;
				}

				InitComponent(CommandsMgr.Init(), "CommandsMgr Setup");
				// if (!)
				// {
				// 	return false;
				// }
				if (!InitComponent(AcademyMgr.Init(), "AcademyMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(BattleMgr.Setup(), "BattleMgr Setup"))
				{
					return false;
				}
				if (!InitComponent(InitGlobalTimer(), "Init Global Timers"))
				{
					return false;
				}
                if (!this.InitComponent(JampsManualMgr.Init(), "初始化手册系统 by久伴"))
                {
                    return false;
                }
                if (!InitComponent(LogMgr.Setup(1, Configuration.ServerID, Configuration.AreaID), "LogMgr Setup"))
				{
					return false;
				}
				GameEventMgr.Notify(ScriptEvent.Loaded);
				if (!InitComponent(InitLoginServer(), "Login To CenterServer"))
				{
					return false;
				}
				/*if(!InitComponent(InitWorldServer(), "Login To WorldServer"))
                {
					return false;
                }*/
				if (!InitComponent(HotSpringMgr.Init(), "HotSpringMgr Init"))
				{
					return false;
				}
				if (!InitComponent(PetMoePropertyMgr.Init(), "PetMoePropertyMgr Init"))
				{
					return false;
				}
				if (!InitComponent(RingStationMgr.Init(), "AutoBot Init"))
				{
					return false;
				}
				if (!InitComponent(RobotManager.Start(), "RobotManager Start"))
				{
					return false;
				}
				if (!InitComponent(LittleGameWorldMgr.Init(), "LittleGameWorldMgr Init"))
				{
					return false;
				}
				if (!InitComponent(TaskMgr.Init(), "TaskMgr Init"))
				{
					return false;
				}
				if (!InitComponent(AccumulActiveLoginMgr.Init(), "AccumulActiveLoginMgr Init"))
                {
					return false;
                }
				if (!InitComponent(NewTitleMgr.Init(), "NewTitleMgr Init"))
                {
					return false;
                }
				if(!InitComponent(QiYuanAwardMgr.Init(), "QiYuanAwardMgr Init"))
                {
					return false;
                }
				if(!InitComponent(ActiveSystemMgr.Init(), "ActiveSystemMgr Init"))
                {
					return false;
                }
				if (!InitComponent(EliteGameMgr.Init(), "EliteGame Init"))
					return false;
				if (!InitComponent(TotemHonorMgr.Init(), "TotemHonorMgr Init"))
					return false;
				if (!InitComponent(TotemMgr.Init(), "TotemMgr Init"))
					return false;
				if (!InitComponent(GodCardMgr.Init(), "GodCardRaise Init"))
					return false;
				if (!InitComponent(FightSpiritTemplateMgr.Init(), "FightSpiritTemplateMgr Init"))
					return false;
				if (!InitComponent(LoveLevelMgr.Init(), "LoveLevelMgr Init"))
					return false;
				if (!InitComponent(SetsBuildTempMgr.Init(), "SetsBuildTempMgr Init"))
					return false;
				if (!InitComponent(SpiritInfoMgr.Init(), "SpiritInfoMgr Init"))
					return false;
				if (!InitComponent(AvatarColectionMgr.Init(), "AvatarColectionMgr Init"))
					return false;
				if (!InitComponent(ActiveMgr.Init(), "ActiveMgr Init"))
					return false;
				if (!InitComponent(JampsManualMgr.Init(), "JampsManualMgr Init"))
					return false;
				RoomMgr.Start();
				GameMgr.Start();
				BattleMgr.Start();
				MacroDropMgr.Start();
				GlobalConstants.Start();
				if (!InitComponent(base.Start(), "base.Start()"))
				{
					return false;
				}
				GameEventMgr.Notify(GameServerEvent.Started, this);
				GC.Collect(GC.MaxGeneration);
				if (log.IsInfoEnabled)
				{
					log.Info("GameServer is now open for connections!");
				}
				m_isRunning = true;
				return true;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Failed to start the server", exception);
				}
				return false;
			}
		}

		protected bool StartScriptComponents()
		{
			try
			{
				if (log.IsInfoEnabled)
				{
					log.Info("Server rules: true");
				}
				ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
				foreach (Assembly item in new ArrayList(ScriptMgr.Scripts))
				{
					GameEventMgr.RegisterGlobalEvents(item, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(item, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(item, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(item, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				if (log.IsInfoEnabled)
				{
					log.Info("Registering global event handlers: true");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("StartScriptComponents", exception);
				}
				return false;
			}
			return true;
		}

		public override void Stop()
		{
			if (m_isRunning)
			{
				m_isRunning = false;
				if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
				{
					Console.WriteLine("Update BreakTime failed");
				}
				EliteGameMgr.Save();
				RoomMgr.Stop();
				GameMgr.Stop();
				WorldMgr.Stop();
				GmActivityMgr.Stop();
				ConsortiaTaskMgr.Stop();
				GlobalConstants.StopGlobalTimer();
				if (_loginServer != null)
				{
					_loginServer.Disconnected -= loginServer_Disconnected;
					_loginServer.Disconnect();
				}
				/*if(_wordServer != null)
                {
					_wordServer.Disconnected -= worldServer_Disconnected;
					_wordServer.Disconnect();
				}*/
				if (m_pingCheckTimer != null)
				{
					m_pingCheckTimer.Change(-1, -1);
					m_pingCheckTimer.Dispose();
					m_pingCheckTimer = null;
				}
				if (m_saveDbTimer != null)
				{
					m_saveDbTimer.Change(-1, -1);
					m_saveDbTimer.Dispose();
					m_saveDbTimer = null;
				}
				if (m_saveRecordTimer != null)
				{
					m_saveRecordTimer.Change(-1, -1);
					m_saveRecordTimer.Dispose();
					m_saveRecordTimer = null;
				}
				if (m_buffScanTimer != null)
				{
					m_buffScanTimer.Change(-1, -1);
					m_buffScanTimer.Dispose();
					m_buffScanTimer = null;
				}
				if (m_qqTipScanTimer != null)
				{
					m_qqTipScanTimer.Change(-1, -1);
					m_qqTipScanTimer.Dispose();
					m_qqTipScanTimer = null;
				}
				if (m_bagMailScanTimer != null)
				{
					m_bagMailScanTimer.Change(-1, -1);
					m_bagMailScanTimer.Dispose();
					m_bagMailScanTimer = null;
				}
				base.Stop();
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				log.Info("Server Stopped!");
				Console.WriteLine("Server Stopped!");
			}
		}
	}
}
