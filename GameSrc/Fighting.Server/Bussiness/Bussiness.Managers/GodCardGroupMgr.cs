using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class GodCardGroupMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, GodCardGroupInfo> m_godCardGroups = new Dictionary<int, GodCardGroupInfo>();

        private static Dictionary<int, List<GodCardGroupDetailInfo>> m_cardGroupDetails =
            new Dictionary<int, List<GodCardGroupDetailInfo>>();

        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, GodCardGroupInfo> tempGodCardGroups = LoadFromDatabase();
                Dictionary<int, List<GodCardGroupDetailInfo>> tempCardGroupDetails =
                    LoadCardGroupDetailDb(tempGodCardGroups);
                if (tempGodCardGroups.Values.Count > 0)
                {
                    Interlocked.Exchange(ref m_godCardGroups, tempGodCardGroups);
                    Interlocked.Exchange(ref m_cardGroupDetails, tempCardGroupDetails);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("GodCardGroupMgr init error:", ex);
            }

            return false;
        }

        private static Dictionary<int, GodCardGroupInfo> LoadFromDatabase()
        {
            Dictionary<int, GodCardGroupInfo> list = new Dictionary<int, GodCardGroupInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                GodCardGroupInfo[] godCardGroupInfos = db.GetAllGodCardGroup();
                foreach (GodCardGroupInfo info in godCardGroupInfos)
                {
                    if (!list.ContainsKey(info.GroupID))
                    {
                        list.Add(info.GroupID, info);
                    }
                }
            }

            return list;
        }

        public static Dictionary<int, List<GodCardGroupDetailInfo>> LoadCardGroupDetailDb(
            Dictionary<int, GodCardGroupInfo> godCardGroupInfos)
        {
            Dictionary<int, List<GodCardGroupDetailInfo>> list = new Dictionary<int, List<GodCardGroupDetailInfo>>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                GodCardGroupDetailInfo[] infos = db.GetAllGodCardGroupDetail();
                foreach (GodCardGroupInfo quest in godCardGroupInfos.Values)
                {
                    IEnumerable<GodCardGroupDetailInfo> temp = infos.Where(s => s.GroupID == quest.GroupID);
                    list.Add(quest.GroupID, temp.ToList());
                }
            }

            return list;
        }

        public static List<GodCardGroupInfo> GetAllGodCardGroup()
        {
            if (m_godCardGroups.Count == 0)
                Init();
            return m_godCardGroups.Values.ToList();
        }

        public static GodCardGroupInfo FindGodCardGroup(int id)
        {
            if (m_godCardGroups.Count == 0)
                Init();
            if (m_godCardGroups.ContainsKey(id))
                return m_godCardGroups[id];
            return null;
        }

        public static List<GodCardGroupDetailInfo> FindGodCardGroupDetail(int id)
        {
            if (m_cardGroupDetails.ContainsKey(id))
                return m_cardGroupDetails[id];
            return new List<GodCardGroupDetailInfo>();
        }
    }
}