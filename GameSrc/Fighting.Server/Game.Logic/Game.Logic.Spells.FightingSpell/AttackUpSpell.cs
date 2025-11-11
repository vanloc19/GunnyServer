using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ATTACKUP)]
    public class AttackUpSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.AddDander(item.Property2);
            }
            else
            {
                if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                    game.CurrentLiving.Team == player.Team)
                {
                    game.CurrentLiving.AddDander(item.Property2);
                }
            }
        }
    }
}