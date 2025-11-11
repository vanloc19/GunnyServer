using Game.Logic;
using Game.Logic.AI;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
	public class SixNormalSecondNpc : ABrain
	{
		private int int_0;

		private int int_1;

		private List<Point> list_0;

		private List<Point> list_1;

		private int int_2;

		private bool bool_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (base.Body.Blood != 1)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
				base.Body.PlayMovie("stand", 0, 0);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			if (base.Body.Blood == 1)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
				base.Body.PlayMovie("standB", 0, 0);
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			int_1 = 1;
			int_2 = base.Body.Config.MaxStepMove;
			base.Body.Config.CompleteStep = false;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int_0++;
			if (bool_0)
			{
				int_2 = base.Body.Config.FirstStepMove;
			}
			bool_0 = false;
			if (base.Body.Blood > 1)
			{
				method_0();
			}
		}

		private void method_0()
		{
			if (int_1 >= list_0.Count)
			{
				if ((base.Game as PVEGame).CountMosterPlace < list_1.Count && !base.Body.Config.CompleteStep)
				{
					Point point = list_1[(base.Game as PVEGame).CountMosterPlace];
					(base.Game as PVEGame).CountMosterPlace++;
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "happy");
					base.Body.BoltMove(point.X, point.Y, 0);
					base.Body.PlayMovie("happy", 0, 0);
					base.Body.Config.CompleteStep = true;
				}
				return;
			}
			if (int_1 == list_0.Count - 1)
			{
				int_2++;
			}
			Point point2 = list_0[int_1];
			string action = "walk";
			if (point2.X == base.Body.X && (point2.Y == 920 || point2.Y == 760))
			{
				action = "flyUp";
			}
			else if (point2.X >= 620)
			{
				action = "flyLR";
			}
			int_1++;
			if (int_1 <= int_2 && int_1 <= list_0.Count)
			{
				base.Body.MoveTo(point2.X, point2.Y, action, 0, method_0, 5);
				return;
			}
			int_2 = int_1 + base.Body.Config.MaxStepMove;
			base.Body.MoveTo(point2.X, point2.Y, action, 0, 5);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public SixNormalSecondNpc()
		{

			list_0 = new List<Point>
			{
				new Point(620, 1080),
				new Point(620, 980),
				new Point(720, 980),
				new Point(820, 980),
				new Point(920, 980),
				new Point(1020, 980),
				new Point(1120, 980),
				new Point(1220, 980),
				new Point(1320, 980),
				new Point(1420, 980),
				new Point(1520, 980),
				new Point(1620, 980),
				new Point(1620, 830),
				new Point(1520, 830),
				new Point(1420, 830),
				new Point(1320, 830),
				new Point(1220, 830),
				new Point(1120, 830),
				new Point(1020, 830),
				new Point(920, 830),
				new Point(820, 830),
				new Point(720, 830),
				new Point(620, 830),
				new Point(620, 680),
				new Point(720, 680),
				new Point(820, 680),
				new Point(920, 680),
				new Point(1020, 680),
				new Point(1120, 680),
				new Point(1220, 680),
				new Point(1320, 680),
				new Point(1420, 680),
				new Point(1520, 680),
				new Point(1620, 680),
				new Point(1620, 530),
				new Point(1520, 530),
				new Point(1420, 530),
				new Point(1320, 530),
				new Point(1220, 530),
				new Point(1120, 530),
				new Point(1020, 530),
				new Point(920, 530),
				new Point(820, 530),
				new Point(720, 530),
				new Point(620, 530),
				new Point(620, 380),
				new Point(720, 380),
				new Point(820, 380),
				new Point(920, 380),
				new Point(1020, 380),
				new Point(1120, 380),
				new Point(1220, 380),
				new Point(1320, 380),
				new Point(1420, 380),
				new Point(1520, 380),
				new Point(1620, 380),
				new Point(1620, 260)
			};
			list_1 = new List<Point>
			{
				new Point(700, 260),
				new Point(800, 260),
				new Point(900, 260),
				new Point(1000, 260),
				new Point(1100, 260),
				new Point(1200, 260),
				new Point(1300, 260),
				new Point(1400, 260),
				new Point(1400, 260),
				new Point(1500, 260)
			};
			bool_0 = true;

		}
	}
}
