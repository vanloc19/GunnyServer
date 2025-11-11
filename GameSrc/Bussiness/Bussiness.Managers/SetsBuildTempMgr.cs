using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class SetsBuildTempMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, SetsBuildTempInfo> m_setsBuildTemps = new Dictionary<int, SetsBuildTempInfo>();
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, SetsBuildTempInfo> tempSetsBuildTemps = LoadFromDatabase();
                if (tempSetsBuildTemps.Values.Count > 0)
                {
                    Interlocked.Exchange(ref m_setsBuildTemps, tempSetsBuildTemps);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("SetsBuildTempMgr init error:", ex);
            }

            return false;
        }

        private static Dictionary<int, SetsBuildTempInfo> LoadFromDatabase()
        {
            Dictionary<int, SetsBuildTempInfo> list = new Dictionary<int, SetsBuildTempInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                SetsBuildTempInfo[] setsBuildTempInfos = db.GetAllSetsBuildTemp();
                foreach (SetsBuildTempInfo info in setsBuildTempInfos)
                {
                    if (!list.ContainsKey(info.Level))
                    {
                        list.Add(info.Level, info);
                    }
                }
            }

            return list;
        }

        public static List<SetsBuildTempInfo> GetAllSetsBuildTemp()
        {
            if (m_setsBuildTemps.Count == 0)
                Init();
            return m_setsBuildTemps.Values.ToList();
        }

        public static int SetsBuildMax()
        {
            int maxLv = m_setsBuildTemps.Count;
            return m_setsBuildTemps[maxLv].Exp;
        }

        public static SetsBuildTempInfo FindNextSetsBuildExp(int exp)
        {
            List<SetsBuildTempInfo> infos = GetAllSetsBuildTemp();
            SetsBuildTempInfo info = null;
            for (int i = 0; i < infos.Count; i++)
            {
                info = infos[i];
                if (exp < info.Exp)
                    return info;
            }

            return info;
        }

        public static void GetSetsBuildProp(int exp, ref int def, ref int blood, ref int luck, ref int agi,
            ref int mgDef)
        {
            List<SetsBuildTempInfo> infos = GetAllSetsBuildTemp();
            SetsBuildTempInfo info = null;
            for (int i = 0; i < infos.Count; i++)
            {
                info = infos[i];
                if (info != null && exp >= info.Exp)
                {
                    def += info.DefenceGrow;
                    blood += info.BloodGrow;
                    luck += info.LuckGrow;
                    agi += info.AgilityGrow;
                    mgDef += info.MagicDefenceGrow;
                }
            }
        }

        public static void GetSetsBuildProp(int exp, ref double guard)
        {
            List<SetsBuildTempInfo> infos = GetAllSetsBuildTemp();
            SetsBuildTempInfo info = null;
            for (int i = 0; i < infos.Count; i++)
            {
                info = infos[i];
                if (info != null && exp >= info.Exp)
                {
                    guard += info.GuardGrow;
                }
            }
        }
    }
}