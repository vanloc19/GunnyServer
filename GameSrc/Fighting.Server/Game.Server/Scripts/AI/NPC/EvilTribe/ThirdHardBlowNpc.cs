using Game.Logic.AI;

namespace GameServerScript.AI.NPC
{
	public class ThirdHardBlowNpc : ABrain
	{
		private int int_0;

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
			if (int_0 == 1)
			{
				NoDame();
			}
			else if (int_0 == 2)
			{
				Beat();
			}
			else
			{
				NoDame();
				int_0 = 1;
			}
			int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public void NoDame()
		{
			base.Body.PlayMovie("stand", 100, 0);
		}

		public void Beat()
		{
			base.Body.PlayMovie("beatA", 500, 0);
			base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1500, null);
			base.Body.Die(1000);
		}

		public ThirdHardBlowNpc()
		{
			int_0 = 1;
		}
	}
}
