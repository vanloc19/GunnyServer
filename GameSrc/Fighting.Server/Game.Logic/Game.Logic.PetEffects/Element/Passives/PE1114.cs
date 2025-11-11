using Game.Logic.PetEffects.ContinueElement;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE1114 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public PE1114(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE1114, elementID)
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
            PE1114 effect = living.PetEffectList.GetOfType(ePetEffectType.PE1114) as PE1114;
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

        private void Player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (rand.Next(100) <= 20)
            {
                m_added = 500;
                if (m_added > 0)
                {
                    target.SyncAtTime = true;
                    target.AddBlood(-m_added, 1);
                    target.SyncAtTime = false;
                    if (target.Blood <= 0)
                    {
                        target.Die();
                        if (living != null && living is Player)
                            (living as Player).PlayerDetail.OnKillingLiving(living.Game, 2, living.Id, living.IsLiving,
                                m_added);
                    }
                    else
                    {
                        target.AddPetEffect(
                            new CE1114(0, m_probability, m_type, m_currentId, m_delay, ElementInfo.ID.ToString()), 0);
                    }

                    living.PetEffects.ActiveEffect = false;
                }
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKilledByLiving -= Player_AfterKilledByLiving;
        }
    }
}