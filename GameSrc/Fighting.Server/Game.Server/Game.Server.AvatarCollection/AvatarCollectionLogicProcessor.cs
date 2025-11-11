using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.AvatarCollection.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.AvatarCollection
{
    [AvatarCollectionProcessorAtribute(40, "礼堂逻辑")]
    public class AvatarCollectionLogicProcessor : AbstractAvatarCollectionProcessor
    {
        public AvatarCollectionLogicProcessor()
        {
            _commandMgr = new AvatarCollectionHandleMgr();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private AvatarCollectionHandleMgr _commandMgr;

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            AvatarCollectionPackageType type = (AvatarCollectionPackageType)packet.ReadByte();
            Console.WriteLine(type);
            try
            {
                IAvatarCollectionCommandHadler commandHandler = _commandMgr.LoadCommandHandler((int)type);
                if (commandHandler != null)
                {
                    commandHandler.CommandHandler(player, packet);
                }
                else
                {
                    //log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("LoadCommandHandler not found!");
                    Console.WriteLine("_______________END_______________");
                }
            }
            catch (Exception e)
            {
                //log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(), player.Client.TcpEndpoint));
                //log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
                Console.WriteLine("______________ERROR______________");
                Console.WriteLine("AvatarCollectionLogicProcessor PackageType {0} not found!", type);
                log.Error(string.Format("Detail: {0}", e));
                Console.WriteLine("_______________END_______________");
            }
        }
    }
}