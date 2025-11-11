using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class QiYuanAwardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, QiYuanAwardInfo> m_qiYuanAwards = new Dictionary<int, QiYuanAwardInfo>();

        private static Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>> m_qiYuanTowerAwards =
            new Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>>();

        public static Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>> TowerAwards
        {
            get { return m_qiYuanTowerAwards; }
        }

        public static string QiYuanRewardBoxTemplateIds = "";

        public static string QiYuanTreasureBoxTemplateIds = "";

        private static ThreadSafeRandom rand = new ThreadSafeRandom();

        public static bool ReLoad()
        {
            try
            {
                QiYuanAwardInfo[] tempData = LoadDb();
                Dictionary<int, QiYuanAwardInfo> tempItems = LoadItems(tempData);
                if (tempData.Length > 0)
                {
                    Interlocked.Exchange(ref m_qiYuanAwards, tempItems);

                    Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>> tempDataBox =
                        new Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>>();

                    // import template reward
                    List<string> itemStrArr = null;
                    List<QiYuanAwardInfo> rewardList = FindQiYuanAward(eQiYuanAwardType.REWARD_BOX);
                    if (rewardList.Count > 0)
                    {
                        itemStrArr = new List<string>();
                        foreach (QiYuanAwardInfo item in rewardList)
                            itemStrArr.Add(item.TemplateID.ToString());

                        QiYuanRewardBoxTemplateIds = string.Join(",", itemStrArr);
                    }

                    // import template treasure
                    rewardList = FindQiYuanAward(eQiYuanAwardType.TREASURE_BOX);
                    if (rewardList.Count > 0)
                    {
                        itemStrArr = new List<string>();
                        foreach (QiYuanAwardInfo item in rewardList)
                            itemStrArr.Add(item.TemplateID.ToString());

                        QiYuanTreasureBoxTemplateIds = string.Join(",", itemStrArr);
                    }

                    //import box
                    Dictionary<int, QiYuanAwardInfo> sortLists = m_qiYuanAwards
                        .Where(a => a.Value.Type == (int)eQiYuanAwardType.TOWER_TASK)
                        .ToDictionary(a => a.Key, a => a.Value);
                    foreach (KeyValuePair<int, QiYuanAwardInfo> key in sortLists)
                    {
                        if (key.Value.Rank == 0 && !tempDataBox.ContainsKey(key.Value))
                        {
                            List<QiYuanAwardInfo> subDatas =
                                sortLists.Values.Where(a => a.Rank == key.Value.ID).ToList();
                            tempDataBox.Add(key.Value, subDatas);
                        }
                    }

                    Interlocked.Exchange(ref m_qiYuanTowerAwards, tempDataBox);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ReLoad QiYuanAwardMgr", e);
                return false;
            }

            return true;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static QiYuanAwardInfo[] LoadDb()
        {
            using (ProduceBussiness ab = new ProduceBussiness())
            {
                QiYuanAwardInfo[] infos = ab.GetAllQiYuanAward();
                return infos;
            }
        }

        public static Dictionary<int, QiYuanAwardInfo> LoadItems(QiYuanAwardInfo[] QiYuanData)
        {
            Dictionary<int, QiYuanAwardInfo> infos = new Dictionary<int, QiYuanAwardInfo>();
            foreach (QiYuanAwardInfo info in QiYuanData)
            {
                if (!infos.Keys.Contains(info.ID))
                {
                    infos.Add(info.ID, info);
                }
            }

            return infos;
        }

        public static List<QiYuanAwardInfo> FindQiYuanAward(eQiYuanAwardType type)
        {
            return m_qiYuanAwards.Values.Where(a => a.Type == (int)type).ToList();
        }

        public static QiYuanAwardInfo FindQiYuanAward(eQiYuanAwardType type, int rank)
        {
            List<QiYuanAwardInfo> data = FindQiYuanAward(type);
            if (data != null && data.Count > 0)
            {
                return data.SingleOrDefault(a => a.Rank == rank);
            }

            return null;
        }

        public static QiYuanAwardInfo FindQiYuanAward(int id)
        {
            if (m_qiYuanAwards.ContainsKey(id))
                return m_qiYuanAwards[id];
            return null;
        }

        public static QiYuanAwardInfo[] GetRandom(eQiYuanAwardType type)
        {
            List<QiYuanAwardInfo> FiltInfos = new List<QiYuanAwardInfo>();
            List<QiYuanAwardInfo> unFiltInfos = FindQiYuanAward(type);
            int dropItemCount = 1;
            int maxRound = rand.Next(unFiltInfos.Select(s => s.Probability).Max());
            List<QiYuanAwardInfo> RoundInfos = unFiltInfos.Where(s => s.Probability >= maxRound).ToList();
            int maxItems = RoundInfos.Count();
            if (maxItems > 0)
            {
                dropItemCount = dropItemCount > maxItems ? maxItems : dropItemCount;
                int[] randomArray = EventAwardMgr.GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);
                foreach (int i in randomArray)
                {
                    QiYuanAwardInfo item = RoundInfos[i];
                    FiltInfos.Add(item);
                }
            }

            return FiltInfos.ToArray();
        }
    }

    public enum eQiYuanAwardType
    {
        BELIEF_AWARD = 1,
        AREA_RANK_AWARD = 2,
        ALL_AREA_RANK_AWARD = 3,
        JOIN_REWARD = 4,
        JOIN_PROBABILITY_REWARD = 5,
        REWARD_BOX = 6,
        TREASURE_BOX = 7,
        TOWER_TASK = 8,
    }
}