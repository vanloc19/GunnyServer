using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	// Token: 0x02000181 RID: 385
	public class NguoiCa_Thuong_S2 : AMissionControl
	{
		// Token: 0x06001492 RID: 5266 RVA: 0x0009FD34 File Offset: 0x0009DF34
		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			bool flag = score > 1750;
			int result;
			if (flag)
			{
				result = 3;
			}
			else
			{
				bool flag2 = score > 1675;
				if (flag2)
				{
					result = 2;
				}
				else
				{
					bool flag3 = score > 1600;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x0009FD8C File Offset: 0x0009DF8C
		public override void OnMoving()
		{
			base.OnMoving();
			Player[] allPlayers = base.Game.GetAllPlayers();
			foreach (Player player in allPlayers)
			{
				bool flag = player.X > 1067;
				if (flag)
				{
					player.ReducedBlood(99999999);
					player.IsLiving = false;
				}
			}
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x0009FDF0 File Offset: 0x0009DFF0
		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[]
			{
				this.bossID,
				this.kenkenId,
				this.cotbuomID
			};
			int[] npcIds2 = new int[]
			{
				this.bossID,
				this.kenkenId,
				this.cotbuomID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1505);
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x0009FE74 File Offset: 0x0009E074
		public override void OnStartGame()
		{
			base.OnStartGame();
			base.Game.IsBossWar = "2002103";
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			LivingConfig livingConfig2 = base.Game.BaseLivingConfig();
			livingConfig2.IsFly = true;
			base.Game.SendFreeFocus(1802, 1017, 1, 0, 5000);
			this.boss = base.Game.CreateBoss(this.bossID, 1767, 1291, -1, 1, "", livingConfig);
			this.boss.Say("Các ngươi sẽ vào bụng cá hết", 1, 0);
			this.boss.CallFuction(new LivingCallBack(this.taocotbuom), 5000);
			this.boss.SetRelateDemagemRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
			this.Call();
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x0009FF8B File Offset: 0x0009E18B
		public void taocotbuom()
		{
			this.Cotbuom = base.Game.CreateNpc(this.cotbuomID, 743, 351, 1, -1);
			this.Cotbuom.Say("Không được bắn ta, nếu không thuyền sẽ chìm", 1, 1000);
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x00002A44 File Offset: 0x00000C44
		public override void OnNewTurnStarted()
		{
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x00077BC5 File Offset: 0x00075DC5
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x0009FFC8 File Offset: 0x0009E1C8
		private void Call()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			this.k1 = base.Game.CreateNpc(this.kenkenId, 1345, 398, 1, -1, livingConfig);
			this.k2 = base.Game.CreateNpc(this.kenkenId, 1535, 320, 1, -1, livingConfig);
			this.k3 = base.Game.CreateNpc(this.kenkenId, 1793, 385, 1, -1, livingConfig);
			this.k4 = base.Game.CreateNpc(this.kenkenId, 1911, 510, 1, -1, livingConfig);
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x000A007C File Offset: 0x0009E27C
		public override bool CanGameOver()
		{
			bool flag = (this.boss != null && !this.boss.IsLiving) || (this.Cotbuom != null && !this.Cotbuom.IsLiving);
			bool result;
			if (flag)
			{
				this.kill++;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x000A00DC File Offset: 0x0009E2DC
		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.kill;
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x000A00FC File Offset: 0x0009E2FC
		public override void OnGameOver()
		{
			base.OnGameOver();
			bool flag = this.boss != null && !this.boss.IsLiving;
			if (flag)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		// Token: 0x04000B5D RID: 2909
		private SimpleBoss boss = null;

		// Token: 0x04000B5E RID: 2910
		private SimpleNpc Cotbuom = null;

		// Token: 0x04000B5F RID: 2911
		private SimpleNpc k1 = null;

		// Token: 0x04000B60 RID: 2912
		private SimpleNpc k2 = null;

		// Token: 0x04000B61 RID: 2913
		private SimpleNpc k3 = null;

		// Token: 0x04000B62 RID: 2914
		private SimpleNpc k4 = null;

		// Token: 0x04000B63 RID: 2915
		private int kenkenId = 2002105;

		// Token: 0x04000B64 RID: 2916
		private int bossID = 2002106;

		// Token: 0x04000B65 RID: 2917
		private int cotbuomID = 2002107;

		// Token: 0x04000B66 RID: 2918
		private int kill = 0;

		// Token: 0x04000B67 RID: 2919
		private PhysicalObj m_moive;

		// Token: 0x04000B68 RID: 2920
		private PhysicalObj m_front;
	}
}
