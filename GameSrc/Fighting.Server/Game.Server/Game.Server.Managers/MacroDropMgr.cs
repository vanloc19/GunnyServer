using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Timers;
using Game.Base.Packets;
using Game.Logic;
using log4net;

namespace Game.Server.Managers
{
	public class MacroDropMgr
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected static ReaderWriterLock m_lock = new ReaderWriterLock();

		public static bool Init()
		{
			m_lock = new ReaderWriterLock();
			return ReLoad();
		}

		private static void OnTimeEvent(object source, ElapsedEventArgs e)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, MacroDropInfo> pair in DropInfoMgr.DropInfo)
				{
					int key = pair.Key;
					MacroDropInfo info = pair.Value;
					if (info.SelfDropCount > 0)
					{
						dictionary.Add(key, info.SelfDropCount);
						info.SelfDropCount = 0;
					}
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("DropInfoMgr OnTimeEvent", exception);
				}
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
			if (dictionary.Count <= 0)
			{
				return;
			}
			GSPacketIn packet = new GSPacketIn(178);
			packet.WriteInt(dictionary.Count);
			foreach (KeyValuePair<int, int> pair2 in dictionary)
			{
				packet.WriteInt(pair2.Key);
				packet.WriteInt(pair2.Value);
			}
			GameServer.Instance.LoginServer.SendPacket(packet);
		}

		public static bool ReLoad()
		{
			try
			{
				DropInfoMgr.DropInfo = new Dictionary<int, MacroDropInfo>();
				return true;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("DropInfoMgr", exception);
				}
			}
			return false;
		}

		public static void Start()
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += OnTimeEvent;
			timer.Interval = 5000.0;
			timer.Enabled = true;
		}

		public static void UpdateDropInfo(Dictionary<int, MacroDropInfo> temp)
		{
			m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, MacroDropInfo> pair in temp)
				{
					if (DropInfoMgr.DropInfo.ContainsKey(pair.Key))
					{
						DropInfoMgr.DropInfo[pair.Key].DropCount = pair.Value.DropCount;
						DropInfoMgr.DropInfo[pair.Key].MaxDropCount = pair.Value.MaxDropCount;
					}
					else
					{
						DropInfoMgr.DropInfo.Add(pair.Key, pair.Value);
					}
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("MacroDropMgr UpdateDropInfo", exception);
				}
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
		}
	}
}
