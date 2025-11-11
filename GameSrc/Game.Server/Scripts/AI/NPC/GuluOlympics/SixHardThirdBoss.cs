using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class SixHardThirdBoss : ABrain
	{
		private Player player_0;

		private int int_0;

		private int int_1;

		private int int_2;

		private SimpleBoss simpleBoss_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (simpleBoss_0 == null)
			{
				simpleBoss_0 = base.Game.FindLivingTurnBossWithID(int_2)[0];
			}
			if (!simpleBoss_0.Config.CompleteStep)
			{
				simpleBoss_0.PlayMovie("stand", 0, 0);
				(base.Game as PVEGame).SendObjectFocus(simpleBoss_0, 1, 1000, 0);
				simpleBoss_0.PlayMovie("beida", 2000, 0);
				base.Body.CallFuction(method_0, 2000);
				simpleBoss_0.Config.CompleteStep = true;
				base.Body.CallFuction(method_1, 4000);
			}
			else
			{
				method_1();
			}
		}

		private void method_0()
		{
			(base.Game as PVEGame).SendLivingActionMapping(simpleBoss_0, "stand", "standB");
		}

		private void method_1()
		{
			player_0 = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
			int_0 = (int)player_0.Distance(base.Body.X, base.Body.Y);
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			if (int_1 == 0)
			{
				MoveToPlayerA(player_0);
				int_1++;
			}
			else if (int_1 == 1)
			{
				MoveToPlayerB(player_0);
				int_1++;
			}
			else if (int_1 == 2)
			{
				MoveToPlayerC(player_0);
				int_1++;
			}
			else if (int_1 == 3)
			{
				MoveToPlayerD(player_0);
				int_1++;
			}
			else if (int_1 == 4)
			{
				MoveToPlayerE(player_0);
				int_1++;
			}
			else
			{
				MoveToPlayerF(player_0);
				int_1 = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public void MoveToPlayerA(Player player)
		{
			base.Body.PlayMovie("walk", 0, 0);
			if (base.Body.X > player.X)
			{
				base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			else
			{
				base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			base.Body.CallFuction(BeatA, 1000);
		}

		public void BeatA()
		{
			base.Body.PlayMovie("beatA", 0, 0);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 900, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1400, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1900, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 2300, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 2700, null);
		}

		public void MoveToPlayerB(Player player)
		{
			base.Body.PlayMovie("walk", 0, 0);
			if (base.Body.X > player.X)
			{
				base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			else
			{
				base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			base.Body.CallFuction(BeatB, 1000);
		}

		public void BeatB()
		{
			base.Body.PlayMovie("beatB", 0, 0);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 900, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1400, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1900, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 2300, null);
		}

		public void MoveToPlayerC(Player player)
		{
			base.Body.PlayMovie("walk", 0, 0);
			if (base.Body.X > player.X)
			{
				base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			else
			{
				base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			base.Body.CallFuction(BeatC, 1000);
		}

		public void BeatC()
		{
			base.Body.PlayMovie("beatC", 0, 0);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 900, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1400, null);
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1900, null);
		}

		public void MoveToPlayerD(Player player)
		{
			base.Body.PlayMovie("walk", 0, 0);
			if (base.Body.X > player.X)
			{
				base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			else
			{
				base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			base.Body.CallFuction(BeatD, 1000);
		}

		public void BeatD()
		{
			base.Body.PlayMovie("beatD", 0, 0);
			base.Body.CurrentDamagePlus = 2f;
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1200, null);
		}

		public void MoveToPlayerE(Player player)
		{
			base.Body.PlayMovie("walk", 0, 0);
			if (base.Body.X > player.X)
			{
				base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			else
			{
				base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			base.Body.CallFuction(BeatE, 1000);
		}

		public void BeatE()
		{
			base.Body.PlayMovie("beatE", 0, 0);
			base.Body.CurrentDamagePlus = 2f;
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1200, null);
		}

		public void MoveToPlayerF(Player player)
		{
			if (base.Body.X > player.X)
			{
				base.Body.JumpTo(player.X + 5, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			else
			{
				base.Body.JumpTo(player.X - 5, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
			}
			base.Body.CallFuction(BeatF, 1000);
		}

		public void BeatF()
		{
			base.Body.PlayMovie("beatF", 0, 0);
			base.Body.CurrentDamagePlus = 2f;
			base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1500, null);
		}

		public SixHardThirdBoss()
		{
			
			int_2 = 6232;
			
		}
	}
}
