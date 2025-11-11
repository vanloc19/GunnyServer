using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class BLTTT1124 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

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
			int[] npcIds = new int[5]
			{
				int_1,
				int_2,
				int_3,
				int_4,
				int_5
			};
			int[] npcIds2 = new int[2]
			{
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
			base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
			base.Game.AddLoadingFile(2, "image/map/1124/object/1124object.swf", "game.crazytank.assetmap.Dici");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanBrotherAsset");
			base.Game.SetMap(1124);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			base.Game.PveGameDelay = 0;
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(650, 400, "front", "game.asset.living.ClanBrotherAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 234, 357, 1, 1, "");
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_1 = base.Game.CreateBoss(int_2, 1368, 357, -1, 1, "");
			simpleBoss_1.SetRelateDemagemRect(simpleBoss_1.NpcInfo.X, simpleBoss_1.NpcInfo.Y, simpleBoss_1.NpcInfo.Width, simpleBoss_1.NpcInfo.Height);
			base.Game.SendObjectFocus(simpleBoss_0, 1, 2000, 0);
			simpleBoss_0.PlayMovie("call", 3000, 0);
			simpleBoss_0.Say("Chạy à? Định chạy đi đâu?", 0, 3300);
			base.Game.SendObjectFocus(simpleBoss_1, 1, 6200, 0);
			simpleBoss_1.PlayMovie("castA", 7000, 0);
			simpleBoss_1.Say("Đại ca. Đây là vật tế cuối cùng rồi đấy!", 0, 7300);
			base.Game.SendFreeFocus(827, 534, 1, 10000, 0);
			physicalObj_0.PlayMovie("in", 10000, 0);
			physicalObj_1.PlayMovie("in", 11000, 0);
			physicalObj_0.PlayMovie("out", 14000, 0);
			physicalObj_1.PlayMovie("out", 15000, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
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
			base.CanGameOver();
			if (base.Game.FindAllTurnBossLiving().Count <= 0)
			{
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
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (base.Game.FindAllTurnBossLiving().Count <= 0)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public override void OnDied()
		{
			base.OnDied();
		}

		public BLTTT1124()
		{
			int_1 = 3108;
			int_2 = 3109;
			int_3 = 3106;
			int_4 = 3110;
			int_5 = 3111;
		}
	}
}
