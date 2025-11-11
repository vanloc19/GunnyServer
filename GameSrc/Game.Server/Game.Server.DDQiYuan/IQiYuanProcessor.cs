using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DDTQiYuan
{
    public interface IQiYuanProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}