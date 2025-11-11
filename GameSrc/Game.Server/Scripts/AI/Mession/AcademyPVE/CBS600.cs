using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class CBS600 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private List<SimpleNpc> list_1;

		private int int_0;

		private int int_1;

		private int int_2;

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
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1184);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			method_0();
		}

		public override void OnNewTurnStarted()
		{
			if (base.Game.GetLivedLivings().Count <= 0)
			{
				base.Game.PveGameDelay = 0;
				method_0();
			}
		}

		private void method_0()
		{
			int num = base.Game.MissionInfo.TotalCount - base.Game.TotalKillCount;
			if (num <= 0)
			{
				return;
			}
			int num2 = 4;
			if (4 > num)
			{
				num2 = num;
			}
			for (int i = 0; i < num2; i++)
			{
				if (i == 0)
				{
					list_1.Add(base.Game.CreateNpc(int_2, 49, 810, 1, -1, base.Game.BaseLivingConfig()));
					continue;
				}
				int x = base.Game.Random.Next(78, 1691);
				int y = base.Game.Random.Next(74, 600);
				LivingConfig livingConfig = base.Game.BaseLivingConfig();
				livingConfig.IsFly = true;
				list_0.Add(base.Game.CreateNpc(int_1, x, y, 1, -1, livingConfig));
			}
			int_0++;
			base.Game.PveGameDelay = 0;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			if (base.Game.GetAllLivingPlayers().Count < 2)
			{
				return true;
			}
			if (base.Game.TotalKillCount < base.Game.MissionInfo.TotalCount && base.Game.TurnIndex <= base.Game.MissionInfo.TotalTurn - 1)
			{
				return false;
			}
			return true;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (base.Game.TotalKillCount >= base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public CBS600()
		{
			list_0 = new List<SimpleNpc>();
			list_1 = new List<SimpleNpc>();
			int_1 = 11001;
			int_2 = 10002;
		}
	}
}
