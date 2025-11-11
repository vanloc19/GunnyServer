using Game.Base.Packets;
using Game.Logic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_CMD, "游戏数据")]
    public class GameDataHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null)
            {
                if (UseBackupID(client.Player.CurrentRoom.RoomType))
                {
                    packet.Parameter1 = client.Player.TempGameId;
                }
                else
                {
                    packet.Parameter1 = client.Player.GamePlayerId;
                }

                if (client.Player.CurrentRoom.Game == null)
                {
                    GSPacketIn clonePkg = packet.Clone();
                    clonePkg.ClearOffset();

                    if (clonePkg.ReadByte() == (byte)eTankCmdType.TAKE_CARD)
                    {
                        int index = clonePkg.ReadByte();
                        if (index > 9)
                        {
                            index = 8;
                        }

                        GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, clonePkg.Parameter1);
                        pkg.Parameter1 = clonePkg.Parameter1;
                        pkg.WriteByte((byte)eTankCmdType.TAKE_CARD);
                        pkg.WriteBoolean(true);
                        pkg.WriteByte((byte)index);
                        pkg.WriteInt(0);
                        pkg.WriteInt(0);
                        pkg.WriteBoolean(false);
                        client.Player.SendTCP(pkg);
                    }

                    return 0;
                }

                client.Player.CurrentRoom.ProcessData(packet);
            }

            return 0;
        }

        private bool UseBackupID(Logic.eRoomType roomType)
        {
            switch (roomType)
            {
                case Logic.eRoomType.Match:
                    return true;
            }

            return false;
        }
    }
}