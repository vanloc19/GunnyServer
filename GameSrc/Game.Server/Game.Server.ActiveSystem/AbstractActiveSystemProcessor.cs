using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem
{
    public abstract class AbstractActiveSystemProcessor : IActiveSystemProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}