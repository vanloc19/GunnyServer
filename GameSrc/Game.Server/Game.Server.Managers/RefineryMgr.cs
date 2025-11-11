using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class RefineryMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static Dictionary<int, RefineryInfo> m_Item_Refinery = new Dictionary<int, RefineryInfo>();

		private static ThreadSafeRandom rand = new ThreadSafeRandom();

		public static bool Init()
		{
			return Reload();
		}

		public static Dictionary<int, RefineryInfo> LoadFromBD()
		{
			new List<RefineryInfo>();
			Dictionary<int, RefineryInfo> dictionary = new Dictionary<int, RefineryInfo>();
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				foreach (RefineryInfo info in bussiness.GetAllRefineryInfo())
				{
					if (!dictionary.ContainsKey(info.RefineryID))
					{
						dictionary.Add(info.RefineryID, info);
					}
				}
				return dictionary;
			}
		}

		public static ItemTemplateInfo Refinery(GamePlayer player, List<ItemInfo> Items, ItemInfo Item, bool Luck, int OpertionType, ref bool result, ref int defaultprobability, ref bool IsFormula)
		{
			new ItemTemplateInfo();
			foreach (int num in m_Item_Refinery.Keys)
			{
				if (m_Item_Refinery[num].m_Equip.Contains(Item.TemplateID))
				{
					IsFormula = true;
					int num2 = 0;
					List<int> list = new List<int>();
					foreach (ItemInfo info in Items)
					{
						if (info.TemplateID == m_Item_Refinery[num].Item1 && info.Count >= m_Item_Refinery[num].Item1Count && !list.Contains(info.TemplateID))
						{
							list.Add(info.TemplateID);
							if (OpertionType != 0)
							{
								info.Count -= m_Item_Refinery[num].Item1Count;
							}
							num2++;
						}
						if (info.TemplateID == m_Item_Refinery[num].Item2 && info.Count >= m_Item_Refinery[num].Item2Count && !list.Contains(info.TemplateID))
						{
							list.Add(info.TemplateID);
							if (OpertionType != 0)
							{
								info.Count -= m_Item_Refinery[num].Item2Count;
							}
							num2++;
						}
						if (info.TemplateID == m_Item_Refinery[num].Item3 && info.Count >= m_Item_Refinery[num].Item3Count && !list.Contains(info.TemplateID))
						{
							list.Add(info.TemplateID);
							if (OpertionType != 0)
							{
								info.Count -= m_Item_Refinery[num].Item3Count;
							}
							num2++;
						}
					}
					if (num2 != 3)
					{
						continue;
					}
					for (int i = 0; i < m_Item_Refinery[num].m_Reward.Count; i++)
					{
						if (Items[Items.Count - 1].TemplateID == m_Item_Refinery[num].m_Reward[i])
						{
							if (Luck)
							{
								defaultprobability += 20;
							}
							if (OpertionType == 0)
							{
								return ItemMgr.FindItemTemplate(m_Item_Refinery[num].m_Reward[i + 1]);
							}
							if (rand.Next(100) < defaultprobability)
							{
								int templateId = m_Item_Refinery[num].m_Reward[i + 1];
								result = true;
								return ItemMgr.FindItemTemplate(templateId);
							}
						}
					}
				}
				else
				{
					IsFormula = false;
				}
			}
			return null;
		}

		public static ItemTemplateInfo RefineryTrend(int Operation, ItemInfo Item, ref bool result)
		{
			if (Item != null)
			{
				foreach (int num in m_Item_Refinery.Keys)
				{
					if (!m_Item_Refinery[num].m_Reward.Contains(Item.TemplateID))
					{
						continue;
					}
					for (int i = 0; i < m_Item_Refinery[num].m_Reward.Count; i++)
					{
						if (m_Item_Refinery[num].m_Reward[i] == Operation)
						{
							int templateId = m_Item_Refinery[num].m_Reward[i + 2];
							result = true;
							return ItemMgr.FindItemTemplate(templateId);
						}
					}
				}
			}
			return null;
		}

		public static bool Reload()
		{
			try
			{
				Dictionary<int, RefineryInfo> dictionary = new Dictionary<int, RefineryInfo>();
				dictionary = LoadFromBD();
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange(ref m_Item_Refinery, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				log.Error("NPCInfoMgr", exception);
			}
			return false;
		}
	}
}
