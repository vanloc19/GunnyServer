using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GodCardRaise
{
    public interface IGodCardRaiseProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}