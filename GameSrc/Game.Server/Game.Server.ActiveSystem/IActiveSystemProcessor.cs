using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem
{
    public interface IActiveSystemProcessor
    {
        void OnGameData(GamePlayer player, GSPacketIn packet);
    }
}