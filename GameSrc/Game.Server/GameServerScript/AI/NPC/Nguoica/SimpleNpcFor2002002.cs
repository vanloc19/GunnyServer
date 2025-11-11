using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x0200008E RID: 142
	public class SimpleNpcFor2002002 : ABrain
	{
		// Token: 0x0600087A RID: 2170 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x0003DF74 File Offset: 0x0003C174
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

		// Token: 0x0600087C RID: 2172 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x0003DFE6 File Offset: 0x0003C1E6
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			this.m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			this.Beating();
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x0003E01E File Offset: 0x0003C21E
		public void MoveBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x0003E03E File Offset: 0x0003C23E
		public void FallBeat()
		{
			base.Body.Beat(this.m_targer, "beatA", 100, 0, 2000, 1, 1);
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0003E064 File Offset: 0x0003C264
		public void Jump()
		{
			base.Body.Direction = 1;
			base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, new LivingCallBack(this.Beating));
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0003E0BC File Offset: 0x0003C2BC
		public void Beating()
		{
			base.Body.PlayMovie("beatA", 0, 2000);
			base.Body.RangeAttacking(base.Body.X - 2500, base.Body.X + 2500, "cry", 3000, null);
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0003E11C File Offset: 0x0003C31C
		public void Fall()
		{
			base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, new LivingCallBack(this.Beating));
		}

		// Token: 0x0400039A RID: 922
		protected Player m_targer;

		// Token: 0x0400039B RID: 923
		private static Random random = new Random();
	}
}
