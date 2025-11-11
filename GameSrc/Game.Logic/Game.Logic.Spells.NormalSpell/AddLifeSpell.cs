using System.Collections.Generic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.ADD_LIFE)]
    public class AddLifeSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            switch (item.Property2)
            {
                case 0:
                    int bloodadd = item.Property3;
                    if (player.IsLiving)
                    {
                        if (player.FightBuffers.ConsortionAddSpellCount > 0)
                        {
                            bloodadd += player.FightBuffers.ConsortionAddSpellCount;
                        }

                        player.AddBlood(bloodadd);
                    }
                    else
                    {
                        if (game.CurrentLiving != null && game.CurrentLiving is Player &&
                            game.CurrentLiving.Team == player.Team)
                        {
                            game.CurrentLiving.AddBlood(bloodadd);
                        }
                    }

                    break;
                case 1:
                    List<Player> temps = player.Game.GetAllFightPlayers();
                    foreach (Player p in temps)
                    {
                        if (p.IsLiving && p.Team == player.Team)
                        {
                            p.AddBlood(item.Property3);
                        }
                    }

                    break;
                default:
                    break;
            }
        }
    }
}