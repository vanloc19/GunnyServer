using System;
using System.Reflection;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.DDTQiYuan.Handle;
using Game.Server.GameObjects;
using log4net;

namespace Game.Server.DDTQiYuan
{
    [QiYuanProcessorAtribute(40, "礼堂逻辑")]
    public class QiYuanLogicProcessor : AbstractQiYuanProcessor
    {
        public QiYuanLogicProcessor()
        {
            _commandMgr = new QiYuanHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private QiYuanHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            QiYuanPackageType type = (QiYuanPackageType)packet.ReadByte();
            try
            {
                IQiYuanCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("QiYuanLogicProcessor QiYuanPackageType.{0} not found!", type);
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("QiYuanLogicProcessor PackageType:{1}, OnGameData is Error: {0}", e.ToString(),
                    type));
                log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
            }
        }
    }
}