using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DDTQiYuan.Handle
{
    public interface IQiYuanCommandHadler
    {
        int CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}