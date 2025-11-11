using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler(293, "Отметки")]
    class OpenDailyView : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(293);
            client.Player.Out.SendTCP(pkg);
            return 0;
        }
    }
}
