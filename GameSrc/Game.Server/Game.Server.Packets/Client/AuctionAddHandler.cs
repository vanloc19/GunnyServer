using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(192, "添加拍卖")]
	public class AuctionAddHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eBageType eBageType = (eBageType)packet.ReadByte();
			int place = packet.ReadInt();
			packet.ReadByte();
			int num13 = packet.ReadInt();
			int num14 = packet.ReadInt();
			int num15 = packet.ReadInt();
			int count = packet.ReadInt();
			string translateId = "AuctionAddHandler.Fail";
			int num16 = 1;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			if (num13 < 0 || (num14 != 0 && num14 < num13))
			{
				return 0;
			}
			int num17 = 1;
			if (num16 != 0)
			{
				num17 = 1;
				num16 = 1;
			}
			double num18 = (double)(num17 * num13) * 0.03;
			int num19;
			switch (num15)
			{
			case 0:
				num19 = 1;
				break;
			case 1:
				num19 = 3;
				break;
			default:
				num19 = 6;
				break;
			}
			double num20 = num19;
			int num10 = (int)(num18 * num20);
			int num11 = ((num10 < 1) ? 1 : num10);
			ItemInfo itemAt = client.Player.GetItemAt(eBageType, place);
			if (num13 < 0)
			{
				translateId = "AuctionAddHandler.Msg1";
			}
			else if (num14 != 0 && num14 < num13)
			{
				translateId = "AuctionAddHandler.Msg2";
			}
			else if (num11 > client.Player.PlayerCharacter.Gold)
			{
				translateId = "AuctionAddHandler.Msg3";
			}
			else if (itemAt == null)
			{
				translateId = "AuctionAddHandler.Msg4";
			}
			else if (itemAt.IsBinds)
			{
				translateId = "AuctionAddHandler.Msg5";
			}
			else if (itemAt.Template.BagType == eBageType.BankBag)
            {
				translateId = "Thao tác thất bại vui lòng thử lại sau.";
            }
			else if (itemAt.Count >= count && count > 0)
			{
				ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
				ItemInfo itemInfo = ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
				itemInfo.Count = count;
				if (itemInfo.ItemID == 0)
				{
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						playerBussiness2.AddGoods(itemInfo);
					}
				}
				AuctionInfo info = new AuctionInfo();
				info.AuctioneerID = client.Player.PlayerCharacter.ID;
				info.AuctioneerName = client.Player.PlayerCharacter.NickName;
				info.BeginDate = DateTime.Now;
				info.BuyerID = 0;
				info.BuyerName = "";
				info.IsExist = true;
				info.ItemID = itemInfo.ItemID;
				info.Mouthful = num14;
				info.PayType = num16;
				info.Price = num13;
				info.Rise = num13 / 10;
				info.Rise = ((info.Rise < 1) ? 1 : info.Rise);
				info.Name = itemInfo.Template.Name;
				info.Category = itemInfo.Template.CategoryID;
				info.goodsCount = itemInfo.Count;
				AuctionInfo auctionInfo = info;
				int num12;
				switch (num15)
				{
				case 0:
					num12 = 8;
					break;
				case 1:
					num12 = 24;
					break;
				default:
					num12 = 48;
					break;
				}
				auctionInfo.ValidDate = num12;
				info.TemplateID = itemInfo.TemplateID;
				info.Random = ThreadSafeRandom.NextStatic(GameProperties.BeginAuction, GameProperties.EndAuction);
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					if (playerBussiness.AddAuction(info))
					{
						client.Player.GetInventory(eBageType)?.RemoveCountFromStack(itemAt, count);
						client.Player.SaveIntoDatabase();
						client.Player.RemoveGold(num11);
						translateId = "AuctionAddHandler.Msg6";
						client.Out.SendAuctionRefresh(info, info.AuctionID, isExist: true, itemAt);
						if (num13 > 1)
                        {
							string msg = string.Format("|Đấu Giá| - [{0}] vừa bán đấu giá vật phẩm {1} với giá {2} xu. Mau mau vào đấu giá xúc ngay nào!", client.Player.PlayerCharacter.NickName, itemInfo.TemplateID, num13);
							GSPacketIn pkg = WorldMgr.SendSysNotice(eMessageType.ChatNormal, msg, 0, itemInfo.TemplateID, null);
							GameServer.Instance.LoginServer.SendPacket(pkg);
                        }							
					}
				}
			}
			else
			{
				translateId = "AuctionAddHandler.Msg13";
			}
			client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(translateId));
			return 0;
		}
	}
}
