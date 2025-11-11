using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Game.Server
{
    public class GlobalConstants
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<eActive, ActiveStatusInfo> _activeStatus = new Dictionary<eActive, ActiveStatusInfo>();

        protected static Timer _globalTimer;

        private static eActive[] _activeArray;

        public static bool Start()
        {
            try
            {
                Array arr = Enum.GetValues(typeof(eActive));
                List<eActive> listCommand = new List<eActive>();
                for (int i = 0; i < arr.Length; i++)
                {
                    eActive type = (eActive)arr.GetValue(i);
                    if (listCommand.Contains(type) == false)
                    {
                        listCommand.Add(type);
                    }
                }

                _activeArray = listCommand.ToArray();
                LoadActive();

                return InitGlobalTimer();
            }
            catch(Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GlobalConstants", e);
                return false;
            }
        }

        private static bool SanXiaoTime()
        {
            DateTime end = Convert.ToDateTime(GameProperties.SanXiaoEndTime);
            return DateTime.Now.Date < end.Date;
        }

        private static bool QiYuanGameTime()
        {
            DateTime end = Convert.ToDateTime(GameProperties.QiYuanEndTime);
            return DateTime.Now.Date < end.Date;
        }

        public static void LoadActive()
        {
            try
            {
                foreach(eActive ea in _activeArray)
                {
                    ActiveStatusInfo info = new ActiveStatusInfo { Id = ea };
                    info.OnClose();
                    switch(ea)
                    {
                        case eActive.GodCardRaise:
                            info.State = SanXiaoTime() ? 1 : 0;
                            break;
                        case eActive.SanXiao:
                            info.State = SanXiaoTime() ? 1 : 0;
                            break;
                        case eActive.QiYuan:
                            info.State = QiYuanGameTime() ? 1 : 0;
                            break;
                    }
                    lock (_activeStatus)
                    {
                        if (_activeStatus.ContainsKey(ea) == false)
                        {
                            _activeStatus.Add(ea, info);
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Load active", e);
            }
        }

        public static void UpdateActive()
        {
            lock(_activeStatus)
            {
                foreach(eActive key in _activeStatus.Keys)
                {
                    ActiveStatusInfo asi = _activeStatus[key];
                    switch(asi.Id)
                    {
                        case eActive.GodCardRaise:
                            asi.State = SanXiaoTime() ? 1 : 0;
                            break;
                        case eActive.SanXiao:
                            asi.State = SanXiaoTime() ? 1 : 0;
                            break;
                        case eActive.QiYuan:
                            asi.State = QiYuanGameTime() ? 1 : 0;
                            break;
                    }
                }
            }
        }

        public static bool IsOpenActive(eActive type)
        {
            return GetActiveStatus(type).State == 1;
        }

        public static ActiveStatusInfo GetActiveStatus(eActive type)
        {
            lock (_activeStatus)
            {
                if (_activeStatus.ContainsKey(type))
                {
                    return _activeStatus[type];
                }
            }

            return new ActiveStatusInfo();
        }

        public static bool InitGlobalTimer()
        {
            int interval = 60 * 1000;
            if (_globalTimer == null)
            {
                _globalTimer = new Timer(GlobalTimerProc, null, interval, interval);
            }
            else
            {
                _globalTimer.Change(interval, interval);
            }

            return true;
        }

        public static void StopGlobalTimer()
        {
            if (_globalTimer != null)
            {
                _globalTimer.Dispose();
                _globalTimer = null;
            }
        }

        protected static void GlobalTimerProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if(log.IsErrorEnabled)
                {
                    log.Info("Global Proccess...");
                    log.Debug("Global ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                UpdateActive();
                lock(_activeStatus)
                {
                    foreach (ActiveStatusInfo asi in _activeStatus.Values)
                    {
                        bool OpenOrClose = asi.State == 1;
                        switch(asi.Id)
                        {
                            case eActive.GodCardRaise:
                                if(asi.OnChange())
                                {
                                    GamePlayer[] list = WorldMgr.GetAllPlayers();
                                    foreach(GamePlayer p in list)
                                    {
                                        p.Actives.SendGodCardRaiseOpenClose();
                                    }
                                    asi.OnSuccess(asi.State);
                                }
                                break;
                            case eActive.SanXiao:
                                if(asi.OnChange())
                                {
                                    GamePlayer[] list = WorldMgr.GetAllPlayers();
                                    foreach(GamePlayer p in list)
                                    {
                                        p.Actives.SendSanXiaoOpenClose();
                                    }
                                    asi.OnSuccess(asi.State);
                                }
                                break;
                            case eActive.QiYuan:
                                if (asi.OnChange())
                                {
                                    GamePlayer[] list = WorldMgr.GetAllPlayers();
                                    foreach (GamePlayer p in list)
                                    {
                                        p.Actives.SendDDTQiYuanOpenClose();
                                    }

                                    asi.OnSuccess(asi.State);
                                }

                                break;
                        }
                    }
                }
                Thread.CurrentThread.Priority = oldprio;
                if (log.IsInfoEnabled)
                {
                    log.Info("Global scan complete!");
                }
            }
            catch(Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error(" GlobalTimerProc", e);
            }
        }
    }
}