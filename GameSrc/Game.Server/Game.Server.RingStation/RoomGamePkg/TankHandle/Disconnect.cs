using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
	[GameCommandAttbute(83)]
	public class Disconnect : IGameCommandHandler
	{
		public bool HandleCommand(TankGameLogicProcessor process, RingStationGamePlayer player, GSPacketIn packet)
		{
			return true;
		}
	}
}
