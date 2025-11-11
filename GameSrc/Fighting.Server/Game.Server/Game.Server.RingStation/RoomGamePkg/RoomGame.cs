using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg
{
	public class RoomGame
	{
		private IGameProcessor _processor = new TankGameLogicProcessor();

		private static object _syncStop = new object();

		protected void OnTick(object obj)
		{
			_processor.OnTick(this);
		}

		public void ProcessData(RingStationGamePlayer player, GSPacketIn data)
		{
			lock (_syncStop)
			{
				_processor.OnGameData(this, player, data);
			}
		}
	}
}
