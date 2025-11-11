using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DDTQiYuan
{
    public abstract class AbstractQiYuanProcessor : IQiYuanProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}