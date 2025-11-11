using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
	[GameCommandAttbute(186)]
	public class BuffObtain : IGameCommandHandler
	{
		public bool HandleCommand(TankGameLogicProcessor process, RingStationGamePlayer player, GSPacketIn packet)
		{
			return true;
		}
	}
}
