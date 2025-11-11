using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000092 RID: 146
	public class SimpleNpcFor2002009 : ABrain
	{
		// Token: 0x060008A5 RID: 2213 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x0003E790 File Offset: 0x0003C990
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			this.MovetoRandom();
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x0003E7A4 File Offset: 0x0003C9A4
		public void MovetoRandom()
		{
			int x = SimpleNpcFor2002009.random.Next(1310, 1535);
			base.Body.MoveToSpeed(x, base.Body.Y, "walk", 0, 7, new LivingCallBack(this.Attack));
			base.Body.ChangeDirection(-1, 100);
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x0003E804 File Offset: 0x0003CA04
		public void Attack()
		{
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.Say("Hãy để chúng trở thành kẻ hầu của ngài", 1, 500);
			base.Body.PlayMovie("beatA", 1000, 2000);
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.X + 2000, "cry", 4000, null);
		}

		// Token: 0x040003A2 RID: 930
		protected Player m_targer;

		// Token: 0x040003A3 RID: 931
		private static Random random = new Random();
	}
}
