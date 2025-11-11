using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	public interface IMarryCommandHandler
	{
		bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet);
	}
}
