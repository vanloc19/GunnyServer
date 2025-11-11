using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FourHardFrantCattleBoss : ABrain
	{
		private int int_0;

		private Point point_0;

		private bool bool_0;

		protected Player m_targer;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			bool_0 = false;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int_0++;
			switch (int_0)
			{
				default:
					base.Body.CallFuction(method_1, 1000);
					break;
				case 10:
					base.Body.CallFuction(method_0, 1000);
					int_0 = 0;
					break;
				case 1:
					base.Body.CallFuction(method_0, 4000);
					break;
			}
		}

		private void method_0()
		{
			m_targer = base.Game.FindRandomPlayer();
			if (m_targer != null)
			{
				int num = base.Game.Random.Next(100);
				base.Body.PlayMovie("jump", 1000, 0);
				base.Body.BoltMove(m_targer.X, m_targer.Y, 3000);
				((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 3000, 0);
				if (num < 50)
				{
					base.Body.CurrentDamagePlus = 2f;
					base.Body.PlayMovie("fallB", 4000, 0);
				}
				else
				{
					base.Body.PlayMovie("fall", 4000, 0);
				}
				base.Body.RangeAttacking(m_targer.X - 50, m_targer.X + 50, "cry", 5000, directDamage: true);
			}
		}

		private void method_1()
		{
			m_targer = base.Game.FindFarPlayer(base.Body.X, base.Body.Y);
			if (m_targer == null)
			{
				return;
			}
			base.Body.ChangeDirection(m_targer, 100);
			if (m_targer.X + 200 < base.Game.Map.Info.DeadWidth && m_targer.X - 200 > 0)
			{
				if (base.Body.Distance(m_targer.X, m_targer.Y) <= 100.0)
				{
					base.Body.CallFuction(method_3, 1000);
				}
				else
				{
					base.Body.CallFuction(method_2, 500);
				}
			}
			else
			{
				base.Body.CallFuction(method_0, 1000);
			}
		}

		private void method_2()
		{
			point_0 = new Point(base.Body.X, base.Body.Y);
			if (base.Body.Direction == -1)
			{
				base.Body.MoveTo(m_targer.X - 100, m_targer.Y, "walk", 1000, method_4, 12);
			}
			else
			{
				base.Body.MoveTo(m_targer.X + 100, m_targer.Y, "walk", 1000, method_4, 12);
			}
		}

		private void method_3()
		{
			point_0 = new Point(base.Body.X, base.Body.Y);
			if (base.Body.Direction == -1)
			{
				base.Body.MoveTo(m_targer.X - 200, m_targer.Y, "walk", 500, method_4, 12);
			}
			else
			{
				base.Body.MoveTo(m_targer.X + 200, m_targer.Y, "walk", 500, method_4, 12);
			}
		}

		private void method_4()
		{
			base.Body.ChangeDirection(m_targer, 100);
			if (base.Body.X < point_0.X)
			{
				base.Body.RangeAttacking(base.Body.X, point_0.X, "cry", 0, directDamage: true);
			}
			else
			{
				base.Body.RangeAttacking(point_0.X, base.Body.X, "cry", 0, directDamage: true);
			}
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
