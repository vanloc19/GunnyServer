using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class TwelveSimpleCaptainBoss : ABrain
	{
		private int turnToFirstAttack = 0;

		private Player player_0;

		private int int_1;

		private void method_1()
		{
			this.player_0 = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			base.Body.ChangeDirection(this.player_0, 500);
			if (this.player_0 != null && this.player_0.IsLiving)
			{
				this.int_1 = (int)this.player_0.Distance(base.Body.X, base.Body.Y);
				if (this.int_1 > base.Body.MaxBeatDis)
				{
					base.Body.CallFuction(new LivingCallBack(this.method_2), 1000);
					return;
				}
				base.Body.CallFuction(new LivingCallBack(this.method_3), 1000);
			}
		}

		private void method_2()
		{
			int int1 = base.Game.Random.Next(((SimpleBoss)base.Body).NpcInfo.MoveMax, ((SimpleBoss)base.Body).NpcInfo.MoveMax);
			if (this.int_1 < int1)
			{
				int1 = this.int_1;
			}
			int num = (base.Body.Direction == -1 ? base.Body.X - int1 : base.Body.X + int1);
			base.Body.MoveTo(num, base.Body.Y, "walk", 1200, "", ((SimpleBoss)base.Body).NpcInfo.speed, new LivingCallBack(this.method_3));
		}

		private void method_3()
		{
			base.Body.Beat(this.player_0, "beatA", 100, 0, 500, 1, 1);
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.MaxBeatDis = 190;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (turnToFirstAttack == 1)
			{
				this.method_1();
				return;
			}
			if (turnToFirstAttack < 1)
				turnToFirstAttack++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}