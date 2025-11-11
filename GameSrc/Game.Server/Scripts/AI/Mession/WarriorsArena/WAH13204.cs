using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;

namespace GameServerScript.AI.Messions
{
	public class WAH13204 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private SimpleBoss simpleBoss_2;

		public WAH13204()
		{
			
			this.int_0 = 13208;
			this.int_1 = 13209;
			this.int_2 = 13212;
			this.int_3 = 13213;
			this.int_4 = 13214;
			
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
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving && this.simpleBoss_1 != null && !this.simpleBoss_1.IsLiving)
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
			this.simpleBoss_1 = base.Game.CreateBoss(this.int_1, 1912, 1020, -1, 1, "", livingConfig);
			this.simpleBoss_1.SetRelateDemagemRect(this.simpleBoss_1.NpcInfo.X, this.simpleBoss_1.NpcInfo.Y, this.simpleBoss_1.NpcInfo.Width, this.simpleBoss_1.NpcInfo.Height);
			this.simpleBoss_1.Delay = 1;
			base.Game.SendObjectFocus(this.simpleBoss_1, 0, 0, 0);
			base.Game.SendFreeFocus(1000, 900, 1, 2000, 0);
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
			if (!this.simpleBoss_0.IsLiving && !this.simpleBoss_1.IsLiving)
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
			int[] int0 = new int[] { this.int_0, this.int_1, this.int_2, this.int_3, this.int_4 };
			base.Game.AddLoadingFile(2, "image/game/effect/10/danbao.swf", "asset.game.ten.danbao");
			base.Game.AddLoadingFile(2, "image/game/effect/10/qunbao.swf", "asset.game.ten.qunbao");
			base.Game.AddLoadingFile(2, "image/game/effect/10/tedabiaoji.swf", "asset.game.ten.tedabiaoji");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.shuangwangAsset");
			base.Game.LoadResources(int0);
			base.Game.LoadNpcGameOverResources(int0);
			base.Game.SetMap(1217);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			this.physicalObj_1 = base.Game.Createlayer(810, 750, "front", "game.asset.living.shuangwangAsset", "out", 1, 0);
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 83, 1020, 1, 1, "");
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