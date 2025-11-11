using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE1106 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public PE1106(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE1106, elementID)
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
            PE1106 effect = living.PetEffectList.GetOfType(ePetEffectType.PE1106) as PE1106;
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
            player.PlayerShoot += player_Shooted;
            player.AfterKilledByLiving += Player_AfterKilledByLiving;
        }

        private void Player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (m_currentId == 99)
            {
                ((Player)living).AddPetMP(1);
            }
        }

        private void player_Shooted(Player player)
        {
            if (m_currentId == 95 || m_currentId == 116)
            {
                player.AddPetMP(1);
            }
        }


        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= player_Shooted;
            player.AfterKilledByLiving -= Player_AfterKilledByLiving;
        }
    }
}