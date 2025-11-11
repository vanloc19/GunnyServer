using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem
{
    public class ActiveSystemProcessor
    {
        private static object _syncStop = new object();
        private IActiveSystemProcessor _processor;

        public ActiveSystemProcessor(IActiveSystemProcessor processor)
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