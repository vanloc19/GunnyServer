using Game.Logic.CardEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Logic.CardEffect.Effects
{
    public class RunRunChicken2Effect : BaseCardEffect
    {
        private int m_indexValue = 0;
        private int m_value = 0;
        private double m_added = 0;
        public RunRunChicken2Effect(int index, CardBuffInfo info)
            : base(eCardEffectType.RunRunChicken2, info)
        {
            m_indexValue = index;
            string[] values = info.Value.Split('|');
            if (m_indexValue < values.Length)
                m_value = int.Parse(values[m_indexValue]);
        }

        public override bool Start(Living living)
        {
            if (living.CardEffectList.GetOfType(eCardEffectType.RunRunChicken2) is RunRunChicken2Effect effect)
                return true;
            else
                return base.Start(living);
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerAfterReset += ChangeProperty;            
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerAfterReset -= ChangeProperty;
        }

        private void ChangeProperty(Player player)
        {
            if (m_added != 0)
            {
                player.BaseGuard -= m_added;
                m_added = 0;
            }
            if (player.Game is PVEGame && (player.Game as PVEGame).Info.ID == 7)
            {
                m_added = (player.BaseGuard * m_value) / 100;
                player.BaseGuard += m_added;
            }
        }
    }
}
