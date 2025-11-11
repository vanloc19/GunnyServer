using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.LittleGame.Handle
{
    public class LittleGameHandleMgr
    {
        private Dictionary<int, ILittleGameCommandHandler> handlers;

        public LittleGameHandleMgr()
        {
            this.handlers = new Dictionary<int, ILittleGameCommandHandler>();
            this.handlers.Clear();
            this.SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        public ILittleGameCommandHandler LoadCommandHandler(int code)
        {
            return this.handlers[code];
        }

        protected void RegisterCommandHandler(int code, ILittleGameCommandHandler handle)
        {
            this.handlers.Add(code, handle);
        }

        protected int SearchCommandHandlers(Assembly assembly)
        {
            int num = 0;
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (type.IsClass && !(type.GetInterface("Game.Server.LittleGame.Handle.ILittleGameCommandHandler") == null))
                {
                    LittleGameAttribute[] customAttributes = (LittleGameAttribute[])type.GetCustomAttributes(typeof(LittleGameAttribute), true);
                    if (customAttributes.Length != 0)
                    {
                        num++;
                        this.RegisterCommandHandler(customAttributes[0].GetByteCode(), Activator.CreateInstance(type) as ILittleGameCommandHandler);
                    }
                }
            }
            return num;
        }
    }
}
