using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000088 RID: 136
	public class SimpleBossFor2002004 : ABrain
	{
		// Token: 0x06000830 RID: 2096 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0003C394 File Offset: 0x0003A594
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			this.isSay = 0;
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0003C3C8 File Offset: 0x0003A5C8
		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			int num = 0;
			foreach (Player player in base.Game.GetAllFightPlayers())
			{
				bool flag = player.IsLiving && player.X > 1169 && player.X < base.Game.Map.Info.ForegroundWidth + 1;
				if (flag)
				{
					int num2 = (int)base.Body.Distance(player.X, player.Y);
					bool flag2 = num2 > num;
					if (flag2)
					{
						num = num2;
					}
				}
			}
			bool flag3 = this.m_attackTurn == 0;
			if (flag3)
			{
				this.Summon();
				this.m_attackTurn++;
			}
			else
			{
				this.PersonalAttack();
				this.m_attackTurn = 0;
			}
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0003C4E4 File Offset: 0x0003A6E4
		private void PersonalAttack()
		{
			Random random = new Random();
			string action = "beatA";
			bool flag = random.Next(0, 2000) > 1000;
			if (flag)
			{
				action = "beatB";
			}
			base.Body.Say("Hãy xem đây", 1, 0);
			base.Body.PlayMovie(action, 1000, 2000);
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.Y + 2000, "", 3000, null);
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0003C580 File Offset: 0x0003A780
		private void KillAttack(int fx, int tx)
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.PlayMovie("beatB", 3000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0003C5D0 File Offset: 0x0003A7D0
		private void Summon()
		{
			bool flag = this.cuagai == null || !this.cuagai.IsLiving;
			if (flag)
			{
				base.Body.Say("Người đâu ta bị đánh rồi", 1, 600);
				base.Body.PlayMovie("call", 1700, 2000, new LivingCallBack(this.Call));
				base.Body.CallFuction(new LivingCallBack(this.Call), 2000);
			}
			else
			{
				this.PersonalAttack();
			}
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0003C663 File Offset: 0x0003A863
		private void Call()
		{
			this.cuagai = ((PVEGame)base.Game).CreateNpc(this.cuagaiID, 992, 400, 1, -1);
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00002A44 File Offset: 0x00000C44
		public override void OnDiedSay()
		{
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00002A44 File Offset: 0x00000C44
		private void CreateChild()
		{
		}

		// Token: 0x0400037F RID: 895
		private int m_attackTurn = 0;

		// Token: 0x04000380 RID: 896
		private int cuagaiID = 2002102;

		// Token: 0x04000381 RID: 897
		private int isSay = 0;

		// Token: 0x04000382 RID: 898
		private SimpleNpc cuagai;
	}
}
