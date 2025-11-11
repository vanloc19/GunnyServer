using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg
{
	public interface IGameProcessor
	{
		void OnGameData(RoomGame game, RingStationGamePlayer player, GSPacketIn packet);

		void OnTick(RoomGame room);
	}
}
