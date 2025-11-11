using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class EighthSimpleFirstBoss : ABrain
	{
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
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			base.Body.ChangeDirection(base.Body.FindDirection(m_targer), 500);
			bool flag = false;
			if (!((base.Body.X <= m_targer.X) ? base.Body.MoveTo(m_targer.X - 60, m_targer.Y, "walk", 1000, method_0, 3) : base.Body.MoveTo(m_targer.X + 60, m_targer.Y, "walk", 1000, method_0, 3)))
			{
				base.Body.CallFuction(method_0, 1000);
			}
		}

		private void method_0()
		{
			base.Body.PlayMovie("beatA", 500, 0);
			base.Body.BeatDirect(m_targer, "", 2000, 1, 1);
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
