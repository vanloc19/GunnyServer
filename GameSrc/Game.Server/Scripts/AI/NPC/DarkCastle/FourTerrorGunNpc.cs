using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class FourTerrorGunNpc : ABrain
	{
		private bool bool_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.Properties1 = 0;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			bool flag = false;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
				{
					base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
					flag = true;
				}
			}
			if (flag)
			{
				method_0(base.Body.X - 300, base.Body.X + 300);
			}
			else if (!bool_0)
			{
				base.Body.PlayMovie("beatA", 1000, 0);
				((PVEGame)base.Game).SendFreeFocus(900, 698, 1, 2600, 0);
				base.Body.CallFuction(method_1, 3000);
				base.Body.Properties1 = 1;
				bool_0 = true;
			}
			else
			{
				base.Body.PlayMovie("beatB", 1000, 0);
				((PVEGame)base.Game).SendFreeFocus(900, 698, 2, 2500, 1000);
				base.Body.RangeAttacking(409, 1900, "cry", 4000, removeFrost: false, null);
				base.Body.CallFuction(method_1, 4000);
				base.Body.Properties1 = 0;
				bool_0 = false;
			}
		}

		private void method_0(int int_0, int int_1)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatC", 1000, 0);
			base.Body.RangeAttacking(int_0, int_1, "cry", 2000, null);
			base.Body.CallFuction(method_1, 2500);
		}

		private void method_1()
		{
			if (bool_0)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			}
			else
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
