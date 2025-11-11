using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1105 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1105(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1105, elementID)
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
            AE1105 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1105) as AE1105;
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
            player.PlayerBuffSkillPet += Player_PlayerBuffSkillPet;
            player.PlayerCompleteShoot += Player_PlayerCompleteShoot;
        }

        private void Player_PlayerCompleteShoot(Player player)
        {
            player.PetEffects.AddBloodPercent = 0;
        }

        private void Player_PlayerBuffSkillPet(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                m_added = 1000;
                player.PetEffects.AddBloodPercent = m_added;
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= Player_PlayerBuffSkillPet;
            player.PlayerCompleteShoot -= Player_PlayerCompleteShoot;
        }
    }
}