using System;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HotSpringRooms.TankHandle;
using log4net;

namespace Game.Server.HotSpringRooms
{
	[HotSpringProcessor(9, "礼堂逻辑")]
	public class TankHotSpringLogicProcessor : AbstractHotSpringProcessor
	{
		private HotSpringCommandMgr hotSpringCommandMgr_0 = new HotSpringCommandMgr();

		private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public readonly int TIMEOUT = 60000;

		private ThreadSafeRandom threadSafeRandom_0 = new ThreadSafeRandom();

		public override void OnGameData(HotSpringRoom room, GamePlayer player, GSPacketIn packet)
		{
			HotSpringCmdType type = (HotSpringCmdType)packet.ReadByte();
			try
			{
				IHotSpringCommandHandler handler = hotSpringCommandMgr_0.LoadCommandHandler((int)type);
				if (handler != null)
				{
					handler.HandleCommand(this, player, packet);
				}
				else
				{
					ilog_0.Error($"IP: {player.Client.TcpEndpoint}");
				}
			}
			catch (Exception exception)
			{
				ilog_0.Error(string.Format("IP:{1}, OnGameData is Error: {0}", exception.ToString(), player.Client.TcpEndpoint));
			}
		}

		public override void OnTick(HotSpringRoom room)
		{
		}
	}
}
