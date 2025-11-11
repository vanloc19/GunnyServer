using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DHTAH20301 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1330)
			{
				return 3;
			}
			if (score > 1150)
			{
				return 2;
			}
			if (score > 970)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[2]
			{
				int_2,
				int_1
			};
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ZhenBombKingAsset");
			base.Game.SetMap(1084);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			simpleBoss_0 = base.Game.CreateBoss(int_1, 888, 590, -1, 1, "");
			physicalObj_0 = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(710, 380, "font", "game.asset.living.ZhenBombKingAsset", "out", 1, 1);
			simpleBoss_0.FallFrom(888, 590, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(-41, -187, 83, 140);
			physicalObj_0.PlayMovie("in", 1000, 0);
			physicalObj_1.PlayMovie("in", 2000, 2000);
			simpleBoss_0.AddDelay(16);
			base.Game.BossCardCount = 1;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (simpleBoss_0.State == 0)
			{
				simpleBoss_0.SetRelateDemagemRect(-41, -187, 83, 140);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
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

		public override bool CanGameOver()
		{
			if (!simpleBoss_0.IsLiving)
			{
				int_0++;
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
			bool flag = true;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving)
				{
					flag = false;
				}
			}
			if (!simpleBoss_0.IsLiving && !flag)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public DHTAH20301()
		{
			int_1 = 20301; // boss
			int_2 = 20302; // nhoc luu dan 
		}
	}
}
