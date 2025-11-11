using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading;
using log4net;

namespace Tank.Request
{
	public class PlayerManager
	{
		private class PlayerData
		{
			public string Name;

			public string Pass;

			public DateTime Date;

			public int Count;
		}

		private static Dictionary<string, PlayerData> m_players = new Dictionary<string, PlayerData>();

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static object sys_obj = new object();

		private static int m_timeout = 30;

		private static Timer m_timer;

		public static void Setup()
		{
			m_timeout = int.Parse(ConfigurationManager.AppSettings["LoginSessionTimeOut"]);
			m_timer = new Timer(CheckTimerCallback, null, 0, 60000);
		}

		protected static bool CheckTimeOut(DateTime dt)
		{
			return (DateTime.Now - dt).TotalMinutes > (double)m_timeout;
		}

		private static void CheckTimerCallback(object state)
		{
			lock (sys_obj)
			{
				List<string> stringList = new List<string>();
				foreach (PlayerData playerData in m_players.Values)
				{
					if (CheckTimeOut(playerData.Date))
					{
						stringList.Add(playerData.Name);
					}
				}
				foreach (string key in stringList)
				{
					m_players.Remove(key);
				}
			}
		}

		public static void Add(string name, string pass)
		{
			lock (sys_obj)
			{
				if (m_players.ContainsKey(name))
				{
					m_players[name].Name = name;
					m_players[name].Pass = pass;
					m_players[name].Date = DateTime.Now;
					m_players[name].Count = 0;
				}
				else
				{
					m_players.Add(name, new PlayerData
					{
						Name = name,
						Pass = pass,
						Date = DateTime.Now
					});
				}
			}
		}

		public static bool Login(string name, string pass)
		{
			lock (sys_obj)
			{
				if (m_players.ContainsKey(name))
				{
					log.Error(name + "|" + m_players[name].Pass);
				}
				else
				{
					log.Error("NOHAVE " + name);
				}
				if (!m_players.ContainsKey(name) || !(m_players[name].Pass == pass))
				{
					return false;
				}
				PlayerData player = m_players[name];
				if (player.Pass == pass && !CheckTimeOut(player.Date))
				{
					return true;
				}
				log.Error(name + "|timeout:" + m_players[name].Date);
				return false;
			}
		}

		public static bool Update(string name, string pass)
		{
			lock (sys_obj)
			{
				if (m_players.ContainsKey(name))
				{
					m_players[name].Pass = pass;
					m_players[name].Count++;
					return true;
				}
			}
			return false;
		}

		public static bool Remove(string name)
		{
			lock (sys_obj)
			{
				return m_players.Remove(name);
			}
		}

		public static bool GetByUserIsFirst(string name)
		{
			lock (sys_obj)
			{
				if (m_players.ContainsKey(name))
				{
					return m_players[name].Count == 0;
				}
			}
			return false;
		}
	}
}
