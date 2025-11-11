using System;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveNormalThirdNpc2 : ABrain
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
			((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 1000, 0);
			base.Body.PlayMovie("beatA", 1500, 0);
			m_targer.Die(3000);
			base.Body.CallFuction(method_1, 4000);
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
				base.Body.Die(1000);
			}
		}

		public override void OnDie()
		{
			if (m_targer.IsLiving)
			{
				m_targer.BoltMove(((Point)base.Body.Properties2).X, ((Point)base.Body.Properties2).Y, 0);
			}
			m_targer.SetVisible(true);
			m_targer.BlockTurn = false;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
