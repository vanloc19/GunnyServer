using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Farm.Handle
{
    public interface IFarmCommandHadler
    {
        bool CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}