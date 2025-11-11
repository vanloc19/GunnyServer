using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.CLEARBUFF)]
    public class ClearEffectSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.ClearBuff = true;
            }
            else
            {
                if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                    game.CurrentLiving.Team == player.Team)
                {
                    game.CurrentLiving.ClearBuff = true;
                }
            }
        }
    }
}