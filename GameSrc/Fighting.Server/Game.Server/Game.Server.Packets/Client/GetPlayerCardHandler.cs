using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(18, "场景用户离开")]
	public class GetPlayerCardHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			PlayerInfo player;
			List<UsersCardInfo> userCard;
			if (playerById != null)
			{
				player = playerById.PlayerCharacter;
				userCard = playerById.CardBag.GetCards(0, 4);
			}
			else
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					player = playerBussiness.GetUserSingleByUserID(num);
					userCard = playerBussiness.GetUserCardEuqip(num);
				}
			}
			if (userCard != null && player != null)
			{
				client.Player.Out.SendUpdateCardData(player, userCard);
			}
			return 0;
		}
	}
}
