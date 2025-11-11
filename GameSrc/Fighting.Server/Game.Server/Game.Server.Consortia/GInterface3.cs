using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia
{
	public interface GInterface3
	{
		void OnGameData(GamePlayer player, GSPacketIn packet);
	}
}
