using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class NTM1087 : AMissionControl
	{
		private int int_0;

		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_1;

		private int lkbcDgcreua;

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
			int[] npcIds = new int[3]
			{
				lkbcDgcreua,
				int_2,
				int_1
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(int_0);
		}

		public override void OnStartGame()
		{
			method_0();
		}

		public override void OnNewTurnStarted()
		{
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

		public void CreateBoss()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(200, 470, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 260, 560, 1, 1, "");
			simpleBoss_0.FallFrom(260, 620, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Say("Loài người kia，đến được đây quả nhiên có chút bản lĩnh！", 0, 3000);
			physicalObj_0.PlayMovie("in", 6000, 0);
			physicalObj_1.PlayMovie("in", 6000, 0);
			physicalObj_0.PlayMovie("out", 9000, 0);
			physicalObj_1.PlayMovie("out", 9000, 0);
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

		private void method_0()
		{
			int[,] array = new int[5, 2]
			{
				{
					260,
					620
				},
				{
					312,
					625
				},
				{
					350,
					621
				},
				{
					285,
					620
				},
				{
					331,
					625
				}
			};
			for (int i = 0; i <= 2; i++)
			{
				list_0.Add(base.Game.CreateNpc(lkbcDgcreua, array[i, 0], array[i, 1], 1, 1));
			}
			for (int j = 3; j <= 4; j++)
			{
				list_0.Add(base.Game.CreateNpc(int_2, array[j, 0], array[j, 1], 1, 1));
			}
		}

		public NTM1087()
		{
			
			int_0 = 2012;
			int_1 = 23003;
			lkbcDgcreua = 23001;
			int_2 = 23002;
			list_0 = new List<SimpleNpc>();
			
		}
	}
}
