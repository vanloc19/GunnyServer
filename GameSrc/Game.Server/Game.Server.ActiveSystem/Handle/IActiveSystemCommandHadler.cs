using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem.Handle
{
    public interface IActiveSystemCommandHadler
    {
        bool CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}