using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class PDHAT1143 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

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

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[3]
			{
				int_0,
				int_1,
				int_2
			};
			int[] npcIds2 = new int[2]
			{
				int_0,
				int_1
			};
			base.Game.AddLoadingFile(2, "image/game/effect/4/feather.swf", "asset.game.4.feather");
			base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.tingyuanlieshouAsset");
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1143);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(1098, 706, "front", "game.asset.living.tingyuanlieshouAsset", "out", 1, 0);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			simpleBoss_0 = base.Game.CreateBoss(int_0, 354, 344, -1, 1, "born", livingConfig);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			base.Game.SendObjectFocus(simpleBoss_0, 1, 100, 0);
			base.Game.SendFreeFocus(1460, 962, 1, 3000, 0);
			simpleBoss_0.CallFuction(method_0, 4000);
		}

		private void method_0()
		{
			simpleBoss_1 = base.Game.CreateBoss(int_1, 1460, 962, -1, 1, "born");
			simpleBoss_1.SetRelateDemagemRect(simpleBoss_1.NpcInfo.X, simpleBoss_1.NpcInfo.Y, simpleBoss_1.NpcInfo.Width, simpleBoss_1.NpcInfo.Height);
			physicalObj_0.PlayMovie("in", 3000, 0);
			physicalObj_1.PlayMovie("in", 3200, 0);
			physicalObj_0.PlayMovie("out", 6000, 0);
			physicalObj_1.PlayMovie("out", 6000, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1)
			{
				if (physicalObj_0 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_0, true);
					physicalObj_0 = null;
				}
				if (physicalObj_1 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_1, true);
					physicalObj_1 = null;
				}
			}
		}

		public override bool CanGameOver()
		{
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleBoss_1 != null && !simpleBoss_1.IsLiving)
			{
				int_3++;
				return true;
			}
			if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_3;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleBoss_1 != null && !simpleBoss_1.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public override void OnShooted()
		{
			base.OnShooted();
		}

		public PDHAT1143()
		{
			int_0 = 4105;
			int_1 = 4106;
			int_2 = 4102;
		}
	}
}
