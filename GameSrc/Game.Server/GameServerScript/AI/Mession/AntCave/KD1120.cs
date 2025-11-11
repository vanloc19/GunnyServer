using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class KD1120 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private int int_0;

		private int[] int_1;

		private int[] int_2;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1870)
			{
				return 3;
			}
			if (score > 1825)
			{
				return 2;
			}
			if (score > 1780)
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
				int_1[0],
				int_1[1]
			};
			int[] npcIds2 = new int[4]
			{
				int_1[1],
				int_1[0],
				int_1[0],
				int_1[0]
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1120);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			int num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 52, 206, 1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 100, 207, 1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 155, 208, 1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 210, 207, 1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 253, 207, 1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 1275, 208, -1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 1325, 206, -1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 1360, 208, -1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 1410, 206, -1, 1));
			num = base.Game.Random.Next(0, int_1.Length);
			list_0.Add(base.Game.CreateNpc(int_1[num], 1475, 208, -1, 1));
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.PveGameDelay = 0;
			}
			if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || base.Game.GetLivedLivings().Count >= 10)
			{
				return;
			}
			for (int i = 0; i < 10 - base.Game.GetLivedLivings().Count; i++)
			{
				if (list_0.Count == base.Game.MissionInfo.TotalCount)
				{
					break;
				}
				int num = base.Game.Random.Next(0, int_2.Length);
				int num2 = int_2[num];
				num = base.Game.Random.Next(0, int_1.Length);
				if (num == 1 && GetNpcCountByID(int_1[1]) < 10)
				{
					if (num2 > 700)
					{
						list_0.Add(base.Game.CreateNpc(int_1[1], num2, 506, -1, 1));
					}
					else
					{
						list_0.Add(base.Game.CreateNpc(int_1[1], num2, 506, 1, 1));
					}
				}
				else if (num2 > 700)
				{
					list_0.Add(base.Game.CreateNpc(int_1[1], num2, 506, -1, 1));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_1[1], num2, 506, 1, 1));
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
			base.CanGameOver();
			int_0 = 0;
			foreach (SimpleNpc item in list_0)
			{
				if (item.IsLiving)
				{
					flag = false;
				}
				else
				{
					int_0++;
				}
			}
			if (flag && int_0 == base.Game.MissionInfo.TotalCount)
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
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		protected int GetNpcCountByID(int Id)
		{
			int num = 0;
			foreach (SimpleNpc item in list_0)
			{
				if (item.NpcInfo.ID == Id)
				{
					num++;
				}
			}
			return num;
		}

		public KD1120()
		{

			list_0 = new List<SimpleNpc>();
			int_1 = new int[2]
			{
				2001,
				2002
			};
			int_2 = new int[10]
			{
				52,
				115,
				183,
				253,
				320,
				1206,
				1275,
				1342,
				1410,
				1475
			};

		}
	}
}
