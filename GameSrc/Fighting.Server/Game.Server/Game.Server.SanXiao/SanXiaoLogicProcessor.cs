using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.SanXiao.Handle;
using log4net;

namespace Game.Server.SanXiao
{
    [SanXiaoProcessorAtribute(40, "礼堂逻辑")]
    public class SanXiaoLogicProcessor : AbstractSanXiaoProcessor
    {
        public SanXiaoLogicProcessor()
        {
            _commandMgr = new SanXiaoHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SanXiaoHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            SanXiaoPackageType type = (SanXiaoPackageType)packet.ReadByte();
            try
            {
                ISanXiaoCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                Console.Write("SX_Pkg = {0}", type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("SanXiaoLogicProcessor SanXiaoPackageType.{0} not found!", type);
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("SanXiaoLogicProcessor PackageType:{1}, OnGameData is Error: {0}", e.ToString(),
                    type));
                log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
            }
        }
    }
}