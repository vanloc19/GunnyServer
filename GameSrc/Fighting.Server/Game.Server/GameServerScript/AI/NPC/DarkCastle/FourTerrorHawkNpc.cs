using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FourTerrorHawkNpc : ABrain
	{
		private int int_0;

		private List<PhysicalObj> list_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private SimpleBoss simpleBoss_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			method_6();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			if ((int)base.Body.Properties1 == 0)
			{
				base.Body.Config.HaveShield = true;
			}
			else
			{
				base.Body.Config.HaveShield = false;
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.Properties1 = 0;
		}

		public override void OnStartAttacking()
		{
			if (int_0 == 0)
			{
				if ((int)base.Body.Properties1 == 0)
				{
					method_1(method_4);
				}
				int_0++;
			}
			else if (int_0 == 1)
			{
				if ((int)base.Body.Properties1 == 0)
				{
					method_1(method_4);
				}
				int_0++;
			}
			else if (int_0 == 2)
			{
				if ((int)base.Body.Properties1 == 0)
				{
					method_1(method_7);
					base.Body.Properties1 = 1;
				}
				else
				{
					fdYwJjyaba6();
					base.Body.Properties1 = 0;
				}
				int_0 = 0;
			}
		}

		public override void OnAfterTakedBomb()
		{
			base.OnAfterTakedBomb();
			if ((int)base.Body.Properties1 == 1)
			{
				if (simpleBoss_0 == null)
				{
					method_0();
				}
				int num = simpleBoss_0.Blood - base.Body.Blood;
				simpleBoss_0.AddBlood(-num, 1);
			}
		}

		private void method_0()
		{
			SimpleBoss[] array = base.Game.FindLivingTurnBossWithID(int_3);
			int num = 0;
			if (array.Length != 0)
			{
				SimpleBoss simpleBoss = (simpleBoss_0 = array[num]);
			}
		}

		private void method_1(LivingCallBack livingCallBack_0)
		{
			int x = base.Game.Random.Next(400, 1283);
			int y = base.Game.Random.Next(400, 654);
			base.Body.MoveTo(x, y, "fly", 1000, livingCallBack_0, 7);
		}

		private void method_2()
		{
			for (int i = 0; i < int_1; i++)
			{
				int num = base.Body.X + base.Game.Random.Next(-300, 300);
				int num2 = base.Body.Y + base.Game.Random.Next(0, 300);
				num = ((num >= 0) ? num : 0);
				num = ((num > base.Game.Map.Info.DeadWidth) ? base.Game.Map.Info.DeadWidth : num);
				num2 = ((num2 > 778) ? 778 : num2);
				LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
				livingConfig.IsFly = true;
				((SimpleBoss)base.Body).CreateChild(int_2, num, num2, showBlood: false, livingConfig);
			}
			base.Body.CallFuction(method_8, 2000);
		}

		private void method_3()
		{
			((SimpleBoss)base.Body).RemoveAllChild();
		}

		private void method_4()
		{
			base.Body.PlayMovie("beatA", 500, 0);
			Player obj = base.Game.FindRandomPlayer();
			((PVEGame)base.Game).SendObjectFocus(obj, 1, 1500, 0);
			base.Body.CallFuction(method_5, 2000);
			base.Body.RangeAttacking(0, base.Game.Map.Info.DeadWidth, "cry", 2100, null);
		}

		private void method_5()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.4.feather", "", 1, 0));
			}
		}

		private void method_6()
		{
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, true);
			}
			list_0 = new List<PhysicalObj>();
		}

		private void method_7()
		{
			base.Body.PlayMovie("AtoB", 500, 0);
			base.Body.CallFuction(method_2, 3500);
		}

		private void fdYwJjyaba6()
		{
			method_3();
			base.Body.PlayMovie("BtoA", 1000, 0);
			base.Body.CallFuction(method_8, 4000);
		}

		private void method_8()
		{
			if ((int)base.Body.Properties1 == 1)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			}
			else
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public FourTerrorHawkNpc()
		{
			list_0 = new List<PhysicalObj>();
			int_1 = 5;
			int_2 = 4302;
			int_3 = 4306;
		}
	}
}
