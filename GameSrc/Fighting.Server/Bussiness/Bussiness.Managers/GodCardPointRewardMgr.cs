using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class GodCardPointRewardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, GodCardPointRewardInfo> m_godCardPointRewards =
            new Dictionary<int, GodCardPointRewardInfo>();

        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, GodCardPointRewardInfo> tempGodCardPointRewards = LoadFromDatabase();
                if (tempGodCardPointRewards.Values.Count > 0)
                {
                    Interlocked.Exchange(ref m_godCardPointRewards, tempGodCardPointRewards);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("GodCardPointRewardMgr init error:", ex);
            }

            return false;
        }

        private static Dictionary<int, GodCardPointRewardInfo> LoadFromDatabase()
        {
            Dictionary<int, GodCardPointRewardInfo> list = new Dictionary<int, GodCardPointRewardInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                GodCardPointRewardInfo[] godCardPointRewardInfos = db.GetAllGodCardPointReward();
                foreach (GodCardPointRewardInfo info in godCardPointRewardInfos)
                {
                    if (!list.ContainsKey(info.ID))
                    {
                        list.Add(info.ID, info);
                    }
                }
            }

            return list;
        }

        public static List<GodCardPointRewardInfo> GetAllGodCardPointReward()
        {
            if (m_godCardPointRewards.Count == 0)
                Init();
            return m_godCardPointRewards.Values.ToList();
        }

        public static GodCardPointRewardInfo FindGodCardPointReward(int id)
        {
            if (m_godCardPointRewards.Count == 0)
                Init();
            if (m_godCardPointRewards.ContainsKey(id))
                return m_godCardPointRewards[id];
            return null;
        }
    }
}