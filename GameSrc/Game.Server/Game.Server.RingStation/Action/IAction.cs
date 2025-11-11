namespace Game.Server.RingStation.Action
{
	public interface IAction
	{
		void Execute(RingStationGamePlayer player, long tick);

		bool IsFinished(long tick);
	}
}
