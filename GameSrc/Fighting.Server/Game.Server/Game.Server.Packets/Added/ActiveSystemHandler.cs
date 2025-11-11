using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler(145, "场景用户离开")]
    public class ActiveSystemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.ActiveSystemHandler != null)
            {
                client.Player.ActiveSystemHandler.ProcessData(client.Player, packet);
                return 0;
            }

            return 1;
        }
    }
}