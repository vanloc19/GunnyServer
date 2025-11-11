using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdTerrorFixureLongNpc : ABrain
	{
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
			method_0();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void method_0()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = 1.8f;
			if (player != null && base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 1, 2f, 2300))
			{
				base.Body.PlayMovie("beatA", 1500, 0);
			}
		}
	}
}
