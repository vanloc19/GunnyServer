using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.GameRoom.Handle
{
    public class GameRoomHandleMgr
    {
        private Dictionary<int, IGameRoomCommandHadler> handles = new Dictionary<int, IGameRoomCommandHadler>();

        public IGameRoomCommandHadler LoadCommandHandler(int code)
        {
            if (handles.ContainsKey(code))
            {
                return handles[code];
            }
            log.Error("LoadCommandHandler code009: " + code.ToString());
            return null;
        }

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GameRoomHandleMgr()
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
                {
                    continue;
                }

                if (type.GetInterface("Game.Server.GameRoom.Handle.IGameRoomCommandHadler") == null)
                {
                    continue;
                }

                GameRoomHandleAttbute[] attr =
                    (GameRoomHandleAttbute[])type.GetCustomAttributes(typeof(GameRoomHandleAttbute), true);
                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IGameRoomCommandHadler);
                }
            }

            return count;
        }

        protected void RegisterCommandHandler(int code, IGameRoomCommandHadler handle)
        {
            handles.Add(code, handle);
        }
    }
}