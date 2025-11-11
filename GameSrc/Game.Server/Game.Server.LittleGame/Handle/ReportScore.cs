using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.LittleGame.Objects;
using Game.Server.Packets;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(64)]
    class ReportScore : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int score = packet.ReadInt();
            int livingId = packet.ReadInt();

            Bogu bogu;
            if (LittleGameWorldMgr.ScenariObjects.ContainsKey(livingId))
            {
                bogu = LittleGameWorldMgr.ScenariObjects[livingId] as Bogu;
            }
            else
            {
                if (Player.LittleGameInfo.Actions.Peek() == "livingInhale")
                {
                    Player.LittleGameInfo.Actions.Clear();
                }
                Player.LittleGameInfo.Actions.Enqueue("livingUnInhale");
                Player.Out.SendMessage(eMessageType.ChatERROR, @"哎呀呀呀......它竟然溜走了.");
                return 0;
            }

            if (bogu == null)
            {
                if (Player.LittleGameInfo.Actions.Peek() == "livingInhale")
                {
                    Player.LittleGameInfo.Actions.Clear();
                }
                Player.LittleGameInfo.Actions.Enqueue("livingUnInhale");
                Player.Out.SendMessage(eMessageType.ChatERROR, @"哎呀呀呀......它竟然被你抓住了.");
                return 0;
            }

            lock (bogu.Locker)
            {
                bogu.Catchers.Remove(Player);
                if (bogu.Catchers.Count == 0)
                {
                    bogu.Action = "livingDie";
                    LittleGameWorldMgr.RemoveBogu(bogu);
                }
                Player.LittleGameInfo.Bogu = null;
                Player.AddScore(score);
                if (Player.LittleGameInfo.Actions.Peek() == "livingInhale")
                {
                    Player.LittleGameInfo.Actions.Clear();
                }
                Player.LittleGameInfo.Actions.Enqueue("livingUnInhale");
            }
            return 0;
        }
    }
}
