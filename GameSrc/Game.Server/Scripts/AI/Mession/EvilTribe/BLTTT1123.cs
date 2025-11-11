using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class BLTTT1123 : AMissionControl
	{
		private List<SimpleNpc> list_0;

		private SimpleBoss simpleBoss_0;

		protected int m_maxBlood;

		protected int m_blood;

		private SimpleBoss simpleBoss_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private SimpleBoss simpleBoss_2;

		private int int_6;

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
			int[] npcIds = new int[4]
			{
				int_0,
				int_1,
				int_2,
				int_3
			};
			int[] npcIds2 = new int[3]
			{
				int_0,
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
			base.Game.SetMap(1123);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			simpleBoss_1 = base.Game.CreateBoss(int_3, 100, 444, 1, 0, "");
			simpleBoss_1.FallFrom(simpleBoss_1.X, simpleBoss_1.Y, "", 0, 0, 2000);
			simpleBoss_1.PlayMovie("castA", 500, 0);
			simpleBoss_1.Say("Các ngươi dám mò vào đây là chết chắc rồi! Sẽ không có lối ra cho tụi bay đâu...", 0, 300);
			simpleBoss_1.CallFuction(CreateStarGame, 2500);
		}

		public void CreateStarGame()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsHelper = true;
			livingConfig.ReduceBloodStart = 2;
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1100, 444, -1, 1, "born", livingConfig);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "", 0, 0, 1000, null);
			CreateBoss();
			method_0();
			base.Game.SendObjectFocus(simpleBoss_0, 1, 500, 3000);
			simpleBoss_0.Say(LanguageMgr.GetTranslation("Hồi máu cho tôi, tôi sẽ dẫn các cậu ra khỏi đây !"), 0, 1500, 0);
		}

		public void CreateBoss()
		{
			simpleBoss_2 = base.Game.CreateBoss(int_2, 300, 444, 1, 0, "");
			simpleBoss_2.SetRelateDemagemRect(simpleBoss_2.NpcInfo.X, simpleBoss_2.NpcInfo.Y, simpleBoss_2.NpcInfo.Width, simpleBoss_2.NpcInfo.Height);
			simpleBoss_2.FallFrom(simpleBoss_2.X, simpleBoss_2.Y, "", 0, 0, 1000, null);
			int_6 = 1;
		}

		public void CreateOutGame()
		{
			simpleBoss_1.Blood = 0;
			simpleBoss_1.Die();
			base.Game.RemoveLiving(simpleBoss_1.Id);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (simpleBoss_2 != null && !simpleBoss_2.IsLiving && !(base.Game.CurrentLiving is Player))
			{
				if (int_6 <= 0)
				{
					CreateBoss();
				}
				else
				{
					int_6--;
				}
			}
			if (method_1() <= 0)
			{
				method_0();
			}
		}

		private void method_0()
		{
			int num = 350;
			for (int i = 0; i < int_4; i++)
			{
				list_0.Add(base.Game.CreateNpc(int_0, num, 344, 1, 1));
				num += 50;
			}
		}

		private int method_1()
		{
			int num = 0;
			foreach (SimpleNpc item in list_0)
			{
				if (item.IsLiving)
				{
					num++;
				}
			}
			return num;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex == 1)
			{
				simpleBoss_1.PlayMovie("out", 0, 2000);
				simpleBoss_1.CallFuction(CreateOutGame, 1200);
			}
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (simpleBoss_0 != null && simpleBoss_0.Blood >= simpleBoss_0.NpcInfo.Blood)
			{
				return true;
			}
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int_5++;
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
			if (simpleBoss_0.Blood >= simpleBoss_0.NpcInfo.Blood)
			{
				simpleBoss_0.PlayMovie("grow", 0, 3000);
				base.Game.IsWin = true;
			}
			if (!simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = false;
			}
		}

		public BLTTT1123()
		{
			list_0 = new List<SimpleNpc>();
			int_0 = 3102;
			int_1 = 3107;
			int_2 = 3105;
			int_3 = 3108;
			int_4 = 3;
		}
	}
}
