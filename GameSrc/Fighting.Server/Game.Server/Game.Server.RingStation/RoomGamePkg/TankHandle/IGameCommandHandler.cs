using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
	public interface IGameCommandHandler
	{
		bool HandleCommand(TankGameLogicProcessor process, RingStationGamePlayer player, GSPacketIn packet);
	}
}
