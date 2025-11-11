using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class GK1271 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private List<SimpleNpc> YeDcuuLhhwT;

		private int int_0;

		private int int_1;

		private int mgvcuEqUsHK;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

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
				int_5,
				int_6
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1072);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			for (int i = 0; i < 4; i++)
			{
				mgvcuEqUsHK++;
				if (i < 1)
				{
					list_0.Add(base.Game.CreateNpc(int_5, 900 + (i + 1) * 100, 505, -1, 1));
				}
				else if (i < 3)
				{
					list_0.Add(base.Game.CreateNpc(int_5, 920 + (i + 1) * 100, 505, -1, 1));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_5, 1000 + (i + 1) * 100, 515, -1, 1));
				}
			}
			int_2++;
			YeDcuuLhhwT.Add(base.Game.CreateNpc(int_6, 1467, 495, -1, 1));
		}

		public override void OnNewTurnStarted()
		{
			int_0 = mgvcuEqUsHK - int_3;
			int_1 = int_2 - int_4;
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || (int_1 == 3 && int_0 == 12))
			{
				return;
			}
			if (mgvcuEqUsHK < 12 && int_2 < 3)
			{
				for (int i = 0; i < 4; i++)
				{
					mgvcuEqUsHK++;
					if (i < 1)
					{
						list_0.Add(base.Game.CreateNpc(int_5, 900 + (i + 1) * 100, 505, -1, 1));
					}
					else if (i < 3)
					{
						list_0.Add(base.Game.CreateNpc(int_5, 920 + (i + 1) * 100, 505, -1, 1));
					}
					else
					{
						list_0.Add(base.Game.CreateNpc(int_5, 1000 + (i + 1) * 100, 515, -1, 1));
					}
				}
				int_2++;
				YeDcuuLhhwT.Add(base.Game.CreateNpc(int_6, 1467, 495, -1, 1));
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
						if (mgvcuEqUsHK < 15 && int_0 != 12)
						{
							mgvcuEqUsHK++;
							if (j < 1)
							{
								list_0.Add(base.Game.CreateNpc(int_5, 900 + (j + 1) * 100, 505, -1, 1));
							}
							else if (j < 3)
							{
								list_0.Add(base.Game.CreateNpc(int_5, 920 + (j + 1) * 100, 505, -1, 1));
							}
							else
							{
								list_0.Add(base.Game.CreateNpc(int_5, 1000 + (j + 1) * 100, 515, -1, 1));
							}
						}
					}
				}
				else if (12 - int_0 > 0)
				{
					for (int k = 0; k < 12 - int_0; k++)
					{
						if (mgvcuEqUsHK < 15 && int_0 != 12)
						{
							mgvcuEqUsHK++;
							if (k < 1)
							{
								list_0.Add(base.Game.CreateNpc(int_5, 900 + (k + 1) * 100, 505, -1, 1));
							}
							else if (k < 3)
							{
								list_0.Add(base.Game.CreateNpc(int_5, 920 + (k + 1) * 100, 505, -1, 1));
							}
							else
							{
								list_0.Add(base.Game.CreateNpc(int_5, 1000 + (k + 1) * 100, 515, -1, 1));
							}
						}
					}
				}
				if (int_1 < 3 && int_2 < 5)
				{
					int_2++;
					YeDcuuLhhwT.Add(base.Game.CreateNpc(int_6, 1467, 495, -1, 1));
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
			int_3 = 0;
			int_4 = 0;
			foreach (SimpleNpc item in list_0)
			{
				if (item.IsLiving)
				{
					flag = false;
				}
				else
				{
					int_3++;
				}
			}
			foreach (SimpleNpc item2 in YeDcuuLhhwT)
			{
				if (item2.IsLiving)
				{
					flag = false;
				}
				else
				{
					int_4++;
				}
			}
			if (flag && mgvcuEqUsHK == 15 && int_2 == 5)
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

		public GK1271()
		{

			list_0 = new List<SimpleNpc>();
			YeDcuuLhhwT = new List<SimpleNpc>();
			int_5 = 1201;
			int_6 = 1202;

		}
	}
}
