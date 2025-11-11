using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DTGT1167 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private SimpleNpc simpleNpc_0;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private PhysicalObj physicalObj_2;

		private int int_0;

		private int int_1;

		private int int_2;

		private int pWswzgwueht;

		private int int_3;

		private int int_4;

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
			base.Game.AddLoadingFile(2, "image/game/effect/6/popcan.swf", "asset.game.six.popcan");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguquanwangAsset");
			int[] npcIds = new int[4]
			{
				int_1,
				int_2,
				pWswzgwueht,
				int_3
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1167);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_2 = base.Game.Createlayer(1250, 520, "moive", "game.living.Living189", "", 1, 0);
			physicalObj_1 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_0 = base.Game.Createlayer(1455, 773, "front", "game.asset.living.boguquanwangAsset", "out", 1, 0);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsTurn = false;
			livingConfig.CanTakeDamage = false;
			livingConfig.IsFly = true;
			simpleNpc_0 = base.Game.CreateNpc(int_3, 1250, 700, 1, 1, livingConfig);
			base.Game.SendHideBlood(simpleNpc_0, 0);
			simpleNpc_0.OnSmallMap(state: false);
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1650, 1000, -1, 1, "");
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			LivingConfig livingConfig2 = base.Game.BaseLivingConfig();
			livingConfig2.IsFly = true;
			simpleBoss_1 = base.Game.CreateBoss(pWswzgwueht, 1250, 600, 1, 1, "", livingConfig2);
			simpleBoss_1.SetRelateDemagemRect(simpleBoss_1.NpcInfo.X, simpleBoss_1.NpcInfo.Y, simpleBoss_1.NpcInfo.Width, simpleBoss_1.NpcInfo.Height);
			base.Game.SendObjectFocus(simpleBoss_1, 1, 1000, 0);
			simpleBoss_1.Say("Chào mừng quý vị đến với trận đấu boxing gây cấn.", 0, 1500);
			simpleBoss_1.Say("Có sự hiện diện của Vua Boxing Oai Tử lừng lẫy giang hồ.", 0, 3000);
			base.Game.SendObjectFocus(simpleBoss_0, 1, 4000, 0);
			physicalObj_1.PlayMovie("in", 5000, 0);
			physicalObj_0.PlayMovie("in", 5100, 0);
			physicalObj_1.PlayMovie("out", 7000, 0);
			physicalObj_0.PlayMovie("out", 7100, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			int_0 = 0;
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
			if (!simpleBoss_0.IsLiving && int_4 == 0)
			{
				base.Game.SendObjectFocus(simpleBoss_1, 1, 1000, 0);
				simpleBoss_1.Say("Oai Tử đã gục rồi. Đếm ngược nào!!!", 0, 2000);
				base.Game.RemoveLiving(simpleBoss_0.Id);
				simpleBoss_0 = base.Game.CreateBoss(int_1, simpleBoss_0.X, simpleBoss_0.Y, simpleBoss_0.Direction, 1, "");
				simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
				simpleBoss_0.PlayMovie("standA", 0, 0);
				base.Game.SendObjectFocus(simpleBoss_0, 1, 4000, 0);
				simpleBoss_0.PlayMovie("dieA", 5000, 5000);
				base.Game.SendObjectFocus(simpleBoss_1, 1, 10000, 0);
				simpleBoss_1.Say("Oai Tử đã đứng dậy, tiếp tục nào.", 11000, 2000);
				int_4 = 1;
				return false;
			}
			if (!simpleBoss_0.IsLiving && int_4 > 0)
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

		public DTGT1167()
		{

			int_1 = 6131;
			int_2 = 6134;
			pWswzgwueht = 6132;
			int_3 = 6135;

		}

		static DTGT1167()
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
