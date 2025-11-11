using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class KT1121 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private static string[] string_0;

		private static string[] string_1;

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
			int[] npcIds = new int[2]
			{
				int_1,
				int_0
			};
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/51.swf", "tank.resource.bombs.Bomb51");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.AntQueenAsset");
			base.Game.SetMap(1121);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(1131, 150, "font", "game.asset.living.AntQueenAsset", "out", 1, 1);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1316, 444, -1, 1, "");
			simpleBoss_0.SetRelateDemagemRect(-42, -200, 84, 194);
			simpleBoss_0.Say(LanguageMgr.GetTranslation("Rương là của ta, bảo bối điều là của ta, chỉ cần ta nhìn thấy đều là của ta!"), 0, 200, 0);
			physicalObj_0.PlayMovie("in", 6000, 0);
			physicalObj_1.PlayMovie("in", 6100, 0);
			physicalObj_0.PlayMovie("out", 10000, 1000);
			physicalObj_1.PlayMovie("out", 9900, 0);
		}

		public override void OnNewTurnStarted()
		{
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
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
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int_3++;
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_3;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
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
			int num = base.Game.Random.Next(0, string_0.Length);
			simpleBoss_0.Say(string_0[num], 0, 0);
		}

		public override void OnShooted()
		{
			if (simpleBoss_0.IsLiving && int_2 == 0)
			{
				int num = base.Game.Random.Next(0, string_1.Length);
				simpleBoss_0.Say(string_1[num], 0, 1500);
				int_2 = 1;
			}
		}

		public KT1121()
		{

			int_0 = 2104;
			int_1 = 2103;

		}

		static KT1121()
		{

			string_0 = new string[3]
			{
				"Ah, mặt tôi .....",
				"Ặc, giáp trụ xinh đẹp của mình đã bị trầy rồi.....",
				"Ui za, đau quá !"
			};
			string_1 = new string[1]
			{
				"Ah, của tôi hết đừng lấy！"
			};
		}
	}
}
