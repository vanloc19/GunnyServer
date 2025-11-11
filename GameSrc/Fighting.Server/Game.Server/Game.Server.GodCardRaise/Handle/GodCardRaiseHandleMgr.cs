using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.GodCardRaise.Handle
{
    public class GodCardRaiseHandleMgr
    {
        private Dictionary<int, IGodCardRaiseCommandHadler> handles = new Dictionary<int, IGodCardRaiseCommandHadler>();

        public IGodCardRaiseCommandHadler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        public GodCardRaiseHandleMgr()
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
                if (type.GetInterface("Game.Server.GodCardRaise.Handle.IGodCardRaiseCommandHadler") == null)
                    continue;

                GodCardRaiseHandleAttbute[] attr =
                    (GodCardRaiseHandleAttbute[])type.GetCustomAttributes(typeof(GodCardRaiseHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IGodCardRaiseCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IGodCardRaiseCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}