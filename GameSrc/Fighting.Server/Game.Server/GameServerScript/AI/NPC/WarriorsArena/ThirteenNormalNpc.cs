using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
	public class ThirteenNormalNpc : ABrain
	{
		private int int_0;

		private Player player_0;

		public ThirteenNormalNpc()
		{


		}

		private void method_0()
		{
			base.Body.PlayMovie("die", 1000, 0);
			if (this.player_0 != null)
			{
				base.Body.CallFuction(new LivingCallBack(this.method_1), 2500);
				this.player_0.Die(3000);
			}
			base.Body.Die(4000);
		}

		private void method_1()
		{
			this.player_0.SetVisible(true);
			this.player_0.BlockTurn = false;
		}

		private void nSusVjDjvAj()
		{
			if (base.Body.Properties1 != null)
			{
				this.player_0 = base.Game.FindPlayerWithId((int)base.Body.Properties1);
				if (this.player_0 != null && this.player_0.IsLiving)
				{
					base.Body.PlayMovie("beatA", 500, 0);
					base.Body.BeatDirect(this.player_0, "", 2000, 1, 1);
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int int0 = this.int_0;
			if (int0 == 0)
			{
				this.nSusVjDjvAj();
			}
			else if (int0 == 1)
			{
				this.method_0();
			}
			this.int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}