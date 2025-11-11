using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FiveNormalFourBoss : ABrain
	{
		private int int_0;

		private List<PhysicalObj> list_0;

		private int int_1;

		protected Player m_targer;

		private int int_2;

		private int int_3;

		private int int_4;

		private static string[] string_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, true);
			}
			list_0 = new List<PhysicalObj>();
			if ((int)base.Body.Properties2 == 3)
			{
				int_1 = base.Body.Blood - base.Body.MaxBlood / 100 * int_4;
				if (int_1 < 0)
				{
					int_1 = 0;
				}
				base.Body.Properties2 = 0;
			}
			method_0();
		}

		private void method_0()
		{
			if ((int)base.Body.Properties2 == 1)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryB");
				base.Body.SetRect(-120, -190, 260, 200);
				base.Body.SetRelateDemagemRect(-120, -190, 260, 200);
			}
			else
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryA");
				base.Body.SetRect(-180, -90, 300, 100);
				base.Body.SetRelateDemagemRect(-60, -200, 116, 100);
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			int_1 = base.Body.MaxBlood - base.Body.MaxBlood / 100 * int_4;
			base.Body.Properties1 = 0;
			base.Body.Properties2 = 0;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			bool flag = false;
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if ((allLivingPlayer.IsLiving && allLivingPlayer.X >= 1580) || allLivingPlayer.X <= 470)
				{
					flag = true;
					return;
				}
			}
			if (flag)
			{
				method_1();
				return;
			}
			if ((int)base.Body.Properties1 == 1)
			{
				method_3();
				base.Body.Properties1 = 0;
				return;
			}
			int_0++;
			switch (int_0)
			{
				case 1:
					method_6();
					break;
				case 2:
					method_2();
					base.Body.Properties1 = 1;
					break;
				case 3:
					method_4();
					break;
				case 4:
					method_5();
					int_0 = 0;
					break;
			}
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatC", 1000, 0);
			base.Body.RangeAttacking(1580, 2000, "cry", 3000, directDamage: true);
		}

		private void method_2()
		{
			base.Body.PlayMovie("beatA", 1000, 4000);
			base.Body.Properties1 = 1;
		}

		private void method_3()
		{
			bool flag = true;
			base.Body.CurrentDamagePlus = 10f;
			SimpleNpc[] nPCLivingWithID = ((PVEGame)base.Game).GetNPCLivingWithID(int_2);
			if (nPCLivingWithID.Length != 0 && (int)nPCLivingWithID[0].Properties1 == 2)
			{
				flag = false;
			}
			if (flag)
			{
				base.Body.PlayMovie("beatF", 1000, 5000);
				base.Body.RangeAttacking(480, 1580, "cry", 4000, directDamage: true);
			}
			else
			{
				base.Body.PlayMovie("beatB", 1000, 0);
				base.Body.RangeAttacking(1330, 2000, "cry", 4000, directDamage: true);
			}
			SimpleNpc[] array = nPCLivingWithID;
			SimpleNpc[] array2 = array;
			foreach (SimpleNpc simpleNpc in array2)
			{
				switch ((int)simpleNpc.Properties1)
				{
					case 0:
						simpleNpc.PlayMovie("die", 3500, 0);
						break;
					case 1:
						simpleNpc.PlayMovie("dieB", 3500, 0);
						break;
					case 2:
						simpleNpc.PlayMovie("dieC", 3500, 0);
						break;
				}
				simpleNpc.Die(5000);
			}
		}

		private void method_4()
		{
			m_targer = base.Game.FindRandomPlayer();
			if (m_targer != null)
			{
				base.Body.PlayMovie("beatC", 1000, 0);
				((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 2800, 0);
				base.Body.CallFuction(method_9, 3500);
				base.Body.RangeAttacking(m_targer.X - 50, m_targer.X + 50, "cry", 4000, directDamage: true);
			}
		}

		private void method_5()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			base.Body.PlayMovie("beatD", 1000, 0);
			base.Body.CallFuction(method_7, 1500);
			base.Body.CallFuction(method_8, 5000);
		}

		private void method_6()
		{
			base.Body.CurrentDamagePlus = 2f;
			base.Body.PlayMovie("beatE", 1000, 5000);
			base.Body.RangeAttacking(480, 1580, "cry", 4000, directDamage: true);
		}

		private void method_7()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				list_0.Add(((PVEGame)base.Game).CreatePhysicalObj(0, 0, "top", "asset.game.4.cuipao", "", 1, 1, allLivingPlayer.Id + 1));
				allLivingPlayer.SpeedMultX(18);
				allLivingPlayer.StartSpeedMult(allLivingPlayer.X - 400, allLivingPlayer.Y, 0);
				base.Body.BeatDirect(allLivingPlayer, "", 100, 3, 1);
			}
		}

		private void method_8()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				allLivingPlayer.SpeedMultX(3);
			}
		}

		private void method_9()
		{
			list_0.Add(((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.guang", "", 1, 1));
		}

		private void method_10()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.IsFly = true;
			int x = base.Game.Random.Next(570, 647);
			((SimpleBoss)base.Body).CreateChild(int_3, x, 550, true, livingConfig);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public override void OnAfterTakedBomb()
		{
			if (base.Body.Blood <= int_1 && (int)base.Body.Properties2 == 0 && base.Body.IsLiving)
			{
				base.Body.Properties2 = 1;
				base.Body.BlockTurn = true;
				base.Body.PlayMovie("AtoB", 100, 1100);
				method_0();
				base.Body.CallFuction(method_10, 1200);
			}
		}

		public FiveNormalFourBoss()
		{
			list_0 = new List<PhysicalObj>();
			int_2 = 5134;
			int_3 = 5132;
			int_4 = 30;
		}

		static FiveNormalFourBoss()
		{
			string_0 = new string[1]
			{
				""
			};
		}
	}
}
