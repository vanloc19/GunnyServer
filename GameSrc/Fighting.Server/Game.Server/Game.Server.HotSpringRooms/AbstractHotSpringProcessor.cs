using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms
{
	public abstract class AbstractHotSpringProcessor : GInterface2
	{
		public virtual void OnGameData(HotSpringRoom game, GamePlayer player, GSPacketIn packet)
		{
		}

		public virtual void OnTick(HotSpringRoom room)
		{
		}
	}
}
