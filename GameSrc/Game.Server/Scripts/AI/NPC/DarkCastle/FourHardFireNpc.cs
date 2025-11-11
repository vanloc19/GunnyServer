using Game.Logic.AI;

namespace GameServerScript.AI.NPC
{
	public class FourHardFireNpc : ABrain
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
			int num = base.Game.Random.Next(base.Body.X - 300, base.Body.X + 300);
			int num2 = base.Game.Random.Next(base.Body.Y - 300, base.Body.Y + 300);
			num = ((num < 50) ? 50 : num);
			num = ((num > base.Game.Map.Info.DeadWidth - 50) ? (base.Game.Map.Info.DeadWidth - 50) : num);
			num2 = ((num2 > 750) ? 750 : num2);
			num2 = ((num2 < 50) ? 50 : num2);
			base.Body.MoveTo(num, num2, "fly", 1000);
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}
