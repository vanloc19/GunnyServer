using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class GAH1374 : AMissionControl
	{
		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private PhysicalObj[] physicalObj_2;

		private PhysicalObj[] physicalObj_3;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		private int int_7;

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
			base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
			base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
			base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
			int[] npcIds = new int[3]
			{
				int_4,
				int_5,
				int_6
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_4
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1076);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(720, 495, "font", "game.asset.living.boguoKingAsset", "out", 1, 1);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 888, 590, -1, 1, "");
			simpleBoss_0.FallFrom(simpleBoss_0.X, 0, "", 0, 2, 2000);
			simpleBoss_0.SetRelateDemagemRect(-21, -87, 72, 59);
			simpleBoss_0.AddDelay(10);
			simpleBoss_0.Say(LanguageMgr.GetTranslation("Tất cả các bạn dân thường thấp hèn, dám tự tin trong cung điện của tôi!"), 0, 3000);
			physicalObj_0.PlayMovie("in", 9000, 0);
			physicalObj_1.PlayMovie("in", 9000, 0);
			physicalObj_0.PlayMovie("out", 13000, 0);
			physicalObj_1.PlayMovie("out", 13400, 0);
			int_2 = base.Game.TurnIndex;
			base.Game.BossCardCount = 1;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > int_2 + 1)
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
			if (!simpleBoss_0.IsLiving && int_1 == int_4)
			{
				int_1++;
			}
			if (int_1 == int_5 && simpleBoss_1 == null)
			{
				simpleBoss_1 = base.Game.CreateBoss(int_1, simpleBoss_0.X, simpleBoss_0.Y, simpleBoss_0.Direction, 1, "");
				base.Game.RemoveLiving(simpleBoss_0.Id);
				if (simpleBoss_1.Direction == 1)
				{
					simpleBoss_1.SetRect(-21, -87, 72, 59);
				}
				simpleBoss_1.SetRelateDemagemRect(-21, -87, 72, 59);
				simpleBoss_1.Say(LanguageMgr.GetTranslation("Bạn tức giận tôi, tôi không tha thứ cho bạn!"), 0, 3000);
				List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
				Player player = base.Game.FindRandomPlayer();
				int num = 0;
				if (player != null)
				{
					num = player.Delay;
				}
				foreach (Player item in allFightPlayers)
				{
					if (item.Delay < num)
					{
						num = item.Delay;
					}
				}
				simpleBoss_1.AddDelay(num - 2000);
				int_2 = base.Game.TurnIndex;
			}
			if (simpleBoss_1 != null && !simpleBoss_1.IsLiving)
			{
				int_7 = simpleBoss_1.Direction;
				int_0++;
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_0;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (int_1 == int_5 && !simpleBoss_1.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
			List<LoadingFileInfo> list = new List<LoadingFileInfo>();
			list.Add(new LoadingFileInfo(2, "image/map/show7.jpg", ""));
			base.Game.SendLoadResource(list);
			physicalObj_2 = base.Game.FindPhysicalObjByName("wallLeft");
			physicalObj_3 = base.Game.FindPhysicalObjByName("wallRight");
			for (int i = 0; i < physicalObj_2.Length; i++)
			{
				base.Game.RemovePhysicalObj(physicalObj_2[i], true);
			}
			for (int j = 0; j < physicalObj_3.Length; j++)
			{
				base.Game.RemovePhysicalObj(physicalObj_3[j], true);
			}
		}

		public override void DoOther()
		{
			base.DoOther();
			if (simpleBoss_0 != null)
			{
				if (simpleBoss_0.IsLiving)
				{
					int num = base.Game.Random.Next(0, string_0.Length);
					simpleBoss_0.Say(string_0[num], 0, 0);
				}
				else
				{
					int num2 = base.Game.Random.Next(0, string_0.Length);
					simpleBoss_0.Say(string_0[num2], 0, 0);
				}
			}
		}

		public override void OnShooted()
		{
			base.OnShooted();
			if (int_3 == 0)
			{
				if (simpleBoss_0.IsLiving)
				{
					int num = base.Game.Random.Next(0, string_1.Length);
					simpleBoss_0.Say(string_1[num], 0, 1500);
				}
				else
				{
					int num2 = base.Game.Random.Next(0, string_1.Length);
					simpleBoss_1.Say(string_1[num2], 0, 1500);
				}
				int_3 = 1;
			}
		}

		public GAH1374()
		{

			int_1 = 1305;
			int_4 = 1305;
			int_5 = 1306;
			int_6 = 1310;

		}

		static GAH1374()
		{

			string_0 = new string[1]
			{
				"Tôi cuối cùng cũng thoát <br/> khỏi khống chế của <br/> Matthias, thật nhức đầu! "
			};
			string_1 = new string[2]
			{
				"Ai ya, các bạn <br/> sao lại đánh tôi? <br/> Tôi làm gì ?... ",
				"Ui~đau quá, sao phải đánh nhau, mình phải chiến đấu ?"
			};
		}
	}
}
