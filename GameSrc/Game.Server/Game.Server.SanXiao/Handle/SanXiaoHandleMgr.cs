using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.SanXiao.Handle
{
    public class SanXiaoHandleMgr
    {
        private Dictionary<int, ISanXiaoCommandHadler> handles = new Dictionary<int, ISanXiaoCommandHadler>();

        public ISanXiaoCommandHadler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        public SanXiaoHandleMgr()
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
                if (type.GetInterface("Game.Server.SanXiao.Handle.ISanXiaoCommandHadler") == null)
                    continue;

                SanXiaoHandleAttbute[] attr =
                    (SanXiaoHandleAttbute[])type.GetCustomAttributes(typeof(SanXiaoHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as ISanXiaoCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, ISanXiaoCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}