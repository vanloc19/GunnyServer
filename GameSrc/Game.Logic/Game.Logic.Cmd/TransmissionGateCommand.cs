using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.DELIVER, "Transmission Gate")]
    public class TransmissionGateCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game.GameState == eGameState.SessionPrepared || game.GameState == eGameState.GameOver)
            {
                bool isReady = packet.ReadBoolean();
                if (isReady)
                {
                    player.Ready = true;
                    game.CheckState(0);
                }
            }
        }
    }
}