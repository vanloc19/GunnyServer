using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.VANE)]
    public class VaneSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            game.UpdateWind(-game.Wind, true);
        }
    }
}