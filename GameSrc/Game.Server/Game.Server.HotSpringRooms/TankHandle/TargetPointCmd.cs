using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms.TankHandle
{
	[HotSpringCommandAttbute(1)]
	public class TargetPointCmd : IHotSpringCommandHandler
	{
		public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentHotSpringRoom != null)
			{
				string str = packet.ReadString();
				int playerId = packet.ReadInt();
				int val = packet.ReadInt();
				int num3 = packet.ReadInt();
				packet.ReadInt();
				int num4 = packet.ReadInt();
				GamePlayer playerWithID = player.CurrentHotSpringRoom.GetPlayerWithID(playerId);
				if (playerWithID != null)
				{
					playerWithID.Hot_X = val;
					playerWithID.Hot_Y = num3;
					playerWithID.Hot_Direction = num4;
					GSPacketIn @in = new GSPacketIn(191);
					@in.WriteByte(1);
					@in.WriteString(str);
					@in.WriteInt(playerId);
					@in.WriteInt(val);
					@in.WriteInt(num3);
					player.CurrentHotSpringRoom.SendToPlayerExceptSelf(@in, player);
				}
			}
			return false;
		}
	}
}
