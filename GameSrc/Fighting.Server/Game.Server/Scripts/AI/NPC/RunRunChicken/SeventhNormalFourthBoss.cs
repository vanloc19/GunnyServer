using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class SeventhNormalFourthBoss : ABrain
	{
		private int int_0;

		private List<PhysicalObj> list_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			method_3();
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
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			int_0++;
			if (int_0 == 1)
			{
				method_0();
				return;
			}
			method_1();
			int_0 = 0;
		}

		private void method_0()
		{
			base.Body.CurrentDamagePlus = 2f;
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.CallFuction(method_2, 4000);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 4500, null);
		}

		private void method_1()
		{
			Player player = base.Game.FindRandomPlayer();
			if (base.Body.ShootPoint(player.X, player.Y, 84, 1200, 10000, 1, 3f, 3000))
			{
				base.Body.PlayMovie("beat", 1000, 0);
			}
		}

		private void method_2()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				list_0.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "moive", "asset.game.seven.cao", "in", 1, 0));
			}
		}

		private void method_3()
		{
			foreach (PhysicalObj item in list_0)
			{
				if (item != null)
				{
					base.Game.RemovePhysicalObj(item, sendToClient: true);
				}
			}
			list_0 = new List<PhysicalObj>();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public SeventhNormalFourthBoss()
		{

			list_0 = new List<PhysicalObj>();

		}
	}
}
