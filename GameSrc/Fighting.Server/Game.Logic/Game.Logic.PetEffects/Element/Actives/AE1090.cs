using Bussiness;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Actives
{
    public class AE1090 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public AE1090(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.AE1090, elementID)
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
            AE1090 effect = living.PetEffectList.GetOfType(ePetEffectType.AE1090) as AE1090;
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
            player.PlayerBeginMoving += Player_PlayerBeginMoving;
            player.PlayerBuffSkillPet += Player_PlayerBuffSkillPet;
        }

        private void Player_PlayerBuffSkillPet(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                IsTrigger = true;
            }
        }

        private void Player_PlayerBeginMoving(Player player)
        {
            if (IsTrigger)
            {
                AE1082 effect1 = player.PetEffectList.GetOfType(ePetEffectType.AE1082) as AE1082;
                if (effect1 != null)
                {
                    effect1.Pause();
                }

                AE1083 effect2 = player.PetEffectList.GetOfType(ePetEffectType.AE1083) as AE1083;
                if (effect2 != null)
                {
                    effect2.Pause();
                }

                AE1084 effect3 = player.PetEffectList.GetOfType(ePetEffectType.AE1084) as AE1084;
                if (effect3 != null)
                {
                    effect3.Pause();
                }

                AE1085 effect4 = player.PetEffectList.GetOfType(ePetEffectType.AE1085) as AE1085;
                if (effect4 != null)
                {
                    effect4.Pause();
                }

                player.Game.SendPlayerPicture(player, (int)BuffType.NoHole, false);
                player.IsNoHole = false;
                IsTrigger = false;
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBeginMoving -= Player_PlayerBeginMoving;
        }
    }
}