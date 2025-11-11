using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(8)]
	public class ForbidCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom != null && (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.BrideID))
			{
				int userID = packet.ReadInt();
				if (userID != player.CurrentMarryRoom.Info.BrideID && userID != player.CurrentMarryRoom.Info.GroomID)
				{
					player.CurrentMarryRoom.KickPlayerByUserID(player, userID);
					player.CurrentMarryRoom.SetUserForbid(userID);
				}
				return true;
			}
			return false;
		}
	}
}
