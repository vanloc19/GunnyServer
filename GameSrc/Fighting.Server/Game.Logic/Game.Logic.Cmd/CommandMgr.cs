using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Events;

namespace Game.Logic.Cmd
{
    public class CommandMgr
    {
        private static Dictionary<int, ICommandHandler> handles = new Dictionary<int, ICommandHandler>();

        public static ICommandHandler LoadCommandHandler(int code)
        {
            if (handles.ContainsKey(code))
                return handles[code];
            log.Error("LoadCommandHandler code030: " + code.ToString());
            return null;
        }

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            handles.Clear();
            SearchCommandHandlers(Assembly.GetAssembly(typeof(BaseGame)));
        }

        protected static int SearchCommandHandlers(Assembly assembly)
        {
            int count = 0;

            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Pick up a class
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Logic.Cmd.ICommandHandler") == null)
                    continue;

                GameCommandAttribute[] attr =
                    (GameCommandAttribute[])type.GetCustomAttributes(typeof(GameCommandAttribute), true);

                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as ICommandHandler);
                }
            }

            return count;
        }

        protected static void RegisterCommandHandler(int code, ICommandHandler handle)
        {
            handles.Add(code, handle);
        }
    }
}