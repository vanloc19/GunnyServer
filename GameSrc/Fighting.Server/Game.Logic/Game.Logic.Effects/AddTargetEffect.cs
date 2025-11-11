using Bussiness;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class AddTargetEffect : BasePlayerEffect
    {
        public AddTargetEffect()
            : base(eEffectType.AddTargetEffect)
        {
            /// indexer = 0;
        }

        public override bool Start(Living living)
        {
            AddTargetEffect effect = living.EffectList.GetOfType(eEffectType.AddTargetEffect) as AddTargetEffect;
            if (effect != null)
            {
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.Game.SendPlayerPicture(player, (int)BuffType.Targeting, true);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.Game.SendPlayerPicture(player, (int)BuffType.Targeting, false);
        }
    }
}