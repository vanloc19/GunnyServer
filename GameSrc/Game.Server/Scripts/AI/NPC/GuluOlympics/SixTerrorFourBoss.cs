using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class SixTerrorFourBoss : ABrain
	{
		private int int_0;

		private PhysicalObj physicalObj_0;

		private Player player_0;

		private bool bool_0;

		private SimpleNpc simpleNpc_0;

		private int int_1;

		private int int_2;

		private int int_3;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			if (bool_0 && simpleNpc_0 != null && !simpleNpc_0.IsLiving)
			{
				bool_0 = false;
				base.Body.Config.HaveShield = false;
				base.Game.RemoveLiving(simpleNpc_0.Id);
				(base.Game as PVEGame).SendLivingActionMapping(base.Body, "shield", "shield");
				(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "stand");
				(base.Game as PVEGame).SendHideBlood(base.Body, 1);
			}
			else if (bool_0 && simpleNpc_0 != null && simpleNpc_0.IsLiving && (simpleNpc_0.X != base.Body.X || simpleNpc_0.Y != base.Body.Y))
			{
				simpleNpc_0.BoltMove(base.Body.X, base.Body.Y, 0);
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			bool_0 = false;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			if (int_0 == 0)
			{
				method_3();
				int_0++;
			}
			else if (int_0 == 1)
			{
				method_2();
				int_0++;
			}
			else if (int_0 == 2)
			{
				method_0();
				int_0++;
			}
			else if (int_0 == 3)
			{
				if (!bool_0)
				{
					method_11();
				}
				else
				{
					method_5();
					int_0++;
				}
				int_0++;
			}
			else if (int_0 == 4)
			{
				method_5();
				int_0++;
			}
			else if (int_0 == 5)
			{
				method_9();
				int_0++;
			}
			else
			{
				method_7();
				int_0 = 0;
			}
		}

		private void method_0()
		{
			player_0 = base.Game.FindRandomPlayer();
			if (base.Body.X > player_0.X)
			{
				base.Body.MoveTo(player_0.X + 150, base.Body.Y, "walk", 1000, "", 5, method_1);
			}
			else
			{
				base.Body.MoveTo(player_0.X - 150, base.Body.Y, "walk", 1000, "", 5, method_1);
			}
			if (player_0.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 1200);
			}
			else
			{
				base.Body.ChangeDirection(-1, 1200);
			}
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			base.Body.PlayMovie("beatB", 1500, 0);
			base.Body.BeatDirect(player_0, "", 4000, 3, 1);
		}

		private void method_2()
		{
			base.Body.CurrentDamagePlus = 2.3f;
			base.Body.PlayMovie("beatC", 1500, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
		}

		private void method_3()
		{
			player_0 = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = 1.7f;
			base.Body.PlayMovie("beatD", 1000, 3000);
			base.Body.JumpTo(player_0.X, base.Body.Y - 475, "", 1500, 0, 5, method_4, 1);
		}

		private void method_4()
		{
			base.Body.BeatDirect(player_0, "", 4000, int_2, 1);
		}

		private void method_5()
		{
			base.Body.PlayMovie("beatE", 3000, 0);
			base.Body.CallFuction(method_6, 3200);
		}

		private void method_6()
		{
			base.Body.PlayMovie("stand", 8500, 1000);
			physicalObj_0 = ((PVEGame)base.Game).Createlayerboss(base.Body.X - 500, 550, "front", "asset.game.six.chang", "in", 1, 0);
			((PVEGame)base.Game).SendGameFocus(1250, 450, 1, 0, 4000);
			base.Body.CurrentDamagePlus = 2f;
			base.Body.RangeAttacking(base.Body.X - 3000, base.Body.X + 3000, "cry", 7000, null);
		}

		private void method_7()
		{
			int x = base.Game.Random.Next(900, 1400);
			base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, method_8);
		}

		private void method_8()
		{
			Player player = base.Game.FindRandomPlayer();
			if (player.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 800);
			}
			else
			{
				base.Body.ChangeDirection(-1, 800);
			}
			base.Body.CurrentDamagePlus = 1.3f;
			if (player != null)
			{
				if (base.Body.ShootPoint(player.X, player.Y, 61, 1000, 10000, 1, 3f, 2300))
				{
					base.Body.PlayMovie("beatA", 1500, 0);
				}
				if (base.Body.ShootPoint(player.X, player.Y, 61, 1000, 10000, 1, 3f, 4100))
				{
					base.Body.PlayMovie("beatA", 3300, 0);
				}
			}
		}

		private void method_9()
		{
			base.Body.CallFuction(method_10, 2000);
		}

		private void method_10()
		{
			base.Body.SyncAtTime = true;
			base.Body.PlayMovie("shieldB", 0, 2500);
			base.Body.AddBlood(int_1);
		}

		private void method_11()
		{
			base.Body.SyncAtTime = true;
			base.Body.PlayMovie("inX", 0, 4500);
			base.Body.CallFuction(method_12, 5000);
		}

		private void method_12()
		{
			LivingConfig config = (base.Game as PVEGame).BaseLivingConfig();
			simpleNpc_0 = (base.Game as PVEGame).CreateNpc(int_3, base.Body.X, base.Body.Y, 1, base.Body.Direction, config);
			bool_0 = true;
			base.Body.Config.HaveShield = true;
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "shield", "shieldB");
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "standC");
			(base.Game as PVEGame).SendHideBlood(base.Body, 0);
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public SixTerrorFourBoss()
		{
			
			int_1 = 6000;
			int_2 = 5;
			int_3 = 6344;
			
		}
	}
}
