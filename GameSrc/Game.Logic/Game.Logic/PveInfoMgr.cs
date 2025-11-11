using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic
{
	public static class PveInfoMgr
	{
		private static readonly ILog ilog_0;

		private static Dictionary<int, PveInfo> dictionary_0;

		private static ReaderWriterLock readerWriterLock_0;

		private static ThreadSafeRandom threadSafeRandom_0;

		public static bool Init()
		{
			return ReLoad();
		}

		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, PveInfo> dictionary = LoadFromDatabase();
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange(ref dictionary_0, dictionary);
				}
				return true;
			}
			catch (Exception ex)
			{
				ilog_0.Error("PveInfoMgr", ex);
			}
			return false;
		}

		public static Dictionary<int, PveInfo> LoadFromDatabase()
		{
			Dictionary<int, PveInfo> dictionary = new Dictionary<int, PveInfo>();
			using (PveBussiness pveBussiness = new PveBussiness())
			{
				PveInfo[] allPveInfos = pveBussiness.GetAllPveInfos();
				foreach (PveInfo allPveInfo in allPveInfos)
				{
					if (!dictionary.ContainsKey(allPveInfo.ID))
					{
						dictionary.Add(allPveInfo.ID, allPveInfo);
					}
				}
				return dictionary;
			}
		}

		public static PveInfo GetPveInfoById(int id)
		{
			if (dictionary_0.ContainsKey(id))
			{
				return dictionary_0[id];
			}
			return null;
		}

		public static PveInfo[] GetPveInfo()
		{
			if (dictionary_0 == null)
			{
				ReLoad();
			}
			return dictionary_0.Values.ToArray();
		}

		public static PveInfo GetPveInfoByType(eRoomType roomType, int levelLimits)
		{
			if (roomType == eRoomType.Boss || roomType == eRoomType.Dungeon || roomType == eRoomType.Academy)
			{
				foreach (PveInfo pveInfo in dictionary_0.Values)
				{
					if (pveInfo.Type == (int)roomType)
					{
						return pveInfo;
					}
				}
			}
			else if (roomType == eRoomType.Exploration)
			{
				foreach (PveInfo pveInfo in dictionary_0.Values)
				{
					if ((pveInfo.Type == (int)roomType) && (pveInfo.LevelLimits == levelLimits))
					{
						return pveInfo;
					}
				}
			}
			return null;
		}

		public static PveInfo GetRandomPve()
		{
			var pves = dictionary_0.Values.Where(x => x.Type == (int)eRoomType.Dungeon).ToList();
			return pves[threadSafeRandom_0.Next(0, pves.Count())];
		}

		static PveInfoMgr()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			dictionary_0 = new Dictionary<int, PveInfo>();
			readerWriterLock_0 = new ReaderWriterLock();
			threadSafeRandom_0 = new ThreadSafeRandom();
		}
	}
}
