using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000094 RID: 148
	public class SimpleNpcFor2002013 : ABrain
	{
		// Token: 0x060008B9 RID: 2233 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x0003EABC File Offset: 0x0003CCBC
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

		// Token: 0x060008BD RID: 2237 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0003EB00 File Offset: 0x0003CD00
		public void MovetoRandom()
		{
			int x = SimpleNpcFor2002013.random.Next(this.BaseX - 50, this.BaseX + 50);
			base.Body.MoveTo(x, base.Body.Y, "walk", 0, "", 7);
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0003EB50 File Offset: 0x0003CD50
		public override void OnDieAction(Player p)
		{
			base.OnDieAction(p);
			((PVEGame)base.Game).dieX = base.Body.X;
			((PVEGame)base.Game).dieY = base.Body.Y;
			bool sayWhenDie = base.Body.SayWhenDie;
			if (sayWhenDie)
			{
				base.Body.Say("Đã quá muộn, ngài Poseidon đã thức giấc!", 1, 0);
			}
		}

		// Token: 0x040003A7 RID: 935
		protected Player m_targer;

		// Token: 0x040003A8 RID: 936
		private static Random random = new Random();

		// Token: 0x040003A9 RID: 937
		private int BaseX = 0;

		// Token: 0x040003AA RID: 938
		private bool IsFist = false;
	}
}
