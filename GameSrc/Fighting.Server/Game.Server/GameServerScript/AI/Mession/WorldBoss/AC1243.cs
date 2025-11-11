using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class AC1243 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		public AC1243()
		{

			this.int_0 = 1243;

		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1750)
			{
				return 3;
			}
			if (score > 1675)
			{
				return 2;
			}
			if (score > 1600)
			{
				return 1;
			}
			return 0;
		}

		public override bool CanGameOver()
		{
			if (this.simpleBoss_0 == null || this.simpleBoss_0.IsLiving)
			{
				return false;
			}
			this.int_1++;
			return true;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (this.simpleBoss_0 == null || this.simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = false;
			}
			else
			{
				base.Game.IsWin = true;
			}
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.PlayerDetail.UpdatePveResult("worldboss", allFightPlayer.TotalDameLiving, base.Game.IsWin);
			}
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] int0 = new int[] { this.int_0 };
			int[] numArray = new int[] { this.int_0 };
			base.Game.LoadResources(int0);
			base.Game.LoadNpcGameOverResources(numArray);
			base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/5/guang.swf", "asset.game.4.guang");
			base.Game.AddLoadingFile(2, "image/game/effect/5/tang.swf", "asset.game.4.tang");
			base.Game.AddLoadingFile(2, "image/game/effect/5/ruodian.swf", "asset.game.4.ruodian");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.xieyanjulongAsset");
			base.Game.SetMap(1243);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			livingConfig.IsWorldBoss = true;
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 1170, 370, -1, 1, "", livingConfig);
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.int_1;
		}
	}
}