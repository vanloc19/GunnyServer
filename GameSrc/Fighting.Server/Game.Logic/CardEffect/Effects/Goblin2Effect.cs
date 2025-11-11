using Game.Logic.CardEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Logic.CardEffect.Effects
{
    public class Goblin2Effect : BaseCardEffect
    {
        private int m_indexValue = 0;
        private int m_value = 0;
        private int m_added = 0;
        public Goblin2Effect(int index, CardBuffInfo info)
            : base(eCardEffectType.Goblin2, info)
        {
            m_indexValue = index;
            string[] values = info.Value.Split('|');
            if (m_indexValue < values.Length)
                m_value = int.Parse(values[m_indexValue]);
        }

        public override bool Start(Living living)
        {
            if (living.CardEffectList.GetOfType(eCardEffectType.Goblin2) is Goblin2Effect effect)
                return true;
            else
                return base.Start(living);
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginNextTurn += ChangeProperty;            
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginNextTurn -= ChangeProperty;
        }

        private void ChangeProperty(Living player)
        {
            if (m_added != 0)
                return;

            if (player.Game is PVEGame && (player.Game as PVEGame).Info.ID == 5)
            {
                (player as Player).AddDander(m_value);
                m_added = m_value;
            }
        }
    }
}
