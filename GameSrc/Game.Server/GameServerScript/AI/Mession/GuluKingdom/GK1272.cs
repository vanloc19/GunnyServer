using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GK1272 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_0;

		private int rumcuJicygk;

		private int int_1;

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
			base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
			base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
			int[] npcIds = new int[2]
			{
				rumcuJicygk,
				int_1
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
			simpleBoss_0 = base.Game.CreateBoss(rumcuJicygk, 770, -1500, -1, 1, "");
			simpleBoss_0.FallFrom(simpleBoss_0.X, simpleBoss_0.Y, "fall", 0, 1, 1000);
			simpleBoss_0.SetRelateDemagemRect(34, -35, 11, 18);
			simpleBoss_0.AddDelay(10);
			simpleBoss_0.Say("Bạn dám đột nhập vào Vương Quốc của tôi hãy sẵn sàng chết đi!", 0, 6000);
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
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
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

		public GK1272()
		{

			rumcuJicygk = 1203;
			int_1 = 1209;

		}

		static GK1272()
		{

			string_0 = new string[2]
			{
				"Gửi cho bạn trở về nhà!",
				"Một mình, bạn có ảo tưởng có thể đánh bại tôi?"
			};
			string_1 = new string[2]
			{
				"Rất tiếc!Đau ...",
				"Tôi cũng trên cùng của sự sống ..."
			};
		}
	}
}
