using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.DDTQiYuan.Handle
{
    public class QiYuanHandleMgr
    {
        private Dictionary<int, IQiYuanCommandHadler> handles = new Dictionary<int, IQiYuanCommandHadler>();

        public IQiYuanCommandHadler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        public QiYuanHandleMgr()
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
                if (type.GetInterface("Game.Server.DDTQiYuan.Handle.IQiYuanCommandHadler") == null)
                    continue;

                QiYuanHandleAttbute[] attr =
                    (QiYuanHandleAttbute[])type.GetCustomAttributes(typeof(QiYuanHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IQiYuanCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IQiYuanCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}