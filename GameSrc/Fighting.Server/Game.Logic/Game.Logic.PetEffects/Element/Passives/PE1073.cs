using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE1073 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public PE1073(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE1073, elementID)
        {
            m_count = count;
            m_coldDown = 0;
            m_probability = probability == -1 ? 10000 : probability;
            m_type = type;
            m_delay = delay;
            m_currentId = skillId;
        }

        public override bool Start(Living living)
        {
            PE1073 effect = living.PetEffectList.GetOfType(ePetEffectType.PE1073) as PE1073;
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
            player.BeginSelfTurn += player_BeginSelfTurn;
            player.AfterKilledByLiving += zplayer_AfterKilledByLiving;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= player_BeginSelfTurn;
            player.AfterKilledByLiving -= zplayer_AfterKilledByLiving;
        }

        private void player_BeginSelfTurn(Living liv)
        {
            if (m_coldDown < 5)
            {
                if (m_added == 0)
                    m_added = 30;
                Player player = liv as Player;
                player.Game.SendPetBuff(liv, ElementInfo, true);
                player.BaseDamage += m_added;
                m_count += m_added;
                m_coldDown++;
            }
        }

        private void zplayer_AfterKilledByLiving(Living liv , Living target, int damageAmount, int criticalAmount)
        {
            ExitEffect(liv);
        }

        private void ExitEffect(Living liv)
        {
            if(m_coldDown > 0)
            {
                Player player = liv as Player;
                var remove = 30 * m_coldDown;
                Console.WriteLine($"remove ={remove}");
                player.BaseDamage -= remove;
                player.Game.SendPetBuff(player, ElementInfo, false);
                m_coldDown = 0;
                m_added = 0;
            }
        }
    }
}