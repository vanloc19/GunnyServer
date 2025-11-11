using System;
using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler(321, "客户端日记")]
    public class GetPlayerSpecialPropertyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadInt();
            GSPacketIn pkg = new GSPacketIn(321);
            pkg.WriteInt(type);
            switch (type)
            {
                case 2:
                    pkg.WriteInt(3);
                    pkg.WriteInt(0); //coupleNum 20
                    pkg.WriteInt(client.Player.Extra.Info.coupleNum);
                    pkg.WriteInt(1); //dungeonNum 4
                    pkg.WriteInt(client.Player.Extra.Info.dungeonNum);
                    pkg.WriteInt(2); //propsNum 5
                    pkg.WriteInt(client.Player.Extra.Info.propsNum);
                    break;
                default:
                    Console.WriteLine("GETPLAYERSPECIAL: {0}", type);
                    break;
            }

            client.Out.SendTCP(pkg);
            return 0;
        }
    }
}