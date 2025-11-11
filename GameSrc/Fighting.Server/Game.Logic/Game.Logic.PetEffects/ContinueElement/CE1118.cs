using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.PetEffects.ContinueElement
{
	public class CE1118 : AbstractPetEffect
	{
		private int m_count;
		private int m_added;
		public CE1118(int count, int skilId, string elementID) : base(ePetEffectType.CE1118, elementID)
		{
			this.m_count = count;
			switch(skilId)
            {
				case 74:
					m_added = 300;
					break;
				case 75:
					m_added = 500;
					break;
            }
		}
		public override bool Start(Living living)
		{
			CE1118 effect = living.PetEffectList.GetOfType(ePetEffectType.CE1118) as CE1118;
			if (effect != null)
			{
				effect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.Defence += (double)this.m_added;
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
		}
		public override void OnRemoved(Living living)
		{
			living.Defence -= (double)this.m_added;
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (this.m_count < 0)
			{
				this.Stop();
			}
		}
	}
}
