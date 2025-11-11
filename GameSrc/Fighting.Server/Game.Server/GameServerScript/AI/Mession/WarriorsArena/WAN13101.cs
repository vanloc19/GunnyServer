using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;

namespace GameServerScript.AI.Messions
{
	public class WAN13101 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		public WAN13101()
		{
			
			this.int_0 = 13101;
			this.int_1 = 13102;
			
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
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving && this.simpleBoss_1 != null && !this.simpleBoss_1.IsLiving || base.Game.TotalTurn > 200)
			{
				return true;
			}
			return false;
		}

		private void method_0()
		{
			this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 1295, 445, -1, 1, "");
			this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
			this.simpleBoss_0.Delay = 1;
			base.Game.SendObjectFocus(this.simpleBoss_0, 0, 0, 0);
			this.physicalObj_0.PlayMovie("in", 2000, 0);
			this.physicalObj_1.PlayMovie("in", 2200, 0);
			this.physicalObj_0.PlayMovie("out", 6000, 0);
			this.physicalObj_1.PlayMovie("out", 6200, 0);
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving && this.simpleBoss_1 != null && !this.simpleBoss_1.IsLiving)
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
			int[] int0 = new int[] { this.int_0, this.int_1 };
			base.Game.AddLoadingFile(1, "bombs/51.swf", "tank.resource.bombs.Bomb51");
			base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
			base.Game.AddLoadingFile(1, "bombs/99.swf", "tank.resource.bombs.Bomb99");
			base.Game.AddLoadingFile(2, "image/game/effect/10/jianyu.swf", "asset.game.ten.jianyu");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.canbaoAsset");
			base.Game.LoadResources(int0);
			base.Game.LoadNpcGameOverResources(int0);
			base.Game.SetMap(1214);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			this.physicalObj_1 = base.Game.Createlayer(820, 400, "front", "game.asset.living.canbaoAsset", "out", 1, 0);
			this.simpleBoss_1 = base.Game.CreateBoss(this.int_1, 1290, 1013, -1, 1, "");
			this.simpleBoss_1.SetRelateDemagemRect(this.simpleBoss_1.NpcInfo.X, this.simpleBoss_1.NpcInfo.Y, this.simpleBoss_1.NpcInfo.Width, this.simpleBoss_1.NpcInfo.Height);
			base.Game.SendObjectFocus(this.simpleBoss_1, 0, 0, 0);
			this.simpleBoss_1.CallFuction(new LivingCallBack(this.method_0), 1500);
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}
	}
}