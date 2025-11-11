using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((short)ePackageType.AVATAR_COLLECTION, "场景用户离开")]
    public class AvatarCollectionHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.AvatarCollection != null)
                client.Player.AvatarCollection.ProcessData(client.Player, packet);
            return 0;
        }
    }
}