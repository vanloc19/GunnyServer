using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Cmd
{
    [GameCommand(144, "使用道具")]
    public class PetKillCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game.GameState == eGameState.Playing && !player.GetSealState())
            {
                int skillID = packet.ReadInt();
                int type = packet.ReadInt();
                if (!player.YHM_UsePow)
                {
                    player.PetUseKill(skillID, type);
                    player.YHM_UseSkillPet = true;
                }
            }
        }
    }
}
