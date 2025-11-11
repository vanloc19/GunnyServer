using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AvatarCollection
{
    public abstract class AbstractAvatarCollectionProcessor : IAvatarCollectionProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}