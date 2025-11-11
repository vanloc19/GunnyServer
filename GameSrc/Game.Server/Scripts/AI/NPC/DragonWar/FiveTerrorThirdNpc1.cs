using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveTerrorThirdNpc1 : ABrain
	{
		private int int_0;

		protected Player m_targer;

		private int int_1;

		private int int_2;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			if (m_targer == null)
			{
				m_targer = ((PVEGame)base.Game).FindPlayer((int)base.Body.Properties1);
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			method_0();
		}

		private void method_0()
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.Beat(m_targer, "beatA", 100, 0, 1000);
			base.Body.CallFuction(method_1, 3000);
		}

		private void method_1()
		{
			if (!m_targer.IsLiving)
			{
				method_2();
			}
		}

		private void method_2()
		{
			if (base.Body.IsLiving)
			{
				base.Body.PlayMovie("die", 500, 2000);
				base.Body.Die(2000);
			}
		}

		public override void OnDie()
		{
			m_targer.SetVisible(true);
			m_targer.BlockTurn = false;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
