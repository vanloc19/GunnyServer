using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1084 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1084(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1084, elementID)
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
            AE1084 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1084) as AE1084;
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
        }

        protected override void OnPausedOnPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(player_AfterBuffSkillPetByLiving);
            player.Game.SendPetBuff(player, ElementInfo, false);
            player.Attack -= m_added;
            IsTrigger = false;
        }

        void player_AfterBuffSkillPetByLiving(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId && !IsTrigger)
            {
                IsTrigger = true;
                m_added = 800;
                player.Attack += m_added;
                player.Game.SendPetBuff(player, ElementInfo, true);
            }
        }
    }
}