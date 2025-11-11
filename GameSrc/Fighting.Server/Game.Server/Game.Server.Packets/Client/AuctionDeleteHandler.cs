using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(194, "撤消拍卖")]
	public class AuctionDeleteHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int auctionID = packet.ReadInt();
			string translation = LanguageMgr.GetTranslation("AuctionDeleteHandler.Fail");
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				if (bussiness.DeleteAuction(auctionID, client.Player.PlayerCharacter.ID, ref translation))
				{
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
					client.Out.SendAuctionRefresh(null, auctionID, isExist: false, null);
				}
				else
				{
					AuctionInfo auctionSingle = bussiness.GetAuctionSingle(auctionID);
					client.Out.SendAuctionRefresh(auctionSingle, auctionID, auctionSingle != null, null);
				}
				client.Out.SendMessage(eMessageType.GM_NOTICE, translation);
			}
			return 0;
		}
	}
}
