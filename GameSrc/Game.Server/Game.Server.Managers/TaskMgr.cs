using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Server.LittleGame;
using Game.Server.LittleGame.Data;
using log4net;

namespace Game.Server.Managers
{
    public static class TaskMgr
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static bool Init()
        {
            try
            {
                TaskScheduler.Instance.ScheduleTask(
                    "LittleGame",
                    (GameProperties.LittleGameStartHourse, GameProperties.LittleGameStartHourse + GameProperties.LittleGameTimeSpending),
                    (0, 0),
                    new List<DayOfWeek>
                    {
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday,
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                    },
                    (LittleGameWorldMgr.OpenLittleGame, LittleGameWorldMgr.CloseLittleGame));
                if (Log.IsInfoEnabled)
                    Log.Info("LittleGame timer initialized!");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}