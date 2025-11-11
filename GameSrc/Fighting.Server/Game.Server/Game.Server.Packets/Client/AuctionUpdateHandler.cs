using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(193, "更新拍卖")]
	public class AuctionUpdateHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int auctionID = packet.ReadInt();
			int num2 = packet.ReadInt();
			bool val = false;
			int num3 = GameProperties.LimitLevel(0);
			if (client.Player.PlayerCharacter.Grade < num3)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Você necessita ser nível {1} para continuar.", num3));
				return 0;
			}
			GSPacketIn @in = new GSPacketIn(193, client.Player.PlayerCharacter.ID);
			string translateId = "AuctionUpdateHandler.Fail";
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				AuctionInfo auctionSingle = bussiness.GetAuctionSingle(auctionID);
				if (auctionSingle == null)
				{
					translateId = "AuctionUpdateHandler.Msg1";
				}
				else if (auctionSingle.PayType == 0 && num2 > client.Player.PlayerCharacter.Gold)
				{
					translateId = "AuctionUpdateHandler.Msg2";
				}
				else if (auctionSingle.PayType == 1 && !client.Player.MoneyDirect(num2, IsAntiMult: true))
				{
					translateId = "";
				}
				else if (auctionSingle.BuyerID == 0 && auctionSingle.Price > num2)
				{
					translateId = "AuctionUpdateHandler.Msg4";
				}
				else if (auctionSingle.BuyerID != 0 && auctionSingle.Price + auctionSingle.Rise > num2 && (auctionSingle.Mouthful == 0 || auctionSingle.Mouthful > num2))
				{
					translateId = "AuctionUpdateHandler.Msg5";
				}
				else
				{
					int buyerID = auctionSingle.BuyerID;
					auctionSingle.BuyerID = client.Player.PlayerCharacter.ID;
					auctionSingle.BuyerName = client.Player.PlayerCharacter.NickName;
					auctionSingle.Price = num2;
					if (auctionSingle.Mouthful != 0 && num2 >= auctionSingle.Mouthful)
					{
						auctionSingle.Price = auctionSingle.Mouthful;
						auctionSingle.IsExist = false;
					}
					if (bussiness.UpdateAuction(auctionSingle, GameProperties.Cess))
					{
						if (auctionSingle.PayType == 0)
						{
							client.Player.RemoveGold(auctionSingle.Price);
						}
						if (auctionSingle.IsExist)
						{
							translateId = "AuctionUpdateHandler.Msg6";
						}
						else
						{
							translateId = "AuctionUpdateHandler.Msg7";
							client.Out.SendMailResponse(auctionSingle.AuctioneerID, eMailRespose.Receiver);
							client.Out.SendMailResponse(auctionSingle.BuyerID, eMailRespose.Receiver);
						}
						if (buyerID != 0)
						{
							client.Out.SendMailResponse(buyerID, eMailRespose.Receiver);
						}
						val = true;
					}
				}
				client.Out.SendAuctionRefresh(auctionSingle, auctionID, auctionSingle?.IsExist ?? false, null);
				if (translateId != "")
				{
					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(translateId));
				}
			}
			@in.WriteBoolean(val);
			@in.WriteInt(auctionID);
			client.Out.SendTCP(@in);
			return 0;
		}
	}
}
