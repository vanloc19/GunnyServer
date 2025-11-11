using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class EighthBigScorpionBoss : ABrain
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
			m_targer = base.Game.FindRandomPlayer();
			int x = base.Game.Random.Next(917, 1383);
			if (!base.Body.MoveTo(x, m_targer.Y, "walk", 1000, method_0, 3))
			{
				base.Body.CallFuction(method_0, 1000);
			}
		}

		private void method_0()
		{
			base.Body.PlayMovie("beatA", 500, 0);
			(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 3000, 1000);
			base.Body.BeatDirect(m_targer, "", 4000, 1, 1);
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
