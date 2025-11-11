using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class NoHoleEquipEffect : BasePlayerEffect //Ãâ¿Ó
    {
        private int m_count = 0;
        private int m_probability = 0;

        public NoHoleEquipEffect(int count, int probability) : base(eEffectType.NoHoleEquipEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            NoHoleEquipEffect effect = living.EffectList.GetOfType(eEffectType.NoHoleEquipEffect) as NoHoleEquipEffect;
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
            player.CollidByObject += new PlayerEventHandle(player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.CollidByObject -= new PlayerEventHandle(player_AfterKilledByLiving);
        }

        void player_AfterKilledByLiving(Living living)
        {
            if (rand.Next(100) < m_probability)
            {
                living.EffectTrigger = true;
                new NoHoleEffect(1).Start(living);
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success"));
                living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("NoHoleEquipEffect.msg"),9, 0, 1000));
            }
        }
    }
}