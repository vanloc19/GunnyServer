using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdTerrorBloomNpc : ABrain
	{
		private Player player_0;

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
			if (base.Body.Blood >= base.Body.MaxBlood)
			{
				base.Body.PlayMovie("grow", 100, 0);
				base.Body.Die(1000);
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
