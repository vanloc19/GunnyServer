using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
	public class ActiveMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static Dictionary<int, ActiveAwardInfo> m_ActiveAwardInfo = new Dictionary<int, ActiveAwardInfo>();

		public static Dictionary<int, List<ActiveConditionInfo>> m_ActiveConditionInfo = new Dictionary<int, List<ActiveConditionInfo>>();

		private static Dictionary<int, List<ActivitySystemItemInfo>> m_ActivitySystemItems = new Dictionary<int, List<ActivitySystemItemInfo>>();

		public static List<ActiveAwardInfo> GetAwardInfo(DateTime lastDate, int playerGrade)
		{
			string awardId = null;
			int days = (DateTime.Now - lastDate).Days;
			if (DateTime.Now.DayOfYear > lastDate.DayOfYear)
			{
				days++;
			}
			List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
			foreach (List<ActiveConditionInfo> value in m_ActiveConditionInfo.Values)
			{
				foreach (ActiveConditionInfo info in value)
				{
					if (IsValid(info) && IsInGrade(info.LimitGrade, playerGrade) && info.Condition <= days)
					{
						awardId = info.AwardId;
						_ = info.ActiveID;
					}
				}
			}
			if (!string.IsNullOrEmpty(awardId))
			{
				string[] array = awardId.Split(',');
				foreach (string str2 in array)
				{
					if (!string.IsNullOrEmpty(str2) && m_ActiveAwardInfo.ContainsKey(Convert.ToInt32(str2)))
					{
						list.Add(m_ActiveAwardInfo[Convert.ToInt32(str2)]);
					}
				}
			}
			return list;
		}

		public static bool Init()
		{
			return ReLoad();
		}

		private static bool IsInGrade(string limitGrade, int playerGrade)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			if (limitGrade != null)
			{
				string[] strArray = limitGrade.Split('-');
				if (strArray.Length == 2)
				{
					num = Convert.ToInt32(strArray[0]);
					num2 = Convert.ToInt32(strArray[1]);
				}
				if (num <= playerGrade && num2 >= playerGrade)
				{
					flag = true;
				}
			}
			return flag;
		}

		public static bool IsValid(ActiveConditionInfo info)
		{
			_ = info.StartTime;
			_ = info.EndTime;
			if (info.StartTime.Ticks <= DateTime.Now.Ticks)
			{
				return info.EndTime.Ticks >= DateTime.Now.Ticks;
			}
			return false;
		}

		public static Dictionary<int, ActiveAwardInfo> LoadActiveAwardDb(Dictionary<int, List<ActiveConditionInfo>> conditions)
		{
			Dictionary<int, ActiveAwardInfo> dictionary = new Dictionary<int, ActiveAwardInfo>();
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				ActiveAwardInfo[] allActiveAwardInfo = bussiness.GetAllActiveAwardInfo();
				foreach (int num in conditions.Keys)
				{
					ActiveAwardInfo[] array = allActiveAwardInfo;
					foreach (ActiveAwardInfo info in array)
					{
						if (num == info.ActiveID && !dictionary.ContainsKey(info.ID))
						{
							dictionary.Add(info.ID, info);
						}
					}
				}
				return dictionary;
			}
		}

		public static Dictionary<int, List<ActiveConditionInfo>> LoadActiveConditionDb()
		{
			Dictionary<int, List<ActiveConditionInfo>> dictionary = new Dictionary<int, List<ActiveConditionInfo>>();
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				ActiveConditionInfo[] allActiveConditionInfo = bussiness.GetAllActiveConditionInfo();
				foreach (ActiveConditionInfo info in allActiveConditionInfo)
				{
					List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
					if (!dictionary.ContainsKey(info.ActiveID))
					{
						list.Add(info);
						dictionary.Add(info.ActiveID, list);
					}
					else
					{
						dictionary[info.ActiveID].Add(info);
					}
				}
				return dictionary;
			}
		}

		public static bool ReLoad()
		{
			try
			{
				/*Dictionary<int, List<ActiveConditionInfo>> conditions = LoadActiveConditionDb();
				Dictionary<int, ActiveAwardInfo> dictionary2 = LoadActiveAwardDb(conditions);
				if (conditions.Count > 0)
				{
					Interlocked.Exchange(ref m_ActiveConditionInfo, conditions);
					Interlocked.Exchange(ref m_ActiveAwardInfo, dictionary2);
				}*/
				ActivitySystemItemInfo[] tempActivitySystemItem = LoadActivitySystemItemDb();
				Dictionary<int, List<ActivitySystemItemInfo>> tempActivitySystemItems = LoadActivitySystemItems(tempActivitySystemItem);
				if (tempActivitySystemItem.Length > 0)
				{
					Interlocked.Exchange(ref m_ActivitySystemItems, tempActivitySystemItems);
				}
				return true;
			}
			catch (Exception exception)
			{
				log.Error("QuestMgr", exception);
			}
			return false;
		}

		public static ActivitySystemItemInfo[] LoadActivitySystemItemDb()
		{
			using (ProduceBussiness pb = new ProduceBussiness())
			{
				ActivitySystemItemInfo[] infos = pb.GetAllActivitySystemItem();
				return infos;
			}
		}

		public static Dictionary<int, List<ActivitySystemItemInfo>> LoadActivitySystemItems(
			ActivitySystemItemInfo[] ActivitySystemItem)
		{
			Dictionary<int, List<ActivitySystemItemInfo>> infos = new Dictionary<int, List<ActivitySystemItemInfo>>();
			foreach (ActivitySystemItemInfo info in ActivitySystemItem)
			{
				if (!infos.Keys.Contains(info.ActivityType))
				{
					IEnumerable<ActivitySystemItemInfo> temp =
						ActivitySystemItem.Where(s => s.ActivityType == info.ActivityType);
					infos.Add(info.ActivityType, temp.ToList());
				}
			}

			return infos;
		}

		public static List<ActivitySystemItemInfo> FindActivitySystemItem(int ActivityType)
		{
			if (m_ActivitySystemItems.ContainsKey(ActivityType))
			{
				List<ActivitySystemItemInfo> items = new List<ActivitySystemItemInfo>();
				foreach (ActivitySystemItemInfo sysItem in m_ActivitySystemItems[ActivityType])
					items.Add(sysItem);

				return items;
			}

			return null;
		}

		public static List<ActivitySystemItemInfo> FindChickActivePakage(int quality)
		{
			List<ActivitySystemItemInfo> lists = new List<ActivitySystemItemInfo>();
			List<ActivitySystemItemInfo> infos = FindActivitySystemItem(40);
			foreach (ActivitySystemItemInfo info in infos)
			{
				if (info.Quality == quality)
				{
					lists.Add(info);
				}
			}

			return lists;
		}

		public static List<ActivitySystemItemInfo> GetActivitySystemItemByLayer(int layer)
		{
			List<ActivitySystemItemInfo> lists = new List<ActivitySystemItemInfo>();
			List<ActivitySystemItemInfo> infos = FindActivitySystemItem(8);
			foreach (ActivitySystemItemInfo info in infos)
			{
				if (info.Quality == layer)
				{
					lists.Add(info);
				}
			}

			return lists;
		}
	}
}