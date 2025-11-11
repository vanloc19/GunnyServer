using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ExplorerManual
{
    public class ExplorerManualProcessor
    {
        private static object _syncStop = new object();
        private IExplorerManualProcessor _processor;

        public ExplorerManualProcessor(IExplorerManualProcessor processor)
        {
            _processor = processor;
        }

        public void ProcessData(GamePlayer player, GSPacketIn data)
        {
            lock (_syncStop)
            {
                _processor.OnGameData(player, data);
            }
        }
    }
}