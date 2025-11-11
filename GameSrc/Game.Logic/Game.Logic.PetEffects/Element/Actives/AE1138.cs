using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1138 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1138(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1138, elementID)
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
            AE1138 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1138) as AE1138;
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
            player.PlayerBuffSkillPet += new PlayerEventHandle(player_AfterBuffSkillPetByLiving);
            player.AfterPlayerShooted += new PlayerEventHandle(player_AfterPlayerShooted);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(player_AfterBuffSkillPetByLiving);
            player.AfterPlayerShooted -= new PlayerEventHandle(player_AfterPlayerShooted);
        }

        void player_AfterBuffSkillPetByLiving(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                m_added = 100;
                player.YHM_UseSkillPetWithProp = true;
                player.PetEffects.CritRate += m_added;
            }
        }

        private void player_AfterPlayerShooted(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                player.PetEffects.CritRate -= m_added;
                m_added = 0;
            }
        }
    }
}