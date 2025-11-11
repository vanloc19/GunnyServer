using System.Reflection;
using log4net;

namespace Game.Logic.Actions
{
	public class CheckPVPGameStateAction : IAction
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private bool m_isFinished;

		private long m_tick;

		public CheckPVPGameStateAction(int delay)
		{
			m_tick += TickHelper.GetTickCount() + delay;
		}

		public void Execute(BaseGame game, long tick)
		{
			if (m_tick > tick)
			{
				return;
			}
			PVPGame game2 = game as PVPGame;
			if (game2 != null)
			{
				switch (game.GameState)
				{
					case eGameState.Inited:
						game2.Prepare();
						break;
					case eGameState.Prepared:
						game2.StartLoading();
						break;
					case eGameState.Loading:
						if (game2.IsAllComplete())
						{
							game2.StartGame();
						}
						break;
					case eGameState.Playing:
						if (game2.CurrentPlayer == null || !game2.CurrentPlayer.IsAttacking)
						{
							if (game2.TurnIndex >= 100 && game2.RoomType == eRoomType.Match)
							{
								game2.GameOver();
							}
							if (game2.CanGameOver())
							{
								game2.GameOver();
							}
							else
							{
								game2.NextTurn();
							}
						}
						break;
					case eGameState.GameOver:
						game2.Stop();
						break;
				}
			}
			m_isFinished = true;
		}

		public bool IsFinished(long tick)
		{
			return m_isFinished;
		}
	}
}
