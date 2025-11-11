using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.ExplorerManual.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.ExplorerManual
{
    [ExplorerManualProcessorAtribute(40, "礼堂逻辑")]
    public class ExplorerManualLogicProcessor : AbstractExplorerManualProcessor
    {
        public ExplorerManualLogicProcessor()
        {
            _commandMgr = new ExplorerManualHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ExplorerManualHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            ExplorerManualPackageType type = (ExplorerManualPackageType)packet.ReadByte();
            try
            {
                IExplorerManualCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("ExplorerManualLogicProcessor ExplorerManualPackageType.{0} not found!", type);
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("ExplorerManualLogicProcessor PackageType:{1}, OnGameData is Error: {0}",
                    e.ToString(), type));
                log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
            }
        }
    }
}