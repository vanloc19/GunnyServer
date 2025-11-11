using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.ContinueElement
{
    public class CE1114 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public CE1114(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.CE1114, elementID)
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
            CE1114 effect = living.PetEffectList.GetOfType(ePetEffectType.CE1114) as CE1114;
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
            player.BeginNextTurn += Player_BeginNextTurn;
            player.PlayerClearBuffSkillPet += Player_PlayerClearBuffSkillPet;
        }

        private void Player_PlayerClearBuffSkillPet(Player player)
        {
            Stop();
        }

        private void Player_BeginNextTurn(Living living)
        {
            m_count--;
            if (m_count < 0)
            {
                Stop();
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginNextTurn -= Player_BeginNextTurn;
        }
    }
}