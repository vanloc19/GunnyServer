using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Game.Server.Managers
{
    public class StrengthenMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, StrengthenInfo> _strengthens;

        private static Dictionary<int, StrengthenInfo> m_Refinery_Strengthens;

        private static Dictionary<int, StrengthenGoodsInfo> Strengthens_Goods;

        private static Dictionary<int, StrengThenExpInfo> _Strengthens_Exps;

        private static System.Threading.ReaderWriterLock m_lock;

        private static ThreadSafeRandom rand;

        public static readonly List<double> RateItems = new List<double> { 0.75, 3.0, 12.0, 48.0, 240.0, 768.0 };

        public static readonly double VIPStrengthenEx = 0.3;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, StrengthenInfo> tempStrengthens = new Dictionary<int, StrengthenInfo>();
                Dictionary<int, StrengthenInfo> tempRefineryStrengthens = new Dictionary<int, StrengthenInfo>();
                Dictionary<int, StrengthenGoodsInfo> tempStrengthenGoodsInfos = new Dictionary<int, StrengthenGoodsInfo>();
                Dictionary<int, StrengThenExpInfo> tempStrengthenExpInfos = new Dictionary<int, StrengThenExpInfo>();
                if (LoadStrengthen(tempStrengthens, tempRefineryStrengthens, tempStrengthenExpInfos))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _strengthens = tempStrengthens;
                        m_Refinery_Strengthens = tempRefineryStrengthens;
                        //Strengthens_Goods = tempStrengthenGoodsInfos;
                        return true;
                    }
                    catch
                    { }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }

                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("StrengthenMgr", e);
                }
            }

            return false;
        }

        /// <summary>
        /// Initializes the StrengthenMgr.
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _strengthens = new Dictionary<int, StrengthenInfo>();
                m_Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
                Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
                _Strengthens_Exps = new Dictionary<int, StrengThenExpInfo>();
                rand = new ThreadSafeRandom();
                return LoadStrengthen(_strengthens, m_Refinery_Strengthens, _Strengthens_Exps);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("StrengthenMgr", e);
                }

                return false;
            }

        }

        private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen, Dictionary<int, StrengThenExpInfo> StrengthenExpInfo)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                StrengthenInfo[] infos = db.GetAllStrengthen();

                StrengthenInfo[] Refineryinfos = db.GetAllRefineryStrengthen();

                StrengthenGoodsInfo[] StrengthGoodInfos = db.GetAllStrengthenGoodsInfo();

                StrengThenExpInfo[] StrengthExpInfos = db.GetAllStrengThenExp();

                foreach (StrengthenInfo info in infos)
                {
                    if (!strengthen.ContainsKey(info.StrengthenLevel))
                    {
                        strengthen.Add(info.StrengthenLevel, info);
                    }
                }
                foreach (StrengthenInfo info in Refineryinfos)
                {
                    if (!RefineryStrengthen.ContainsKey(info.StrengthenLevel))
                    {
                        RefineryStrengthen.Add(info.StrengthenLevel, info);
                    }
                }

                foreach (StrengthenGoodsInfo info in StrengthGoodInfos)
                {
                    if (!Strengthens_Goods.ContainsKey(info.ID))
                    {
                        Strengthens_Goods.Add(info.ID, info);
                    }
                }
                foreach (StrengThenExpInfo info in StrengthExpInfos)
                {
                    if(!_Strengthens_Exps.ContainsKey(info.ID))
                    {
                        _Strengthens_Exps.Add(info.ID, info);
                    }
                }
            }

            return true;
        }

        public static StrengthenInfo FindStrengthenInfo(int level)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_strengthens.ContainsKey(level))
                {
                    return _strengthens[level];
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static StrengthenInfo FindRefineryStrengthenInfo(int level)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (m_Refinery_Strengthens.ContainsKey(level))
                {
                    return m_Refinery_Strengthens[level];
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }
        public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int TemplateId)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (int i in Strengthens_Goods.Keys)
                {
                    if (Strengthens_Goods[i].Level == level && TemplateId == Strengthens_Goods[i].CurrentEquip)
                    {
                        return Strengthens_Goods[i];
                    }
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static StrengthenGoodsInfo FindTransferInfo(int level, int templateId)
        {
            foreach (StrengthenGoodsInfo info in Strengthens_Goods.Values)
            {
                if (info.Level == level && (templateId == info.CurrentEquip))
                {
                    return info;
                }
            }
            return null;
        }

        public static StrengthenGoodsInfo FindTransferInfo(int templateId)
        {
            foreach (StrengthenGoodsInfo info in Strengthens_Goods.Values)
            {
                if (templateId == info.GainEquip || templateId == info.CurrentEquip)
                {
                    return info;
                }
            }

            return null;
        }
        public static StrengthenGoodsInfo FindRealStrengthenGoodInfo(int level, int templateid)
        {
            StrengthenGoodsInfo info = FindTransferInfo(templateid);
            if (info != null)
            {
                return FindStrengthenGoodsInfo(level, info.OrginEquip);
            }
            return null;
        }

        public static int GetNeedRate(ItemInfo mainItem)
        {
            int result = 0;
            StrengthenInfo info = FindStrengthenInfo(mainItem.StrengthenLevel + 1);
            switch (mainItem.Template.CategoryID)
            {
                case 5:
                    result = info.Rock2;
                    break;
                case 1:
                    result = info.Rock1;
                    break;
                case 17:
                    result = info.Rock3;
                    break;
                case 7:
                    result = info.Rock;
                    break;
            }
            return result;
        }


        public static void InheritProperty(ItemInfo Item, ref ItemInfo item)
        {
            if (Item.Hole1 >= 0)
            {
                item.Hole1 = Item.Hole1;
            }

            if (Item.Hole2 >= 0)
            {
                item.Hole2 = Item.Hole2;
            }

            if (Item.Hole3 >= 0)
            {
                item.Hole3 = Item.Hole3;
            }

            if (Item.Hole4 >= 0)
            {
                item.Hole4 = Item.Hole4;
            }

            if (Item.Hole5 >= 0)
            {
                item.Hole5 = Item.Hole5;
            }

            if (Item.Hole6 >= 0)
            {
                item.Hole6 = Item.Hole6;
            }

            item.AttackCompose = Item.AttackCompose;
            item.DefendCompose = Item.DefendCompose;
            item.LuckCompose = Item.LuckCompose;
            item.AgilityCompose = Item.AgilityCompose;
            item.IsBinds = Item.IsBinds;
            item.ValidDate = Item.ValidDate;
        }

        public static void InheritTransferProperty(ref ItemInfo itemZero, ref ItemInfo itemOne, bool tranHole, bool tranHoleFivSix)
        {
            int _item0Hole1 = itemZero.Hole1;
            int _item0Hole2 = itemZero.Hole2;
            int _item0Hole3 = itemZero.Hole3;
            int _item0Hole4 = itemZero.Hole4;
            int _item0Hole5 = itemZero.Hole5;
            int _item0Hole6 = itemZero.Hole6;
            int _item0Hole5Exp = itemZero.Hole5Exp;
            int _item0Hole5Level = itemZero.Hole5Level;
            int _item0Hole6Exp = itemZero.Hole6Exp;
            int _item0Hole6Level = itemZero.Hole6Level;
            int _item0AttackCompose = itemZero.AttackCompose;
            int _item0DefendCompose = itemZero.DefendCompose;
            int _item0AgilityCompose = itemZero.AgilityCompose;
            int _item0LuckCompose = itemZero.LuckCompose;
            int _item0StrengthenLevel = itemZero.StrengthenLevel;
            int _item0StrengthenExp = itemZero.StrengthenExp;
            bool _item0IsGold = itemZero.GoldValidDate();
            int _item0goldValidDate = itemZero.goldValidDate;
            DateTime _item0goldBeginTime = itemZero.goldBeginTime;
            string _item0latentEnergyCurStr = itemZero.latentEnergyCurStr;
            string _item0latentEnergyNewStr = itemZero.latentEnergyNewStr;
            DateTime _item0latentEnergyEndTime = itemZero.latentEnergyEndTime;

            int _item1Hole1 = itemOne.Hole1;
            int _item1Hole2 = itemOne.Hole2;
            int _item1Hole3 = itemOne.Hole3;
            int _item1Hole4 = itemOne.Hole4;
            int _item1Hole5 = itemOne.Hole5;
            int _item1Hole6 = itemOne.Hole6;
            int _item1Hole5Exp = itemOne.Hole5Exp;
            int _item1Hole5Level = itemOne.Hole5Level;
            int _item1Hole6Exp = itemOne.Hole6Exp;
            int _item1Hole6Level = itemOne.Hole6Level;
            int _item1AttackCompose = itemOne.AttackCompose;
            int _item1DefendCompose = itemOne.DefendCompose;
            int _item1AgilityCompose = itemOne.AgilityCompose;
            int _item1LuckCompose = itemOne.LuckCompose;
            int _item1StrengthenLevel = itemOne.StrengthenLevel;
            int _item1StrengthenExp = itemOne.StrengthenExp;
            bool _item1IsGold = itemOne.GoldValidDate();
            int _item1goldValidDate = itemOne.goldValidDate;
            DateTime _item1goldBeginTime = itemOne.goldBeginTime;
            string _item1latentEnergyCurStr = itemOne.latentEnergyCurStr;
            string _item1latentEnergyNewStr = itemOne.latentEnergyNewStr;
            DateTime _item1latentEnergyEndTime = itemOne.latentEnergyEndTime;

            if (tranHole)
            {
                itemOne.Hole1 = _item0Hole1;
                itemZero.Hole1 = _item1Hole1;
                itemOne.Hole2 = _item0Hole2;
                itemZero.Hole2 = _item1Hole2;
                itemOne.Hole3 = _item0Hole3;
                itemZero.Hole3 = _item1Hole3;
                itemOne.Hole4 = _item0Hole4;
                itemZero.Hole4 = _item1Hole4;
            }

            if (tranHoleFivSix)
            {
                itemOne.Hole5 = _item0Hole5;
                itemZero.Hole5 = _item1Hole5;
                itemOne.Hole6 = _item0Hole6;
                itemZero.Hole6 = _item1Hole6;
            }

            itemOne.Hole5Exp = _item0Hole5Exp;
            itemZero.Hole5Exp = _item1Hole5Exp;
            itemOne.Hole5Level = _item0Hole5Level;
            itemZero.Hole5Level = _item1Hole5Level;
            itemOne.Hole6Exp = _item0Hole6Exp;
            itemZero.Hole6Exp = _item1Hole6Exp;
            itemOne.Hole6Level = _item0Hole6Level;
            itemZero.Hole6Level = _item1Hole6Level;

            itemZero.StrengthenLevel = _item1StrengthenLevel;
            itemOne.StrengthenLevel = _item0StrengthenLevel;
            itemZero.StrengthenExp = _item1StrengthenExp;
            itemOne.StrengthenExp = _item0StrengthenExp;
            itemZero.AttackCompose = _item1AttackCompose;
            itemOne.AttackCompose = _item0AttackCompose;
            itemZero.DefendCompose = _item1DefendCompose;
            itemOne.DefendCompose = _item0DefendCompose;
            itemZero.LuckCompose = _item1LuckCompose;
            itemOne.LuckCompose = _item0LuckCompose;
            itemZero.AgilityCompose = _item1AgilityCompose;
            itemOne.AgilityCompose = _item0AgilityCompose;
            if (itemZero.IsBinds || itemOne.IsBinds)
            {
                itemOne.IsBinds = true;
                itemZero.IsBinds = true;
            }

            itemZero.goldBeginTime = _item1goldBeginTime;
            itemOne.goldBeginTime = _item0goldBeginTime;
            itemZero.goldValidDate = _item1goldValidDate;
            itemOne.goldValidDate = _item0goldValidDate;

            itemZero.latentEnergyCurStr = _item1latentEnergyCurStr;
            itemOne.latentEnergyCurStr = _item0latentEnergyCurStr;

            itemZero.latentEnergyNewStr = _item1latentEnergyNewStr;
            itemOne.latentEnergyNewStr = _item0latentEnergyNewStr;

            itemZero.latentEnergyEndTime = _item1latentEnergyEndTime;
            itemOne.latentEnergyEndTime = _item0latentEnergyEndTime;
        }

        public static int GetNecklaceMaxPlus(int lv)
        {
            StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(lv);
            if (strengThenExpInfo == null)
            {
                return 0;
            }
            return strengThenExpInfo.NecklaceStrengthPlus;
        }

        public static int GetNecklacePlus(int exp, int currentPlus)
        {
            foreach (StrengThenExpInfo current in StrengthenMgr._Strengthens_Exps.Values)
            {
                if (exp < current.NecklaceStrengthExp)
                {
                    int necklaceLevel = StrengthenMgr.GetNecklaceLevel(exp);
                    StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(necklaceLevel);
                    int result;
                    if (strengThenExpInfo == null)
                    {
                        result = currentPlus;
                        return result;
                    }
                    result = strengThenExpInfo.NecklaceStrengthPlus;
                    return result;
                }
            }
            return currentPlus;
        }

        public static int GetNecklaceLevel(int exp)
        {
            int maxLevel = 0;
            int maxExp = 0;

            // Tìm level cao nhất có exp <= exp hiện tại
            foreach (StrengThenExpInfo current in StrengthenMgr._Strengthens_Exps.Values)
            {
                if (current.NecklaceStrengthExp <= exp && current.NecklaceStrengthExp >= maxExp)
                {
                    maxExp = current.NecklaceStrengthExp;
                    maxLevel = current.Level;
                }
            }

            return maxLevel;
        }

        public static StrengThenExpInfo FindStrengthenExpInfo(int level)
        {
            StrengthenMgr.m_lock.AcquireReaderLock(10000);
            try
            {
                if (StrengthenMgr._Strengthens_Exps.ContainsKey(level))
                {
                    return StrengthenMgr._Strengthens_Exps[level];
                }
            }
            catch
            {
            }
            finally
            {
                StrengthenMgr.m_lock.ReleaseReaderLock();
            }
            return null;
        }
    }
}
