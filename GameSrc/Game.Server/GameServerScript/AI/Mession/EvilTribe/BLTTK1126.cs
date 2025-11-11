using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class BLTTK1126 : AMissionControl
	{
		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		private int int_7;

		private int int_8;

		private int int_9;

		private int int_10;

		private static string[] string_0;

		private static string[] string_1;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1150)
			{
				return 3;
			}
			if (score > 925)
			{
				return 2;
			}
			if (score > 700)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(1, "bombs/55.swf", "tank.resource.bombs.Bomb55");
			base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
			base.Game.AddLoadingFile(1, "bombs/53.swf", "tank.resource.bombs.Bomb53");
			base.Game.AddLoadingFile(2, "image/map/1126/object/1126object.swf", "game.assetmap.Flame");
			base.Game.AddLoadingFile(2, "image/map/1076/objects/1076mapasset.swf", "com.mapobject.asset.wordtip75");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanLeaderAsset");
			int[] npcIds = new int[6]
			{
				int_4,
				int_5,
				int_6,
				int_8,
				int_9,
				int_7
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_4
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1126);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(700, 355, "font", "game.asset.living.ClanLeaderAsset", "out", 1, 1);
			simpleBoss_0 = base.Game.CreateBoss(int_2, 800, 400, -1, 1, "");
			simpleBoss_0.FallFrom(800, 400, "fall", 0, 2, 1200, null);
			simpleBoss_0.SetRelateDemagemRect(-42, -187, 75, 187);
			simpleBoss_0.Say("Đến đây thôi, dám ngăn cản nghi lễ của ta, không muốn sống à!", 0, 2000);
			physicalObj_0.PlayMovie("in", 7000, 0);
			physicalObj_1.PlayMovie("in", 7000, 0);
			physicalObj_0.PlayMovie("out", 13000, 0);
			physicalObj_1.PlayMovie("out", 13400, 0);
			int_3 = base.Game.TurnIndex;
			base.Game.BossCardCount = 1;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > int_3 + 1)
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
			int_0 = 0;
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (!simpleBoss_0.IsLiving && int_2 == int_4)
			{
				int_2 = int_5;
			}
			if (!simpleBoss_0.IsLiving && simpleBoss_1 == null)
			{
				base.Game.ClearAllChild();
			}
			if (int_2 == int_5 && simpleBoss_1 == null)
			{
				simpleBoss_1 = base.Game.CreateBoss(int_2, simpleBoss_0.X, simpleBoss_0.Y, simpleBoss_0.Direction, 1, "born");
				base.Game.RemoveLiving(simpleBoss_0.Id);
				simpleBoss_1.SetRelateDemagemRect(simpleBoss_1.NpcInfo.X, simpleBoss_1.NpcInfo.Y, simpleBoss_1.NpcInfo.Width, simpleBoss_1.NpcInfo.Height);
				int_3 = base.Game.TurnIndex;
			}
			if (int_2 == int_5 && simpleBoss_1 != null && !simpleBoss_1.IsLiving)
			{
				int_10 = simpleBoss_1.Direction;
				int_1++;
				return true;
			}
			if (base.Game.TurnIndex > 200)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_1;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (int_2 == int_5 && !simpleBoss_1.IsLiving)
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
			if (simpleBoss_0 != null && simpleBoss_0.IsLiving)
			{
				int num = base.Game.Random.Next(0, string_0.Length);
				simpleBoss_0.Say(string_0[num], 0, 0);
			}
		}

		public override void OnShooted()
		{
			if (int_0 == 0 && simpleBoss_0.IsLiving)
			{
				int num = base.Game.Random.Next(0, string_1.Length);
				simpleBoss_0.Say(string_1[num], 0, 1000);
				int_0 = 1;
			}
		}

		public BLTTK1126()
		{
			int_2 = 3216;
			int_4 = 3216;
			int_5 = 3217;
			int_6 = 3203;
			int_7 = 3218;
			int_8 = 3212;
			int_9 = 3213;
		}

		static BLTTK1126()
		{
			string_0 = new string[3]
			{
				"Đùa với các ngươi chán quá!",
				"Trình chỉ tới vậy sao?",
				"Ta mới sử dụng 1 phần công lực thôi đó."
			};
			string_1 = new string[3]
			{
				"Dám chọc giận ta à?",
				"Ta né, ta né!!!",
				"Đồ khốn nạn. Dám đánh ta!!"
			};
		}
	}
}
