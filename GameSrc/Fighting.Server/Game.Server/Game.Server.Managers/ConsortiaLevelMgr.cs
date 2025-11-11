using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class ConsortiaLevelMgr
	{
		private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static ReaderWriterLock m_lock;

		private static ThreadSafeRandom rand;

		public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
		{
			m_lock.AcquireReaderLock(-1);
			try
			{
				if (_consortiaLevel.ContainsKey(level))
				{
					return _consortiaLevel[level];
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

		public static bool Init()
		{
			try
			{
				m_lock = new ReaderWriterLock();
				_consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
				rand = new ThreadSafeRandom();
				return Load(_consortiaLevel);
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ConsortiaLevelMgr", exception);
				}
				return false;
			}
		}

		private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel)
		{
			using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
			{
				ConsortiaLevelInfo[] allConsortiaLevel = bussiness.GetAllConsortiaLevel();
				foreach (ConsortiaLevelInfo info in allConsortiaLevel)
				{
					if (!consortiaLevel.ContainsKey(info.Level))
					{
						consortiaLevel.Add(info.Level, info);
					}
				}
			}
			return true;
		}

		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, ConsortiaLevelInfo> consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
				if (Load(consortiaLevel))
				{
					m_lock.AcquireWriterLock(-1);
					try
					{
						_consortiaLevel = consortiaLevel;
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
					log.Error("ConsortiaLevelMgr", exception);
				}
			}
			return false;
		}
	}
}
