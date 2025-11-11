using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
	public class EventAwardMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static EventAwardInfo[] m_eventAward;

		private static Dictionary<int, List<EventAwardInfo>> m_EventAwards;

		private static ThreadSafeRandom rand = new ThreadSafeRandom();

		public static void CreateEventAward(eEventType DateId)
		{
		}

		public static EventAwardInfo CreateSearchGoodsAward(eEventType DataId)
		{
			List<EventAwardInfo> list = new List<EventAwardInfo>();
			List<EventAwardInfo> list2 = FindEventAward(DataId);
			int count = 1;
			int maxRound = ThreadSafeRandom.NextStatic(list2.Select((EventAwardInfo s) => s.Random).Max());
			List<EventAwardInfo> source = list2.Where((EventAwardInfo s) => s.Random >= maxRound).ToList();
			int num2 = source.Count();
			if (num2 > 0)
			{
				count = ((count > num2) ? num2 : count);
				int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, count);
				foreach (int num3 in randomUnrepeatArray)
				{
					EventAwardInfo item = source[num3];
					list.Add(item);
				}
			}
			foreach (EventAwardInfo info2 in list)
			{
				if (ItemMgr.FindItemTemplate(info2.TemplateID) != null)
				{
					return info2;
				}
			}
			return null;
		}

		public static List<EventAwardInfo> FindEventAward(eEventType DataId)
		{
			if (m_EventAwards.ContainsKey((int)DataId))
			{
				return m_EventAwards[(int)DataId];
			}
			return null;
		}

		public static List<NewChickenBoxItemInfo> GetNewChickenBoxAward(eEventType DataId)
		{
			List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
			List<EventAwardInfo> list2 = new List<EventAwardInfo>();
			List<EventAwardInfo> source = EventAwardMgr.FindEventAward(DataId);
			int num = 1;
			int maxRound = ThreadSafeRandom.NextStatic((from s in source select s.Random).Max());
			List<EventAwardInfo> list3 = (from s in source where s.Random >= maxRound select s).ToList<EventAwardInfo>();
			int num2 = list3.Count<EventAwardInfo>();
			if (num2 > 0)
			{
				num = ((num > num2) ? num2 : num);
				int[] randomUnrepeatArray = EventAwardMgr.GetRandomUnrepeatArray(0, num2 - 1, num);
				int[] array = randomUnrepeatArray;
				for (int i = 0; i < array.Length; i++)
				{
					int index = array[i];
					EventAwardInfo item = list3[index];
					list2.Add(item);
				}
			}
			foreach (EventAwardInfo current in list2)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
				newChickenBoxItemInfo.TemplateID = current.TemplateID;
				newChickenBoxItemInfo.IsBinds = current.IsBinds;
				newChickenBoxItemInfo.ValidDate = current.ValidDate;
				newChickenBoxItemInfo.Count = current.Count;
				newChickenBoxItemInfo.StrengthenLevel = current.StrengthenLevel;
				newChickenBoxItemInfo.AttackCompose = 0;
				newChickenBoxItemInfo.DefendCompose = 0;
				newChickenBoxItemInfo.AgilityCompose = 0;
				newChickenBoxItemInfo.LuckCompose = 0;
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(current.TemplateID);
				newChickenBoxItemInfo.Quality = ((itemTemplateInfo == null) ? 2 : itemTemplateInfo.Quality);
				newChickenBoxItemInfo.IsSelected = false;
				newChickenBoxItemInfo.IsSeeded = false;
				list.Add(newChickenBoxItemInfo);
			}
			return list;
		}

		public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
		{
			int[] numArray = new int[count];
			for (int i = 0; i < count; i++)
			{
				int num2 = rand.Next(minValue, maxValue + 1);
				int num3 = 0;
				for (int j = 0; j < i; j++)
				{
					if (numArray[j] == num2)
					{
						num3++;
					}
				}
				if (num3 == 0)
				{
					numArray[i] = num2;
				}
				else
				{
					i--;
				}
			}
			return numArray;
		}

		public static bool Init()
		{
			return ReLoad();
		}

		public static EventAwardInfo[] LoadEventAwardDb()
		{
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				return bussiness.GetEventAwardInfos();
			}
		}

		public static Dictionary<int, List<EventAwardInfo>> LoadEventAwards(EventAwardInfo[] EventAwards)
		{
			Dictionary<int, List<EventAwardInfo>> dictionary = new Dictionary<int, List<EventAwardInfo>>();
			foreach (EventAwardInfo info in EventAwards)
			{
				if (!dictionary.Keys.Contains(info.ActivityType))
				{
					IEnumerable<EventAwardInfo> source = EventAwards.Where((EventAwardInfo s) => s.ActivityType == info.ActivityType);
					dictionary.Add(info.ActivityType, source.ToList());
				}
			}
			return dictionary;
		}

		public static bool ReLoad()
		{
			try
			{
				EventAwardInfo[] eventAwards = LoadEventAwardDb();
				Dictionary<int, List<EventAwardInfo>> dictionary = LoadEventAwards(eventAwards);
				if (eventAwards != null)
				{
					Interlocked.Exchange(ref m_eventAward, eventAwards);
					Interlocked.Exchange(ref m_EventAwards, dictionary);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ReLoad", exception);
				}
				return false;
			}
			return true;
		}

		public static List<ItemInfo> GetEventAwardByType(eEventType DataId)
        {
			List<ItemInfo> infos = new List<ItemInfo>();
			List<EventAwardInfo> FiltInfos = new List<EventAwardInfo>();
			List<EventAwardInfo> unFiltInfos = FindEventAward(DataId);
			if (unFiltInfos.Count == 0)
				return null;
			FiltInfos = unFiltInfos.Where(s => s.IsSelect == true).ToList();
			int dropItemCount = 1;
			int maxRound = rand.Next(unFiltInfos.Select(s => s.Random).Max());
			List<EventAwardInfo> RoundInfos = unFiltInfos.Where(s => s.Random >= maxRound && s.IsSelect == false).ToList();
			int maxItems = RoundInfos.Count();
			if (maxItems > 0)
			{
				dropItemCount = dropItemCount > maxItems ? maxItems : dropItemCount;
				int[] randomArray = GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);
				foreach (int i in randomArray)
				{
					EventAwardInfo item = RoundInfos[i];
					FiltInfos.Add(item);
				}
			}
			foreach (EventAwardInfo info in FiltInfos)
			{
				ItemTemplateInfo tempInfo = ItemMgr.FindItemTemplate(info.TemplateID);
				ItemInfo item = ItemInfo.CreateFromTemplate(tempInfo, info.Count, 105);
				item.TemplateID = info.TemplateID;
				item.IsBinds = info.IsBinds;
				item.ValidDate = info.ValidDate;
				item.Count = info.Count;
				item.StrengthenLevel = info.StrengthenLevel;
				item.AttackCompose = info.AttackCompose;
				item.DefendCompose = info.DefendCompose;
				item.AgilityCompose = info.AgilityCompose;
				item.LuckCompose = info.LuckCompose;
				infos.Add(item);
			}
			return infos;
		}
	}
}