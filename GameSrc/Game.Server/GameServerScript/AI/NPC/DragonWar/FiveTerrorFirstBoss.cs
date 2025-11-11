using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveTerrorFirstBoss : ABrain
	{
		private int int_0;

		private SimpleNpc simpleNpc_0;

		protected Player m_targer;

		private List<PhysicalObj> list_0;

		private int int_1;

		private int pdrwiXiwRaA;

		private int int_2;

		private int int_3;

		private int int_4;

		private static string[] string_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (simpleNpc_0 == null)
			{
				SimpleNpc[] nPCLivingWithID = ((PVEGame)base.Game).GetNPCLivingWithID(int_2);
				if (nPCLivingWithID.Length != 0)
				{
					simpleNpc_0 = nPCLivingWithID[0];
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			fmQwiPplelc();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			int_1 = 0;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int_0++;
			List<Player> list = new List<Player>();
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsActive && allLivingPlayer.IsLiving && allLivingPlayer.X > 1199)
				{
					list.Add(allLivingPlayer);
				}
				else if (allLivingPlayer.IsActive && allLivingPlayer.IsLiving && allLivingPlayer.X < 290 && allLivingPlayer.Y < 555)
				{
					list.Add(allLivingPlayer);
				}
			}
			if (list.Count > 0)
			{
				method_0(list);
				return;
			}
			switch (int_1)
			{
				case 0:
					TymwihjThf5();
					break;
				case 1:
					method_1();
					break;
				case 2:
					method_2();
					break;
				case 3:
					method_3();
					break;
				case 4:
					if (simpleNpc_0.IsLiving)
					{
						method_4();
					}
					else
					{
						method_5();
					}
					break;
				case 5:
					if (simpleNpc_0.IsLiving)
					{
						method_9();
					}
					else
					{
						method_6();
					}
					break;
			}
		}

		private void method_0(List<Player> lARZCnODJQtPboGdpk)
		{
			base.Body.CurrentDamagePlus = 1000f;
			foreach (Player item in lARZCnODJQtPboGdpk)
			{
				base.Body.BeatDirect(item, "", 2000, 1, 1);
			}
		}

		private void TymwihjThf5()
		{
			int_1 = 1;
			base.Body.PlayMovie("beatA", 1000, 10000);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 10000, directDamage: false);
			base.Body.CallFuction(method_13, 10000);
		}

		private void method_1()
		{
			int_1 = 2;
			method_8();
			base.Body.CurrentDamagePlus = 2.5f;
			m_targer = base.Game.FindRandomPlayer();
			if (base.Body.ShootPoint(m_targer.X, m_targer.Y, 56, 1000, 10000, 1, 1.5f, 8000))
			{
				base.Body.PlayMovie("beatB", 1000, 10000);
			}
		}

		private void method_2()
		{
			int_1 = 3;
			method_8();
			base.Body.CurrentDamagePlus = 2f;
			base.Body.PlayMovie("beatC", 1000, 11000);
			if (base.Game.Random.Next(50) < 50)
			{
				base.Body.CallFuction(method_12, 9000);
			}
			else
			{
				base.Body.CallFuction(method_11, 9000);
			}
		}

		private void method_3()
		{
			int_1 = 4;
			base.Body.CurrentDamagePlus = 2f;
			((PVEGame)base.Game).SendObjectFocus(simpleNpc_0, 1, 1000, 500);
			((PVEGame)base.Game).SendLivingActionMapping(simpleNpc_0, "stand", "standB");
			simpleNpc_0.PlayMovie("AtoB", 2000, 0);
			simpleNpc_0.PlayMovie("walkB", 6000, 0);
			simpleNpc_0.Config.CanTakeDamage = true;
			((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 9000, 500);
			base.Body.PlayMovie("beatD", 10000, 10000);
			base.Body.CallFuction(method_10, 18000);
		}

		private void method_4()
		{
			int_1 = 5;
			method_8();
			base.Body.PlayMovie("DtoE", 1000, 8000);
			base.Body.CallFuction(method_9, 8000);
		}

		private void method_5()
		{
			int_1 = 0;
			fmQwiPplelc();
			base.Body.PlayMovie("fallingA", 800, 3000);
			((PVEGame)base.Game).SendFreeFocus(1441, 681, 1, 1000, 0);
			base.Body.CallFuction(method_7, 3000);
		}

		private void method_6()
		{
			int_1 = 0;
			fmQwiPplelc();
			base.Body.PlayMovie("fallingB", 800, 3000);
			((PVEGame)base.Game).SendFreeFocus(1441, 681, 1, 1000, 0);
			base.Body.CallFuction(method_7, 3000);
		}

		private void method_7()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.IsTurn = false;
			livingConfig.CanTakeDamage = false;
			SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(int_2, simpleNpc_0.X, simpleNpc_0.Y, 1, 1, livingConfig);
			base.Game.RemoveLiving(simpleNpc_0.Id);
			simpleNpc_0 = simpleNpc;
			((PVEGame)base.Game).SendLivingActionMapping(simpleNpc_0, "stand", "standA");
			((PVEGame)base.Game).SendObjectFocus(simpleNpc_0, 1, 2000, 0);
			simpleNpc_0.PlayMovie("in", 3000, 10000);
			base.Body.CallFuction(method_8, 11000);
			((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 14000, 500);
			base.Body.CallFuction(TymwihjThf5, 15000);
		}

		private void method_8()
		{
			if (simpleNpc_0.Config.CanTakeDamage)
			{
				simpleNpc_0.PlayMovie("walkB", 1000, 3000);
			}
			else
			{
				simpleNpc_0.PlayMovie("walkA", 1000, 3000);
			}
		}

		private void method_9()
		{
			fmQwiPplelc();
			base.Body.CurrentDamagePlus = 4f;
			m_targer = base.Game.FindRandomPlayer();
			if (base.Body.ShootPoint(m_targer.X - 20, m_targer.Y, 72, 1000, 10000, 1, 0.1f, 3300))
			{
				base.Body.PlayMovie("beatE", 1000, 0);
			}
		}

		private void method_10()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsActive && allLivingPlayer.IsLiving)
				{
					list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.4.minigun", "", 1, 1));
					allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, int_3), 1000);
					base.Body.BeatDirect(allLivingPlayer, "", 1000, 3, 1);
				}
			}
		}

		private void method_11()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsActive && allLivingPlayer.IsLiving)
				{
					list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 10, "", "asset.game.4.zap", "", 1, 1));
					allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, int_3), 1000);
					base.Body.BeatDirect(allLivingPlayer, "", 1000, 3, 1);
				}
			}
		}

		private void method_12()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsActive && allLivingPlayer.IsLiving)
				{
					list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 10, "", "asset.game.4.zap2", "", 1, 1));
					allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, int_3), 1000);
					base.Body.BeatDirect(allLivingPlayer, "", 1000, 4, 1);
				}
			}
		}

		private void method_13()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsActive && allLivingPlayer.IsLiving)
				{
					allLivingPlayer.AddEffect(new ContinueReduceBloodEffect(2, int_4, base.Body), 500);
				}
			}
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void fmQwiPplelc()
		{
			switch (int_1)
			{
				default:
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cry");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "die");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
					base.Body.FireX = ((SimpleBoss)base.Body).NpcInfo.FireX;
					base.Body.FireY = ((SimpleBoss)base.Body).NpcInfo.FireY;
					break;
				case 1:
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryA");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieA");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
					base.Body.SetOffsetY(-75);
					base.Body.SetRect(-110, -144, 230, 149);
					base.Body.SetRelateDemagemRect(-110, -144, 230, 149);
					break;
				case 2:
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryB");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieB");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
					base.Body.SetOffsetY(-150);
					base.Body.FireY = -268;
					base.Body.SetRect(-110, -268, 230, 149);
					base.Body.SetRelateDemagemRect(-110, -268, 230, 149);
					break;
				case 3:
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryC");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieC");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standC");
					base.Body.SetOffsetY(-225);
					base.Body.SetRect(-110, -391, 230, 149);
					base.Body.SetRelateDemagemRect(-110, -391, 230, 149);
					break;
				case 4:
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryD");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieD");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standD");
					base.Body.SetOffsetY(-300);
					base.Body.SetRect(-110, -500, 230, 149);
					base.Body.SetRelateDemagemRect(-110, -500, 230, 149);
					break;
				case 5:
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryE");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieE");
					((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standE");
					base.Body.SetOffsetY(-420);
					base.Body.FireX = -120;
					base.Body.FireY = -500;
					base.Body.SetRect(-110, -630, 230, 149);
					base.Body.SetRelateDemagemRect(-110, -630, 230, 149);
					break;
			}
		}

		public FiveTerrorFirstBoss()
		{
			list_0 = new List<PhysicalObj>();
			pdrwiXiwRaA = 5301;
			int_2 = 5302;
			int_3 = 150;
			int_4 = 900;
		}

		static FiveTerrorFirstBoss()
		{
			string_0 = new string[1]
			{
				""
			};
		}
	}
}
