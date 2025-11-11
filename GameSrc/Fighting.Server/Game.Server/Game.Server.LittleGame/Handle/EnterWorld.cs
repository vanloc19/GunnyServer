using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(2)]
    public class EnterWorld : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            LittleGameWorldMgr.AddPlayer(Player);
            return 0;
        }
    }
}
