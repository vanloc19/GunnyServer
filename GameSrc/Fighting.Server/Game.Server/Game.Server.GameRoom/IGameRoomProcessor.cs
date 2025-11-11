using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GameRoom
{
    public interface IGameRoomProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}