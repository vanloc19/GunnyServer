using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(250, "求婚答复")]
	internal class MarryApplyReplyHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			bool result = packet.ReadBoolean();
			int userID = packet.ReadInt();
			int answerId = packet.ReadInt();
			if (result && client.Player.PlayerCharacter.IsMarried)
			{
				client.Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg2"));
			}
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(userID);
				if (!result)
				{
					SendGoodManCard(userSingleByUserID.NickName, userSingleByUserID.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ID, bussiness);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
				}
				if (userSingleByUserID == null || userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex)
				{
					return 1;
				}
				if (userSingleByUserID.IsMarried)
				{
					client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg3"));
				}
				MarryApplyInfo info = new MarryApplyInfo
				{
					UserID = userID,
					ApplyUserID = client.Player.PlayerCharacter.ID,
					ApplyUserName = client.Player.PlayerCharacter.NickName,
					ApplyType = 2,
					LoveProclamation = "",
					ApplyResult = result
				};
				int id = 0;
				DailyRecordInfo record = new DailyRecordInfo
				{
					UserID = client.Player.PlayerCharacter.ID,
					Type = 3,
					Value = client.Player.PlayerCharacter.SpouseName
				};
				new PlayerBussiness().AddDailyRecord(record);
				if (bussiness.SavePlayerMarryNotice(info, answerId, ref id))
				{
					if (result)
					{
						client.Player.Out.SendMarryApplyReply(client.Player, userSingleByUserID.ID, userSingleByUserID.NickName, result, isApplicant: false, id);
						client.Player.LoadMarryProp();
					}
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
					return 0;
				}
			}
			return 1;
		}

		public void SendGoodManCard(string receiverName, int receiverID, string senderName, int senderID, PlayerBussiness db)
		{
			ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11105), 1, 112);
			item.IsBinds = true;
			item.UserID = 0;
			db.AddGoods(item);
			MailInfo mail = new MailInfo
			{
				Annex1 = item.ItemID.ToString(),
				Content = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Content"),
				Gold = 0,
				IsExist = true,
				Money = 0,
				Receiver = receiverName,
				ReceiverID = receiverID,
				Sender = senderName,
				SenderID = senderID,
				Title = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Title"),
				Type = 14
			};
			db.SendMail(mail);
		}

		public void SendSYSMessages(GamePlayer player, PlayerInfo spouse)
		{
			string str = (player.PlayerCharacter.Sex ? player.PlayerCharacter.NickName : spouse.NickName);
			string str2 = (player.PlayerCharacter.Sex ? spouse.NickName : player.PlayerCharacter.NickName);
			string translation = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg1", str, str2);
			player.OnPlayerMarry();
			GSPacketIn packet = new GSPacketIn(10);
			packet.WriteInt(2);
			packet.WriteString(translation);
			GameServer.Instance.LoginServer.SendPacket(packet);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].Out.SendTCP(packet);
			}
		}
	}
}
