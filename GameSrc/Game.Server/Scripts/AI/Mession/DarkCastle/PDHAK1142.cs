using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class PDHAK1142 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		public int CountKill
		{
			get
			{
				return int_5;
			}
			set
			{
				int_5 = value;
			}
		}

		public SimpleBoss Helper
		{
			get
			{
				return simpleBoss_1;
			}
			set
			{
				simpleBoss_1 = value;
			}
		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1750)
			{
				return 3;
			}
			if (score > 1675)
			{
				return 2;
			}
			if (score > 1600)
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
				int_2,
				int_1,
				int_0
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.AddLoadingFile(2, "image/game/effect/4/gate.swf", "game.asset.Gate");
			base.Game.SetMap(1142);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			simpleBoss_0 = base.Game.CreateBoss(int_2, 1520, 350, -1, 1, "", livingConfig);
			base.Game.SendHideBlood(simpleBoss_0, 0);
			method_0();
			base.Game.SendFreeFocus(1500, 250, 1, 2000, 3000);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (simpleBoss_1 != null && simpleBoss_1.IsLiving && simpleBoss_1.X >= 600 && base.Game.FindAllNpcLiving().Length == 0 && base.Game.CurrentTurnLiving is Player)
			{
				method_1();
			}
			if (simpleBoss_1 != null && !simpleBoss_1.IsLiving)
			{
				base.Game.RemoveLiving(simpleBoss_1, true);
				method_0();
			}
			if (base.Game.CurrentTurnLiving is Player && simpleBoss_1 != null && simpleBoss_1.IsLiving && (int)simpleBoss_0.Properties1 == 1)
			{
				simpleBoss_1.PlayMovie("standB", 1200, 0);
			}
		}

		private void method_0()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsHelper = true;
			simpleBoss_1 = base.Game.CreateBoss(int_0, 321, 746, 1, 0, "", livingConfig);
			simpleBoss_1.AddEffect(new ContinueReduceBloodEffect(2, int_3, simpleBoss_1), 0);
		}

		private void method_1()
		{
			int num = 0;
			list_0 = new List<SimpleNpc>();
			for (int i = 0; i < int_4; i++)
			{
				if (num > 0)
				{
					int num2 = base.Game.Random.Next(5, 110);
					list_0.Add(base.Game.CreateNpc(int_1, simpleBoss_1.X - num2, simpleBoss_1.Y, 0));
				}
				else
				{
					list_0.Add(base.Game.CreateNpc(int_1, simpleBoss_1.X - 20, simpleBoss_1.Y, 0));
				}
				num++;
			}
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (int_5 >= base.Game.MissionInfo.TotalCount)
			{
				return true;
			}
			if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_5;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (int_5 >= base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public PDHAK1142()
		{
			list_0 = new List<SimpleNpc>();
			int_0 = 4201;
			int_1 = 4203;
			int_2 = 4204;
			int_3 = 2000;
			int_4 = 1;
		}
	}
}
