using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
	public class NPCLauDaiPhepThuat01 : ABrain
	{
		protected Player m_targer;

		private int m_attackTurn = 0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 10f;
			this.m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (this.m_attackTurn == 0)
			{
				this.MoveBeat();
				this.m_attackTurn++;
			}
			else if (this.m_attackTurn == 1)
			{
				this.MoveBeat();
				this.m_attackTurn++;
			}
			else
			{
				this.MoveBeat();
				this.m_attackTurn = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void Move()
		{
			base.Body.MoveTo(base.Game.Random.Next(318, 1300), base.Game.Random.Next(416, 765), "fly", 500, "", 12, null);
		}

		private void MoveBeat()
		{
			base.Body.MoveTo(base.Game.Random.Next(318, 1300), base.Game.Random.Next(416, 765), "fly", 500, "", 12, new LivingCallBack(this.Beating));
		}

		public void Beating()
		{
			base.Body.PlayMovie("beatA", 2000, 0);
			base.Body.CallFuction(new LivingCallBack(this.RangeAttacking), 3000);
		}

		private void RangeAttacking()
		{
			base.Body.RangeAttacking(0, base.Body.Game.Map.Info.ForegroundWidth + 1, "cry", 1000, null);
		}
	}
}
