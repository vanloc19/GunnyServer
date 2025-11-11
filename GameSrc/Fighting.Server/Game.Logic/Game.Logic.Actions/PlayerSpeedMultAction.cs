using System.Drawing;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
	public class PlayerSpeedMultAction : BaseAction
	{
		private Point point_0;

		private Player player_0;

		private bool bool_0;

		public PlayerSpeedMultAction(Player player, Point target, int delay)
			: base(0, delay)
		{
			player_0 = player;
			point_0 = target;
			bool_0 = false;
		}

		protected override void ExecuteImp(BaseGame game, long tick)
		{
			int int_4 = 4;
			if (!bool_0)
			{
				bool_0 = true;
				game.SendPlayerMove(player_0, int_4, point_0.X, point_0.Y, (byte)((point_0.X > player_0.X) ? 1 : byte.MaxValue), player_0.IsLiving);
			}
			if (player_0.Distance(point_0) > (double)player_0.StepX && point_0.X != player_0.X)
			{
				player_0.Direction = ((point_0.X > player_0.X) ? 1 : (-1));
				Point point2 = player_0.getNextWalkPoint(player_0.Direction);
				if (point2 == Point.Empty)
				{
					int num = player_0.X + player_0.Direction * player_0.MOVE_SPEED;
					point2 = player_0.FindYLineNotEmptyPointDown(num, player_0.Y - player_0.StepY);
					if (point2 == Point.Empty)
					{
						int_4 = 1;
						point2 = new Point(num, point_0.Y);
					}
				}
				player_0.SetXY(point2.X, point2.Y);
				if ((player_0.Direction > 0 && point2.X >= point_0.X) || (player_0.Direction < 0 && point2.X <= point_0.X) || int_4 == 1)
				{
					player_0.StartMoving();
					Finish(tick);
				}
			}
			else
			{
				Finish(tick);
			}
		}
	}
}
