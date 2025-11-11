using Game.Logic.PetEffects.ContinueElement;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
	public class AE1118 : BasePetEffect
	{
		private int m_type;
		private int m_count;
		private int m_probability;
		private int m_delay;
		private int m_currentId;
		public AE1118(int count, int probability, int type, int skillId, int delay, string elementID) : base(ePetEffectType.AE1118, elementID)
		{
			this.m_count = count;
			this.m_probability = ((probability == -1) ? 10000 : probability);
			this.m_type = type;
			this.m_delay = delay;
			this.m_currentId = skillId;
		}
		public override bool Start(Living living)
		{
			AE1118 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1118) as AE1118;
			if (effect != null)
			{
				effect.m_probability = ((this.m_probability > effect.m_probability) ? this.m_probability : effect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerCure += new PlayerEventHandle(this.player_AfterKilledByLiving);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerCure -= new PlayerEventHandle(this.player_AfterKilledByLiving);
		}
		private void player_AfterKilledByLiving(Living living)
		{
			if (this.rand.Next(10000) < this.m_probability)
			{
				living.PetEffectTrigger = true;
				new CE1118(2, this.m_currentId, base.Info.ID.ToString()).Start(living);
			}
		}
	}
}
