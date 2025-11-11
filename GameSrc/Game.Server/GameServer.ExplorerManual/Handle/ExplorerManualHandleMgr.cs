using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.ExplorerManual.Handle
{
    public class ExplorerManualHandleMgr
    {
        private Dictionary<int, IExplorerManualCommandHadler> handles = new Dictionary<int, IExplorerManualCommandHadler>();

        public IExplorerManualCommandHadler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        public ExplorerManualHandleMgr()
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
                if (type.GetInterface("Game.Server.ExplorerManual.Handle.IExplorerManualCommandHadler") == null)
                    continue;

                ExplorerManualHandleAttbute[] attr =
                    (ExplorerManualHandleAttbute[])type.GetCustomAttributes(typeof(ExplorerManualHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IExplorerManualCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IExplorerManualCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}