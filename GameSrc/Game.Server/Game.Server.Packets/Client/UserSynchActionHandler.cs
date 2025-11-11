using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.SYNCH_ACTION, "用户同步动作")]
	public class UserSynchActionHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(packet.ClientID);
			if (playerById != null)
			{
				packet.Code = (short)ePackageType.AC_ACTION;
				packet.ClientID = client.Player.PlayerCharacter.ID;
				playerById.Out.SendTCP(packet);
			}
			return 1;
		}
	}
}
