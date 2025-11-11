using Game.Logic;

namespace Game.Server.GuildBattle
{
	public class BaseGuildBattleAction : GInterface13
	{
		private long long_0;

		private long long_1;

		private long long_2;

		public BaseGuildBattleAction(int delay) : this(delay, 0)
		{
		}

		public BaseGuildBattleAction(int delay, int finishDelay)
		{


			this.long_0 = TickHelper.GetTickCount() + (long)delay;
			this.long_1 = (long)finishDelay;
			this.long_2 = 9223372036854775807L;
		}

		public void Execute(GuildBattleMgr battle, long tick)
		{
			if (this.long_0 <= tick && this.long_2 == 9223372036854775807L)
			{
				this.ExecuteImp(battle, tick);
			}
		}

		protected virtual void ExecuteImp(GuildBattleMgr battle, long tick)
		{
			this.Finish(tick);
		}

		public void Finish(long tick)
		{
			this.long_2 = tick + this.long_1;
		}

		public bool IsFinished(long tick)
		{
			return this.long_2 <= tick;
		}
	}
}