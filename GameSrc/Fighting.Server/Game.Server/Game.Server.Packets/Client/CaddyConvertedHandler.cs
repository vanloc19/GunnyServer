using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(215, "Envia mensagem para todos de sua associação")]
	public class CaddyConvertedHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			string title = packet.ReadString();
			string content = packet.ReadString();
			string text = "";
			bool flag = false;
			int price_message = 1000;
			text = "ConsortiaRichiUpdateHandler.Failed";
			ConsortiaInfo consortiaInfo9 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
			{
				ConsortiaUserInfo[] allMemberByConsortia = playerBussiness3.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
				MailInfo mailInfo = new MailInfo();
				ConsortiaUserInfo[] array = allMemberByConsortia;
				foreach (ConsortiaUserInfo consortiaUserInfo5 in array)
				{
					mailInfo.SenderID = client.Player.PlayerCharacter.ID;
					mailInfo.Sender = "Sua associação " + consortiaInfo9.ConsortiaName;
					mailInfo.ReceiverID = consortiaUserInfo5.UserID;
					mailInfo.Receiver = consortiaUserInfo5.UserName;
					mailInfo.Title = title;
					mailInfo.Content = content;
					mailInfo.Type = 59;
					if (consortiaUserInfo5.UserID != client.Player.PlayerCharacter.ID && playerBussiness3.SendMail(mailInfo))
					{
						text = "ConsortiaRichiUpdateHandler.Success";
						flag = true;
						if (consortiaUserInfo5.State != 0)
						{
							client.Player.Out.SendMailResponse(consortiaUserInfo5.UserID, eMailRespose.Receiver);
						}
						client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
					}
					if (!flag)
					{
						text = "ConsortiaRichiUpdateHandler.Success";
						flag = true;
						if (consortiaUserInfo5.State != 0)
						{
							client.Player.Out.SendMailResponse(consortiaUserInfo5.UserID, eMailRespose.Receiver);
						}
						client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
					}
				}
			}
			if (flag)
			{
				using (ConsortiaBussiness consortiaBussiness26 = new ConsortiaBussiness())
				{
					consortiaBussiness26.ConsortiaRichRemove(client.Player.PlayerCharacter.ConsortiaID, ref price_message);
				}
			}
			client.Out.SendConsortiaMail(flag, client.Player.PlayerCharacter.ID);
			client.Player.SendMessage(LanguageMgr.GetTranslation(text));
			return 0;
		}
	}
}
