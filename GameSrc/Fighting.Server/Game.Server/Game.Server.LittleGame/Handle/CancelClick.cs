using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.LittleGame.Objects;
using Game.Server.Packets;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(66)]
    public class CancelClick : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            var livingId = packet.ReadInt();

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
                Player.Out.SendMessage(eMessageType.ChatNormal, @"哎呀呀呀......它竟然溜走了.");
                return 0;
            }

            if (bogu == null)
            {
                if (Player.LittleGameInfo.Actions.Peek() == "livingInhale")
                {
                    Player.LittleGameInfo.Actions.Clear();
                }
                Player.LittleGameInfo.Actions.Enqueue("livingUnInhale");
                Player.Out.SendMessage(eMessageType.ChatNormal, @"哎呀呀呀......它竟然被你抓住了.");
                return 0;
            }

            lock (bogu.Locker)
            {
                bogu.Catchers.Remove(Player);
                Player.LittleGameInfo.Bogu = null;
                if (Player.LittleGameInfo.Actions.Peek() == "livingInhale")
                {
                    Player.LittleGameInfo.Actions.Clear();
                }
                Player.LittleGameInfo.Actions.Enqueue("livingUnInhale");
                if (bogu.Catchers.Count == 0)
                {
                    bogu.Сaught = false;
                    bogu.Action = "livingUnInhale";
                }
            }
            return 0;
        }
    }
}
