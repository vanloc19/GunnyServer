using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(18)]
	public class ConsortiaUserGradeUpdate : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			bool flag = packet.ReadBoolean();
			bool val = false;
			string msg = "ConsortiaUserGradeUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				string tempUserName = "";
				ConsortiaDutyInfo info = new ConsortiaDutyInfo();
				if (consortiaBussiness.UpdateConsortiaUserGrade(num, Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, flag, ref msg, ref info, ref tempUserName))
				{
					msg = "ConsortiaUserGradeUpdateHandler.Success";
					val = true;
					GameServer.Instance.LoginServer.SendConsortiaDuty(info, flag ? 6 : 7, Player.PlayerCharacter.ConsortiaID, num, tempUserName, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName);
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(18);
			packet2.WriteInt(num);
			packet2.WriteBoolean(flag);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
