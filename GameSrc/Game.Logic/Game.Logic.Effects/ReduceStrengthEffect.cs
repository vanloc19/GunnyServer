using Bussiness;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class ReduceStrengthEffect : AbstractEffect
    {
        private int m_count;
        private int m_reduce;

        public ReduceStrengthEffect(int count, int reduce)
            : base(eEffectType.ReduceStrengthEffect)
        {
            m_count = count;
            m_reduce = reduce;
        }

        public override bool Start(Living living)
        {
            ReduceStrengthEffect effect = living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
            if (effect != null)
            {
                effect.m_count = m_count;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, (int)BuffType.Tired, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, (int)BuffType.Tired, false);
        }

        void player_BeginFitting(Living living)
        {
            m_count--;
            if (living is Player)
                (living as Player).Energy -= m_reduce;
            if (m_count < 0)
            {
                if (living is Player)
                {
                    ((Player)living).LimitEnergy = false;
                }

                Stop();
            }
        }
    }
}