using Game.Logic.AI;

namespace GameServerScript.AI.NPC
{
	public class FourHardCycLoneNpc : ABrain
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
			base.Body.Properties1 = 0;
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			int num = base.Game.Random.Next(base.Body.X - 150, base.Body.X + 150);
			int num2 = base.Game.Random.Next(base.Body.Y - 150, base.Body.Y + 150);
			num = ((num >= 0) ? num : 0);
			num = ((num > base.Game.Map.Info.DeadWidth) ? base.Game.Map.Info.DeadWidth : num);
			num2 = ((num2 > 778) ? 778 : num2);
			base.Body.MoveTo(num, num2, "fly", 1000);
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
