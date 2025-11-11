using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(3)]
	public class ConsortiaRenegade : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			bool val = false;
			string nickName = "";
			string msg = ((num == Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed");
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.DeleteConsortiaUser(Player.PlayerCharacter.ID, num, Player.PlayerCharacter.ConsortiaID, ref msg, ref nickName))
				{
					msg = ((num == Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess");
					int consortiaId = Player.PlayerCharacter.ConsortiaID;
					if (num == Player.PlayerCharacter.ID)
					{
						//Player.ClearConsortia(isclear: true);
						Player.ClearConsortia();
						Player.AddLog("ConsortiaRenegade", "Quit G");
						Player.Out.SendMailResponse(Player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					GameServer.Instance.LoginServer.SendConsortiaUserDelete(num, consortiaId, num != Player.PlayerCharacter.ID, nickName, Player.PlayerCharacter.NickName);
					Player.PlayerCharacter.QuitConsortiaDate = DateTime.Now;
					val = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(3);
			packet2.WriteInt(num);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
