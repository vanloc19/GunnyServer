using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.LittleGame
{
    public abstract class AbstractLittleGameProcessor : ILittleGameProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}
