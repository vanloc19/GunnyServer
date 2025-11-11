using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GameRoom
{
    public abstract class AbstractGameRoomProcessor : IGameRoomProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}