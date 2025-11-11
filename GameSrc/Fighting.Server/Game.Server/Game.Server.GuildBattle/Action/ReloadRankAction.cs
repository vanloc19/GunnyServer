using Game.Logic;
using Game.Server.GuildBattle;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Game.Server.GuildBattle.Action
{
	public class ReloadRankAction : GInterface13
	{
		private readonly static ILog ilog_0;

		private long long_0;

		private bool bool_0;

		static ReloadRankAction()
		{

			ReloadRankAction.ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public ReloadRankAction(int delay)
		{


			this.bool_0 = false;
			this.long_0 = this.long_0 + TickHelper.GetTickCount() + (long)delay;
		}

		public void Execute(GuildBattleMgr game, long tick)
		{
			int num = 1;
			GuildBattleConsortiaInfo[] array = (
				from a in (IEnumerable<GuildBattleConsortiaInfo>)game.GetAllConsortia()
				orderby a.Score descending
				select a).ToArray<GuildBattleConsortiaInfo>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].Rank = num;
				num++;
			}
			this.bool_0 = true;
		}

		public bool IsFinished(long tick)
		{
			return this.bool_0;
		}
	}
}