using System.Collections.Generic;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GZS800 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private List<SimpleNpc> list_1;

		private SimpleBoss simpleBoss_0;

		private int int_0;

		private bool bool_0;

		private int int_1;

		private int int_2;

		private int int_3;

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
			int[] npcIds = new int[3]
			{
				int_1,
				int_2,
				int_3
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1184);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			simpleBoss_0 = base.Game.CreateBoss(int_3, 1159, 850, -1, 1, "born");
			simpleBoss_0.CallFuction(method_0, 4000);
		}

		public override void OnNewTurnStarted()
		{
			if (base.Game.GetLivedLivings().Count <= 0)
			{
				base.Game.PveGameDelay = 0;
				method_2();
			}
		}

		private void method_0()
		{
			simpleBoss_0.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.GZS800.Msg1"), 0, 0);
			simpleBoss_0.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.GZS800.Msg2"), 0, 2000);
			simpleBoss_0.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.GZS800.Msg3"), 0, 7000);
			simpleBoss_0.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.GZS800.Msg4"), 0, 11000);
			simpleBoss_0.PlayMovie("out", 14000, 3000);
			simpleBoss_0.CallFuction(method_1, 18000);
		}

		private void method_1()
		{
			base.Game.RemoveLiving(simpleBoss_0, sendToClient: true);
			method_2();
		}

		private void method_2()
		{
			int num = base.Game.MissionInfo.TotalCount - base.Game.TotalKillCount;
			if (num <= 0)
			{
				return;
			}
			int num2 = base.Game.Random.Next(3, 6);
			if (num2 > num)
			{
				num2 = num;
			}
			for (int i = 0; i < num2; i++)
			{
				if (base.Game.Random.Next(100) < 40)
				{
					if (bool_0)
					{
						list_1.Add(base.Game.CreateNpc(int_2, 1748 - i * 100, 877, 1, -1, base.Game.BaseLivingConfig()));
					}
					else
					{
						list_1.Add(base.Game.CreateNpc(int_2, 25 + i * 50, 909, 1, 1, base.Game.BaseLivingConfig()));
					}
				}
				else if (bool_0)
				{
					list_0.Add(base.Game.CreateNpc(int_1, 1700 - i * 100, 877, 1, -1, base.Game.BaseLivingConfig()));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_1, 30 + i * 50, 909, 1, 1, base.Game.BaseLivingConfig()));
				}
			}
			bool_0 = ((!bool_0) ? true : false);
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

		public GZS800()
		{
			list_0 = new List<SimpleNpc>();
			list_1 = new List<SimpleNpc>();
			int_1 = 8001;
			int_2 = 8002;
			int_3 = 8104;
		}
	}
}
