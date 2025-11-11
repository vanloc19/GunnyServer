using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GodCardRaise
{
    public class GodCardRaiseProcessor
    {
        private static object _syncStop = new object();
        private IGodCardRaiseProcessor _processor;

        public GodCardRaiseProcessor(IGodCardRaiseProcessor processor)
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