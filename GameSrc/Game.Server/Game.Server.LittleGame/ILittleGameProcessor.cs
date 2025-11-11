using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.LittleGame
{
    public interface ILittleGameProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}
