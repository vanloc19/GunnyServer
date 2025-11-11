using Game.Logic.PetEffects.ContinueElement;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE1235 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public PE1235(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE1235, elementID)
        {
            m_count = count;
            m_coldDown = 3;
            m_probability = probability == -1 ? 10000 : probability;
            m_type = type;
            m_delay = delay;
            m_currentId = skillId;
        }

        public override bool Start(Living living)
        {
            PE1235 effect = living.PetEffectList.GetOfType(ePetEffectType.PE1235) as PE1235;
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
            player.AfterKilledByLiving += new KillLivingEventHanlde(player_afterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKilledByLiving -= new KillLivingEventHanlde(player_afterKilledByLiving);
        }

        void player_afterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (living.PetEffects.ReboundDamage > 0)
            {
                target.Game.SendPetBuff(target, ElementInfo, true);
                if (target.Blood < 0)
                {
                    target.Die();
                    if (living != null && living is Player)
                        (living as Player).PlayerDetail.OnKillingLiving(living.Game, 2, living.Id, living.IsLiving,
                            target.PetEffects.ReboundDamage);
                }
                else
                {
                    target.AddPetEffect(
                        new CE1235(0, m_probability, m_type, m_currentId, m_delay, ElementInfo.ID.ToString()), 0);
                }
                living.PetEffects.ReboundDamage = 0;
            }
        }
    }
}