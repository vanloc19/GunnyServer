using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DDTQiYuan.Handle
{
    [QiYuanHandleAttbute((byte)QiYuanPackageType.PACK_TYPE_MY_INFO)]
    public class MyInfo : IQiYuanCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            packet.ClientID = player.PlayerId;
            packet.WriteHeader();
            GameServer.Instance.LoginServer.SendPacket(packet);

            return 1;
        }
    }
}