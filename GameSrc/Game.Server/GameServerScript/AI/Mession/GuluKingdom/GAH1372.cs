using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GAH1372 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private static string[] string_0;

		private static string[] string_1;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1540)
			{
				return 3;
			}
			if (score > 1410)
			{
				return 2;
			}
			if (score > 1285)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
			int[] npcIds = new int[2]
			{
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1073);
		}

		public override void OnStartGame()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(680, 330, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 1);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 770, -1500, -1, 1, "");
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "", 0, 2, 2000);
			simpleBoss_0.SetRelateDemagemRect(34, -35, 11, 18);
			simpleBoss_0.AddDelay(10);
			simpleBoss_0.Say("Dám xâm phạm địa bàn của ta, chờ chết!", 0, 6000);
			simpleBoss_0.PlayMovie("call", 5900, 0);
			physicalObj_0.PlayMovie("in", 9000, 0);
			simpleBoss_0.PlayMovie("weakness", 10000, 5000);
			physicalObj_1.PlayMovie("in", 9000, 0);
			physicalObj_0.PlayMovie("out", 15000, 0);
			base.Game.BossCardCount = 1;
			base.OnStartGame();
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1)
			{
				if (physicalObj_0 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_0, true);
					physicalObj_0 = null;
				}
				if (physicalObj_1 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_1, true);
					physicalObj_1 = null;
				}
			}
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (!simpleBoss_0.IsLiving)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			if (simpleBoss_0 == null)
			{
				return 0;
			}
			if (!simpleBoss_0.IsLiving)
			{
				return 1;
			}
			return base.UpdateUIData();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (!simpleBoss_0.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public override void DoOther()
		{
			base.DoOther();
			if (simpleBoss_0 != null)
			{
				int num = base.Game.Random.Next(0, string_0.Length);
				if (simpleBoss_0 != null)
				{
					simpleBoss_0.Say(string_0[num], 0, 0);
				}
			}
		}

		public override void OnShooted()
		{
			base.OnShooted();
			if (simpleBoss_0 != null && simpleBoss_0.IsLiving && int_0 == 0)
			{
				int num = base.Game.Random.Next(0, string_1.Length);
				simpleBoss_0.Say(string_1[num], 0, 1500);
				int_0 = 1;
			}
		}

		public GAH1372()
		{
			int_1 = 1303;
			int_2 = 1309;
		}

		static GAH1372()
		{
			string_0 = new string[2]
			{
				"Gửi cho bạn trở về nhà!",
				"Một mình, bạn có ảo tưởng có thể đánh bại tôi?"
			};
			string_1 = new string[2]
			{
				" Đau ah! Đau ...",
				"Quốc vương vạn tuế ..."
			};
		}
	}
}
