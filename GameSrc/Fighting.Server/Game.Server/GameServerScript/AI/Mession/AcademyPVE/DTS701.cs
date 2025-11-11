using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DTS701 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private Player player_0;

		private Player player_1;

		private SimpleBoss simpleBoss_0;

		private int int_5;

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
				int_0,
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.AddLoadingFile(1, $"bombs/{int_3}.swf", $"tank.resource.bombs.Bomb{int_3}");
			base.Game.AddLoadingFile(1, $"bombs/{int_4}.swf", $"tank.resource.bombs.Bomb{int_4}");
			base.Game.SetMap(1188);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (player_0 != null)
			{
				player_0.SetBall(int_3);
			}
			if (player_1 != null)
			{
				player_1.SetBall(int_4);
			}
		}

		public override void OnPrepareNewGame()
		{
			base.OnPrepareNewGame();
			method_0();
			player_0.BoltMove(833, 990, 0);
			player_1.BoltMove(388, 851, 0);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.BallCanDamage = int_3;
			livingConfig.IsFly = true;
			simpleBoss_0 = base.Game.CreateBoss(int_0, 986, 438, -1, 1, "born", livingConfig);
			simpleBoss_0.PlayMovie("stand", 4000, 0);
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
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int_5 = 1;
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
			if (int_5 >= base.Game.MissionInfo.TotalCount && base.Game.GetLivedLivings().Count == 0)
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
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.PlayerDetail.PlayerCharacter.masterID != 0 && allLivingPlayer.PlayerDetail.PlayerCharacter.Grade < 20)
				{
					player_0 = allLivingPlayer;
				}
				else
				{
					player_1 = allLivingPlayer;
				}
			}
		}

		public DTS701()
		{
			int_0 = 10101;
			int_1 = 10102;
			int_2 = 8104;
			int_3 = 32;
			int_4 = 35;
		}
	}
}
