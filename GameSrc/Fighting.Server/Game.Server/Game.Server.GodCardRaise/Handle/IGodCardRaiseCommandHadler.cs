using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GodCardRaise.Handle
{
    public interface IGodCardRaiseCommandHadler
    {
        int CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}