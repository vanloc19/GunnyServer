using System;

namespace Game.Server.RingStation.Action
{
	public class PlayerLoadingAction : BaseAction
	{
		private int m_loading;

		public PlayerLoadingAction(int state, int delay)
			: base(delay, 0)
		{
			m_loading = state;
		}

		protected override void ExecuteImp(RingStationGamePlayer player, long tick)
		{
			if (m_loading > 100)
			{
				m_loading = 100;
			}
			player.SendLoadingComplete(m_loading);
			if (m_loading < 100)
			{
				Random random = new Random();
				player.AddAction(new PlayerLoadingAction(m_loading + random.Next(20, 40), 1000));
			}
			Finish(tick);
		}
	}
}
