using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(12)]
	public class ConsortiaInvitePass : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID != 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			bool val = false;
			int consortiaID = 0;
			string consortiaName = "";
			string msg = "ConsortiaInvitePassHandler.Failed";
			int tempID = 0;
			string tempName = "";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				int consortiaRepute = 0;
				ConsortiaUserInfo info = new ConsortiaUserInfo();
				if (consortiaBussiness.PassConsortiaInviteUsers(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, ref consortiaID, ref consortiaName, ref msg, info, ref tempID, ref tempName, ref consortiaRepute))
				{
					Player.PlayerCharacter.ConsortiaID = consortiaID;
					Player.PlayerCharacter.ConsortiaName = consortiaName;
					Player.PlayerCharacter.DutyLevel = info.Level;
					Player.PlayerCharacter.DutyName = info.DutyName;
					Player.PlayerCharacter.Right = info.Right;
					ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(consortiaID);
					if (consortiaInfo != null)
					{
						Player.PlayerCharacter.ConsortiaLevel = consortiaInfo.Level;
					}
					msg = "ConsortiaInvitePassHandler.Success";
					val = true;
					info.UserID = Player.PlayerCharacter.ID;
					info.UserName = Player.PlayerCharacter.NickName;
					info.Grade = Player.PlayerCharacter.Grade;
					info.Offer = Player.PlayerCharacter.Offer;
					info.RichesOffer = Player.PlayerCharacter.RichesOffer;
					info.RichesRob = Player.PlayerCharacter.RichesRob;
					info.Win = Player.PlayerCharacter.Win;
					info.Total = Player.PlayerCharacter.Total;
					info.Escape = Player.PlayerCharacter.Escape;
					info.honor = Player.PlayerCharacter.Honor;
					info.AchievementPoint = Player.PlayerCharacter.AchievementPoint;
					info.UseOffer = Player.PlayerCharacter.Riches;
					info.LoginName = Player.PlayerCharacter.UserName;
					info.ConsortiaID = consortiaID;
					info.ConsortiaName = consortiaName;
					info.FightPower = Player.PlayerCharacter.FightPower;
					GameServer.Instance.LoginServer.SendConsortiaUserPass(tempID, tempName, info, isInvite: true, consortiaRepute);
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(12);
			packet2.WriteInt(num);
			packet.WriteBoolean(val);
			packet.WriteInt(consortiaID);
			packet.WriteString(consortiaName);
			packet.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
