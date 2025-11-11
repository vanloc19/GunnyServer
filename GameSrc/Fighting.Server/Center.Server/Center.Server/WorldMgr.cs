using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Bussiness;
using Bussiness.CenterService;
using Bussiness.Managers;
using Center.Server.Managers;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server
{
    public class WorldMgr
    {

        public static bool IsLeagueOpen;

        public static DateTime LeagueOpenTime;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<string> NotceList = new List<string>();

        private static Dictionary<string, RankingPersonInfo> m_rankList;

        public static bool fightOver;

        public static bool roomClose;

        public static long current_blood = 0;

        public static readonly long MAX_BLOOD = 2100000000;

        public static int currentPVE_ID;

        public static string[] name = new string[] { "Cuồng Long", "Bá Tước", "Bá Tước", "Đội Trưởng" };

        public static string[] bossResourceId = new string[] { "1", "2", "2", "4" };

        public static int[] Pve_Id = new int[] { 1243, 30001, 30002, 30004 };

        public static DateTime begin_time;

        public static DateTime end_time;

        public static int fight_time;

        public static bool worldOpen;

        private static readonly int worldbossTime = 60;

        private static ReaderWriterLock m_lock;
        private static string SystemNoticeFile => ConfigurationManager.AppSettings["SystemNoticePath"];

        public static bool LoadNotice(string path)
        {
            string str = path + SystemNoticeFile;
            if (!File.Exists(str))
            {
                log.Error("SystemNotice file : " + str + " not found !");
            }
            else
            {
                try
                {
                    foreach (XElement element in XDocument.Load(str).Root.Nodes())
                    {
                        try
                        {
                            int.Parse(element.Attribute("id").Value);
                            string item = element.Attribute("notice").Value;
                            NotceList.Add(item);
                        }
                        catch (Exception exception)
                        {
                            log.Error("BattleMgr setup error:", exception);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    log.Error("BattleMgr setup error:", exception2);
                }
            }
            log.InfoFormat("Total {0} syterm notice loaded.", NotceList.Count);
            return true;
        }

        public static bool Start()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                m_rankList = new Dictionary<string, RankingPersonInfo>();
                current_blood = MAX_BLOOD;
                begin_time = DateTime.Now;
                end_time = begin_time.AddDays(1.0);
                fightOver = true;
                roomClose = true;
                worldOpen = false;
                LeagueOpenTime = DateTime.Now;
                IsLeagueOpen = false;
                return LoadNotice("");
            }
            catch (Exception exception)
            {
                log.ErrorFormat("Load server list from db failed:{0}", exception);
                return false;
            }
        }

        public static void SetupWorldBoss(int id)
        {
            current_blood = MAX_BLOOD;
            begin_time = DateTime.Now;
            end_time = begin_time.AddDays(1.0);
            fight_time = worldbossTime - begin_time.Minute;
            fightOver = false;
            roomClose = false;
            currentPVE_ID = id;
            worldOpen = true;
        }

        public static void WorldBossClearRank()
        {
            WorldMgr.m_rankList.Clear();
        }

        public static void UpdateRank(int damage, int honor, string nickName)
        {
            if (WorldMgr.m_rankList.Keys.Contains(nickName))
            {
                WorldMgr.m_rankList[nickName].Damage += damage;
                WorldMgr.m_rankList[nickName].Honor += honor;
                return;
            }
            RankingPersonInfo rankingPersonInfo = new RankingPersonInfo();
            rankingPersonInfo.ID = WorldMgr.m_rankList.Count + 1;
            rankingPersonInfo.Name = nickName;
            rankingPersonInfo.Damage = damage;
            rankingPersonInfo.Honor = honor;
            WorldMgr.m_rankList.Add(nickName, rankingPersonInfo);
        }

        public static void WorldBossFightOver()
        {
            fightOver = true;
        }

        public static void WorldBossRoomClose()
        {
            roomClose = true;
        }

        public static void UpdateFightTime()
        {
            if (!WorldMgr.fightOver)
            {
                WorldMgr.fight_time = WorldMgr.worldbossTime - WorldMgr.begin_time.Minute;
            }
        }

        public static void WorldBossClose()
        {
            worldOpen = false;
        }

        public static void ReduceBlood(int value)
        {
            if (current_blood > 0L)
            {
                current_blood -= (long)value;
            }
        }


        public static List<RankingPersonInfo> SelectTopTen()
        {
            List<RankingPersonInfo> list = new List<RankingPersonInfo>();
            IOrderedEnumerable<KeyValuePair<string, RankingPersonInfo>> orderedEnumerable = from pair in WorldMgr.m_rankList orderby pair.Value.Damage descending select pair;
            foreach (KeyValuePair<string, RankingPersonInfo> current in orderedEnumerable)
            {
                if (list.Count == 10)
                {
                    break;
                }
                list.Add(current.Value);
            }
            return list;
        }

        public static bool CheckName(string NickName)
        {
            return WorldMgr.m_rankList.Keys.Contains(NickName);
        }

        public static RankingPersonInfo GetSingleRank(string name)
        {
            return WorldMgr.m_rankList[name];
        }
    }
}
