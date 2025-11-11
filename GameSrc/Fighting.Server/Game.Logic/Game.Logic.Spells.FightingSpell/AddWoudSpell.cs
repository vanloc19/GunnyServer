using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ADDWOUND)]
    public class AddWoudSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.CurrentDamagePlus += (float)item.Property2 / 100;
            }
            else
            {
                if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                    game.CurrentLiving.Team == player.Team)
                {
                    game.CurrentLiving.CurrentDamagePlus += (float)item.Property2 / 100;
                }
            }
        }
    }
}