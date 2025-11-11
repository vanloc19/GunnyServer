using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ExplorerManual
{
    public abstract class AbstractExplorerManualProcessor : IExplorerManualProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}