using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet
{
	public interface IPetProcessor
	{
		void OnGameData(GamePlayer player, GSPacketIn packet);
	}
}
