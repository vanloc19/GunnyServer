using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	// Token: 0x02000183 RID: 387
	public class NguoiCa_Thuong_S4 : AMissionControl
	{
		// Token: 0x060014A8 RID: 5288 RVA: 0x000A0528 File Offset: 0x0009E728
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

		// Token: 0x060014A9 RID: 5289 RVA: 0x0009E0A3 File Offset: 0x0009C2A3
		public override void OnMoving()
		{
			base.OnMoving();
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x000A0580 File Offset: 0x0009E780
		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[]
			{
				this.cuaruongId,
				this.bossId,
				this.doiId,
				this.tuongId
			};
			int[] npcIds2 = new int[]
			{
				this.cuaruongId,
				this.bossId,
				this.doiId,
				this.tuongId
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(2, "image/game/effect/7/du.swf", "asset.game.seven.du");
			base.Game.SetMap(1507);
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x000A0630 File Offset: 0x0009E830
		public override void OnStartGame()
		{
			base.OnStartGame();
			base.Game.CreateNpc(this.cuaruongId, 330, 376, 1, -1);
			base.Game.CreateNpc(this.cuaruongId, 857, 150, 1, -1);
			base.Game.CreateNpc(this.cuaruongId, 1377, 152, 1, -1);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.isShowBlood = false;
			livingConfig.isShowSmallMapPoint = false;
			this.tuongboss = base.Game.CreateNpc(this.tuongId, 1116, 750, 1, -1, livingConfig);
			this.tuongboss.BanXuyenThau = true;
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x00002A44 File Offset: 0x00000C44
		public override void OnNewTurnStarted()
		{
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x000A06EC File Offset: 0x0009E8EC
		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			bool flag = base.Game.FindAllNpcByName("Cua Rương").Count == 0;
			if (flag)
			{
				bool flag2 = this.boss == null;
				if (flag2)
				{
					base.Game.RemoveLiving2(this.tuongboss.Id);
					this.boss = base.Game.CreateBoss(this.bossId, 1116, 750, -1, 1, "");
					base.Game.CreateNpc(this.doiId, base.Game.dieX + 70, base.Game.dieY - 50, 1, -1, livingConfig);
					base.Game.CreateNpc(this.doiId, base.Game.dieX - 70, base.Game.dieY - 50, 1, -1, livingConfig);
				}
			}
			else
			{
				int count = base.Game.FindAllNpc2().Count;
				bool flag3 = count == 3 && !this.TaoQua;
				if (flag3)
				{
					this.TaoQua = true;
					SimpleNpc simpleNpc = base.Game.CreateNpc(this.doiId, base.Game.dieX + 70, base.Game.dieY - 50, 1, -1, livingConfig);
					SimpleNpc simpleNpc2 = base.Game.CreateNpc(this.doiId, base.Game.dieX - 70, base.Game.dieY - 50, 1, -1, livingConfig);
					base.Game.CreateNpc(this.cuaruongId, 1865, 354, 1, -1);
					this.khoidoctrai = base.Game.Createlayer(553, 777, "khoidoctrai", "asset.game.seven.du", "", 1, 1);
					this.khoidocphai = base.Game.Createlayer(1661, 777, "khoidocphai", "asset.game.seven.du", "", 1, 1);
					foreach (Player player in base.Game.GetAllLivingPlayers())
					{
						bool flag4 = (player.X > 371 && player.X < 699) || (player.X > 1474 && player.X < 1836);
						if (flag4)
						{
							player.AddEffect(new ContinueReduceGreenBloodEffect(5, this.m_bloodReduce, player), 1500);
						}
					}
				}
			}
			List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
			foreach (Player player2 in allLivingPlayers)
			{
				bool flag5 = player2.Y < 404;
				if (flag5)
				{
					player2.Die();
					player2.IsLiving = false;
				}
				bool flag6 = ((player2.X > 371 && player2.X < 699) || (player2.X > 1474 && player2.X < 1836)) && this.khoidocphai != null;
				if (flag6)
				{
					player2.AddEffect(new ContinueReduceGreenBloodEffect(5, this.m_bloodReduce, player2), 1500);
				}
			}
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x000A0A84 File Offset: 0x0009EC84
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

		// Token: 0x060014AF RID: 5295 RVA: 0x000A0ACC File Offset: 0x0009ECCC
		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.kill;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x000A0AEC File Offset: 0x0009ECEC
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

		// Token: 0x04000B73 RID: 2931
		private SimpleBoss boss = null;

		// Token: 0x04000B74 RID: 2932
		private Physics tuongboss = null;

		// Token: 0x04000B75 RID: 2933
		private Physics khoidoctrai = null;

		// Token: 0x04000B76 RID: 2934
		private Physics khoidocphai = null;

		// Token: 0x04000B77 RID: 2935
		private int tuongId = 2002111;

		// Token: 0x04000B78 RID: 2936
		private int bossId = 2002112;

		// Token: 0x04000B79 RID: 2937
		private int cuaruongId = 2002113;

		// Token: 0x04000B7A RID: 2938
		private int doiId = 2002114;

		// Token: 0x04000B7B RID: 2939
		private int kill = 0;

		// Token: 0x04000B7C RID: 2940
		private PhysicalObj m_moive;

		// Token: 0x04000B7D RID: 2941
		private PhysicalObj m_front;

		// Token: 0x04000B7E RID: 2942
		private bool TaoQua = false;

		// Token: 0x04000B7F RID: 2943
		private int m_bloodReduce = 1000;
	}
}
