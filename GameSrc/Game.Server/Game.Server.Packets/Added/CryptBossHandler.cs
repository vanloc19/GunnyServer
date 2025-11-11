using Bussiness;
using Game.Base.Packets;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(275, "场景用户离开")]
    public class CryptBossHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte cmd = packet.ReadByte();
            if(cmd == 3)
            {
                string msg = string.Empty;
                int id = packet.ReadInt();
                int level = packet.ReadInt();
                CryptBossItemInfo info = client.Player.Actives.GetCryptBossData(id);
                if(info == null)
                {
                    client.Player.SendMessage("Dữ liệu lỗi, thao tác thất bại.");
                }
                else
                {
                    if (info.state == 1)
                    {
                        if(info.star <= level && level <= 5)
                        {
                            RoomMgr.CreateCryptBossRoom(client.Player, id, level);
                        }
                        else
                        {
                            msg = "Cấp sao chưa đủ.";
                        }
                    }
                    else
                    {
                        msg = "Số lần khiêu chiến hôm nay đã hết.";
                    }
                }
                if (string.IsNullOrEmpty(msg))
                {
                    client.Player.SendMessage(msg);
                }
            }
            return 0;
        }
    }
}