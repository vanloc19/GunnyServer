using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class FightSpiritTemplateMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, List<FightSpiritTemplateInfo>> m_fightSpiritTemplates =
            new Dictionary<int, List<FightSpiritTemplateInfo>>();

        public static bool ReLoad()
        {
            try
            {
                FightSpiritTemplateInfo[] tempFightSpiritTemplate = LoadFightSpiritTemplateDb();
                Dictionary<int, List<FightSpiritTemplateInfo>> tempFightSpiritTemplates =
                    LoadFightSpiritTemplates(tempFightSpiritTemplate);
                if (tempFightSpiritTemplate.Length > 0)
                {
                    Interlocked.Exchange(ref m_fightSpiritTemplates, tempFightSpiritTemplates);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ReLoad FightSpiritTemplate", e);
                return false;
            }

            return true;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static FightSpiritTemplateInfo[] LoadFightSpiritTemplateDb()
        {
            using (ProduceBussiness pb = new ProduceBussiness())
            {
                FightSpiritTemplateInfo[] infos = pb.GetAllFightSpiritTemplate();
                return infos;
            }
        }

        public static Dictionary<int, List<FightSpiritTemplateInfo>> LoadFightSpiritTemplates(
            FightSpiritTemplateInfo[] fightSpiritTemplates)
        {
            Dictionary<int, List<FightSpiritTemplateInfo>> infos = new Dictionary<int, List<FightSpiritTemplateInfo>>();
            foreach (FightSpiritTemplateInfo info in fightSpiritTemplates)
            {
                if (!infos.Keys.Contains(info.FightSpiritID))
                {
                    IEnumerable<FightSpiritTemplateInfo> temp =
                        fightSpiritTemplates.Where(s => s.FightSpiritID == info.FightSpiritID);
                    infos.Add(info.FightSpiritID, temp.ToList());
                }
            }

            return infos;
        }

        private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

        public static List<FightSpiritTemplateInfo> FindFightSpiritTemplates(int id)
        {
            m_clientLocker.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (m_fightSpiritTemplates.ContainsKey(id))
                {
                    List<FightSpiritTemplateInfo> items = m_fightSpiritTemplates[id];
                    return items;
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }

            return new List<FightSpiritTemplateInfo>();
        }

        public static FightSpiritTemplateInfo FindFightSpiritTemplateInfo(int FigSpiritId, int lv)
        {
            List<FightSpiritTemplateInfo> infos = FindFightSpiritTemplates(FigSpiritId);
            foreach (FightSpiritTemplateInfo fs in infos)
            {
                if (fs.Level == lv)
                {
                    return fs;
                }
            }

            return null;
        }

        public static int GOLDEN_LEVEL(int lv)
        {
            try
            {
                string[] addamages = GameProperties.FightSpiritLevelAddDamage.Split('|');
                foreach (string add in addamages)
                {
                    if (add.Split(',')[0] == lv.ToString())
                        return int.Parse(add.Split(',')[1]);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("FightSpiritTemplate.GOLDEN_LEVEL: ", e);
            }

            return 0;
        }

        public static int[] Exps()
        {
            List<FightSpiritTemplateInfo> infos = FindFightSpiritTemplates(100001);
            List<int> exps = new List<int>();
            foreach (FightSpiritTemplateInfo fs in infos)
            {
                exps.Add(fs.Exp);
            }

            return exps.ToArray();
        }

        public static int GetProp(int figSpiritId, int lv, int place, ref int addAtt, ref int rdcDama)
        {
            FightSpiritTemplateInfo temp = FindFightSpiritTemplateInfo(figSpiritId, lv);

            if (temp == null)
            {
                List<FightSpiritTemplateInfo> infos = FindFightSpiritTemplates(figSpiritId);
                if (infos.Count > 0)
                {
                    temp = infos[infos.Count - 1];
                    log.ErrorFormat("FigSpiritId: {0}, level: {1} not found! Return Max level in database is {2}",
                        figSpiritId, lv, temp.Level);
                }
                else
                {
                    log.ErrorFormat("FigSpiritId: {0} not found! Return 0", figSpiritId);
                    return 0;
                }
            }

            switch (figSpiritId)
            {
                case 100001:
                case 100003:
                    addAtt += GOLDEN_LEVEL(lv);
                    break;
                default:
                    rdcDama += GOLDEN_LEVEL(lv);
                    break;
            }

            switch (place)
            {
                case 2:
                    return temp.Attack;
                case 5:
                    return temp.Agility;
                case 11:
                    return temp.Defence;
                case 3:
                    return temp.Lucky;
                case 13:
                    return temp.Blood;
            }

            return 0;
        }
    }
}