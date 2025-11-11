using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Game.Base;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
    public class SpiritInfoMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static List<SpiritInfo> _spiritList;

        public static int MAX_LEVEL = 0;

        public static bool Init()
        {
            try
            {
                _spiritList = new List<SpiritInfo>();
                return ReLoad();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("LevelMgr", e);
                return false;
            }
        }

        public static bool ReLoad()
        {
            try
            {
                List<SpiritInfo> tempDatas = new List<SpiritInfo>();
                if (LoadSpirits(tempDatas))
                {
                    try
                    {
                        _spiritList = tempDatas;
                        return true;
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("LevelMgr", e);
            }

            return false;
        }

        private static bool LoadSpirits(List<SpiritInfo> datas)
        {
            try
            {
                XmlDocument xDoc = Marshal.LoadXMLData("spiritinfolist", true);
                XmlNodeList xNodeList = xDoc.SelectNodes("Result/child::node()");

                foreach (XmlNode xNode in xNodeList)
                {
                    int level = int.Parse(xNode.Attributes["Level"].Value);

                    SpiritInfo info = new SpiritInfo();
                    info.Level = level;
                    info.MustGetTimes = int.Parse(xNode.Attributes["MustGetTimes"].Value);
                    info.BaseSuccessPro = int.Parse(xNode.Attributes["BaseSuccessPro"].Value);
                    info.RefrenceValue = int.Parse(xNode.Attributes["RefrenceValue"].Value);
                    info.SkillId = int.Parse(xNode.Attributes["SkillId"].Value);
                    info.AttackAdd = int.Parse(xNode.Attributes["AttackAdd"].Value);
                    info.LuckAdd = int.Parse(xNode.Attributes["LuckAdd"].Value);
                    info.DefendAdd = int.Parse(xNode.Attributes["DefendAdd"].Value);
                    info.AgilityAdd = int.Parse(xNode.Attributes["AgilityAdd"].Value);
                    info.BagType = int.Parse(xNode.Attributes["BagType"].Value);
                    info.BagPlace = int.Parse(xNode.Attributes["BagPlace"].Value);
                    info.CategoryId = int.Parse(xNode.Attributes["CategoryId"].Value);
                    datas.Add(info);

                    if (MAX_LEVEL < level)
                        MAX_LEVEL = level;
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return false;
        }

        public static List<SpiritInfo> GetSpirit(int categoryId)
        {
            List<SpiritInfo> lists = new List<SpiritInfo>();
            lock (_spiritList)
            {
                foreach (SpiritInfo info in _spiritList)
                {
                    if (info.CategoryId == categoryId)
                    {
                        lists.Add(info);
                    }
                }
            }

            if (lists.Count > 0)
            {
                lists = lists.OrderBy(a => a.Level).ToList();
            }

            return lists;
        }

        public static SpiritInfo GetSingleSpirit(int bagType, int place, int level)
        {
            lock (_spiritList)
            {
                foreach (SpiritInfo info in _spiritList)
                {
                    if (info.BagType == bagType && info.BagPlace == place && info.Level == level)
                    {
                        return info;
                    }
                }
            }

            return null;
        }
    }
}