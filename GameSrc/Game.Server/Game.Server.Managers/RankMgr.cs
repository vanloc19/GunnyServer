using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
    public class RankMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static System.Threading.ReaderWriterLock m_lock = new ReaderWriterLock();

        private static Dictionary<int, UserMatchInfo> _matchs;

        private static Dictionary<int, UserRankDateInfo> _newRanks;

        protected static Timer _timer;

        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _matchs = new Dictionary<int, UserMatchInfo>();
                _newRanks = new Dictionary<int, UserRankDateInfo>();
                BeginTimer();
                return ReLoad();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("RankMgr", e);
                return false;
            }
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, UserMatchInfo> tempMatchs = new Dictionary<int, UserMatchInfo>();
                Dictionary<int, UserRankDateInfo> tempNewRanks = new Dictionary<int, UserRankDateInfo>();
                if (LoadData(tempMatchs, tempNewRanks))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _matchs = tempMatchs;
                        _newRanks = tempNewRanks;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("RankMgr", e);
            }

            return false;
        }

        public static UserMatchInfo FindRank(int UserID)
        {
            if (_matchs.ContainsKey(UserID))
                return _matchs[UserID];
            return null;
        }

        public static UserRankDateInfo FindRankDate(int UserID)
        {
            if (_newRanks.ContainsKey(UserID))
                return _newRanks[UserID];
            return null;
        }

        private static bool LoadData(Dictionary<int, UserMatchInfo> Match, Dictionary<int, UserRankDateInfo> NewRanks)
        {
            using (PlayerBussiness db = new PlayerBussiness())
            {
                db.UpdateRank();
                UserMatchInfo[] infos = db.GetAllUserMatchInfo();
                foreach (UserMatchInfo info in infos)
                {
                    if (!Match.ContainsKey(info.UserID))
                    {
                        Match.Add(info.UserID, info);
                    }
                }

                UserRankDateInfo[] ranks = db.GetAllUserRankDate();
                foreach (UserRankDateInfo info in ranks)
                {
                    if (!NewRanks.ContainsKey(info.UserID))
                    {
                        NewRanks.Add(info.UserID, info);
                    }
                }
            }

            return true;
        }

        public static void BeginTimer()
        {
            int interval = 60 * 60 * 1000;
            if (_timer == null)
            {
                _timer = new Timer(new TimerCallback(TimeCheck), null, interval, interval);
            }
            else
            {
                _timer.Change(interval, interval);
            }
        }

        protected static void TimeCheck(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                //some code  
                ReLoad();
                //end code
                Thread.CurrentThread.Priority = oldprio;
                startTick = Environment.TickCount - startTick;
            }
            catch (Exception e)
            {
                Console.WriteLine("TimeCheck Rank: " + e);
            }
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}