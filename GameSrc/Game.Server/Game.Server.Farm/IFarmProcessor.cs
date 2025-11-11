using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Farm
{
    public interface IFarmProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}