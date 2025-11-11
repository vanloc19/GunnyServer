using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GCGCD1162 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private SimpleBoss simpleBoss_2;

		private int int_0;

		private int int_1;

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
			int[] npcIds = new int[1]
			{
				int_0
			};
			int[] npcIds2 = new int[1]
			{
				int_0
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
			base.Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
			base.Game.AddLoadingFile(2, "image/game/effect/7/jinquhd.swf", "asset.game.seven.jinquhd");
			base.Game.SetMap(1162);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			simpleBoss_1 = base.Game.CreateBoss(int_0, 1680, 315, -1, 1, "");
			simpleBoss_0 = base.Game.CreateBoss(int_0, 1615, 565, -1, 1, "");
			simpleBoss_2 = base.Game.CreateBoss(int_0, 1600, 849, -1, 1, "");
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "", 0, 0, 2000);
			simpleBoss_2.FallFrom(simpleBoss_2.X, simpleBoss_2.Y, "", 0, 0, 2000);
			simpleBoss_1.FallFrom(simpleBoss_1.X, simpleBoss_1.Y, "", 0, 0, 2000);
			simpleBoss_1.SetRelateDemagemRect(simpleBoss_1.NpcInfo.X, simpleBoss_1.NpcInfo.Y, simpleBoss_1.NpcInfo.Width, simpleBoss_1.NpcInfo.Height);
			simpleBoss_2.SetRelateDemagemRect(simpleBoss_2.NpcInfo.X, simpleBoss_2.NpcInfo.Y, simpleBoss_2.NpcInfo.Width, simpleBoss_2.NpcInfo.Height);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_2.Delay += 2;
			simpleBoss_0.Delay++;
			method_0();
		}

		private void method_0()
		{
			base.Game.SendObjectFocus(simpleBoss_1, 1, 1000, 0);
			simpleBoss_1.PlayMovie("speak", 1500, 0);
			simpleBoss_1.Say("Loài người sao lại mò tới đây?", 0, 1500);
			base.Game.SendObjectFocus(simpleBoss_0, 1, 4000, 0);
			simpleBoss_0.PlayMovie("speak", 4500, 0);
			simpleBoss_0.Say("Không cần biết. Tiêu diệt bọn chúng!", 0, 4500);
			base.Game.SendObjectFocus(simpleBoss_2, 1, 8000, 0);
			simpleBoss_2.PlayMovie("speak", 8500, 0);
			simpleBoss_2.Say("Bây giờ bọn ngươi bỏ chạy còn kịp đó.", 0, 8500, 10000);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (base.Game.CurrentTurnLiving == null || !(base.Game.CurrentTurnLiving is SimpleBoss))
			{
				return;
			}
			int_1++;
			switch (int_1)
			{
				case 4:
					if (simpleBoss_0.IsLiving)
					{
						simpleBoss_0.Properties1 = 1;
					}
					else if (simpleBoss_1.IsLiving)
					{
						simpleBoss_1.Properties1 = 1;
					}
					else if (simpleBoss_2.IsLiving)
					{
						simpleBoss_2.Properties1 = 1;
					}
					break;
				case 1:
					if (simpleBoss_2.IsLiving)
					{
						simpleBoss_2.Properties1 = 1;
					}
					else if (simpleBoss_0.IsLiving)
					{
						simpleBoss_0.Properties1 = 1;
					}
					else if (simpleBoss_1.IsLiving)
					{
						simpleBoss_1.Properties1 = 1;
					}
					break;
				case 9:
					int_1 = 0;
					break;
				case 7:
					if (simpleBoss_1.IsLiving)
					{
						simpleBoss_1.Properties1 = 1;
					}
					else if (simpleBoss_2.IsLiving)
					{
						simpleBoss_2.Properties1 = 1;
					}
					else if (simpleBoss_0.IsLiving)
					{
						simpleBoss_0.Properties1 = 1;
					}
					break;
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			if (simpleBoss_1 != null && !simpleBoss_1.IsLiving && simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleBoss_2 != null && !simpleBoss_2.IsLiving)
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
			return base.Game.TotalKillCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_1 != null && !simpleBoss_1.IsLiving && simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleBoss_2 != null && !simpleBoss_2.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public GCGCD1162()
		{

			int_0 = 7011;

		}
	}
}
