using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms
{
	public interface GInterface2
	{
		void OnGameData(HotSpringRoom game, GamePlayer player, GSPacketIn packet);

		void OnTick(HotSpringRoom room);
	}
}
