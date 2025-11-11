using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(19)]
	public class ConsortiaChangeChairman : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			string str1 = packet.ReadString();
			bool val = false;
			string msg = "ConsortiaChangeChairmanHandler.Failed";
			if (string.IsNullOrEmpty(str1))
			{
				msg = "ConsortiaChangeChairmanHandler.NoName";
			}
			else if (str1 == Player.PlayerCharacter.NickName)
			{
				msg = "ConsortiaChangeChairmanHandler.Self";
			}
			else
			{
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					string tempUserName = "";
					int tempUserID = 0;
					ConsortiaDutyInfo info1 = new ConsortiaDutyInfo();
					if (consortiaBussiness.UpdateConsortiaChairman(str1, Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg, ref info1, ref tempUserID, ref tempUserName))
					{
						ConsortiaDutyInfo info2 = new ConsortiaDutyInfo();
						info2.Level = Player.PlayerCharacter.DutyLevel;
						info2.DutyName = Player.PlayerCharacter.DutyName;
						info2.Right = Player.PlayerCharacter.Right;
						msg = "ConsortiaChangeChairmanHandler.Success1";
						val = true;
						GameServer.Instance.LoginServer.SendConsortiaDuty(info2, 9, Player.PlayerCharacter.ConsortiaID, tempUserID, tempUserName, 0, "");
						GameServer.Instance.LoginServer.SendConsortiaDuty(info1, 8, Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, 0, "");
					}
				}
			}
			string translation = LanguageMgr.GetTranslation(msg);
			if (msg == "ConsortiaChangeChairmanHandler.Success1")
			{
				_ = translation + str1 + LanguageMgr.GetTranslation("ConsortiaChangeChairmanHandler.Success2");
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(19);
			packet2.WriteString(str1);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
