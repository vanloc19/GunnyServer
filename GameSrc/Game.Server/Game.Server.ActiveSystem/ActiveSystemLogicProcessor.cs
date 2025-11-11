using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.ActiveSystem.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.ActiveSystem
{
    [ActiveSystemProcessorAtribute(255, "礼堂逻辑")]
    public class ActiveSystemLogicProcessor : AbstractActiveSystemProcessor
    {
        public ActiveSystemLogicProcessor()
        {
            _commandMgr = new ActiveSystemHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ActiveSystemHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            ActiveSystemPackageType type = (ActiveSystemPackageType)packet.ReadByte();
            try
            {
                IActiveSystemCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("ActiveSystemPackageType: {0} not found!", type);
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("ActiveSystemPackageType: {1}, OnGameData is Error: {0}", e, type));
            }
        }
    }
}