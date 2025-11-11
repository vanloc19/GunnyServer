using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ExplorerManual
{
    public interface IExplorerManualProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}
