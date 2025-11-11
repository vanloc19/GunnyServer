using Game.Logic.CardEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Logic.CardEffect.Effects
{
    public class GuluKingdom4Effect : BaseCardEffect
    {
        private int m_indexValue = 0;
        private int m_value = 0;
        private int m_added = 0;
        public GuluKingdom4Effect(int index, CardBuffInfo info)
            : base(eCardEffectType.GuluKingdom4, info)
        {
            m_indexValue = index;
            string[] values = info.Value.Split('|');
            if (m_indexValue < values.Length)
                m_value = int.Parse(values[m_indexValue]);
        }

        public override bool Start(Living living)
        {
            if (living.CardEffectList.GetOfType(eCardEffectType.GuluKingdom4) is GuluKingdom4Effect effect)
                return true;
            else
                return base.Start(living);
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.AfterKillingLiving += ChangeProperty;
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKillingLiving -= ChangeProperty;
        }

        private void ChangeProperty(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (living.Game is PVEGame && (living.Game as PVEGame).Info.ID == 1 && (target is SimpleNpc || target is SimpleBoss))
            {
                Console.WriteLine("DamageAmount = {0}", m_value);
                damageAmount += m_value;
            }
        }
    }
}
