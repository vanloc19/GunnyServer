using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Consortia.Handle
{
	public class ConsortiaHandleMgr
	{
		private Dictionary<int, IConsortiaCommandHadler> dictionary_0;

		public IConsortiaCommandHadler LoadCommandHandler(int code)
		{
			return dictionary_0[code];
		}

		public ConsortiaHandleMgr()
		{
			dictionary_0 = new Dictionary<int, IConsortiaCommandHadler>();
			dictionary_0.Clear();
			SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
		}

		protected int SearchCommandHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && !(type.GetInterface("Game.Server.Consortia.Handle.IConsortiaCommandHadler") == null))
				{
					global::Consortia[] customAttributes = (global::Consortia[])type.GetCustomAttributes(typeof(global::Consortia), inherit: true);
					if (customAttributes.Length != 0)
					{
						num++;
						RegisterCommandHandler(customAttributes[0].method_0(), Activator.CreateInstance(type) as IConsortiaCommandHadler);
					}
				}
			}
			return num;
		}

		protected void RegisterCommandHandler(int code, IConsortiaCommandHadler handle)
		{
			dictionary_0.Add(code, handle);
		}
	}
}
