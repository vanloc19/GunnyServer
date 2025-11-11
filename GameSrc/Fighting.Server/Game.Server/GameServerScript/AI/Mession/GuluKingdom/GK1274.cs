using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GK1274 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private int int_0;

		private int jokcubUcRpN;

		private int mvOcunRbaTM;

		private int int_1;

		private int int_2;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private static string[] string_0;

		private static string[] GytcujpotKg;

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
			int[] npcIds = new int[2]
			{
				int_1,
				mvOcunRbaTM
			};
			base.Game.LoadResources(npcIds);
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BombKingAsset");
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1076);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(720, 455, "font", "game.asset.living.boguoKingAsset", "out", 1, 1);
			simpleBoss_0 = base.Game.CreateBoss(mvOcunRbaTM, 888, 510, -1, 1, "");
			simpleBoss_0.FallFrom(888, 510, "fall", 0, 2, 1000);
			simpleBoss_0.SetRelateDemagemRect(-41, -187, 83, 140);
			physicalObj_0.PlayMovie("in", 9000, 0);
			physicalObj_1.PlayMovie("in", 9000, 0);
			physicalObj_0.PlayMovie("out", 13000, 0);
			physicalObj_1.PlayMovie("out", 13400, 0);
			simpleBoss_0.AddDelay(16);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			jokcubUcRpN = 0;
			if (base.Game.TurnIndex > int_2 + 1)
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
			if (!simpleBoss_0.IsLiving)
			{
				int_0++;
				return true;
			}
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			return int_0;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			bool flag = true;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving)
				{
					flag = false;
				}
			}
			if (!simpleBoss_0.IsLiving && !flag)
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
			if (simpleBoss_0.IsLiving && jokcubUcRpN == 0)
			{
				int num = base.Game.Random.Next(0, GytcujpotKg.Length);
				simpleBoss_0.Say(GytcujpotKg[num], 0, 1500);
				jokcubUcRpN = 1;
			}
		}

		public GK1274()
		{

			mvOcunRbaTM = 1207;
			int_1 = 1204;

		}

		static GK1274()
		{

			string_0 = new string[3]
			{
				"Chỉ được zậy thôi sao ?",
				"Ai ya~đánh đau quá! Ah hahahaha ?",
				"A~cũng được lấm."
			};
			GytcujpotKg = new string[1]
			{
				"Tưởng thắng rồi sao ? Chưa kết thúc đâu! Tôi còn quay lại!"
			};
		}
	}
}
