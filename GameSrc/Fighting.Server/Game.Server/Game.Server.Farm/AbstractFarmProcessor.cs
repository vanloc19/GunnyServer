using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Farm
{
    public abstract class AbstractFarmProcessor : IFarmProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}