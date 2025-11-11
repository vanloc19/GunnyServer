using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class NTM1088 : AMissionControl
	{
		private int int_0;

		private bool bool_0;

		private int int_1;

		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_2;

		private PhysicalObj physicalObj_2;

		private List<SimpleNpc> list_0;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 900)
			{
				return 3;
			}
			if (score > 825)
			{
				return 2;
			}
			if (score > 725)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.jianjiaoAsset");
			base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout96.swf", "bullet96");
			base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet96.swf", "bullet96");
			base.Game.AddLoadingFile(2, "image/game/effect/0/guangquan.swf", "asset.game.0.guangquan");
			int[] npcIds = new int[2]
			{
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(int_0);
		}

		public override void OnStartGame()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(25, 265, "font", "game.asset.living.jianjiaoAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_2, 100, 320, 1, 1, "");
			simpleBoss_0.FallFrom(100, 520, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Say("Bay ra ngoài đi，núp trong đóa ko thịt được ta đâu！", 0, 3000);
			physicalObj_0.PlayMovie("in", 4000, 0);
			physicalObj_1.PlayMovie("in", 4000, 0);
			physicalObj_0.PlayMovie("out", 7000, 0);
			physicalObj_1.PlayMovie("out", 7000, 0);
		}

		public override void OnNewTurnStarted()
		{
			if (physicalObj_2 == null)
			{
				physicalObj_2 = base.Game.Createlayer(1150, 544, "moive", "asset.game.0.guangquan", "in", 1, 0);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			Player player = base.Game.FindRandomPlayer();
			if (player.X > 1050)
			{
				physicalObj_2.PlayMovie("standA", 1000, 0);
				if (bool_0)
				{
					player.AddBlood(2000);
					bool_0 = false;
				}
			}
			else
			{
				physicalObj_2.PlayMovie("standB", 1000, 0);
			}
		}

		public override bool CanGameOver()
		{
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
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
			if (!simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		private void method_0()
		{
			list_0.Add(base.Game.CreateNpc(int_1, 1110, 520, 1, 1));
		}

		public NTM1088()
		{
			
			int_0 = 1132;
			bool_0 = true;
			int_1 = 24001;
			int_2 = 24002;
			list_0 = new List<SimpleNpc>();
			
		}
	}
}
