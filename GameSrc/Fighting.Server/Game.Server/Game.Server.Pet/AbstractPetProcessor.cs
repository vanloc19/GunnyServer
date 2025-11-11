using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet
{
	public abstract class AbstractPetProcessor : IPetProcessor
	{
		public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
		{
		}
	}
}
