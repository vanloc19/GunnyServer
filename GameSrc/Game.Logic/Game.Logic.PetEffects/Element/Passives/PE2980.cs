using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE2980 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;

        public PE2980(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE2980, elementID)
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
            PE2980 effect = living.PetEffectList.GetOfType(ePetEffectType.PE2980) as PE2980;
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
            if (player.PetEffects.MagicDefencePercent < 5)
                player.PetEffects.MagicDefencePercent = 5;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
        }
    }
}

