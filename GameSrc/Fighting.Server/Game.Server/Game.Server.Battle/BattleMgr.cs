using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Game.Server.Rooms;
using log4net;

namespace Game.Server.Battle
{
	public class BattleMgr
	{
		public static readonly ILog log;

		public static List<BattleServer> m_list;

		public static bool AutoReconnect;

		public static bool Setup()
		{
			if (File.Exists("battle.xml"))
			{
				try
				{
					foreach (XElement node in XDocument.Load("battle.xml").Root.Nodes())
					{
						try
						{
							int serverId = int.Parse(node.Attribute("id").Value);
							string str1 = node.Attribute("ip").Value;
							int num = int.Parse(node.Attribute("port").Value);
							string value = node.Attribute("key").Value;
							string ip = str1;
							int port = num;
							string loginKey = value;
							AddBattleServer(new BattleServer(serverId, ip, port, loginKey));
							log.InfoFormat("Battle server {0}:{1} loaded...", str1, num);
						}
						catch (Exception ex2)
						{
							log.Error("BattleMgr setup error:", ex2);
						}
					}
				}
				catch (Exception ex)
				{
					log.Error("BattleMgr setup error:", ex);
				}
			}
			log.InfoFormat("Total {0} battle server loaded.", m_list.Count);
			return true;
		}

		public static BattleServer GetServer(int id)
		{
			foreach (BattleServer battleServer in m_list)
			{
				if (battleServer.ServerId == id)
				{
					return battleServer;
				}
			}
			return null;
		}

		public static void AddBattleServer(BattleServer battle)
		{
			if (battle != null)
			{
				m_list.Add(battle);
				battle.Disconnected += smethod_0;
			}
		}

		private static void smethod_0(object object_0, object object_1)
		{
			BattleServer server = object_0 as BattleServer;
			log.WarnFormat("Disconnect from battle server {0}:{1}", server.Ip, server.Port);
			if (server == null || !AutoReconnect || !m_list.Contains(server))
			{
				return;
			}
			RemoveServer(server);
			if ((DateTime.Now - server.LastRetryTime).TotalMinutes > 3.0)
			{
				server.RetryCount = 0;
			}
			if (server.RetryCount < 3)
			{
				BattleServer battle = server.Clone();
				AddBattleServer(battle);
				battle.RetryCount++;
				battle.LastRetryTime = DateTime.Now;
				try
				{
					battle.Start();
				}
				catch (Exception ex)
				{
					log.ErrorFormat("Batter server {0}:{1} can't connected!", server.Ip, server.Port);
					log.Error(ex.Message);
					server.RetryCount = 0;
				}
			}
		}

		public static void ConnectTo(int id, string ip, int port, string key)
		{
			BattleServer battle = new BattleServer(id, ip, port, key);
			AddBattleServer(battle);
			try
			{
				battle.Start();
			}
			catch (Exception ex)
			{
				log.ErrorFormat("Batter server {0}:{1} can't connected!", battle.Ip, battle.Port);
				log.Error(ex.Message);
			}
		}

		public static void Disconnet(int id)
		{
			BattleServer server = GetServer(id);
			if (server != null && server.IsActive)
			{
				server.LastRetryTime = DateTime.Now;
				server.RetryCount = 4;
				server.Server.Disconnect();
			}
		}

		public static void RemoveServer(BattleServer server)
		{
			if (server != null)
			{
				m_list.Remove(server);
				server.Disconnected += smethod_0;
			}
		}

		public static void Start()
		{
			foreach (BattleServer battleServer in m_list)
			{
				try
				{
					battleServer.Start();
				}
				catch (Exception ex)
				{
					log.ErrorFormat("Batter server {0}:{1} can't connected!", battleServer.Ip, battleServer.Port);
					log.Error(ex.Message);
				}
			}
		}

		public static BattleServer FindActiveServer()
		{
			lock (m_list)
			{
				foreach (BattleServer battleServer in m_list)
				{
					if (battleServer.IsActive)
					{
						return battleServer;
					}
				}
			}
			return null;
		}

		public static BattleServer FindActiveServer(bool isCrosszone)
		{
			lock (m_list)
			{
				foreach (BattleServer battleServer in m_list)
				{
					if ((isCrosszone && battleServer.ServerId == 2 && battleServer.IsActive) || (!isCrosszone && battleServer.IsActive))
					{
						return battleServer;
					}
				}
			}
			return null;
		}

		public static BattleServer AddRoom(BaseRoom room)
		{
			//BattleServer activeServer = FindActiveServer();
			BattleServer activeServer = BattleMgr.FindActiveServer(room.isCrosszone);
			if (activeServer != null && activeServer.AddRoom(room))
			{
				return activeServer;
			}
			return null;
		}

		public static List<BattleServer> GetAllBattles()
		{
			return m_list;
		}

		static BattleMgr()
		{
			log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			m_list = new List<BattleServer>();
			AutoReconnect = true;
		}
	}
}
