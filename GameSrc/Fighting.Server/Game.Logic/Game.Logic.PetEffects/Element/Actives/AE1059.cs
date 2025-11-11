using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1059 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1059(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1059, elementID)
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
            AE1059 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1059) as AE1059;
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
        }

        private void Player_PlayerBuffSkillPet(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                player.SyncAtTime = true;
                player.AddBlood(2000);
                player.SyncAtTime = false;
                List<Living> allies = player.Game.Map.FindAllNearestSameTeam(player.X, player.Y, 250, player);
                foreach (Living ally in allies)
                {
                    m_added = 2000;
                    ally.SyncAtTime = true;
                    ally.AddBlood(m_added);
                    ally.SyncAtTime = false;
                }
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= Player_PlayerBuffSkillPet;
        }
    }
}