using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Game.Server.Managers
{
    public class FusionMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, FusionInfo> _fusions;

        private static System.Threading.ReaderWriterLock m_lock;

        private static ThreadSafeRandom rand;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<string, FusionInfo> tempFusions = new Dictionary<string, FusionInfo>();

                if (LoadFusion(tempFusions))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _fusions = tempFusions;
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
                    log.Error("FusionMgr", e);
                }
            }

            return false;
        }

        /// <summary>
        /// Initializes the BallMgr. 
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _fusions = new Dictionary<string, FusionInfo>();
                rand = new ThreadSafeRandom();
                return LoadFusion(_fusions);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("FusionMgr", e);
                }

                return false;
            }

        }

        private static bool LoadFusion(Dictionary<string, FusionInfo> fusion)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                FusionInfo[] infos = db.GetAllFusion();
                foreach (FusionInfo info in infos)
                {
                    List<int> list = new List<int>();
                    list.Add(info.Item1);
                    list.Add(info.Item2);
                    list.Add(info.Item3);
                    list.Add(info.Item4);
                    list.Sort();

                    StringBuilder items = new StringBuilder();
                    foreach (int i in list)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        items.Append(i);
                    }

                    string key = items.ToString();

                    if (!fusion.ContainsKey(key))
                    {
                        fusion.Add(key, info);
                    }
                }
            }

            return true;
        }

        public static ItemTemplateInfo Fusion(List<ItemInfo> Items, List<ItemInfo> AppendItems, ref bool isBind, ref bool result, ref bool IsTips)
        {
            List<int> list = new List<int>();
            int MaxLevel = 0;
            int TotalRate = 0;
            int FusionNeedRate = 0;
            if (Items == null)
            {
                return null;
            }
            ItemTemplateInfo returnItem = null;

            foreach (ItemInfo p in Items)
            {
                if (p != null)
                {
                    list.Add(p.Template.FusionType);

                    if (p.Template.Level > MaxLevel)
                    {
                        MaxLevel = p.Template.Level;
                    }

                    TotalRate += p.Template.FusionRate;
                    FusionNeedRate += p.Template.FusionNeedRate;
                    if (p.IsBinds)
                    {
                        isBind = true;
                    }
                }

            }

            foreach (ItemInfo p in AppendItems)
            {
                TotalRate += p.Template.FusionRate / 2;
                FusionNeedRate += p.Template.FusionNeedRate / 2;
                if (p.IsBinds)
                {
                    isBind = true;
                }
            }

            list.Sort();
            StringBuilder itemString = new StringBuilder();
            foreach (int i in list)
            {
                itemString.Append(i);
            }
            string key = itemString.ToString();
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_fusions.ContainsKey(key))
                {
                    FusionInfo info = _fusions[key];
                    double rateMax = 0;
                    double rateMin = 0;
                    IsTips = info.IsTips;
                    ItemTemplateInfo temp_0 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel);
                    ItemTemplateInfo temp_1 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 1);
                    ItemTemplateInfo temp_2 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 2);
                    List<ItemTemplateInfo> temps = new List<ItemTemplateInfo>();
                    if (temp_2 != null)
                    {
                        temps.Add(temp_2);
                    }
                    if (temp_1 != null)
                    {
                        temps.Add(temp_1);
                    }
                    if (temp_0 != null)
                    {
                        temps.Add(temp_0);
                    }
                    ItemTemplateInfo tempMax = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) <= 1.1).OrderByDescending(s => (TotalRate / (double)s.FusionNeedRate)).FirstOrDefault();
                    ItemTemplateInfo tempMin = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) > 1.1).OrderBy(s => TotalRate / (double)s.FusionNeedRate).FirstOrDefault();
                    if ((tempMax != null) && (tempMin == null))
                    {
                        //Console.WriteLine($"Step1");
                        returnItem = tempMax;
                        int i = rand.Next(tempMax.FusionNeedRate);
                        //Console.WriteLine($"Rate = {i} < {TotalRate}");
                        if (i < TotalRate)
                        {
                            //Console.WriteLine($"MAX | {returnItem.TemplateID} _ {tempMax.FusionNeedRate} _ {tempMax.FusionRate}");
                            //Console.WriteLine($"MIN | {returnItem.TemplateID} _ {tempMin.FusionNeedRate} _ {tempMin.FusionRate}");
                            //Console.WriteLine($"Final = {i} < {TotalRate}");
                            result = true;
                        }
                    }
                    if ((tempMax != null) && (tempMin != null))
                    {
                        if (tempMax.Level - tempMin.Level == 2)
                        {
                           // Console.WriteLine($"Step2.1");
                            rateMax = 100 * TotalRate * 0.6 / tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                        }
                        else
                        {
                            //Console.WriteLine($"Step2.2");
                            rateMax = 100 * TotalRate / (double)tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                        }

                        if (100 * TotalRate / (double)tempMax.FusionNeedRate > rand.Next(100))
                        {
                            //Console.WriteLine($"Step3.1");
                            returnItem = tempMax;
                            result = true;
                        }
                        else
                        {
                            //Console.WriteLine($"Step3.2");
                            returnItem = tempMin;
                            result = true;
                        }
                    }
                    if ((tempMax == null) && (tempMin != null))
                    {
                        returnItem = tempMin;
                        if (rand.Next(FusionNeedRate) < TotalRate)
                        {
                            result = true;
                        }
                        //Console.WriteLine($"Step4");
                    }
                    if (result)
                    {
                        foreach (ItemInfo p in Items)
                        {
                            if (p.Template.TemplateID == returnItem.TemplateID)
                            {
                                result = false;
                                break;
                            }
                        }
                    }

                    return returnItem;
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


        public static Dictionary<int, double> FusionPreview(List<ItemInfo> Items, List<ItemInfo> AppendItems, ref bool isBind)
        {
            List<int> list = new List<int>();
            int MaxLevel = 0;
            int TotalRate = 0;
            int FusionNeedRate = 0;
            Dictionary<int, double> Item_Rate = new Dictionary<int, double>();
            Item_Rate.Clear();

            foreach (ItemInfo p in Items)
            {
                list.Add(p.Template.FusionType);

                if (p.Template.Level > MaxLevel)
                {
                    MaxLevel = p.Template.Level;
                }
                TotalRate += p.Template.FusionRate;
                //Console.WriteLine($"A _ {p.Template.FusionRate} _ {TotalRate}");
                FusionNeedRate += p.Template.FusionNeedRate;

                if (p.IsBinds)
                {
                    isBind = true;
                }
            }

            foreach (ItemInfo p in AppendItems)
            {
                TotalRate += p.Template.FusionRate / 2;
                //Console.WriteLine($"B _ {p.Template.FusionRate / 2} _ {TotalRate}");
                FusionNeedRate += p.Template.FusionNeedRate / 2;

                if (p.IsBinds)
                {
                    isBind = true;
                }
            }

            list.Sort();
            StringBuilder itemString = new StringBuilder();
            foreach (int i in list)
            {
                itemString.Append(i);
            }

            string key = itemString.ToString();

            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_fusions.ContainsKey(key))
                {
                    FusionInfo info = _fusions[key];

                    double rateMax = 0;
                    double rateMin = 0;
                    ItemTemplateInfo temp_0 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel);
                    ItemTemplateInfo temp_1 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 1);
                    ItemTemplateInfo temp_2 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 2);
                    List<ItemTemplateInfo> temps = new List<ItemTemplateInfo>();
                    if (temp_2 != null)
                    {
                        temps.Add(temp_2);
                    }
                    if (temp_1 != null)
                    {
                        temps.Add(temp_1);
                    }
                    if (temp_0 != null)
                    {
                        temps.Add(temp_0);
                    }
                    ItemTemplateInfo tempMax = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) <= 1.1).OrderByDescending(s => (TotalRate / (double)s.FusionNeedRate)).FirstOrDefault();
                    ItemTemplateInfo tempMin = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) > 1.1).OrderBy(s => TotalRate / (double)s.FusionNeedRate).FirstOrDefault();
                    if ((tempMax != null) && (tempMin == null))
                    {
                        rateMax = (100 * TotalRate) / tempMax.FusionNeedRate;
                        //Console.WriteLine($"100 * {TotalRate} / {tempMax.FusionNeedRate}");
                        Item_Rate.Add(tempMax.TemplateID, rateMax);
                        //Console.WriteLine($"Step1 = {tempMax.TemplateID}_{rateMax}");
                    }
                    if ((tempMax != null) && (tempMin != null))
                    {
                        if (tempMax.Level - tempMin.Level == 2)
                        {
                            rateMax = 100 * TotalRate * 0.6 / tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                            //Console.WriteLine($"Step2.0 = {tempMax.TemplateID}_{rateMax}, {rateMin}");
                        }
                        else
                        {
                            rateMax = 100 * TotalRate / (double)tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                            //Console.WriteLine($"Step2.1 = {tempMax.TemplateID}_{rateMax}, {rateMin}");
                        }
                        Item_Rate.Add(tempMax.TemplateID, rateMax);
                        Item_Rate.Add(tempMin.TemplateID, rateMin);
                    }
                    if ((tempMax == null) && (tempMin != null))
                    {
                        rateMin = (100 * TotalRate) / FusionNeedRate;
                        Item_Rate.Add(tempMin.TemplateID, rateMin);
                        //Console.WriteLine($"Step3.0 = {tempMax.TemplateID}_{rateMax}");
                        //Console.WriteLine($"Step3.1 = {tempMin.TemplateID}_{rateMin}");
                    }
                    int[] templist = Item_Rate.Keys.ToArray();
                    foreach (int ID in templist)
                    {
                        foreach (ItemInfo p in Items)
                        {
                            if (ID == p.Template.TemplateID)
                            {
                                if (Item_Rate.ContainsKey(ID))
                                {
                                    Item_Rate.Remove(ID);
                                }

                            }
                        }
                    }
                    //Console.WriteLine($"Step4 = {Item_Rate}");
                    return Item_Rate;
                }
                else
                {
                    //Console.WriteLine($"Step5 = {Item_Rate}");
                    return Item_Rate;
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

        public static int[] NotAllow()
        {
            int[] items = new int[5];
            items[0] = 11023;
            items[1] = 11004;
            items[2] = 11008;
            items[3] = 11012;
            items[4] = 11016;
            return items;
        }

    }
}
