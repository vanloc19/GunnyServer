using Bussiness;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class AddGuardTurnEffect : BasePlayerEffect
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AddGuardTurnEffect(int count, int probability)
            : base(eEffectType.AddGuardTurnEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            AddGuardTurnEffect effect =
                living.EffectList.GetOfType(eEffectType.AddGuardTurnEffect) as AddGuardTurnEffect;
            if (effect != null)
            {
                m_probability = m_probability > effect.m_probability ? m_probability : effect.m_probability;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeforeTakeDamage += new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
            player.BeginSelfTurn += new LivingEventHandle(player_SelfTurn);
            player.Game.SendPlayerPicture(player, (int)BuffType.GuardEffect, true);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
            player.BeginSelfTurn -= new LivingEventHandle(player_SelfTurn);
            player.Game.SendPlayerPicture(player, (int)BuffType.GuardEffect, false);
        }

        private void player_AfterPlayerShooted(Player player)
        {
            player.FlyingPartical = 0;
        }

        void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            damageAmount -= m_count;
            if (damageAmount <= 0)
                damageAmount = 1;
        }

        void player_SelfTurn(Living living)
        {
            m_probability--;
            if (m_probability < 0)
                Stop();
        }
    }
}