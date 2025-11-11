using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AvatarCollection
{
    public class AvatarCollectionProcessor
    {
        private static object _syncStop = new object();
        private IAvatarCollectionProcessor _processor;

        public AvatarCollectionProcessor(IAvatarCollectionProcessor processor)
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