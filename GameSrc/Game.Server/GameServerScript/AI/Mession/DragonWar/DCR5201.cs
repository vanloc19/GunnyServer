using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DCR5201 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleNpc ruIwqzfwflx;

		private SimpleNpc simpleNpc_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private PhysicalObj physicalObj_2;

		private PhysicalObj physicalObj_3;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1150)
			{
				return 3;
			}
			if (score > 925)
			{
				return 2;
			}
			if (score > 700)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
			base.Game.AddLoadingFile(1, "bombs/72.swf", "tank.resource.bombs.Bomb72");
			base.Game.AddLoadingFile(2, "image/game/effect/5/zap.swf", "asset.game.4.zap");
			base.Game.AddLoadingFile(2, "image/game/effect/5/zap2.swf", "asset.game.4.zap2");
			base.Game.AddLoadingFile(2, "image/game/effect/5/dian.swf", "asset.game.4.dian");
			base.Game.AddLoadingFile(2, "image/game/effect/5/minigun.swf", "asset.game.4.minigun");
			base.Game.AddLoadingFile(2, "image/game/effect/5/jinqud.swf", "asset.game.4.jinqud");
			base.Game.AddLoadingFile(2, "image/game/effect/5/xiaopao.swf", "asset.game.4.xiaopao");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.gebulinzhihuiguanAsset");
			int[] npcIds = new int[4]
			{
				int_1,
				int_2,
				int_3,
				int_4
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(int_5);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_2 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_3 = base.Game.Createlayer(1172, 587, "front", "game.asset.living.gebulinzhihuiguanAsset", "out", 1, 1);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1484, 750, -1, 1, "born", livingConfig);
			base.Game.SendHideBlood(simpleBoss_0, 0);
			livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsTurn = false;
			ruIwqzfwflx = base.Game.CreateNpc(int_4, 1287, 859, 0, 1, livingConfig);
			base.Game.SendHideBlood(ruIwqzfwflx, 0);
			base.Game.SendObjectFocus(ruIwqzfwflx, 1, 700, 0);
			ruIwqzfwflx.Say("Haha, ta thích cái máy này!", 0, 2000);
			ruIwqzfwflx.MoveTo(1388, 867, "walk", 4000, method_0);
		}

		private void method_0()
		{
			physicalObj_0 = base.Game.Createlayer(1470, 822, "", "asset.game.4.jinqud", "", 1, 1);
			physicalObj_1 = base.Game.Createlayer(ruIwqzfwflx.X, ruIwqzfwflx.Y, "", "asset.game.4.dian", "", 1, 1);
			ruIwqzfwflx.PlayMovie("outA", 500, 2000);
			ruIwqzfwflx.Die(2500);
			simpleBoss_0.PlayMovie("in", 3000, 5000);
			physicalObj_2.PlayMovie("in", 5000, 0);
			physicalObj_3.PlayMovie("in", 5200, 0);
			physicalObj_2.PlayMovie("out", 8000, 0);
			physicalObj_3.PlayMovie("out", 8200, 0);
			simpleBoss_0.CallFuction(method_1, 10000);
		}

		private void method_1()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsTurn = false;
			livingConfig.CanTakeDamage = false;
			simpleNpc_0 = base.Game.CreateNpc(int_2, 187, 370, 1, 1, livingConfig);
			base.Game.SendLivingActionMapping(simpleNpc_0, "stand", "standA");
			base.Game.SendObjectFocus(simpleNpc_0, 1, 700, 0);
			simpleNpc_0.PlayMovie("in", 1500, 10000);
			simpleNpc_0.PlayMovie("walkA", 9000, 3000);
		}

		private void method_2()
		{
			if (physicalObj_1 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_1, true);
			}
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (physicalObj_2 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_2, true);
				physicalObj_2 = null;
			}
			if (physicalObj_3 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_3, true);
				physicalObj_3 = null;
			}
			method_2();
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int_0++;
				return true;
			}
			if (base.Game.TurnIndex > 200)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_0;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public override void DoOther()
		{
			base.DoOther();
		}

		private void method_3()
		{
			ruIwqzfwflx = base.Game.CreateNpc(int_4, 243, 368, 0, 1);
			base.Game.SendObjectFocus(ruIwqzfwflx, 1, 0, 500);
			ruIwqzfwflx.Say("Đừng có để hắn trốn thoát.", 0, 1000);
			ruIwqzfwflx.Say("Grrr. Máy bị các ngươi làm hư cmnr.", 0, 3000, 2000);
		}

		public override void OnShooted()
		{
			base.OnShooted();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int waitTimerLeft = base.Game.GetWaitTimerLeft();
				base.Game.ClearAllChild();
				simpleBoss_0.CallFuction(method_3, waitTimerLeft + 3000);
			}
		}

		public override void OnDied()
		{
			base.OnDied();
		}

		public DCR5201()
		{
			int_1 = 5201;
			int_2 = 5202;
			int_3 = 5203;
			int_4 = 5204;
			int_5 = 1151;
		}
	}
}
