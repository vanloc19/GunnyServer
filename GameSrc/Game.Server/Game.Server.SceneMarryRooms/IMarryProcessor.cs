using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SceneMarryRooms
{
	public interface IMarryProcessor
	{
		void OnGameData(MarryRoom game, GamePlayer player, GSPacketIn packet);

		void OnTick(MarryRoom room);
	}
}
