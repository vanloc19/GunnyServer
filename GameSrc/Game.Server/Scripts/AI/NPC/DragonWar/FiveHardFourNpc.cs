using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveHardFourNpc : ABrain
	{
		private int int_0;

		protected Player m_targer;

		private int int_1;

		private int int_2;

		private static string[] string_0;

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
			base.Body.MaxBeatDis = 200;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			if (m_targer != null)
			{
				base.Body.ChangeDirection(m_targer, 500);
				base.Body.MoveTo(m_targer.X, m_targer.Y - 20, "fly", 1000, method_0, 5);
			}
		}

		private void method_0()
		{
			base.Body.Beat(m_targer, "beatA", 100, 0, 500);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		static FiveHardFourNpc()
		{
			string_0 = new string[1]
			{
				""
			};
		}
	}
}
