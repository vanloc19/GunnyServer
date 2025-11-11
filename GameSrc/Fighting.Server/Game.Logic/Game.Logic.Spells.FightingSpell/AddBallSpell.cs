using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ADDBALL)]
    public class AddBallSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (player.IsSpecialSkill)
                return;
            if (player.IsLiving)
            {
                if ((player.CurrentBall.ID == 3 || player.CurrentBall.ID == 5 || player.CurrentBall.ID == 1 || player.IsSpecialSkill) &&
                    item.TemplateID == 10003)
                {
                    player.BallCount = 1;
                }
                else
                {
                    player.CurrentDamagePlus *= 0.5f;
                    player.BallCount = item.Property2;
                }
            }
            else
            {
                if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                    game.CurrentLiving.Team == player.Team)
                {
                    if (((game.CurrentLiving as Player).CurrentBall.ID == 3 ||
                         (game.CurrentLiving as Player).CurrentBall.ID == 5 ||
                         (game.CurrentLiving as Player).CurrentBall.ID == 1) && item.TemplateID == 10003)
                    {
                        (game.CurrentLiving as Player).BallCount = 1;
                    }
                    else
                    {
                        game.CurrentLiving.CurrentDamagePlus *= 0.5f;
                        (game.CurrentLiving as Player).BallCount = item.Property2;
                    }
                }
            }
        }
    }
}