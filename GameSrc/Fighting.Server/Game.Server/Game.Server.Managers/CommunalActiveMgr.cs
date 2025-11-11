using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class CommunalActiveMgr
	{
		private static Dictionary<int, CommunalActiveAwardInfo> _communalActiveAwards;

		private static Dictionary<int, CommunalActiveExpInfo> _communalActiveExps;

		private static Dictionary<int, CommunalActiveInfo> _communalActives;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static ReaderWriterLock m_lock;

		private static ThreadSafeRandom rand;

		public static CommunalActiveInfo FindCommunalActive(int ActiveID)
		{
			if (_communalActives == null)
			{
				Init();
			}
			m_lock.AcquireReaderLock(10000);
			try
			{
				if (_communalActives.ContainsKey(ActiveID))
				{
					return _communalActives[ActiveID];
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

		public static List<CommunalActiveAwardInfo> FindCommunalAwards(int isArea)
		{
			if (_communalActiveAwards == null)
			{
				Init();
			}
			List<CommunalActiveAwardInfo> list = new List<CommunalActiveAwardInfo>();
			m_lock.AcquireReaderLock(10000);
			try
			{
				foreach (CommunalActiveAwardInfo info in _communalActiveAwards.Values)
				{
					if (info.IsArea == isArea)
					{
						list.Add(info);
					}
				}
				return list;
			}
			catch
			{
				return list;
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
		}

		public static List<ItemInfo> GetAwardInfos(int type, int rank)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (CommunalActiveAwardInfo info in FindCommunalAwards(type))
			{
				if (info.RandID == rank)
				{
					ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info.TemplateID), info.Count, 102);
					if (item != null)
					{
						item.Count = info.Count;
						item.IsBinds = info.IsBind;
						item.ValidDate = info.ValidDate;
						item.StrengthenLevel = info.StrengthenLevel;
						item.AttackCompose = info.AttackCompose;
						item.DefendCompose = info.DefendCompose;
						item.AgilityCompose = info.AgilityCompose;
						item.LuckCompose = info.LuckCompose;
						list.Add(item);
					}
				}
			}
			return list;
		}

		public static int GetGP(int level)
		{
			if (_communalActiveExps == null)
			{
				Init();
			}
			if (_communalActiveExps.ContainsKey(level))
			{
				return _communalActiveExps[level].Exp;
			}
			return 0;
		}

		public static bool Init()
		{
			try
			{
				m_lock = new ReaderWriterLock();
				_communalActives = new Dictionary<int, CommunalActiveInfo>();
				_communalActiveAwards = new Dictionary<int, CommunalActiveAwardInfo>();
				_communalActiveExps = new Dictionary<int, CommunalActiveExpInfo>();
				rand = new ThreadSafeRandom();
				return LoadData(_communalActives, _communalActiveAwards, _communalActiveExps);
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("CommunalActiveMgr", exception);
				}
				return false;
			}
		}

		private static bool LoadData(Dictionary<int, CommunalActiveInfo> CommunalActives, Dictionary<int, CommunalActiveAwardInfo> CommunalActiveAwards, Dictionary<int, CommunalActiveExpInfo> CommunalActiveExps)
		{
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				CommunalActiveInfo[] allCommunalActive = bussiness.GetAllCommunalActive();
				foreach (CommunalActiveInfo info in allCommunalActive)
				{
					if (!CommunalActives.ContainsKey(info.ActiveID))
					{
						CommunalActives.Add(info.ActiveID, info);
					}
				}
				CommunalActiveAwardInfo[] allCommunalActiveAward = bussiness.GetAllCommunalActiveAward();
				foreach (CommunalActiveAwardInfo info2 in allCommunalActiveAward)
				{
					if (!CommunalActiveAwards.ContainsKey(info2.ID))
					{
						CommunalActiveAwards.Add(info2.ID, info2);
					}
				}
				CommunalActiveExpInfo[] allCommunalActiveExp = bussiness.GetAllCommunalActiveExp();
				foreach (CommunalActiveExpInfo info3 in allCommunalActiveExp)
				{
					if (!CommunalActiveExps.ContainsKey(info3.Grade))
					{
						CommunalActiveExps.Add(info3.Grade, info3);
					}
				}
			}
			return true;
		}

		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, CommunalActiveInfo> communalActives = new Dictionary<int, CommunalActiveInfo>();
				Dictionary<int, CommunalActiveAwardInfo> communalActiveAwards = new Dictionary<int, CommunalActiveAwardInfo>();
				Dictionary<int, CommunalActiveExpInfo> communalActiveExps = new Dictionary<int, CommunalActiveExpInfo>();
				if (LoadData(communalActives, communalActiveAwards, communalActiveExps))
				{
					m_lock.AcquireWriterLock(-1);
					try
					{
						_communalActives = communalActives;
						_communalActiveAwards = communalActiveAwards;
						_communalActiveExps = communalActiveExps;
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
					log.Error("CommunalActiveMgr", exception);
				}
			}
			return false;
		}

		public static void ResetEvent()
		{
		}
	}
}
