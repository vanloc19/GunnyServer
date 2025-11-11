using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class PDHAT1144 : AMissionControl
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
			int[] npcIds = new int[2]
			{
				int_0,
				int_1
			};
			int[] npcIds2 = new int[1]
			{
				int_0
			};
			base.Game.AddLoadingFile(2, "image/game/effect/4/power.swf", "game.crazytank.assetmap.Buff_powup");
			base.Game.AddLoadingFile(2, "image/game/effect/4/blade.swf", "asset.game.4.blade");
			base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.tingyuanlieshouAsset");
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1144);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.HaveShield = true;
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(1019, 620, "front", "game.asset.living.emozhanshiAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_0, 1255, 958, -1, 1, "born", livingConfig);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.CallFuction(method_0, 1000);
		}

		private void method_0()
		{
			base.Game.SendObjectFocus(simpleBoss_0, 1, 500, 0);
			simpleBoss_0.PlayMovie("in", 2000, 0);
			base.Game.SendObjectFocus(simpleBoss_0, 2, 2000, 3000);
			simpleBoss_0.PlayMovie("standA", 9000, 0);
			simpleBoss_0.Say("Ngọn lửa sôi sục đang cháy trong ta!", 0, 9200);
			physicalObj_0.PlayMovie("in", 13000, 0);
			physicalObj_1.PlayMovie("in", 13200, 0);
			physicalObj_0.PlayMovie("out", 16200, 0);
			physicalObj_1.PlayMovie("out", 16000, 0);
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
					base.Game.RemovePhysicalObj(physicalObj_0, sendToClient: true);
					physicalObj_0 = null;
				}
				if (physicalObj_1 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_1, sendToClient: true);
					physicalObj_1 = null;
				}
			}
		}

		public override bool CanGameOver()
		{
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
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
			return base.Game.TotalKillCount;
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

		public override void OnShooted()
		{
			base.OnShooted();
		}

		public PDHAT1144()
		{
			int_0 = 4108;
			int_1 = 4107;
		}
	}
}
