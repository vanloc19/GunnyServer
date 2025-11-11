using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GAH1371 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private List<SimpleNpc> list_1;

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
			base.Game.LoadResources(npcIds);
			base.Game.SetMap(1072);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			for (int i = 0; i < 4; i++)
			{
				int_2++;
				if (i < 1)
				{
					list_0.Add(base.Game.CreateNpc(int_6, 900 + (i + 1) * 100, 505, -1, 1));
				}
				else if (i < 3)
				{
					list_0.Add(base.Game.CreateNpc(int_6, 920 + (i + 1) * 100, 505, -1, 1));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_6, 1000 + (i + 1) * 100, 515, -1, 1));
				}
			}
			int_3++;
			list_1.Add(base.Game.CreateNpc(int_7, 1467, 495, -1, 1));
		}

		public override void OnNewTurnStarted()
		{
			int_0 = int_2 - int_4;
			int_1 = int_3 - int_5;
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || (int_1 == 3 && int_0 == 12))
			{
				return;
			}
			if (int_2 < 12 && int_3 < 3)
			{
				for (int i = 0; i < 4; i++)
				{
					int_2++;
					if (i < 1)
					{
						list_0.Add(base.Game.CreateNpc(int_6, 900 + (i + 1) * 100, 505, -1, 1));
					}
					else if (i < 3)
					{
						list_0.Add(base.Game.CreateNpc(int_6, 920 + (i + 1) * 100, 505, -1, 1));
					}
					else
					{
						list_0.Add(base.Game.CreateNpc(int_6, 1000 + (i + 1) * 100, 515, -1, 1));
					}
				}
				int_3++;
				list_1.Add(base.Game.CreateNpc(int_7, 1467, 495, -1, 1));
			}
			else
			{
				if (int_0 >= 12)
				{
					return;
				}
				if (12 - int_0 >= 4)
				{
					for (int j = 0; j < 4; j++)
					{
						if (int_2 < 20 && int_0 != 12)
						{
							int_2++;
							if (j < 1)
							{
								list_0.Add(base.Game.CreateNpc(int_6, 900 + (j + 1) * 100, 505, -1, 1));
							}
							else if (j < 3)
							{
								list_0.Add(base.Game.CreateNpc(int_6, 920 + (j + 1) * 100, 505, -1, 1));
							}
							else
							{
								list_0.Add(base.Game.CreateNpc(int_6, 1000 + (j + 1) * 100, 515, -1, 1));
							}
						}
					}
				}
				else if (12 - int_0 > 0)
				{
					for (int k = 0; k < 12 - int_0; k++)
					{
						if (int_2 < 20 && int_0 != 12)
						{
							int_2++;
							if (k < 1)
							{
								list_0.Add(base.Game.CreateNpc(int_6, 900 + (k + 1) * 100, 505, -1, 1));
							}
							else if (k < 3)
							{
								list_0.Add(base.Game.CreateNpc(int_6, 920 + (k + 1) * 100, 505, -1, 1));
							}
							else
							{
								list_0.Add(base.Game.CreateNpc(int_6, 1000 + (k + 1) * 100, 515, -1, 1));
							}
						}
					}
				}
				if (int_1 < 3 && int_3 < 5)
				{
					int_3++;
					list_1.Add(base.Game.CreateNpc(int_7, 1467, 495, -1, 1));
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			bool flag = true;
			int_4 = 0;
			int_5 = 0;
			foreach (SimpleNpc item in list_0)
			{
				if (item.IsLiving)
				{
					flag = false;
				}
				else
				{
					int_4++;
				}
			}
			foreach (SimpleNpc item2 in list_1)
			{
				if (item2.IsLiving)
				{
					flag = false;
				}
				else
				{
					int_5++;
				}
			}
			if (flag && int_2 == 20 && int_3 == 5)
			{
				base.Game.IsWin = true;
				return true;
			}
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
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
			List<LoadingFileInfo> list = new List<LoadingFileInfo>();
			list.Add(new LoadingFileInfo(2, "image/map/3", ""));
			base.Game.SendLoadResource(list);
		}

		public GAH1371()
		{
			list_0 = new List<SimpleNpc>();
			list_1 = new List<SimpleNpc>();
			int_6 = 1301;
			int_7 = 1302;
		}
	}
}
