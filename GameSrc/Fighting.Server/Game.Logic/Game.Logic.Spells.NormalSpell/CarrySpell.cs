using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.CARRY)]
    public class CarrySpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.PlayerDetail.PlayerCharacter.IsAutoBot)
                return;

            if (player.IsLiving)
            {
                player.SetBall(3);
            }
            else
            {
                if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                    game.CurrentLiving.Team == player.Team)
                {
                    (game.CurrentLiving as Player).SetBall(3);
                }
            }
        }
    }
}