using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GD1072 : AMissionControl
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
			base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
			int[] npcIds = new int[2]
			{
				int_1,
				int_2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1073);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(680, 330, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 0);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 770, -1500, -1, 1, "");
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(34, -35, 11, 18);
			simpleBoss_0.AddDelay(10);
			simpleBoss_0.Say("Dám xâm phạm địa bàn của ta, chán sống rồi à?", 0, 6000);
			simpleBoss_0.PlayMovie("call", 5900, 0);
			physicalObj_0.PlayMovie("in", 9000, 0);
			simpleBoss_0.PlayMovie("weakness", 10000, 5000);
			physicalObj_1.PlayMovie("in", 9000, 0);
			physicalObj_0.PlayMovie("out", 15000, 0);
			base.Game.BossCardCount = 1;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			int_0 = 0;
			if (base.Game.TurnIndex > 1)
			{
				if (physicalObj_0 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_0, sendToClient: true);
					physicalObj_0 = null;
				}
				if (physicalObj_1 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_1, sendToClient: true);
					physicalObj_1 = null;
				}
			}
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
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

		public GD1072()
		{
			int_1 = 1003;
			int_2 = 1009;
		}

		static GD1072()
		{
			string_0 = new string[2]
			{
				"Ta sẽ cho ngươi trở về nhà!",
				"Một mình à? Ngươi đang ảo tưởng có thể đánh bại ta?"
			};
			string_1 = new string[2]
			{
				" Đau ah! Đau ...",
				"Quốc vương vạn tuế ..."
			};
		}
	}
}
