using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GD1071 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private int int_0;

		private int int_1;

		private int int_2;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 600)
			{
				return 3;
			}
			if (score > 520)
			{
				return 2;
			}
			if (score > 450)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[1]
			{
				1001
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1072);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			for (int i = 0; i < 4; i++)
			{
				int_0++;
				if (i < 1)
				{
					list_0.Add(base.Game.CreateNpc(int_2, 900 + (i + 1) * 100, 505, -1, 1));
				}
				else if (i < 3)
				{
					list_0.Add(base.Game.CreateNpc(int_2, 920 + (i + 1) * 100, 505, -1, 1));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_2, 1000 + (i + 1) * 100, 515, -1, 1));
				}
			}
			int_0++;
			list_0.Add(base.Game.CreateNpc(int_2, 1467, 495, -1, 1));
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay)
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (int_0 < 15)
				{
					int_0++;
					if (i < 1)
					{
						list_0.Add(base.Game.CreateNpc(int_2, 900 + (i + 1) * 100, 505, -1, 1));
					}
					else if (i < 3)
					{
						list_0.Add(base.Game.CreateNpc(int_2, 920 + (i + 1) * 100, 505, -1, 1));
					}
					else
					{
						list_0.Add(base.Game.CreateNpc(int_2, 1000 + (i + 1) * 100, 515, -1, 1));
					}
				}
			}
			if (int_0 < 15)
			{
				int_0++;
				list_0.Add(base.Game.CreateNpc(int_2, 1467, 495, -1, 1));
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
			if (flag && int_1 == 15)
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
			list.Add(new LoadingFileInfo(2, "image/map/2", ""));
			base.Game.SendLoadResource(list);
		}

		public GD1071()
		{
			list_0 = new List<SimpleNpc>();
			int_2 = 1001;
		}
	}
}
