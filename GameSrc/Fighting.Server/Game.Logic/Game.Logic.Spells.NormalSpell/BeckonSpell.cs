using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.BECKON)]
    public class BeckonSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
        }
    }
}