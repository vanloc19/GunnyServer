using Bussiness;
using Game.Server.GameObjects;
using log4net;
using System;

namespace Game.Server.Managers
{
    class ScheduleMgr
    {
        private static ServiceBussiness serviceBussiness = new ServiceBussiness();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static System.Threading.Timer m_UpdateSchedule;


        public static bool InitGlobalTimers()
        {
            UpdateActivites();
            int time = Convert.ToInt32((DateTime.Now - DateTime.Today).TotalMilliseconds);
            time = 86400000 - time;
            if(m_UpdateSchedule == null)
            {
                m_UpdateSchedule = new System.Threading.Timer(new System.Threading.TimerCallback(ScanSchedulProc), null, time, 86400000);
            }
            else
            {
                m_UpdateSchedule.Change(time, 86400000);
            }
            return true;
        }

        protected static void ScanSchedulProc(object sender)
        {
            System.Threading.ThreadPriority priority = System.Threading.Thread.CurrentThread.Priority;
            try
            {
                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                int num = System.Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan ScheduleMgr ...");
                    log.Debug("Scan ThreadId=" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                num = System.Environment.TickCount - num;
                try
                {
                    ScheduleMgr.UpdateActivites();
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("UpdateActivites error!", e);
                    }
                }
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan ScheduleMgr complete!");
                }
            }
            catch (System.Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Scan ScheduleMgr Proc", exception);
                }
            }
            finally
            {
                System.Threading.Thread.CurrentThread.Priority = priority;
            }
        }

        private static void UpdateActivites()
        {
            UpdateActiveProperties();
            if (DateTime.Parse(GameProperties.HappyRechargeEndDate) < DateTime.Now)
                ResetHappyRecharge();
        }

        private static void UpdateActiveProperties()
        {
            if (DateTime.Parse(GameProperties.HappyRechargeEndDate) < DateTime.Now)
            {
                GameProperties.HappyRechargeOpenClose = false;
            }
            switch(DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if(DateTime.Parse(GameProperties.HappyRechargeEndDate) < DateTime.Now)
                    {
                        GameProperties.HappyRechargeBeginDate = DateTime.Today.ToString();
                        GameProperties.HappyRechargeEndDate = DateTime.Today.AddDays(3).ToString();
                        GameProperties.HappyRechargeOpenClose = true;
                        serviceBussiness.UpdateServerPropertyByKey("HappyRechargeBeginDate", GameProperties.HappyRechargeBeginDate.ToString());
                        serviceBussiness.UpdateServerPropertyByKey("HappyRechargeEndDate", GameProperties.HappyRechargeEndDate.ToString());
                        serviceBussiness.UpdateServerPropertyByKey("HappyRechargeOpenClose", GameProperties.HappyRechargeOpenClose.ToString());
                    }
                    break;
                case DayOfWeek.Tuesday:
                    break;
                case DayOfWeek.Wednesday:
                    break;
                case DayOfWeek.Thursday:
                    break;
                case DayOfWeek.Friday:
                    break;
                case DayOfWeek.Saturday:
                    break;
                case DayOfWeek.Sunday:
                    break;
            }
        }

        private static void ResetHappyRecharge()
        {
            GamePlayer[] allplayer = WorldMgr.GetAllPlayers();
            for (int i = 0; i < allplayer.Length; i++)
            {
                allplayer[i].Out.SendOpenHappyRecharge(allplayer[i].PlayerId);
            }
        }
    }
}