using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
	public class GameCommandMgr
	{
		private Dictionary<int, IGameCommandHandler> handles = new Dictionary<int, IGameCommandHandler>();

		public GameCommandMgr()
		{
			handles.Clear();
			SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
		}

		public IGameCommandHandler LoadCommandHandler(int code)
		{
			return handles[code];
		}

		protected void RegisterCommandHandler(int code, IGameCommandHandler handle)
		{
			handles.Add(code, handle);
		}

		protected int SearchCommandHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && type.GetInterface("Game.Server.RingStation.RoomGamePkg.TankHandle.IGameCommandHandler") != null)
				{
					GameCommandAttbute[] customAttributes = (GameCommandAttbute[])type.GetCustomAttributes(typeof(GameCommandAttbute), inherit: true);
					if (customAttributes.Length != 0)
					{
						num++;
						RegisterCommandHandler(customAttributes[0].Code, Activator.CreateInstance(type) as IGameCommandHandler);
					}
				}
			}
			return num;
		}
	}
}
