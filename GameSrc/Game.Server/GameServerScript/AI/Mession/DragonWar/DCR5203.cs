using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DCR5203 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleBoss simpleBoss_1;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private int JtwwaoEaqIZ;

		private int int_4;

		private int int_5;

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
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.hongpaoxiaoemoAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/5/heip.swf", "asset.game.4.heip");
			base.Game.AddLoadingFile(2, "image/game/effect/5/tang.swf", "asset.game.4.tang");
			base.Game.AddLoadingFile(2, "image/game/effect/5/lanhuo.swf", "asset.game.4.lanhuo");
			int[] npcIds = new int[5]
			{
				int_1,
				int_2,
				int_3,
				JtwwaoEaqIZ,
				int_4
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_1
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(int_5);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(850, 258, "front", "game.asset.living.hongpaoxiaoemoAsset", "out", 1, 1);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			simpleBoss_0 = base.Game.CreateBoss(int_1, 1000, 500, 1, 3, "", livingConfig);
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Say("Ta đợi lâu lắm rồi!", 0, 1000);
			physicalObj_0.PlayMovie("in", 4000, 0);
			physicalObj_1.PlayMovie("in", 4000, 0);
			physicalObj_0.PlayMovie("out", 7000, 0);
			physicalObj_1.PlayMovie("out", 7200, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
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

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				int_0++;
				return true;
			}
			if (base.Game.TurnIndex > 200)
			{
				return true;
			}
			return false;
		}

		private void method_0()
		{
			simpleBoss_1.ChangeDirection(1, 500);
			simpleBoss_1.Say("Thôi ta éo đùa nữa.", 0, 1000);
			simpleBoss_1.PlayMovie("out", 1000, 0);
			simpleBoss_1.CallFuction(RiuwaFkeqKV, 4000);
		}

		private void RiuwaFkeqKV()
		{
			SimpleNpc simpleNpc = base.Game.CreateNpc(int_4, 179, 552, 1, 1, "standC", base.Game.BaseLivingConfig());
			simpleNpc.PlayMovie("cool", 1000, 0);
			simpleNpc.Say("Thôi hãy rời khỏi đây mau. Chúa rồng đã sống lại rồi.", 0, 4000, 2000);
			base.Game.RemoveLiving(simpleBoss_1.Id);
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return int_0;
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
		}

		public override void OnShooted()
		{
			base.OnShooted();
		}

		public override void OnDied()
		{
			base.OnDied();
			if (!simpleBoss_0.IsLiving)
			{
				LivingConfig livingConfig = base.Game.BaseLivingConfig();
				livingConfig.IsFly = true;
				livingConfig.CanTakeDamage = false;
				simpleBoss_1 = base.Game.CreateBoss(int_1, simpleBoss_0.X, simpleBoss_0.Y, simpleBoss_0.Direction, 1, "", livingConfig);
				base.Game.RemoveLiving(simpleBoss_0.Id);
				base.Game.SendHideBlood(simpleBoss_1, 0);
				simpleBoss_1.MoveTo(1000, 485, "fly", base.Game.GetWaitTimerLeft(), method_0, 10);
			}
		}

		public DCR5203()
		{
			int_1 = 5221;
			int_2 = 5222;
			int_3 = 5223;
			JtwwaoEaqIZ = 5224;
			int_4 = 5204;
			int_5 = 1153;
		}
	}
}
