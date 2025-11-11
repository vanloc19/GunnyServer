using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
    public class AddTurnEquipEffect : BasePlayerEffect //立即行动
    {
        private int m_count = 0;
        private int m_probability = 0;
        private int m_templateID = 0;

        public AddTurnEquipEffect(int count, int probability, int templateID) : base(eEffectType.AddTurnEquipEffect)
        {
            m_count = count;
            m_probability = probability;
            m_templateID = templateID;
        }

        public override bool Start(Living living)
        {
            AddTurnEquipEffect effect =
                living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) as AddTurnEquipEffect;
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
            player.PlayerShoot += new PlayerEventHandle(ChangeProperty);
            player.BeginNextTurn += new LivingEventHandle(player_BeginNextTurn);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(ChangeProperty);
            player.BeginNextTurn -= new LivingEventHandle(player_BeginNextTurn);
        }

        public void player_BeginNextTurn(Living living)
        {
            if (IsTrigger && (living is Player))
            {
                int energy = 0;
                switch (m_templateID)
                {
                    case 311112:
                        energy = 100;
                        break;
                    case 311129:
                        energy = 100;
                        break;
                    case 311212:
                        energy = 100;
                        break;
                    case 311229:
                        energy = 160;
                        break;
                    case 311312:
                        energy = 210;
                        break;
                    case 311329:
                        energy = 250;
                        break;
                    case 311412:
                        energy = 260;
                        break;
                    case 311429:
                        energy = 265;
                        break;
                    case 311512:
                        energy = 265;
                        break;
                    case 311529:
                        energy = 265;
                        break;
                }
                (living as Player).Delay = 0;
                (living as Player).Energy = energy;
                (living as Player).PlayerDetail.SendMessage(string.Format("Thể lực thần báo = {0} & ID = {1}", energy, m_templateID));
                IsTrigger = false;
            }
        }

        private void ChangeProperty(Player player)
        {
            if (player.CurrentBall.IsSpecial())
                return;
            if (rand.Next(100) < m_probability && player.AttackGemLimit == 0)
            {
                player.AttackGemLimit = 4;
                player.Delay = player.DefaultDelay;
                IsTrigger = true;
                player.EffectTrigger = true;
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
                player.Game.AddAction(new LivingSayAction((Living)player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg"), 9, 0, 1000));
            }
        }
    }
}