using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE2979 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;

        public PE2979(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE2979, elementID)
        {
            m_count = count;
            m_coldDown = count;
            m_probability = probability == -1 ? 10000 : probability;
            m_type = type;
            m_delay = delay;
            m_currentId = skillId;
        }

        public override bool Start(Living living)
        {
            PE2979 effect = living.PetEffectList.GetOfType(ePetEffectType.PE2979) as PE2979;
            if (effect != null)
            {
                effect.m_probability = m_probability > effect.m_probability ? m_probability : effect.m_probability;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            if (player.PetEffects.MagicAttackPercent < 5)
                player.PetEffects.MagicAttackPercent = 5;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
        }
    }
}

