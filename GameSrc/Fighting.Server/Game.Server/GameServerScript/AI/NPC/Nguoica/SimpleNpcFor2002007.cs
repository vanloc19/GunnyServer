using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000090 RID: 144
	public class SimpleNpcFor2002007 : ABrain
	{
		// Token: 0x0600088F RID: 2191 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0003E27E File Offset: 0x0003C47E
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			this.m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0003E2B0 File Offset: 0x0003C4B0
		public void MoveToPlayer(Player player)
		{
			int num = (int)player.Distance(base.Body.X, base.Body.Y);
			int num2 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
			bool flag = num > 97;
			if (flag)
			{
				bool flag2 = num > ((SimpleNpc)base.Body).NpcInfo.MoveMax;
				if (flag2)
				{
					num = num2;
				}
				else
				{
					num -= 90;
				}
				bool flag3 = player.Y < 420 && player.X < 210;
				if (flag3)
				{
					bool flag4 = player.X > base.Body.X;
					if (flag4)
					{
						base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					}
					else
					{
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					}
				}
				else
				{
					bool flag5 = player.X > base.Body.X;
					if (flag5)
					{
						base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					}
					else
					{
						base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
					}
				}
			}
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x0003E509 File Offset: 0x0003C709
		public void MoveBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x0003E529 File Offset: 0x0003C729
		public void FallBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 2000, 1, 1);
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x0003E550 File Offset: 0x0003C750
		public void Jump()
		{
			base.Body.Direction = 1;
			base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, new LivingCallBack(this.Beating));
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x0003E5A8 File Offset: 0x0003C7A8
		public void Beating()
		{
			bool flag = this.m_targer != null && !base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
			if (flag)
			{
				this.MoveToPlayer(this.m_targer);
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0003E5F4 File Offset: 0x0003C7F4
		public void Fall()
		{
			base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, new LivingCallBack(this.Beating));
		}

		// Token: 0x0400039E RID: 926
		protected Player m_targer;

		// Token: 0x0400039F RID: 927
		private static Random random = new Random();
	}
}
