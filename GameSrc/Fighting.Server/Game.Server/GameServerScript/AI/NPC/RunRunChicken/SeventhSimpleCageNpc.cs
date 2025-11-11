using Game.Logic.AI;

namespace GameServerScript.AI.NPC
{
	public class SeventhSimpleCageNpc : ABrain
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
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public override void OnDie()
		{
			base.OnDie();
			base.Body.Say("Hoan hô được cứu rồi!!!", 0, 1000);
		}

		public SeventhSimpleCageNpc()
		{


		}
	}
}
