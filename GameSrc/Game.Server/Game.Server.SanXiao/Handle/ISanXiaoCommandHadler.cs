using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SanXiao.Handle
{
    public interface ISanXiaoCommandHadler
    {
        int CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}