using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameRoom.Handle;
using Game.Server.Packets;
using log4net;
using System;
using System.Reflection;

namespace Game.Server.GameRoom
{
    [GameRoomProcessorAtribute(40, "礼堂逻辑")]
    public class GameRoomLogicProcessor : AbstractGameRoomProcessor
    {
        public GameRoomLogicProcessor()
        {
            _commandMgr = new GameRoomHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private GameRoomHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            eRoomPackageType type = (eRoomPackageType)packet.ReadInt(); //packet.ReadByte();
            try
            {
                IGameRoomCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("GameRoomLogicProcessor PackageType {0} not found!", type);
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(), player.Client.TcpEndpoint));
            }
        }
    }
}