using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000093 RID: 147
	public class SimpleNpcFor2002010 : ABrain
	{
		// Token: 0x060008AE RID: 2222 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x0002DD0C File Offset: 0x0002BF0C
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0003E89C File Offset: 0x0003CA9C
		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			bool flag = this.turn < 2;
			if (flag)
			{
				this.MovetoRandom();
				this.turn++;
			}
			else
			{
				this.HoiMau();
				this.turn = 0;
			}
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x0003E8E8 File Offset: 0x0003CAE8
		public void HoiMau()
		{
			base.Body.Say("Hỡi Poseidon, hãy ban cho chúng con sức mạnh của biển!", 1, 500);
			base.Body.PlayMovie("beatB", 1000, 2000);
			base.Body.CallFuction(new LivingCallBack(this.HoiMauAll), 3000);
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x0003E948 File Offset: 0x0003CB48
		public void HoiMauAll()
		{
			List<SimpleNpc> list = ((PVEGame)base.Game).FindAllNpc2();
			foreach (SimpleNpc simpleNpc in list)
			{
				simpleNpc.AddBlood(284191);
			}
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x0003E9B4 File Offset: 0x0003CBB4
		public void MovetoRandom()
		{
			int x = SimpleNpcFor2002010.random.Next(1310, 1535);
			base.Body.MoveToSpeed(x, base.Body.Y, "walk", 0, 7, new LivingCallBack(this.Attack));
			base.Body.ChangeDirection(-1, 100);
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x0003EA14 File Offset: 0x0003CC14
		public void Attack()
		{
			base.Body.CurrentDamagePlus = 1.2f;
			base.Body.Say("Hãy để chúng trở thành kẻ hầu của ngài", 1, 500);
			base.Body.PlayMovie("beatA", 1000, 2000);
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.X + 2000, "cry", 4000, null);
		}

		// Token: 0x040003A4 RID: 932
		protected Player m_targer;

		// Token: 0x040003A5 RID: 933
		private static Random random = new Random();

		// Token: 0x040003A6 RID: 934
		private int turn = 0;
	}
}
