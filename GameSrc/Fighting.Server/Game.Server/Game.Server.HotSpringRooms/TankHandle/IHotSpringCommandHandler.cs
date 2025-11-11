using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms.TankHandle
{
	public interface IHotSpringCommandHandler
	{
		bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet);
	}
}
