using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Farm.Handle
{
    public class FarmHandleMgr
    {
        private Dictionary<int, IFarmCommandHadler> handles = new Dictionary<int, IFarmCommandHadler>();

        public IFarmCommandHadler LoadCommandHandler(int code)
        {
            if (handles.ContainsKey(code))
                return handles[code];
            log.Error("LoadCommandHandler code001: " + code.ToString());
            return null;
        }

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FarmHandleMgr()
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
                if (type.GetInterface("Game.Server.Farm.Handle.IFarmCommandHadler") == null)
                    continue;

                FarmHandleAttbute[] attr =
                    (FarmHandleAttbute[])type.GetCustomAttributes(typeof(FarmHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IFarmCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IFarmCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}