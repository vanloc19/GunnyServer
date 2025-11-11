using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000091 RID: 145
	public class SimpleNpcFor2002008 : ABrain
	{
		// Token: 0x0600089C RID: 2204 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0003E647 File Offset: 0x0003C847
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			this.MovetoRandom();
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0003E658 File Offset: 0x0003C858
		public void MovetoRandom()
		{
			int x = SimpleNpcFor2002008.random.Next(1356, 1968);
			base.Body.MoveToSpeed(x, base.Body.Y, "walk", 0, 10, new LivingCallBack(this.Attack));
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x0003E6A8 File Offset: 0x0003C8A8
		public void Attack()
		{
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.Say("Tấn công", 1, 500);
			Player player = base.Game.FindRandomPlayer();
			bool flag = player != null;
			if (flag)
			{
				base.Body.ChangeDirection(base.Body.FindDirection(player), 100);
				int num = 2000;
				for (int i = 0; i < 1; i++)
				{
					bool flag2 = base.Body.ShootPoint(player.X, player.Y, 483, 1000, 10000, 3, 4f, 2500);
					if (flag2)
					{
						base.Body.PlayMovie("beatA", num - 500, 0);
					}
					num += 2000;
				}
			}
		}

		// Token: 0x040003A0 RID: 928
		protected Player m_targer;

		// Token: 0x040003A1 RID: 929
		private static Random random = new Random();
	}
}
