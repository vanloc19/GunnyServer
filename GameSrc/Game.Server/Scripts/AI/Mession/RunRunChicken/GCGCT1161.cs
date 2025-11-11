using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.Messions
{
	public class GCGCT1161 : AMissionControl
	{
		private List<SimpleNpc> gjqcRpibHww;

		private List<SimpleNpc> list_0;

		private List<Point> list_1;

		private List<Point> list_2;

		private PhysicalObj physicalObj_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		private int int_7;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 930)
			{
				return 3;
			}
			if (score > 850)
			{
				return 2;
			}
			if (score > 775)
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
				int_6,
				int_7
			};
			base.Game.AddLoadingFile(2, "image/game/living/living176.swf", "game.living.Living176");
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1161);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(1200, 955, "kingmoive", "game.living.Living176", "in", 1, 0);
			method_1(int_3);
			method_0(int_2);
		}

		private void method_0(int int_8)
		{
			for (int i = 0; i < int_8; i++)
			{
				Point point = (i < list_1.Count) ? list_1[i] : list_1[base.Game.Random.Next(list_1.Count)];
				gjqcRpibHww.Add(base.Game.CreateNpc(int_6, point.X, point.Y, 0, -1));
			}
		}

		private void method_1(int int_8)
		{
			for (int i = 0; i < int_8; i++)
			{
				Point point = (i < list_2.Count) ? list_2[i] : list_2[base.Game.Random.Next(list_2.Count)];
				list_0.Add(base.Game.CreateNpc(int_6, point.X, point.Y, 0, -1));
			}
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (int_4 < int_2 && gjqcRpibHww.Count < int_0)
			{
				int num = (int_2 - int_4 > int_0 - gjqcRpibHww.Count) ? (int_0 - gjqcRpibHww.Count) : (int_2 - int_4);
				if (num > 0)
				{
					method_0(num);
				}
			}
			if (int_5 < int_3 && list_0.Count < int_1)
			{
				int num2 = (int_3 - int_5 > int_1 - list_0.Count) ? (int_1 - list_0.Count) : (int_3 - int_5);
				if (num2 > 0)
				{
					method_1(num2);
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			int_5 = 0;
			int_4 = 0;
			foreach (SimpleNpc item in gjqcRpibHww)
			{
				if (item.IsLiving)
				{
					int_4++;
				}
			}
			foreach (SimpleNpc item2 in list_0)
			{
				if (item2.IsLiving)
				{
					int_5++;
				}
			}
			if (list_0.Count >= int_1 && gjqcRpibHww.Count >= int_0 && base.Game.GetLivedLivings().Count <= 0)
			{
				return true;
			}
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn)
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
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public GCGCT1161()
		{

			gjqcRpibHww = new List<SimpleNpc>();
			list_0 = new List<SimpleNpc>();
			list_1 = new List<Point>
			{
				new Point(958, 950),
				new Point(1400, 950),
				new Point(1034, 950),
				new Point(1472, 950)
			};
			list_2 = new List<Point>
			{
				new Point(1150, 950),
				new Point(1346, 950)
			};
			int_0 = 15;
			int_1 = 5;
			int_2 = 6;
			int_3 = 2;
			int_6 = 7102;
			int_7 = 7101;

		}
	}
}
