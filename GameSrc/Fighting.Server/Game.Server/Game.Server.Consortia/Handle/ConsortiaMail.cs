using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(29)]
	public class ConsortiaMail : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			string str1 = packet.ReadString();
			string str2 = packet.ReadString();
			string msg = "ConsortiaRichiUpdateHandler.Failed";
			bool val = false;
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(Player.PlayerCharacter.ConsortiaID);
			if (consortiaInfo == null)
			{
				return 0;
			}
			if (consortiaInfo.Riches < 1000)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("ConsortiaBussiness.Riches.Msg3"));
				return 0;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(Player.PlayerCharacter.ConsortiaID);
				MailInfo mail = new MailInfo();
				ConsortiaUserInfo[] array = allMemberByConsortia;
				foreach (ConsortiaUserInfo consortiaUserInfo in array)
				{
					mail.SenderID = Player.PlayerCharacter.ID;
					mail.Sender = "Presidente da " + consortiaUserInfo.ConsortiaName;
					mail.ReceiverID = consortiaUserInfo.UserID;
					mail.Receiver = consortiaUserInfo.UserName;
					mail.Title = str1;
					mail.Content = str2;
					mail.Type = 59;
					if (consortiaUserInfo.UserID != Player.PlayerCharacter.ID && playerBussiness.SendMail(mail))
					{
						msg = "ConsortiaRichiUpdateHandler.Success";
						val = true;
						if (consortiaUserInfo.State != 0)
						{
							Player.Out.SendMailResponse(consortiaUserInfo.UserID, eMailRespose.Receiver);
						}
						Player.Out.SendMailResponse(Player.PlayerCharacter.ID, eMailRespose.Send);
					}
				}
			}
			if (val)
			{
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					consortiaBussiness.UpdateConsortiaRiches(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, 1000, ref msg);
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(29);
			packet2.WriteBoolean(val);
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
