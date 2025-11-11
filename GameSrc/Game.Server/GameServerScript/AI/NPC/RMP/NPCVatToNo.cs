using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
	public class NPCVatToNo : ABrain
	{
		protected Player m_targer;

		private PhysicalObj moive;

		private int m_attackTurn = 0;

		private int turn = 0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 30f;
		}

		public override void OnStartAttacking()
		{
			if (turn == 2)
			{
				this.TankA();
			}
			turn++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void TankA()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = (float)base.Game.Random.Next(100, 200);
			base.Body.PlayMovie("beatA", 1700, 0);
			base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 4000, null);
			base.Body.CallFuction(new LivingCallBack(this.ClearAllNpc), 3000);
		}

		private void ClearAllNpc()
		{
			base.Game.ClearAllNpc();
		}
	}
}
