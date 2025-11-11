using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(4)]
	public class ConsortiaTryinPass : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			bool val = false;
			string msg = "ConsortiaApplyLoginPassHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				int consortiaRepute = 0;
				ConsortiaUserInfo info = new ConsortiaUserInfo();
				if (consortiaBussiness.PassConsortiaApplyUsers(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, Player.PlayerCharacter.ConsortiaID, ref msg, info, ref consortiaRepute))
				{
					msg = "ConsortiaApplyLoginPassHandler.Success";
					val = true;
					if (info.UserID != 0)
					{
						info.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
						info.ConsortiaName = Player.PlayerCharacter.ConsortiaName;
						GameServer.Instance.LoginServer.SendConsortiaUserPass(Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, info, isInvite: false, consortiaRepute);
					}
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(4);
			packet2.WriteInt(num);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
