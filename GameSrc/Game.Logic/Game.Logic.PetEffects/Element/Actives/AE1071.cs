using Game.Logic.PetEffects.ContinueElement;
using Game.Logic.PetEffects.Element.Passives;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1071 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1071(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1071, elementID)
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
            AE1071 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1071) as AE1071;
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
            player.AfterKilledByLiving += Player_AfterKilledByLiving;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKilledByLiving -= Player_AfterKilledByLiving;
        }

        private void Player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (damageAmount > 0 || criticalAmount > 0)
            {
                PE1072 eff1 = living.PetEffectList.GetOfType(ePetEffectType.PE1072) as PE1072;
                if (eff1 != null)
                    eff1.Stop();
                PE1073 eff2 = living.PetEffectList.GetOfType(ePetEffectType.PE1073) as PE1073;
                if (eff2 != null)
                    eff2.Stop();
                living.Game.SendPetBuff(living, ElementInfo, false);
            }
        }
    }
}