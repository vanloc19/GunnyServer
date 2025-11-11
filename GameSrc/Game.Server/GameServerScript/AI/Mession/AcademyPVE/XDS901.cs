using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class XDS901 : AMissionControl
	{
		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private Player player_0;

		private Player player_1;

		private SimpleBoss simpleBoss_0;

		private List<SimpleNpc> list_0;

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
			base.Game.SetMap(1187);
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
			player_0.BoltMove(1549, 371, 0);
			player_1.BoltMove(574, 362, 0);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsTurn = false;
			list_0.Add(base.Game.CreateNpc(int_1, 1152, 547, 1, -1, livingConfig));
			list_0.Add(base.Game.CreateNpc(int_1, 1152, 755, 1, -1, livingConfig));
			list_0.Add(base.Game.CreateNpc(int_1, 1152, 963, 1, -1, livingConfig));
			foreach (SimpleNpc item in list_0)
			{
				base.Game.SendHideBlood(item, 0);
				base.Game.SendLivingActionMapping(item, "stand", "standA");
			}
			simpleBoss_0 = base.Game.CreateBoss(int_0, 1162, 1180, -1, 1, "born", livingConfig);
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
			SimpleBoss simpleBoss = base.Game.FindSingleSimpleBossID(int_0);
			if (simpleBoss != null && !simpleBoss.IsLiving)
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

		public XDS901()
		{
			int_0 = 9102;
			int_1 = 9103;
			int_2 = 8104;
			int_3 = 33;
			int_4 = 34;
			list_0 = new List<SimpleNpc>();
		}
	}
}
