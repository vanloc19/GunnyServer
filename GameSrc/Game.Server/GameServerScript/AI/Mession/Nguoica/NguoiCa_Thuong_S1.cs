using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	// Token: 0x02000180 RID: 384
	public class NguoiCa_Thuong_S1 : AMissionControl
	{
		// Token: 0x06001488 RID: 5256 RVA: 0x0009F95C File Offset: 0x0009DB5C
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

		// Token: 0x06001489 RID: 5257 RVA: 0x0009F9B4 File Offset: 0x0009DBB4
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

		// Token: 0x0600148A RID: 5258 RVA: 0x0009FA18 File Offset: 0x0009DC18
		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[]
			{
				this.bossID,
				this.bongbongId,
				this.cuabienID,
				this.cuagaiID
			};
			int[] npcIds2 = new int[]
			{
				this.bossID,
				this.bongbongId,
				this.cuabienID,
				this.cuagaiID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1504);
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x0009FAB0 File Offset: 0x0009DCB0
		public override void OnStartGame()
		{
			base.OnStartGame();
			base.Game.IsBossWar = "2002103";
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			LivingConfig livingConfig2 = base.Game.BaseLivingConfig();
			livingConfig2.IsFly = true;
			this.boss = base.Game.CreateBoss(this.bossID, 1310, 629, -1, 1, "", livingConfig);
			this.Bongbong = base.Game.CreateNpc(this.bongbongId, 1295, 500, 1, -1, livingConfig);
			this.Bongbong.XuyenThau = true;
			this.boss.SetRelateDemagemRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
			this.CuaBien = base.Game.CreateNpc(this.cuabienID, 629, 403, 1, 1);
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00002A44 File Offset: 0x00000C44
		public override void OnNewTurnStarted()
		{
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0009FBC8 File Offset: 0x0009DDC8
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			bool flag = this.CuaBien != null && !this.CuaBien.IsLiving;
			if (flag)
			{
				this.CuaBien = base.Game.CreateNpc(this.cuabienID, 629, 403, 1, 1);
			}
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x0009FC20 File Offset: 0x0009DE20
		public override bool CanGameOver()
		{
			bool flag = this.boss != null && !this.boss.IsLiving;
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

		// Token: 0x0600148F RID: 5263 RVA: 0x0009FC68 File Offset: 0x0009DE68
		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.kill;
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0009FC88 File Offset: 0x0009DE88
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

		// Token: 0x04000B53 RID: 2899
		private SimpleBoss boss = null;

		// Token: 0x04000B54 RID: 2900
		private SimpleNpc Bongbong = null;

		// Token: 0x04000B55 RID: 2901
		private SimpleNpc CuaBien = null;

		// Token: 0x04000B56 RID: 2902
		private int cuabienID = 2002101;

		// Token: 0x04000B57 RID: 2903
		private int cuagaiID = 2002102;

		// Token: 0x04000B58 RID: 2904
		private int bongbongId = 2002103;

		// Token: 0x04000B59 RID: 2905
		private int bossID = 2002104;

		// Token: 0x04000B5A RID: 2906
		private int kill = 0;

		// Token: 0x04000B5B RID: 2907
		private PhysicalObj m_moive;

		// Token: 0x04000B5C RID: 2908
		private PhysicalObj m_front;
	}
}
