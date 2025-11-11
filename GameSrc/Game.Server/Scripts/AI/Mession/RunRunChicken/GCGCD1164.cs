using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class GCGCD1164 : AMissionControl
	{
		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private PhysicalObj physicalObj_2;

		private SimpleBoss simpleBoss_0;

		private SimpleNpc simpleNpc_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

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
			base.Game.AddLoadingFile(1, "bombs/83.swf", "tank.resource.bombs.Bomb83");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.choudanbenbenAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/7/choud.swf", "asset.game.seven.choud");
			base.Game.AddLoadingFile(2, "image/game/effect/7/jinqucd.swf", "asset.game.seven.jinqucd");
			base.Game.AddLoadingFile(2, "image/game/effect/7/du.swf", "asset.game.seven.du");
			int[] npcIds = new int[3]
			{
				int_1,
				int_2,
				int_3
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1164);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(300, 595, "font", "game.asset.living.choudanbenbenAsset", "out", 1, 1);
			physicalObj_2 = base.Game.Createlayer(2170, 636, "", "game.living.Living178", "stand", 1, 1);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsTurn = false;
			livingConfig.IsFly = true;
			simpleNpc_0 = base.Game.CreateNpc(int_2, 1920, 900, 1, -1, livingConfig);
			simpleNpc_0.PlayMovie("stand", 1000, 0);
			simpleNpc_0.Say("Chúng mình không muốn bị lây bệnh. Cứu!! Cứu!!", 0, 2000);
			simpleNpc_0.CallFuction(method_0, 4000);
		}

		private void method_0()
		{
			simpleBoss_0 = base.Game.CreateBoss(int_1, 200, 590, 1, 1, "born");
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			physicalObj_0.PlayMovie("in", 1000, 0);
			physicalObj_1.PlayMovie("in", 2000, 0);
			physicalObj_0.PlayMovie("out", 5000, 0);
			physicalObj_1.PlayMovie("out", 5400, 0);
			simpleBoss_0.Say("Định cứu gà con cuối cùng à? Không dễ vậy đâu.", 0, 6000);
			simpleBoss_0.PlayMovie("skill", 8000, 0);
			simpleBoss_0.Say("Giỏi thì phá lá chắn bảo vệ của ta.", 0, 8000);
			base.Game.SendObjectFocus(simpleNpc_0, 1, 9000, 0);
			simpleNpc_0.PlayMovie("standB", 10000, 0);
			simpleNpc_0.Config.CanTakeDamage = false;
			simpleNpc_0.Say("Chết phải hạ những quả trứng thối mới phá vỡ được lá chắn", 0, 11000);
			base.Game.SendObjectFocus(simpleBoss_0, 1, 13000, 0);
			simpleBoss_0.Say("Đã đến thì đừng hòng đi. Ta sẽ nhốt hết vào lồng.", 0, 14000, 3000);
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
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleNpc_0 != null && !simpleNpc_0.IsLiving)
			{
				int_0++;
				return true;
			}
			if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
			{
				return true;
			}
			return false;
		}

		public override void OnDied()
		{
			base.OnDied();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleNpc_0.IsLiving)
			{
				int waitTimerLeft = base.Game.GetWaitTimerLeft();
				base.Game.SendObjectFocus(simpleNpc_0, 1, waitTimerLeft + 500, 500);
				simpleNpc_0.PlayMovie("out", waitTimerLeft + 1000, 0);
				simpleNpc_0.Say("Nhanh phá lồng cứu chúng tôi với...", 0, waitTimerLeft + 1500, 3500);
				simpleNpc_0.Config.CanTakeDamage = true;
			}
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_0;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving && simpleNpc_0 != null && !simpleNpc_0.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public GCGCD1164()
		{

			int_1 = 7031;
			int_2 = 7032;
			int_3 = 7033;

		}
	}
}
