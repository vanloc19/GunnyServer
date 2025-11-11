using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class LoveLevelMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, LoveLevelInfo> m_loveLevels = new Dictionary<int, LoveLevelInfo>();
        private static ThreadSafeRandom random = new ThreadSafeRandom();
        private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, LoveLevelInfo> tempLoveLevels = LoadFromDatabase();
                if (tempLoveLevels.Values.Count > 0)
                {
                    Interlocked.Exchange(ref m_loveLevels, tempLoveLevels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("LoveLevelMgr init error:", ex);
            }

            return false;
        }

        private static Dictionary<int, LoveLevelInfo> LoadFromDatabase()
        {
            Dictionary<int, LoveLevelInfo> list = new Dictionary<int, LoveLevelInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                LoveLevelInfo[] loveLevelInfos = db.GetAllLoveLevel();
                foreach (LoveLevelInfo info in loveLevelInfos)
                {
                    if (!list.ContainsKey(info.Level))
                    {
                        list.Add(info.Level, info);
                    }
                }
            }

            return list;
        }

        public static List<LoveLevelInfo> GetAllLoveLevel()
        {
            if (m_loveLevels.Count == 0)
                Init();
            return m_loveLevels.Values.ToList();
        }

        public static LoveLevelInfo FindLoveLevel(int lv)
        {
            m_clientLocker.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (m_loveLevels.ContainsKey(lv))
                    return m_loveLevels[lv];
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }

            return null;
        }

        public static int MaxLevel
        {
            get { return m_loveLevels.Count; }
        }

        public static LoveLevelInfo GetLevel(int GP)
        {
            LoveLevelInfo info = FindLoveLevel(MaxLevel);
            if (info == null)
                return null;

            if (GP >= info.Exp)
            {
                return info;
            }
            else
            {
                for (int i = 1; i <= MaxLevel; i++)
                {
                    info = FindLoveLevel(i);
                    if (info == null)
                        continue;

                    if (GP < info.Exp)
                        return info;
                }
            }

            return null;
        }
    }
}