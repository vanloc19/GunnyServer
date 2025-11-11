using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x0200008D RID: 141
	public class SimpleNpcFor2002001 : ABrain
	{
		// Token: 0x0600086D RID: 2157 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0003DB30 File Offset: 0x0003BD30
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
			bool isSay = this.m_body.IsSay;
			if (isSay)
			{
				string msg = "hãy xem sự lợi hại của ta!";
				int delay = base.Game.Random.Next(0, 5000);
				this.m_body.Say(msg, 0, delay);
			}
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0003DBA2 File Offset: 0x0003BDA2
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			this.m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			this.Beating();
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x0003DBDC File Offset: 0x0003BDDC
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

		// Token: 0x06000873 RID: 2163 RVA: 0x0003DE35 File Offset: 0x0003C035
		public void MoveBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x0003DE55 File Offset: 0x0003C055
		public void FallBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 2000, 1, 1);
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x0003DE7C File Offset: 0x0003C07C
		public void Jump()
		{
			base.Body.Direction = 1;
			base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, new LivingCallBack(this.Beating));
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0003DED4 File Offset: 0x0003C0D4
		public void Beating()
		{
			bool flag = this.m_targer != null && !base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
			if (flag)
			{
				this.MoveToPlayer(this.m_targer);
			}
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0003DF20 File Offset: 0x0003C120
		public void Fall()
		{
			base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, new LivingCallBack(this.Beating));
		}

		// Token: 0x04000398 RID: 920
		protected Player m_targer;

		// Token: 0x04000399 RID: 921
		private static Random random = new Random();
	}
}
