using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DDTQiYuan
{
    public class QiYuanProcessor
    {
        private static object _syncStop = new object();
        private IQiYuanProcessor _processor;

        public QiYuanProcessor(IQiYuanProcessor processor)
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