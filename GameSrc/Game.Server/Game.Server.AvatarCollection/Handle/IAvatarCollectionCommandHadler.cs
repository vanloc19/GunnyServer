using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AvatarCollection.Handle
{
    public interface IAvatarCollectionCommandHadler
    {
        bool CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}