using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.LittleGame.Handle
{
    public interface ILittleGameCommandHandler
    {
        int CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}
