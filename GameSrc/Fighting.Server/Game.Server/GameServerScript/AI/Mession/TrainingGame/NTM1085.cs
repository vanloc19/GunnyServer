using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class NTM1085 : AMissionControl
	{
		private int int_0;

		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_1;

		private int int_2;

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
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
			base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
			base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
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
			method_0();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			foreach (SimpleNpc item in list_0)
			{
				if (item.IsLiving)
				{
					return false;
				}
			}
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				return true;
			}
			if (simpleBoss_0 == null)
			{
				CreateBoss();
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
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public void CreateBoss()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(730, 510, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 850, 360, -1, 1, "");
			simpleBoss_0.FallFrom(850, 410, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Say("Dám bén mảng tới đây à?", 0, 6000);
			physicalObj_0.PlayMovie("in", 9000, 0);
			physicalObj_1.PlayMovie("in", 9000, 0);
			physicalObj_0.PlayMovie("out", 15000, 0);
		}

		private void method_0()
		{
			list_0.Add(base.Game.CreateNpc(int_2, 775, 553, 1, 1));
		}

		public NTM1085()
		{
			
			int_0 = 2013;
			int_1 = 21002;
			int_2 = 21001;
			list_0 = new List<SimpleNpc>();
			
		}
	}
}
