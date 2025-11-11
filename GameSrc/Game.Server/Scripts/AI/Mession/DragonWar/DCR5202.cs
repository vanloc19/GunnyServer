using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
	public class DCR5202 : AMissionControl
	{
		private SimpleBoss simpleBoss_0;

		private SimpleNpc simpleNpc_0;

		private SimpleBoss simpleBoss_1;

		private SimpleBoss simpleBoss_2;

		private SimpleNpc simpleNpc_1;

		private SimpleNpc simpleNpc_2;

		private SimpleNpc simpleNpc_3;

		private PhysicalObj physicalObj_0;

		private PhysicalObj physicalObj_1;

		private List<PhysicalObj> list_0;

		private PhysicalObj physicalObj_2;

		private int PcbwakitmFy;

		private int int_0;

		private int int_1;

		private int int_2;

		private int BrfwaPjjNuG;

		private int int_3;

		private int int_4;

		private int int_5;

		private int EtJwaglUlFj;

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
			base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
			base.Game.AddLoadingFile(2, "image/game/effect/5/jinqudan.swf", "asset.game.4.jinqudan");
			base.Game.AddLoadingFile(2, "image/game/effect/5/mubiao.swf", "asset.game.4.mubiao");
			base.Game.AddLoadingFile(2, "image/game/effect/5/zao.swf", "asset.game.4.zao");
			base.Game.AddLoadingFile(2, "image/game/effect/5/xiaopao.swf", "asset.game.4.xiaopao");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.gebulinzhihuiguanAsset");
			int[] npcIds = new int[7]
			{
				int_0,
				int_2,
				BrfwaPjjNuG,
				int_3,
				int_4,
				int_5,
				int_1
			};
			base.Game.LoadResources(npcIds);
			int[] npcIds2 = new int[1]
			{
				int_0
			};
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(EtJwaglUlFj);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			method_0();
			method_1();
		}

		private void method_0()
		{
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			physicalObj_1 = base.Game.Createlayer(1300, 413, "front", "game.asset.living.gebulinzhihuiguanAsset", "out", 1, 1);
			simpleBoss_0 = base.Game.CreateBoss(int_0, 1478, 596, -1, 1, "born", base.Game.BaseLivingConfig());
			simpleBoss_0.SetRelateDemagemRect(simpleBoss_0.NpcInfo.X, simpleBoss_0.NpcInfo.Y, simpleBoss_0.NpcInfo.Width, simpleBoss_0.NpcInfo.Height);
			simpleBoss_0.Config.CanTakeDamage = false;
			base.Game.SendHideBlood(simpleBoss_0, 0);
			physicalObj_2 = base.Game.Createlayer(simpleBoss_0.X, simpleBoss_0.Y, "", "asset.game.4.zao", "stand", 1, 1);
			simpleBoss_2 = base.Game.CreateBoss(int_2, 1323, 663, -1, 1, "", base.Game.BaseLivingConfig());
			simpleBoss_2.SetRelateDemagemRect(simpleBoss_2.NpcInfo.X, simpleBoss_2.NpcInfo.Y, simpleBoss_2.NpcInfo.Width, simpleBoss_2.NpcInfo.Height);
			simpleBoss_2.Config.IsTurn = false;
			simpleBoss_1 = base.Game.CreateBoss(BrfwaPjjNuG, 1664, 532, -1, 1, "", base.Game.BaseLivingConfig());
			simpleBoss_1.SetRelateDemagemRect(simpleBoss_1.NpcInfo.X, simpleBoss_1.NpcInfo.Y, simpleBoss_1.NpcInfo.Width, simpleBoss_1.NpcInfo.Height);
			simpleBoss_1.Config.IsTurn = false;
			simpleNpc_1 = base.Game.CreateNpc(int_3, 1360, 840, 1, -1, base.Game.BaseLivingConfig());
			simpleNpc_1.Config.IsTurn = false;
			simpleNpc_1.OnSmallMap(state: false);
			base.Game.SendHideBlood(simpleNpc_1, 0);
			simpleNpc_2 = base.Game.CreateNpc(int_4, 1546, 650, 1, -1, base.Game.BaseLivingConfig());
			simpleNpc_2.Config.IsTurn = false;
			simpleNpc_2.OnSmallMap(state: false);
			base.Game.SendHideBlood(simpleNpc_2, 0);
			simpleNpc_3 = base.Game.CreateNpc(int_1, 503, 831, 1, 1, base.Game.BaseLivingConfig());
			simpleNpc_3.Config.IsTurn = true;
			simpleNpc_3.Config.CanTakeDamage = false;
			simpleNpc_3.OnSmallMap(state: false);
			base.Game.SendHideBlood(simpleNpc_3, 0);
		}

		private void method_1()
		{
			simpleNpc_0 = base.Game.CreateNpc(int_5, 1022, 828, 0, 1, "standB", null);
			base.Game.SendObjectFocus(simpleNpc_0, 1, 1000, 0);
			simpleNpc_0.Say("Ê, hãy nếm thử sức mạnh này!", 0, 2000);
			simpleNpc_0.PlayMovie("beatA", 2000, 0);
			base.Game.SendObjectFocus(simpleNpc_1, 1, 4000, 0);
			simpleNpc_1.PlayMovie("beatA", 5000, 0);
			simpleNpc_0.PlayMovie("outB", 7000, 0);
			simpleNpc_0.Die(10000);
			base.Game.SendObjectFocus(simpleBoss_0, 1, 10000, 0);
			simpleBoss_0.PlayMovie("beatC", 10500, 0);
			physicalObj_0.PlayMovie("in", 11000, 0);
			physicalObj_1.PlayMovie("in", 11000, 0);
			physicalObj_0.PlayMovie("out", 16000, 6000);
			physicalObj_1.PlayMovie("out", 16000, 6000);
			simpleBoss_0.CallFuction(method_2, 17000);
		}

		private void method_2()
		{
			list_0.Add(base.Game.Createlayer(simpleBoss_0.X, simpleBoss_0.Y, "", "asset.game.4.mubiao", "", 1, 1));
			list_0.Add(base.Game.Createlayer(simpleBoss_2.X, simpleBoss_2.Y, "", "asset.game.4.mubiao", "", 1, 1));
			list_0.Add(base.Game.Createlayer(simpleBoss_1.X, simpleBoss_1.Y, "", "asset.game.4.mubiao", "", 1, 1));
		}

		private void method_3()
		{
			if (physicalObj_2 != null)
			{
				base.Game.RemovePhysicalObj(physicalObj_2, true);
				physicalObj_2 = null;
			}
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
			if (list_0.Count <= 0)
			{
				return;
			}
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, true);
			}
			list_0 = new List<PhysicalObj>();
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			if (simpleBoss_0 != null && !simpleBoss_0.IsLiving)
			{
				PcbwakitmFy++;
				return true;
			}
			if (base.Game.TurnIndex > 200)
			{
				return true;
			}
			if (!simpleBoss_2.IsLiving && !simpleBoss_1.IsLiving && physicalObj_2 != null)
			{
				base.Game.SendObjectFocus(physicalObj_2, 1, 1000, 0);
				physicalObj_2.PlayMovie("die", 2000, 2000);
				simpleBoss_0.Config.CanTakeDamage = true;
				simpleBoss_0.CallFuction(method_3, 5000);
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return PcbwakitmFy;
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
		}

		public DCR5202()
		{
			list_0 = new List<PhysicalObj>();
			int_0 = 5214;
			int_1 = 5211;
			int_2 = 5212;
			BrfwaPjjNuG = 5213;
			int_3 = 5216;
			int_4 = 5217;
			int_5 = 5204;
			EtJwaglUlFj = 1152;
		}
	}
}
