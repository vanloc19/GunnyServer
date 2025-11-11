using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(33)]
    public class PosSync : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            return 0;
        }
    }
}
