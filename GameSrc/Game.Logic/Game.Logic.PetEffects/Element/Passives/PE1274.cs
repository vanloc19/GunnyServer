using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects.Element.Passives
{
    public class PE1274 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public PE1274(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.PE1274, elementID)
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
            PE1274 effect = living.PetEffectList.GetOfType(ePetEffectType.PE1274) as PE1274;
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
            player.PlayerBuffSkillPet += new PlayerEventHandle(this.StartAtombEffect);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(this.StartAtombEffect);
        }

        private void StartAtombEffect(Living liv)
        {
            if(this.rand.Next(100) < 20)
            {
                ((Player)liv).SetBall(4);
            }
        }
    }
}