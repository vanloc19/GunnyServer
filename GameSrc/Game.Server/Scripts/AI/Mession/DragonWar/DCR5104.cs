using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DCR5104 : AMissionControl
	{
		private List<PhysicalObj> list_0;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private PhysicalObj wkewqTbKeDI;

		private PhysicalObj physicalObj_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private int yilwqqJdso0;

		private int int_3;

		private int int_4;

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
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.hongpaoxiaoemoAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/5/cuipao.swf", "asset.game.4.cuipao");
			base.Game.AddLoadingFile(2, "image/game/effect/5/guang.swf", "asset.game.4.guang");
			base.Game.AddLoadingFile(2, "image/game/effect/5/da.swf", "asset.game.4.da");
			base.Game.AddLoadingFile(2, "image/game/effect/5/mubiao.swf", "asset.game.4.mubiao");
			int[] npcIds = new int[4]
			{
				int_1,
				yilwqqJdso0,
				int_3,
				int_2
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(int_4);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			wkewqTbKeDI = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_0 = base.Game.Createlayer(1291, 257, "top", "game.asset.living.xieyanjulongAsset", "out", 1, 1);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1700, 480, -1, 1, "", livingConfig);
			simpleBoss_0.SetRect(-180, -90, 300, 100);
			simpleBoss_0.SetRelateDemagemRect(-60, -200, 116, 100);
			base.Game.SendHideBlood(simpleBoss_0, 0);
			simpleBoss_0.CallFuction(method_1, 3300);
			simpleBoss_0.CallFuction(method_2, 3300);
			simpleBoss_0.CallFuction(method_3, 6000);
			wkewqTbKeDI.PlayMovie("in", 7000, 0);
			physicalObj_0.PlayMovie("in", 7000, 0);
			wkewqTbKeDI.PlayMovie("out", 10000, 0);
			physicalObj_0.PlayMovie("out", 10000, 0);
			simpleBoss_0.CallFuction(method_0, 11000);
		}

		private void method_0()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			livingConfig.CanTakeDamage = false;
			livingConfig.IsHelper = true;
			simpleBoss_1 = base.Game.CreateBoss(yilwqqJdso0, 190, 250, 1, 2, "", livingConfig);
			simpleBoss_1.Delay++;
			base.Game.SendHideBlood(simpleBoss_1, 0);
			simpleBoss_1.Say("Đừng sợ. Đã có ta ở đây.", 0, 3000, 2000);
		}

		private void method_1()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				list_0.Add(base.Game.CreatePhysicalObj(0, 0, "top", "asset.game.4.cuipao", "", 1, 1, allLivingPlayer.Id + 1));
			}
		}

		private void method_2()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				allLivingPlayer.SpeedMultX(18);
				allLivingPlayer.StartSpeedMult(750 + base.Game.Random.Next(0, 50), allLivingPlayer.Y, 0);
			}
		}

		private void method_3()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				allLivingPlayer.SpeedMultX(3);
			}
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (wkewqTbKeDI != null)
			{
				base.Game.RemovePhysicalObj(wkewqTbKeDI, true);
				wkewqTbKeDI = null;
			}
			if (physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_0, true);
				physicalObj_0 = null;
			}
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, true);
			}
			list_0 = new List<PhysicalObj>();
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

		public override void OnShooted()
		{
			base.OnShooted();
		}

		public override void OnDied()
		{
			base.OnDied();
		}

		public DCR5104()
		{
			list_0 = new List<PhysicalObj>();
			int_1 = 5131;
			int_2 = 5132;
			yilwqqJdso0 = 5133;
			int_3 = 5134;
			int_4 = 1154;
		}
	}
}
