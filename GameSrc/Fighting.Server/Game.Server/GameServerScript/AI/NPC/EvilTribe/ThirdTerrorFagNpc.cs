using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdTerrorFagNpc : ABrain
	{
		private int int_0;

		private int int_1;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			int_0++;
			if (int_0 >= 4)
			{
				base.Body.Die();
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
		}

		private void method_0()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				(allLivingPlayer.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect)?.Stop();
			}
		}

		public override void OnDie()
		{
			base.OnDie();
			method_0();
		}

		public override void OnCreated()
		{
			base.OnCreated();
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (!(allLivingPlayer.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) is ReduceStrengthEffect))
				{
					allLivingPlayer.AddEffect(new ReduceStrengthEffect(200, int_1), 0);
				}
			}
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public ThirdTerrorFagNpc()
		{
			int_1 = 150;
		}
	}
}
