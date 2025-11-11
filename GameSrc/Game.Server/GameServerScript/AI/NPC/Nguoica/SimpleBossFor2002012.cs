using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x0200008A RID: 138
	public class SimpleBossFor2002012 : ABrain
	{
		// Token: 0x06000845 RID: 2117 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0003C8A8 File Offset: 0x0003AAA8
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			bool flag = this.turn == 0;
			if (flag)
			{
				this.turn++;
				this.AttackA();
			}
			else
			{
				this.AttackB();
				this.turn = 0;
			}
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0003C8F4 File Offset: 0x0003AAF4
		public void AttackA()
		{
			string action = "beatA";
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.PlayMovie(action, 1000, 2000);
			base.Body.CallFuction(new LivingCallBack(this.AttackPlayer), 2000);
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0003C94C File Offset: 0x0003AB4C
		public void AttackB()
		{
			string action = "beatB";
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.PlayMovie(action, 1000, 2000);
			base.Body.CallFuction(new LivingCallBack(this.EffectBeatB), 2000);
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x0003C9A4 File Offset: 0x0003ABA4
		public void EffectBeatB()
		{
			Player player = ((PVEGame)base.Game).FindRandomPlayer();
			((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset_game_fifteen_486a", "out", 1, 0);
			base.Body.RangeAttacking(player.X - 500, base.Body.X + 500, "cry", 2000, null);
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0003CA25 File Offset: 0x0003AC25
		public void AttackPlayer()
		{
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.X + 2000, "cry", 1000, null);
		}

		// Token: 0x04000383 RID: 899
		protected Player m_targer;

		// Token: 0x04000384 RID: 900
		private static Random random = new Random();

		// Token: 0x04000385 RID: 901
		private int BaseX = 0;

		// Token: 0x04000386 RID: 902
		private bool IsFist = false;

		// Token: 0x04000387 RID: 903
		private int turn = 0;
	}
}
