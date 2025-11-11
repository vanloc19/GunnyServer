using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE2981 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private bool m_triggered = false;

        public PE2981(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE2981, elementID)
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
            PE2981 effect = living.PetEffectList.GetOfType(ePetEffectType.PE2981) as PE2981;
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
            player.BeforeTakeDamage += Player_BeforeTakeDamage;
        }

        private void Player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (m_triggered) return;

            // Trigger when taking critical damage that would kill
            if (criticalAmount > 0 && !living.IsLiving && damageAmount + criticalAmount >= living.Blood)
            {
                m_triggered = true;
                // Phoenix Nirvana effect: revive or heal
                // For now, just prevent death or add health
                criticalAmount = 0;
                damageAmount = living.Blood - 1;
                if (living.Blood <= 0)
                {
                    // Revival effect could be implemented here
                    living.Blood = living.MaxBlood / 2; // Heal to 50%
                }
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeforeTakeDamage -= Player_BeforeTakeDamage;
        }
    }
}

