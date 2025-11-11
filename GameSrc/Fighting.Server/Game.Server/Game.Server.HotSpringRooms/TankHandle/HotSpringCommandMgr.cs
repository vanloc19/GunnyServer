using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.HotSpringRooms.TankHandle
{
	public class HotSpringCommandMgr
	{
		private Dictionary<int, IHotSpringCommandHandler> dictionary_0 = new Dictionary<int, IHotSpringCommandHandler>();

		public HotSpringCommandMgr()
		{
			dictionary_0.Clear();
			SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
		}

		public IHotSpringCommandHandler LoadCommandHandler(int code)
		{
			return dictionary_0[code];
		}

		protected void RegisterCommandHandler(int code, IHotSpringCommandHandler handle)
		{
			dictionary_0.Add(code, handle);
		}

		protected int SearchCommandHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && type.GetInterface("Game.Server.HotSpringRooms.TankHandle.IHotSpringCommandHandler") != null)
				{
					HotSpringCommandAttbute[] customAttributes = (HotSpringCommandAttbute[])type.GetCustomAttributes(typeof(HotSpringCommandAttbute), inherit: true);
					if (customAttributes.Length != 0)
					{
						num++;
						RegisterCommandHandler(customAttributes[0].Code, Activator.CreateInstance(type) as IHotSpringCommandHandler);
					}
				}
			}
			return num;
		}
	}
}
