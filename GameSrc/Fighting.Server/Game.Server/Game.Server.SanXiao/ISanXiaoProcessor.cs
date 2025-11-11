using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SanXiao
{
    public interface ISanXiaoProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}