using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x0200008F RID: 143
	public class SimpleNpcFor2002005 : ABrain
	{
		// Token: 0x06000886 RID: 2182 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x0001553A File Offset: 0x0001373A
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x0003E16F File Offset: 0x0003C36F
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			this.MovetoRandom();
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0003E180 File Offset: 0x0003C380
		public void MovetoRandom()
		{
			int x = SimpleNpcFor2002005.random.Next(1259, 1753);
			int y = SimpleNpcFor2002005.random.Next(264, 471);
			base.Body.NewFlyTo(x, y, "fly", 0, "", 5);
			base.Body.ChangeDirection(-1, 100);
			base.Body.CallFuction(new LivingCallBack(this.Attack), 3000);
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0003E200 File Offset: 0x0003C400
		public void Attack()
		{
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.PlayMovie("beatA", 1000, 2000);
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.X + 2000, "cry", 4000, null);
		}

		// Token: 0x0400039C RID: 924
		protected Player m_targer;

		// Token: 0x0400039D RID: 925
		private static Random random = new Random();
	}
}
