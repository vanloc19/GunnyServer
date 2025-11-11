using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class NTM1086 : AMissionControl
	{
		private int int_0;

		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj nsycDkNsptf;

		private int int_1;

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
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.qiheibojueAsset");
			base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout86.swf", "bullet86");
			base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet86.swf", "bullet86");
			int[] npcIds = new int[1]
			{
				int_1
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(int_0);
		}

		public override void OnStartGame()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			nsycDkNsptf = base.Game.Createlayer(680, 330, "font", "game.asset.living.qiheibojueAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 750, 420, -1, 1, "");
			simpleBoss_0.FallFrom(750, 520, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Say("Đến đúng lúc lắm，ta đang chán đây！", 0, 4000);
			physicalObj_0.PlayMovie("in", 6000, 0);
			nsycDkNsptf.PlayMovie("in", 6000, 0);
			physicalObj_0.PlayMovie("out", 9000, 0);
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1 && nsycDkNsptf != null)
			{
				base.Game.RemovePhysicalObj(nsycDkNsptf, sendToClient: true);
				nsycDkNsptf = null;
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

		public NTM1086()
		{
			
			int_0 = 1015;
			int_1 = 22001;
			
		}
	}
}
