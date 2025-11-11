
// Type: Game.Server.Managers.JampsManualMgr


//\Server\Road\Game.Server.dll

using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Game.Server.Managers
{
    public class JampsManualMgr
    {
        private static Dictionary<int, JampsChapterItemList> _capitulos;
        public static Dictionary<int, JampsManualItemList> _jampsManualItemList;
        public static List<JampsUpgradeItemList> _jampsCondictions;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;
        public static int maxLevel = 0;

        public static int randNumber(int min, int max, HashSet<int> exclude) => Enumerable.Range(min, max).Where<int>((Func<int, bool>)(i => !exclude.Contains(i))).ElementAt<int>(JampsManualMgr.rand.Next(0, max - exclude.Count));

        public static int randNumber(int min, int max) => JampsManualMgr.rand.Next(min, max);

        private static List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> eList = new List<E>();
            while (inputList.Count > 0)
            {
                int index = JampsManualMgr.randNumber(0, inputList.Count);
                eList.Add(inputList[index]);
                inputList.RemoveAt(index);
            }
            return eList;
        }

        public static JampsChapterItemList getRandomChapter()
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                List<JampsChapterItemList> jampsChapterItemListList = JampsManualMgr.ShuffleList<JampsChapterItemList>(JampsManualMgr._capitulos.Values.ToList<JampsChapterItemList>());
                return jampsChapterItemListList[JampsManualMgr.randNumber(0, jampsChapterItemListList.Count)];
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsChapterItemList)null;
        }

        public static JampsUpgradeItemList[] getCondictionsByLevel(int level)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                return JampsManualMgr._jampsCondictions.Where<JampsUpgradeItemList>((Func<JampsUpgradeItemList, bool>)(o => o.ManualLevel == level)).ToArray<JampsUpgradeItemList>();
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: getCondictionsByLevel", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsUpgradeItemList[])null;
        }

        public static JampsUpgradeItemList[] getCondictionsByLevelAndType(
          int level,
          int type)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                return JampsManualMgr._jampsCondictions.Where<JampsUpgradeItemList>((Func<JampsUpgradeItemList, bool>)(o => o.ManualLevel == level)).Where<JampsUpgradeItemList>((Func<JampsUpgradeItemList, bool>)(o => o.ConditionType == type)).ToArray<JampsUpgradeItemList>();
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: getCondictionsByLevel", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsUpgradeItemList[])null;
        }

        public static JampsChapterItemList getChapter(int id)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                if (JampsManualMgr._capitulos.ContainsKey(id))
                    return JampsManualMgr._capitulos[id];
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsChapterItemList)null;
        }

        public static JampsPageItemList getRandomPageFromChapter(int ID, int max)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                if (JampsManualMgr._capitulos.ContainsKey(ID))
                {
                    List<JampsPageItemList> jampsPageItemListList = JampsManualMgr.ShuffleList<JampsPageItemList>(JampsManualMgr._capitulos[ID].paginas.Values.ToList<JampsPageItemList>());
                    int max1 = max > jampsPageItemListList.Count ? jampsPageItemListList.Count : max;
                    return jampsPageItemListList[JampsManualMgr.randNumber(0, max1)];
                }
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsPageItemList)null;
        }

        public static List<JampsPageItemList> getPageFromChapter(int ID)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                List<JampsPageItemList> pageFromChapter = new List<JampsPageItemList>();
                foreach (JampsChapterItemList jampsChapterItemList in JampsManualMgr._capitulos.Values.Where<JampsChapterItemList>((Func<JampsChapterItemList, bool>)(o => o.ID == ID)))
                {
                    foreach (JampsPageItemList jampsPageItemList in jampsChapterItemList.paginas.Values)
                        pageFromChapter.Add(jampsPageItemList);
                }
                return pageFromChapter;
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (List<JampsPageItemList>)null;
        }

        public static List<JampsDebrisItemList> getRandomDebrisFromPage(int ID)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            List<JampsDebrisItemList> randomDebrisFromPage = new List<JampsDebrisItemList>();
            try
            {
                foreach (JampsChapterItemList jampsChapterItemList in JampsManualMgr._capitulos.Values)
                {
                    foreach (JampsPageItemList jampsPageItemList in jampsChapterItemList.paginas.Values.Where<JampsPageItemList>((Func<JampsPageItemList, bool>)(o => o.ID == ID)))
                    {
                        foreach (JampsDebrisItemList jampsDebrisItemList in jampsPageItemList._fragmentos.Values)
                            randomDebrisFromPage.Add(jampsDebrisItemList);
                    }
                }
                return randomDebrisFromPage;
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return randomDebrisFromPage;
        }

        public static List<JampsDebrisItemList> GetAllDebrisByCapitulo(
          int capitulo)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            List<JampsDebrisItemList> debrisByCapitulo = new List<JampsDebrisItemList>();
            try
            {
                foreach (JampsPageItemList jampsPageItemList in JampsManualMgr._capitulos[capitulo].paginas.Values)
                {
                    foreach (JampsDebrisItemList jampsDebrisItemList in jampsPageItemList._fragmentos.Values)
                        debrisByCapitulo.Add(jampsDebrisItemList);
                }
                return debrisByCapitulo;
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return debrisByCapitulo;
        }

        public static int getChapterIDFromDebrisID(int ID)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            List<JampsDebrisItemList> jampsDebrisItemListList = new List<JampsDebrisItemList>();
            try
            {
                foreach (JampsChapterItemList jampsChapterItemList in JampsManualMgr._capitulos.Values)
                {
                    foreach (JampsPageItemList jampsPageItemList in jampsChapterItemList.paginas.Values)
                    {
                        foreach (JampsDebrisItemList jampsDebrisItemList in jampsPageItemList._fragmentos.Values)
                        {
                            if (jampsDebrisItemList.ID == ID)
                                return jampsChapterItemList.ID;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return -1;
        }

        public static JampsPageItemList getPageFromID(int ID)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            List<JampsDebrisItemList> jampsDebrisItemListList = new List<JampsDebrisItemList>();
            try
            {
                foreach (JampsChapterItemList jampsChapterItemList in JampsManualMgr._capitulos.Values)
                {
                    foreach (JampsPageItemList pageFromId in jampsChapterItemList.paginas.Values)
                    {
                        if (pageFromId.ID == ID)
                            return pageFromId;
                    }
                }
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsPageItemList)null;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                JampsManualMgr.m_lock = new ReaderWriterLock();
                JampsManualMgr._capitulos = new Dictionary<int, JampsChapterItemList>();
                JampsManualMgr._jampsManualItemList = new Dictionary<int, JampsManualItemList>();
                JampsManualMgr._jampsCondictions = new List<JampsUpgradeItemList>();
                JampsManualMgr.rand = new ThreadSafeRandom();
                flag = JampsManualMgr.LoadCapitulos(JampsManualMgr._capitulos) && JampsManualMgr.LoadJampsManualItemList(JampsManualMgr._jampsManualItemList) && JampsManualMgr.LoadJampsCondictions(JampsManualMgr._jampsCondictions);
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)nameof(JampsManualMgr), ex);
                flag = false;
            }
            return flag;
        }

        private static bool LoadCapitulos(Dictionary<int, JampsChapterItemList> list)
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (JampsChapterItemList jampsChapterItem in produceBussiness.Get_JampsChapterItemList())
                {
                    jampsChapterItem.paginas = new Dictionary<int, JampsPageItemList>();
                    foreach (JampsPageItemList jampsPageItem in produceBussiness.Get_JampsPageItemList(jampsChapterItem.ID))
                    {
                        jampsPageItem._fragmentos = new Dictionary<int, JampsDebrisItemList>();
                        foreach (JampsDebrisItemList jampsDebrisItem in produceBussiness.Get_JampsDebrisItemList(jampsPageItem.ID))
                            jampsPageItem._fragmentos.Add(jampsDebrisItem.ID, jampsDebrisItem);
                        jampsChapterItem.paginas.Add(jampsPageItem.ID, jampsPageItem);
                    }
                    list.Add(jampsChapterItem.ID, jampsChapterItem);
                }
            }
            return true;
        }

        private static bool LoadJampsManualItemList(Dictionary<int, JampsManualItemList> list)
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (JampsManualItemList jampsManualItem in produceBussiness.Get_JampsManualItemList())
                {
                    if (jampsManualItem.Level > JampsManualMgr.maxLevel)
                        JampsManualMgr.maxLevel = jampsManualItem.Level;
                    if (!list.ContainsKey(jampsManualItem.Level))
                        list.Add(jampsManualItem.Level, jampsManualItem);
                }
            }
            return true;
        }

        private static bool LoadJampsCondictions(List<JampsUpgradeItemList> list)
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (JampsUpgradeItemList jampsUpgradeItem in produceBussiness.Get_JampsUpgradeItemList())
                    list.Add(jampsUpgradeItem);
            }
            return true;
        }

        public static JampsDebrisItemList getRandomDebrisFromPages(
          List<PagesInfo> activesPage)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            List<JampsDebrisItemList> inputList = new List<JampsDebrisItemList>();
            try
            {
                foreach (JampsChapterItemList jampsChapterItemList in JampsManualMgr._capitulos.Values)
                {
                    foreach (JampsDebrisItemList jampsDebrisItemList in JampsManualMgr.GetAllDebrisByCapitulo(jampsChapterItemList.ID))
                    {
                        if (activesPage.Select<PagesInfo, int>((Func<PagesInfo, int>)(o => o.pageID)).Contains<int>(jampsDebrisItemList.PageID))
                            inputList.Add(jampsDebrisItemList);
                    }
                }
                return JampsManualMgr.ShuffleList<JampsDebrisItemList>(inputList)[JampsManualMgr.randNumber(0, inputList.Count)];
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsDebrisItemList)null;
        }

        public static JampsDebrisItemList getRandomDebrisFromPages(
          List<PagesInfo> activesPage,
          int chapter)
        {
            JampsManualMgr.m_lock.AcquireReaderLock(10000);
            List<JampsDebrisItemList> inputList = new List<JampsDebrisItemList>();
            try
            {
                foreach (JampsChapterItemList jampsChapterItemList in JampsManualMgr._capitulos.Values.Where<JampsChapterItemList>((Func<JampsChapterItemList, bool>)(o => o.ID == chapter)))
                {
                    foreach (JampsDebrisItemList jampsDebrisItemList in JampsManualMgr.GetAllDebrisByCapitulo(jampsChapterItemList.ID))
                    {
                        if (activesPage.Select<PagesInfo, int>((Func<PagesInfo, int>)(o => o.pageID)).Contains<int>(jampsDebrisItemList.PageID))
                            inputList.Add(jampsDebrisItemList);
                    }
                }
                int num = JampsManualMgr.randNumber(0, inputList.Count);
                return JampsManualMgr.ShuffleList<JampsDebrisItemList>(inputList)[num < 0 ? 0 : (num > inputList.Count ? inputList.Count : num)];
            }
            catch (Exception ex)
            {
                if (JampsManualMgr.log.IsErrorEnabled)
                    JampsManualMgr.log.Error((object)"JampsManualMgr: GetAllDebrisByPageID", ex);
            }
            finally
            {
                JampsManualMgr.m_lock.ReleaseReaderLock();
            }
            return (JampsDebrisItemList)null;
        }
    }
}
