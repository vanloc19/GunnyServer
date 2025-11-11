using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GodCardRaise.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.GodCardRaise
{
    [GodCardRaiseProcessorAtribute(40, "礼堂逻辑")]
    public class GodCardRaiseLogicProcessor : AbstractGodCardRaiseProcessor
    {
        public GodCardRaiseLogicProcessor()
        {
            _commandMgr = new GodCardRaiseHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private GodCardRaiseHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            GodCardRaisePackageType type = (GodCardRaisePackageType)packet.ReadByte();
            try
            {
                IGodCardRaiseCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("GodCardRaiseLogicProcessor GodCardRaisePackageType.{0} not found!", type);
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("GodCardRaiseLogicProcessor PackageType:{1}, OnGameData is Error: {0}",
                    e.ToString(), type));
                log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
            }
        }
    }
}