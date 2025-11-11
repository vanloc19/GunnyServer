using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.AvatarCollection.Handle
{
    public class AvatarCollectionHandleMgr
    {
        private Dictionary<int, IAvatarCollectionCommandHadler> handles =
            new Dictionary<int, IAvatarCollectionCommandHadler>();

        public IAvatarCollectionCommandHadler LoadCommandHandler(int code)
        {
            if (handles.ContainsKey(code))
                return handles[code];
            log.Error("LoadCommandHandler code004: " + code.ToString());
            return null;
        }

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AvatarCollectionHandleMgr()
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
                if (type.GetInterface("Game.Server.AvatarCollection.Handle.IAvatarCollectionCommandHadler") == null)
                    continue;

                AvatarCollectionHandleAttbute[] attr =
                    (AvatarCollectionHandleAttbute[])type.GetCustomAttributes(typeof(AvatarCollectionHandleAttbute),
                        true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code,
                        Activator.CreateInstance(type) as IAvatarCollectionCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IAvatarCollectionCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}