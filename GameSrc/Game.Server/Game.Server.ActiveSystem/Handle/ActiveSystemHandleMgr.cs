using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.ActiveSystem.Handle
{
    public class ActiveSystemHandleMgr
    {
        private Dictionary<int, IActiveSystemCommandHadler> handles = new Dictionary<int, IActiveSystemCommandHadler>();

        public IActiveSystemCommandHadler LoadCommandHandler(int code)
        {
            if (handles.ContainsKey(code))
                return handles[code];
            log.Error("LoadCommandHandler code002: " + code.ToString());
            return null;
        }

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActiveSystemHandleMgr()
        {
            handles.Clear();
            SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        protected int SearchCommandHandlers(Assembly assembly)
        {
            int count = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Server.ActiveSystem.Handle.IActiveSystemCommandHadler") == null)
                    continue;

                ActiveSystemHandleAttbute[] attr =
                    (ActiveSystemHandleAttbute[])type.GetCustomAttributes(typeof(ActiveSystemHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IActiveSystemCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IActiveSystemCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}