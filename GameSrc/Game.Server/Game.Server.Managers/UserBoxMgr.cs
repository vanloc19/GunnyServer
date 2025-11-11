using System;
using System.Collections.Generic;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class UserBoxMgr
	{
		private static Dictionary<int, LoadUserBoxInfo> m_BoxInfo;

		private static ReaderWriterLock m_lock;

		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, LoadUserBoxInfo> tempTimeBoxInfo = new Dictionary<int, LoadUserBoxInfo>();
				if (LoadStrengthen(tempTimeBoxInfo))
				{
					m_lock.AcquireWriterLock(-1);
					try
					{
						m_BoxInfo = tempTimeBoxInfo;
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
			catch (Exception e)
			{
				Console.WriteLine("UserBoxMgr", e);
			}
			return false;
		}

		public static bool Init()
		{
			try
			{
				m_lock = new ReaderWriterLock();
				m_BoxInfo = new Dictionary<int, LoadUserBoxInfo>();
				return LoadStrengthen(m_BoxInfo);
			}
			catch (Exception e)
			{
				Console.WriteLine("UserBoxMgr", e);
				return false;
			}
		}

		private static bool LoadStrengthen(Dictionary<int, LoadUserBoxInfo> m_TimeBoxInfo)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				LoadUserBoxInfo[] allTimeBoxAward = db.GetAllTimeBoxAward();
				foreach (LoadUserBoxInfo info in allTimeBoxAward)
				{
					if (!m_TimeBoxInfo.ContainsKey(info.ID))
					{
						m_TimeBoxInfo.Add(info.ID, info);
					}
				}
			}
			return true;
		}

		public static LoadUserBoxInfo FindTemplateByCondition(int type, int level, int condition)
		{
			foreach (KeyValuePair<int, LoadUserBoxInfo> info in m_BoxInfo)
			{
				if (type == 0)
				{
					if (type == info.Value.Type && level <= info.Value.Level && condition < info.Value.Condition)
					{
						return info.Value;
					}
				}
				else if (type == info.Value.Type && level < info.Value.Level && condition == info.Value.Condition)
				{
					return info.Value;
				}
			}
			return null;
		}
	}
}
