using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class BLTTT1122 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

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
			int[] npcIds = new int[2]
			{
				int_4,
				int_3
			};
			base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1122);
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
				int_1++;
				if (i < 1)
				{
					list_0.Add(base.Game.CreateNpc(int_3, 900 + (i + 1) * 100, 444, -1, 1));
				}
				else if (i < 3)
				{
					list_0.Add(base.Game.CreateNpc(int_3, 920 + (i + 1) * 100, 444, -1, 1));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_3, 1000 + (i + 1) * 100, 444, -1, 1));
				}
			}
			int_1++;
			simpleBoss_0 = base.Game.CreateBoss(int_4, 1300, 444, -1, 10, "");
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "", 0, 0, 1000, null);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			if (base.Game.TurnIndex <= 1 || base.Game.GetLivedLivings().Count >= 4)
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (int_1 < 16)
				{
					int_1++;
					if (i < 1)
					{
						list_0.Add(base.Game.CreateNpc(int_3, 900 + (i + 1) * 100, 444, -1, 1));
					}
					else if (i < 3)
					{
						list_0.Add(base.Game.CreateNpc(int_3, 920 + (i + 1) * 100, 444, -1, 1));
					}
					else
					{
						list_0.Add(base.Game.CreateNpc(int_3, 1000 + (i + 1) * 100, 444, -1, 1));
					}
				}
			}
			if (int_1 < 16 && !simpleBoss_0.IsLiving)
			{
				int_1++;
				simpleBoss_0 = base.Game.CreateBoss(int_4, 1300, 444, -1, 10, "");
				simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "", 0, 0, 1000, null);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (base.Game.GetLivedLivings().Count == 0 && !simpleBoss_0.IsLiving && base.Game.TotalKillCount == base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = true;
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
			if (base.Game.GetLivedLivings().Count == 0 && !simpleBoss_0.IsLiving && base.Game.TotalKillCount == base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public BLTTT1122()
		{
			list_0 = new List<SimpleNpc>();
			int_3 = 3101;
			int_4 = 3104;
		}
	}
}
