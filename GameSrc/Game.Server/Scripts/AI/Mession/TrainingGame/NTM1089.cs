using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class NTM1089 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int[] int_2;

		private int[] int_3;

		private int int_4;

		private int int_5;

		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

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
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.shikongAsset");
			int[] npcIds = new int[2]
			{
				int_4,
				int_5
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(int_0);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			int y = int_2[0];
			list_0.Add(base.Game.CreateNpc(int_4, 52, y, 1, -1));
			list_0.Add(base.Game.CreateNpc(int_4, 100, y, 1, -1));
			list_0.Add(base.Game.CreateNpc(int_4, 1120, y, 1, 1));
			list_0.Add(base.Game.CreateNpc(int_4, 1155, y, 1, 1));
		}

		public void CreateBoss()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(200, 200, "font", "game.asset.living.shikongAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_5, 160, 330, 1, 1, "");
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			physicalObj_0.PlayMovie("in", 6000, 0);
			physicalObj_1.PlayMovie("in", 6000, 0);
			physicalObj_0.PlayMovie("out", 9000, 0);
		}

		public override void OnNewTurnStarted()
		{
			if (base.Game.TurnIndex <= 1 || simpleBoss_0 != null || base.Game.GetLivedLivings().Count >= 4)
			{
				return;
			}
			for (int i = 0; i < 4 - base.Game.GetLivedLivings().Count; i++)
			{
				if (list_0.Count == 8)
				{
					break;
				}
				int num = base.Game.Random.Next(0, int_2.Length);
				int num2 = int_2[num];
				int direction = 1;
				if (num2 < 200)
				{
					direction = -1;
				}
				list_0.Add(base.Game.CreateNpc(int_4, num2, int_3[0], 1, direction));
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			bool flag = true;
			base.CanGameOver();
			int_1 = 0;
			foreach (SimpleNpc item in list_0)
			{
				if (item.IsLiving)
				{
					flag = false;
				}
				else
				{
					int_1++;
				}
			}
			if (flag && int_1 == 8 && simpleBoss_0 == null)
			{
				CreateBoss();
			}
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			if (simpleBoss_0 == null)
			{
				return 0;
			}
			if (!simpleBoss_0.IsLiving)
			{
				return 1;
			}
			return base.UpdateUIData();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_0 != null)
			{
				if (!simpleBoss_0.IsLiving)
				{
					base.Game.IsWin = true;
				}
				else
				{
					base.Game.IsWin = false;
				}
			}
		}

		public NTM1089()
		{
			
			int_0 = 1129;
			int_2 = new int[4]
			{
				52,
				115,
				1155,
				1106
			};
			int_3 = new int[4]
			{
				388,
				392,
				399,
				387
			};
			int_4 = 25001;
			int_5 = 25002;
			list_0 = new List<SimpleNpc>();
			
		}
	}
}
