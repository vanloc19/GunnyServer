using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.USER_GET_GIFTS, "场景用户离开")]
	public class UserGetGiftHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			UserGiftInfo[] allGifts = null;
			PlayerInfo player = client.Player.PlayerCharacter;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				allGifts = playerBussiness.GetAllUserReceivedGifts(num);
				if (player.ID != num)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(num);
					player = ((playerById == null) ? playerBussiness.GetUserSingleByUserID(num) : playerById.PlayerCharacter);
				}
			}
			if (allGifts != null && player != null)
			{
				client.Out.SendGetUserGift(player, allGifts);
			}
			return 0;
		}
	}
}
