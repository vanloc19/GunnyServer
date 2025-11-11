using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	// Token: 0x02000089 RID: 137
	public class SimpleBossFor2002006 : ABrain
	{
		// Token: 0x0600083C RID: 2108 RVA: 0x00015366 File Offset: 0x00013566
		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x00015AB5 File Offset: 0x00013CB5
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00015370 File Offset: 0x00013570
		public override void OnCreated()
		{
			base.OnCreated();
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0003C6B0 File Offset: 0x0003A8B0
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
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			this.PersonalAttack();
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0003C7BC File Offset: 0x0003A9BC
		private void PersonalAttack()
		{
			float num = (float)((PVEGame)base.Game).FindAllNpcByName("Kên Kên").Count * 1f;
			bool flag = num == 0f;
			if (flag)
			{
				num = 1f;
			}
			base.Body.CurrentDamagePlus = num;
			base.Body.CurrentShootMinus = num;
			Random random = new Random();
			string action = "beatA";
			bool flag2 = random.Next(0, 2000) > 1000;
			if (flag2)
			{
				action = "beatB";
			}
			base.Body.Say("Hãy xem đây", 1, 0);
			base.Body.PlayMovie(action, 1000, 2000);
			base.Body.RangeAttacking(base.Body.X - 2000, base.Body.Y + 2000, "cry", 3000, null);
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x000154F0 File Offset: 0x000136F0
		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x00002A44 File Offset: 0x00000C44
		public override void OnDiedSay()
		{
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00002A44 File Offset: 0x00000C44
		private void CreateChild()
		{
		}
	}
}
