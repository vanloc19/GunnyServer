using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;

namespace GameServerScript.AI.Messions
{
	public class WAN13103 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private SimpleBoss simpleBoss_2;

		public WAN13103()
		{
			
			this.int_0 = 13107;
			this.int_1 = 13106;
			this.int_2 = 13110;
			
		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 600)
			{
				return 3;
			}
			if (score > 520)
			{
				return 2;
			}
			if (score > 450)
			{
				return 1;
			}
			return 0;
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving)
			{
				return true;
			}
			if (base.Game.TotalTurn > 200)
			{
				return true;
			}
			return false;
		}

		private void method_0()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			livingConfig.IsFly = true;
			this.simpleBoss_1 = base.Game.CreateBoss(this.int_1, 1683, 762, -1, 1, "", livingConfig);
			this.simpleBoss_1.Delay = 1;
			base.Game.SendObjectFocus(this.simpleBoss_1, 0, 0, 0);
			this.physicalObj_0.PlayMovie("in", 3000, 0);
			this.physicalObj_1.PlayMovie("in", 3200, 0);
			this.physicalObj_0.PlayMovie("out", 6000, 0);
			this.physicalObj_1.PlayMovie("out", 6200, 0);
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (this.physicalObj_0 != null && this.physicalObj_1 != null)
			{
				base.Game.RemovePhysicalObj(this.physicalObj_0, true);
				base.Game.RemovePhysicalObj(this.physicalObj_1, true);
				this.physicalObj_0 = null;
				this.physicalObj_1 = null;
			}
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (!this.simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
				return;
			}
			base.Game.IsWin = false;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] int0 = new int[] { this.int_0, this.int_1, this.int_2 };
			base.Game.AddLoadingFile(2, "image/game/effect/10/chengtuo.swf", "asset.game.ten.chengtuo");
			base.Game.AddLoadingFile(2, "image/game/effect/10/laotie.swf", "asset.game.ten.laotie");
			base.Game.AddLoadingFile(2, "image/game/effect/10/laotie.swf", "asset.game.ten.laotie2");
			base.Game.AddLoadingFile(2, "image/game/effect/5/lanhuo.swf", "asset.game.4.lanhuo");
			base.Game.AddLoadingFile(2, "image/game/effect/5/heip.swf", "asset.game.4.heip");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.dadangAsset");
			base.Game.LoadResources(int0);
			base.Game.LoadNpcGameOverResources(int0);
			base.Game.SetMap(1216);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			this.physicalObj_1 = base.Game.Createlayer(1200, 700, "front", "game.asset.living.dadangAsset", "out", 1, 0);
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 1683, 1012, -1, 1, "");
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
			base.Game.SendObjectFocus(this.simpleBoss_0, 0, 0, 0);
			this.simpleBoss_0.CallFuction(new LivingCallBack(this.method_0), 2000);
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}
	}
}