using Game.Logic.AI;

namespace GameServerScript.AI.NPC
{
	public class EighthSmallBat : ABrain
	{
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
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int x = base.Game.Random.Next(base.Body.X - 100, base.Body.X + 100);
			int y = base.Game.Random.Next(base.Body.Y - 100, base.Body.Y + 100);
			base.Body.MoveTo(x, y, "stand", 1000);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
