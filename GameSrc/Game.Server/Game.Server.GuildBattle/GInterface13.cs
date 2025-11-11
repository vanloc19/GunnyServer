namespace Game.Server.GuildBattle
{
	public interface GInterface13
	{
		void Execute(GuildBattleMgr battle, long tick);

		bool IsFinished(long tick);
	}
}