using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(4)]
	public class InviteCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom != null && player.CurrentMarryRoom.RoomState == eRoomState.FREE)
			{
				if (!player.CurrentMarryRoom.Info.GuestInvite && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
				{
					return false;
				}
				GSPacketIn @in = packet.Clone();
				@in.ClearContext();
				GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
				if (playerById != null && playerById.CurrentRoom == null && playerById.CurrentMarryRoom == null)
				{
					@in.WriteByte(4);
					@in.WriteInt(player.PlayerCharacter.ID);
					@in.WriteString(player.PlayerCharacter.NickName);
					@in.WriteInt(player.CurrentMarryRoom.Info.ID);
					@in.WriteString(player.CurrentMarryRoom.Info.Name);
					@in.WriteString(player.CurrentMarryRoom.Info.Pwd);
					@in.WriteInt(player.MarryMap);
					playerById.Out.SendTCP(@in);
					return true;
				}
			}
			return false;
		}
	}
}
