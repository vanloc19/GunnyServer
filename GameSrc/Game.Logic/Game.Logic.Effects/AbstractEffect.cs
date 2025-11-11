using System;
using Bussiness;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
	public abstract class AbstractEffect
	{
		public bool IsTrigger;

		protected Living m_living;

		private eEffectType m_type;

		protected Random rand = new Random();

		protected static ThreadSafeRandom random = new ThreadSafeRandom();

		public eEffectType Type => m_type;

		public int TypeValue => (int)m_type;

		public AbstractEffect(eEffectType type)
		{
			m_type = type;
		}

		public virtual void OnAttached(Living living)
		{
		}

		public virtual void OnRemoved(Living living)
		{
		}

		public virtual bool Start(Living living)
		{
			m_living = living;
			return m_living.EffectList.Add(this);
		}

		public virtual bool Stop()
		{
			if (m_living != null)
			{
				return m_living.EffectList.Remove(this);
			}
			return false;
		}
	}
}
