using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(4)]
    class LeaveWorld : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            LittleGameWorldMgr.RemovePlayer(Player);
            return 0;
        }
    }
}
