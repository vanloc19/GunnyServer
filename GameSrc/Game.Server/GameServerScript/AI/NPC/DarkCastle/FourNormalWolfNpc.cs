using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FourNormalWolfNpc : ABrain
	{
		private Player player_0;

		private int int_0;

		private SimpleBoss simpleBoss_0;

		private bool bool_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			if (bool_0)
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
			if (simpleBoss_0 == null)
			{
				SimpleBoss[] array = base.Game.FindLivingTurnBossWithID(int_0);
				int num = 0;
				if (array.Length != 0)
				{
					SimpleBoss simpleBoss = (simpleBoss_0 = array[num]);
				}
			}
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if ((int)simpleBoss_0.Properties1 == 0 && bool_0)
			{
				base.Body.PlayMovie("BtoA", 1000, 4000);
				base.Body.CallFuction(method_3, 5000);
				bool_0 = false;
			}
			else if ((int)simpleBoss_0.Properties1 == 1 && !bool_0)
			{
				base.Body.PlayMovie("AtoB", 2000, 4000);
				base.Body.CallFuction(method_2, 4000);
				bool_0 = true;
			}
			else if (bool_0 && player_0 != null)
			{
				if (base.Game.Random.Next(100) < 50 && base.Body.Distance(player_0.X, player_0.Y) > 600.0)
				{
					method_5();
				}
				else
				{
					method_4();
				}
			}
			else if (!bool_0)
			{
				method_0();
			}
			else
			{
				Console.WriteLine("eye: " + bool_0.ToString() + " - friendBoss.Properties1: " + simpleBoss_0.Properties1);
			}
		}

		private void method_0()
		{
			base.Body.ChangeDirection(-1, 1200);
			base.Body.ChangeDirection(1, 1800);
			base.Body.ChangeDirection(-1, 2300);
			int num = base.Game.Random.Next(-300, 300);
			base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walkA", 3500, method_1);
		}

		private void method_1()
		{
			base.Body.ChangeDirection(1, 500);
			base.Body.ChangeDirection(-1, 1000);
			base.Body.PlayMovie("beatA", 1000, 2000, method_7);
		}

		private void method_2()
		{
			player_0 = base.Game.FindRandomPlayer();
			if (player_0 != null)
			{
				base.Body.ChangeDirection(player_0, 500);
				((PVEGame)base.Game).SendPlayersPicture(player_0, 7, state: true);
				base.Body.Say("Lo chạy đi <p class=\"red\">" + player_0.PlayerDetail.PlayerCharacter.NickName + "</p>Ta đến đây...", 0, 2000, 0);
				base.Body.CallFuction(method_7, 5000);
			}
		}

		private void method_3()
		{
			if (player_0 != null)
			{
				((PVEGame)base.Game).SendPlayersPicture(player_0, 7, state: false);
				player_0 = null;
			}
			base.Body.CallFuction(method_7, 1000);
		}

		private void method_4()
		{
			base.Body.ChangeDirection(player_0, 200);
			if (base.Body.Distance(player_0.X, player_0.Y) > 600.0)
			{
				base.Body.MoveTo(player_0.X, player_0.Y, "walkB", 2200, method_6, 10);
			}
			else
			{
				base.Body.MoveTo(player_0.X, player_0.Y, "walkC", 2200, method_6, 5);
			}
		}

		private void method_5()
		{
			base.Body.ChangeDirection(player_0, 200);
			base.Body.PlayMovie("jump", 2000, 0);
			((PVEGame)base.Game).SendObjectFocus(player_0, 1, 5000, 0);
			base.Body.BoltMove(player_0.X, player_0.Y, 6000);
			base.Body.PlayMovie("fall", 7000, 0);
			base.Body.RangeAttacking(player_0.X - 100, player_0.X + 100, "cry", 8000, null);
			base.Body.CallFuction(method_6, 8500);
		}

		private void method_6()
		{
			base.Body.CurrentDamagePlus = 2f;
			base.Body.Beat(player_0, "beatB", 0, 0, 0);
			base.Body.CallFuction(method_7, 4000);
		}

		public override void OnAfterTakedBomb()
		{
			base.OnAfterTakedBomb();
			if (!bool_0)
			{
				int num = simpleBoss_0.Blood - base.Body.Blood;
				simpleBoss_0.AddBlood(-num, 1);
			}
		}

		private void method_7()
		{
			if (bool_0)
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

		public FourNormalWolfNpc()
		{
			int_0 = 4105;
		}
	}
}
