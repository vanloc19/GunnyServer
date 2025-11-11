using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class NTM1083 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

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
			if (list_0.Count == int_3)
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
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.CanGetProp = true;
			}
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
			list_0.Add(base.Game.CreateNpc(int_2, 775, 553, 1, 1));
		}

		public NTM1083()
		{
			
			int_0 = 2013;
			int_1 = 23003;
			int_2 = 21001;
			int_3 = 3;
			list_0 = new List<SimpleNpc>();
			
		}
	}
}
