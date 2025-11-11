using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.Pet.Handle
{
	public class PetHandleMgr
	{
		private Dictionary<int, IPetCommandHadler> dictionary_0;

		private static readonly ILog ilog_0;

		public IPetCommandHadler LoadCommandHandler(int code)
		{
			if (dictionary_0.ContainsKey(code))
			{
				return dictionary_0[code];
			}
			ilog_0.Error("LoadCommandHandler code024: " + code);
			return null;
		}

		public PetHandleMgr()
		{
			dictionary_0 = new Dictionary<int, IPetCommandHadler>();
			dictionary_0.Clear();
			SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
		}

		protected int SearchCommandHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && !(type.GetInterface("Game.Server.Pet.Handle.IPetCommandHadler") == null))
				{
					global::Pet[] customAttributes = (global::Pet[])type.GetCustomAttributes(typeof(global::Pet), inherit: true);
					if (customAttributes.Length != 0)
					{
						num++;
						RegisterCommandHandler(customAttributes[0].method_0(), Activator.CreateInstance(type) as IPetCommandHadler);
					}
				}
			}
			return num;
		}

		protected void RegisterCommandHandler(int code, IPetCommandHadler handle)
		{
			dictionary_0.Add(code, handle);
		}

		static PetHandleMgr()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
