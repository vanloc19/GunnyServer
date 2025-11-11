using System;
using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE1193 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public PE1193(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE1193, elementID)
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
            PE1193 effect = living.PetEffectList.GetOfType(ePetEffectType.PE1193) as PE1193;
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
            player.Attack += 300;
            foreach (Player allTeamPlayer in player.Game.GetAllTeamPlayers(player))
            {
                allTeamPlayer.Attack += 300;
            }
            player.BeginNextTurn += Player_BeginNextTurn;
        }

        private void Player_BeginNextTurn(Living living)
        {
            if (living.IsShowEffectB)
            {
                living.Game.SendPetBuff(living, Info, true);
                living.IsShowEffectB = false;
            }
            #region
            /*if (!living.IsPassiveEffect)
                return;
            this.m_added = 100;
            (living as Player).Attack += m_added;
            foreach (Player allTeamPlayer in living.Game.GetAllTeamPlayers(living))
            {
                allTeamPlayer.Attack += (double)this.m_added;
                if (allTeamPlayer.IsShowEffectB)
                {
                    allTeamPlayer.Game.SendPetBuff(living, this.Info, true);
                    allTeamPlayer.IsShowEffectB = false;
                }
            }
            living.IsPassiveEffect = false;*/
            #endregion
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.Attack -= 300;
            foreach (Player allTeamPlayer in player.Game.GetAllTeamPlayers(player))
            {
                allTeamPlayer.Attack -= 300;
            }
            player.BeginNextTurn -= Player_BeginNextTurn;
        }
    }
}