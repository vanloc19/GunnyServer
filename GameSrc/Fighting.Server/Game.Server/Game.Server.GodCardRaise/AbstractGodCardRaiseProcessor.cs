using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GodCardRaise
{
    public abstract class AbstractGodCardRaiseProcessor : IGodCardRaiseProcessor
    {
        public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
        {
        }
    }
}