using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ExplorerManual.Handle
{
    public interface IExplorerManualCommandHadler
    {
        int CommandHandler(GamePlayer Player, GSPacketIn packet);
    }
}