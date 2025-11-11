using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(32)]
    public class Move : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            int gX = packet.ReadInt();
            int gY = packet.ReadInt();
            int time = packet.ReadInt();
            Player.LittleGameInfo.X = gX;
            Player.LittleGameInfo.Y = gY;
            LittleGameWorldMgr.Out.SendMoveToAll(Player);
            return 0;
        }
    }
}
