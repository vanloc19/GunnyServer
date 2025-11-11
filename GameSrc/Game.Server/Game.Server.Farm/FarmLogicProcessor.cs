using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.Farm.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Farm
{
    [FarmProcessorAtribute(99, "礼堂逻辑")]
    public class FarmLogicProcessor : AbstractFarmProcessor
    {
        public FarmLogicProcessor()
        {
            _commandMgr = new FarmHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FarmHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            FarmPackageType type = (FarmPackageType)packet.ReadByte();
            //log.WarnFormat("type = {0}", type);
            try
            {
                IFarmCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("LoadCommandHandler not found!");
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