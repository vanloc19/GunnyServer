using Bussiness.Protocol;
using Game.Base.Packets;
using System;

namespace Game.Server.Packets.Client
{
    [PacketHandler(329, "SanxiaoHandler")]
    public class SanXiaoHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn pkg)
        {
            GSPacketIn tempPacket = pkg.Clone();
            tempPacket.ClearOffset();
            byte cmd = tempPacket.ReadByte();
            if (Enum.IsDefined(typeof(SanXiaoPackageType), (int)cmd))
            {
                if (client.Player.SanXiao == null)
                    return 0;

                client.Player.SanXiao.ProcessData(client.Player, pkg);
                return 1;
            }
            else if (Enum.IsDefined(typeof(QiYuanPackageType), (int)cmd))
            {
                if (client.Player.DDTQiYuan == null)
                    return 0;

                client.Player.DDTQiYuan.ProcessData(client.Player, pkg);
                return 1;
            }
            else if (Enum.IsDefined(typeof(GodCardRaisePackageType), (int)cmd))
            {
                if (client.Player.GodCardRaise == null)
                    return 0;

                client.Player.GodCardRaise.ProcessData(client.Player, pkg);
                return 1;
            }
            Console.WriteLine("SanXiaoHandler Not Found For PKG = {0}", pkg);
            return 0;
        }
    }
}