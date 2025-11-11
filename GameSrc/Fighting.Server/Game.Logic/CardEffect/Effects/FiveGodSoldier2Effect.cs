using Game.Logic.CardEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Logic.CardEffect.Effects
{
    public class FiveGodSoldier2Effect : BaseCardEffect
    {
        private int m_indexValue = 0;
        private int m_value = 0;
        private double m_added = 0;
        public FiveGodSoldier2Effect(int index, CardBuffInfo info)
            : base(eCardEffectType.FiveGodSoldier2, info)
        {
            m_indexValue = index;
            string[] values = info.Value.Split('|');
            if (m_indexValue < values.Length)
                m_value = int.Parse(values[m_indexValue]);
        }

        public override bool Start(Living living)
        {
            if (living.CardEffectList.GetOfType(eCardEffectType.FiveGodSoldier2) is FiveGodSoldier2Effect effect)
                return true;
            else
                return base.Start(living);
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            //player.PlayerReset += ChangeProperty;
            player.AfterKillingLiving += Player_AfterKillingLiving;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            //player.PlayerReset -= ChangeProperty;
            player.AfterKillingLiving -= Player_AfterKillingLiving;
        }

        private void Player_AfterKillingLiving(Living player, Living target, int damageAmount, int criticalAmount)
        {
            if (player.Game is PVPGame && (player.Game as PVPGame).IsMatchOrFreedom())
            {
                if (m_added > 0)
                {
                    m_added = m_value * damageAmount / 100;
                    player.SyncAtTime = true;
                    player.AddBlood((int)m_added);
                    player.SyncAtTime = false;
                }
            }
        }

        private void ChangeProperty(Player player)
        {
            if (m_added != 0)
            {
                player.AddMaxBlood(-m_value);
                m_added = 0;
            }
            if (player.Game is PVPGame && (player.Game as PVPGame).IsMatchOrFreedom())
            {
                player.AddMaxBlood(m_value);
                m_added = m_value;
            }
        }
    }
}
