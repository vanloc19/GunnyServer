using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000095 RID: 149
	public class SimpleNpcFor2002014 : ABrain
	{
		// Token: 0x060008C2 RID: 2242 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0003EBE4 File Offset: 0x0003CDE4
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			bool flag = !this.IsFist;
			if (flag)
			{
				this.BaseX = base.Body.X;
				this.IsFist = true;
			}
			this.MovetoRandom();
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0003EC28 File Offset: 0x0003CE28
		public void MovetoRandom()
		{
			int x = SimpleNpcFor2002014.random.Next(this.BaseX - 50, this.BaseX + 50);
			base.Body.NewFlyTo(x, base.Body.Y, "fly", 0, "", 7);
			base.Body.CallFuction(new LivingCallBack(this.Attack), 2000);
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0003EC94 File Offset: 0x0003CE94
		public void Attack()
		{
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.PlayMovie("beatA", 1000, 2000);
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.X + 2000, "cry", 4000, null);
		}

		// Token: 0x040003AB RID: 939
		protected Player m_targer;

		// Token: 0x040003AC RID: 940
		private static Random random = new Random();

		// Token: 0x040003AD RID: 941
		private int BaseX = 0;

		// Token: 0x040003AE RID: 942
		private bool IsFist = false;
	}
}
