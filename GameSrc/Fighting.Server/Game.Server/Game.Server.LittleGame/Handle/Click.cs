using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.LittleGame.Objects;
using Game.Server.Packets;

namespace Game.Server.LittleGame.Handle
{
    [LittleGame(65)]
    class Click : ILittleGameCommandHandler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int livingId = packet.ReadInt();
            int livingX = packet.ReadInt();
            int livingY = packet.ReadInt();
            int playerX = packet.ReadInt();
            int playerY = packet.ReadInt();
            Bogu bogu;
            if (LittleGameWorldMgr.ScenariObjects.ContainsKey(livingId))
            {
                bogu = LittleGameWorldMgr.ScenariObjects[livingId] as Bogu;
            }
            else
            {
                Player.Out.SendMessage(eMessageType.ChatERROR, @"哎呀呀呀......它竟然溜走了.");
                return 0;
            }
            if (bogu == null)
            {
                Player.Out.SendMessage(eMessageType.ChatERROR, @"哎呀呀呀......它竟然被你抓住了.");
                return 0;
            }

            lock (bogu.Locker)
            {
                if (bogu.Сaught && bogu.Catchers.Count >= bogu.MaxCatchers)
                {
                    return 0;
                }

                if (livingX > playerX && livingY >= playerY)
                {
                    Player.LittleGameInfo.Direction = "rightDown";
                    Player.LittleGameInfo.IsBack = false;
                    bogu.Direction = "rightDown";
                    bogu.IsBack = false;
                }
                else if (livingX > playerX && livingY < playerY)
                {
                    Player.LittleGameInfo.Direction = "rightUp";
                    Player.LittleGameInfo.IsBack = true;
                    bogu.Direction = "rightUp";
                    bogu.IsBack = true;
                }
                else if (livingX < playerX && livingY >= playerY)
                {
                    Player.LittleGameInfo.Direction = "leftDown";
                    Player.LittleGameInfo.IsBack = false;
                    bogu.Direction = "leftDown";
                    bogu.IsBack = false;
                }
                else if (livingX < playerX && livingY < playerY)
                {
                    Player.LittleGameInfo.Direction = "leftUp";
                    Player.LittleGameInfo.IsBack = true;
                    bogu.Direction = "leftUp";
                    bogu.IsBack = true;
                }

                bogu.X = livingX;
                bogu.Y = livingY;
                bogu.Сaught = true;
                bogu.Catchers.Add(Player);
                Player.LittleGameInfo.Bogu = bogu;
                Player.LittleGameInfo.Actions.Enqueue("livingInhale");
                if (bogu.Size < 2)
                {
                    LittleGameWorldMgr.Out.SendAddObject(Player, bogu, "normalbogu");
                }
                else
                {
                    if (bogu.Catchers.Count < bogu.MaxCatchers)
                    {
                        LittleGameWorldMgr.Out.SendAddObject(Player, bogu, "bogugiveup");
                    }
                    else
                    {
                        foreach (GamePlayer player in bogu.Catchers)
                        {
                            LittleGameWorldMgr.Out.SendAddObject(player, bogu, "bigbogu");
                        }
                    }
                }
            }
            return 0;
        }
    }
}
