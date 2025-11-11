using Game.Logic.PetEffects.ContinueElement;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1053 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1053(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1053, elementID)
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
            AE1053 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1053) as AE1053;
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
            player.PlayerBeginMoving += Player_PlayerBeginMoving;
            player.PlayerBuffSkillPet += Player_PlayerBuffSkillPet;
        }

        private void Player_PlayerBuffSkillPet(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                IsTrigger = true;
            }
        }

        private void Player_PlayerBeginMoving(Player player)
        {
            if (IsTrigger)
            {
                CE1049 effect1 = player.PetEffectList.GetOfType(ePetEffectType.CE1049) as CE1049;
                if (effect1 != null)
                {
                    effect1.Stop();
                }

                CE1050 effect2 = player.PetEffectList.GetOfType(ePetEffectType.CE1050) as CE1050;
                if (effect2 != null)
                {
                    effect2.Stop();
                }

                IsTrigger = false;
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBeginMoving -= Player_PlayerBeginMoving;
        }
    }
}