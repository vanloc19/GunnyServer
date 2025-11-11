using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SanXiao
{
    public abstract class AbstractSanXiaoProcessor : ISanXiaoProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}