using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(2)]
	public class HymenealCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
			{
				return false;
			}
			if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				return false;
			}
			int num = GameProperties.PRICE_PROPOSE;
			if (player.CurrentMarryRoom.Info.IsHymeneal && player.PlayerCharacter.Money + player.PlayerCharacter.MoneyLock < num)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough"));
				return false;
			}
			GamePlayer playerByUserID = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.GroomID);
			if (playerByUserID == null)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoGroom"));
				return false;
			}
			GamePlayer player2 = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.BrideID);
			if (player2 == null)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoBride"));
				return false;
			}
			bool val = false;
			bool flag2 = false;
			GSPacketIn @in = packet.Clone();
			int num2 = packet.ReadInt();
			if (1 == num2)
			{
				player.CurrentMarryRoom.RoomState = eRoomState.FREE;
			}
			else
			{
				player.CurrentMarryRoom.RoomState = eRoomState.Hymeneal;
				player.CurrentMarryRoom.BeginTimerForHymeneal(170000);
				if (!player.PlayerCharacter.IsGotRing)
				{
					flag2 = true;
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(9022);
					ItemTemplateInfo goods1 = ItemMgr.FindItemTemplate(60000021);//Nam
					ItemTemplateInfo goods2 = ItemMgr.FindItemTemplate(60000022);//Nu
					ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 112);
					ItemInfo item1 = ItemInfo.CreateFromTemplate(goods1, 1, 112);
					item.IsBinds = true;
					item1.IsBinds = true;
					using (PlayerBussiness bussiness = new PlayerBussiness())//Nam
					{
						item.UserID = 0;
						bussiness.AddGoods(item);
						bussiness.AddGoods(item1);
						string translation = LanguageMgr.GetTranslation("HymenealCommand.Content", player2.PlayerCharacter.NickName);
						MailInfo mail = new MailInfo
						{
							Annex1 = item.ItemID.ToString(),
							Annex2 = item1.ItemID.ToString(),
							Content = translation,
							Gold = 0,
							IsExist = true,
							Money = 0,
							Receiver = playerByUserID.PlayerCharacter.NickName,
							ReceiverID = playerByUserID.PlayerCharacter.ID,
							Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender"),
							SenderID = 0,
							Title = LanguageMgr.GetTranslation("HymenealCommand.Title"),
							Type = 14
						};
						if (bussiness.SendMail(mail))
						{
							val = true;
						}
						player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
					}
					ItemInfo info4 = ItemInfo.CreateFromTemplate(goods, 1, 112);
					ItemInfo info8 = ItemInfo.CreateFromTemplate(goods2, 1, 112);
					info4.IsBinds = true;
					info8.IsBinds = true;
					using (PlayerBussiness bussiness2 = new PlayerBussiness())//Nu
					{
						info4.UserID = 0;
						bussiness2.AddGoods(info4);
						bussiness2.AddGoods(info8);
						string str2 = LanguageMgr.GetTranslation("HymenealCommand.Content", playerByUserID.PlayerCharacter.NickName);
						MailInfo info5 = new MailInfo
						{
							Annex1 = info4.ItemID.ToString(),
							Annex2 = info8.ItemID.ToString(),
							Content = str2,
							Gold = 0,
							IsExist = true,
							Money = 0,
							Receiver = player2.PlayerCharacter.NickName,
							ReceiverID = player2.PlayerCharacter.ID,
							Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender"),
							SenderID = 0,
							Title = LanguageMgr.GetTranslation("HymenealCommand.Title"),
							Type = 14
						};
						if (bussiness2.SendMail(info5))
						{
							val = true;
						}
						player.Out.SendMailResponse(info5.ReceiverID, eMailRespose.Receiver);
					}
					player.CurrentMarryRoom.Info.IsHymeneal = true;
					using (PlayerBussiness bussiness3 = new PlayerBussiness())
					{
						bussiness3.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
						bussiness3.UpdatePlayerGotRingProp(playerByUserID.PlayerCharacter.ID, player2.PlayerCharacter.ID);
						playerByUserID.LoadMarryProp();
						player2.LoadMarryProp();
					}
				}
				else
				{
					flag2 = false;
					val = true;
				}
				if (!flag2)
				{
					player.RemoveMoney(num, isConsume: false);
					CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, num, 0, 0, 1);
				}
				@in.WriteInt(player.CurrentMarryRoom.Info.ID);
				@in.WriteBoolean(val);
				player.CurrentMarryRoom.SendToAll(@in);
				if (val)
				{
					string message = LanguageMgr.GetTranslation("HymenealCommand.Succeed", playerByUserID.PlayerCharacter.NickName, player2.PlayerCharacter.NickName);
					GSPacketIn in2 = player.Out.SendMessage(eMessageType.ChatNormal, message);
					player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(in2, player);
				}
			}
			return true;
		}
	}
}
