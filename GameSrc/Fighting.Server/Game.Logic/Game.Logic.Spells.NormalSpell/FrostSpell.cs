using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.FROST)]
    public class FrostSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsLiving)
            {
                player.SetBall(1);
                player.CheckUseFrost = true;
            }
            else
            {
                if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                    game.CurrentLiving.Team == player.Team)
                {
                    (game.CurrentLiving as Player).SetBall(1);
                    (game.CurrentLiving as Player).CheckUseFrost = true;
                }
            }
        }
    }
}