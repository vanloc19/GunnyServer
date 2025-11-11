using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.LittleGame.Handle;
using Game.Server.Packets;
using log4net;
using System;
using System.Reflection;

namespace Game.Server.LittleGame
{
    [LittleGameProcessor((byte)ePackageType.MARRY_ROOM_UPDATE, "温泉小游戏采用礼堂逻辑")]
    public class LittleGameLogicProcessor : AbstractLittleGameProcessor
    {
        private readonly static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private LittleGameHandleMgr littleGameHandleMgr = new LittleGameHandleMgr();

        public override void OnGameData(GamePlayer player, GSPacketIn packet)
        {
            eLittleGamePackageInType littleGamePackageType = (eLittleGamePackageInType)packet.ReadByte();
            try
            {
                ILittleGameCommandHandler littleGameCommandHadler = littleGameHandleMgr.LoadCommandHandler((int)littleGamePackageType);
                if (littleGameCommandHadler == null)
                {
                    Console.WriteLine("______________ERROR______________");
                    Console.WriteLine("LittleGameLogicProcessor LittleGamePackageIn.{0} not found!", littleGamePackageType);
                    Console.WriteLine("_______________END_______________");
                }
                else
                {
                    littleGameCommandHadler.CommandHandler(player, packet);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                log.Error(string.Format("LittleGameLogicProcessor PackageType:{1}, OnGameData is Error: {0}", exception.ToString(), littleGamePackageType));
                log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
            }
        }
    }
}
