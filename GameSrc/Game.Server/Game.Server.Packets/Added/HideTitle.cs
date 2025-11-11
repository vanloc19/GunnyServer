using Game.Base.Packets;
using System;

namespace Game.Server.Packets.Client
{
    [PacketHandler(279, "场景用户离开")]
    public class ShowHideTitleStateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool isShowHide = packet.ReadBoolean();
            if (isShowHide)
            {
            }
            else
            {
            }
            return 0;
        }
    }
}