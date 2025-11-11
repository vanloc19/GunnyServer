using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AvatarCollection
{
    public interface IAvatarCollectionProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}