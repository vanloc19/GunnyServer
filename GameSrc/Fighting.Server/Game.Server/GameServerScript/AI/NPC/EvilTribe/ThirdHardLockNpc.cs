using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class ThirdHardLockNpc : ABrain
	{
		private int int_0;

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

		public override void OnDie()
		{
			base.OnDie();
			method_0();
		}

		private void method_0()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				(allLivingPlayer.EffectList.GetOfType(eEffectType.LockDirectionEffect) as LockDirectionEffect)?.Stop();
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (!(allLivingPlayer.EffectList.GetOfType(eEffectType.LockDirectionEffect) is LockDirectionEffect))
				{
					allLivingPlayer.AddEffect(new LockDirectionEffect(200), 0);
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
	}
}
